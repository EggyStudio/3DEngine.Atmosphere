using System.Numerics;

namespace Sky;

/// <summary>
/// A made <see cref="SkyLight"/>'s tables and the terms its arithmetic reads, for an engine that
/// uploads them and draws the sky on a GPU, the README's section "Drawing the sky on a GPU" giving
/// how each is read and the march that turns them into <see cref="SkyLight.Luminance"/>.
/// </summary>
/// <remarks>
/// The sky is held in lanes of three of the module's wavelength bands, each lane its own tables
/// and the air's coefficients in its bands, and a lane's light turns into linear sRGB by its own
/// matrix, so the sky's luminance is the sum over the lanes of each lane's matrix times its light.
/// </remarks>
public sealed class SkyTables
{
    internal SkyTables(Atmosphere air, Spectrum bands, SkyLane[] lanes)
    {
        PlanetRadius = air.PlanetRadius;
        Top = air.Top;
        RayleighScaleHeight = air.RayleighScaleHeight;
        AerosolScaleHeight = air.AerosolScaleHeight;
        AerosolAnisotropy = air.AerosolAnisotropy;
        OzoneBottom = air.OzoneBottom;
        OzoneWidth = air.OzoneWidth;
        Lanes = [.. lanes.Select((lane, i) => new SkyLaneTables(lane, bands, i))];
    }

    /// <summary>The planet's radius in meters.</summary>
    public float PlanetRadius { get; }

    /// <summary>The height in meters the air ends at, the tables' altitude axes' end.</summary>
    public float Top { get; }

    /// <summary>The molecules' scale height in meters.</summary>
    public float RayleighScaleHeight { get; }

    /// <summary>The aerosols' scale height in meters.</summary>
    public float AerosolScaleHeight { get; }

    /// <summary>The aerosols' asymmetry in Cornette and Shanks's phase function.</summary>
    public float AerosolAnisotropy { get; }

    /// <summary>The height in meters the ozone's layer starts at.</summary>
    public float OzoneBottom { get; }

    /// <summary>The ozone layer's depth in meters, densest halfway up it.</summary>
    public float OzoneWidth { get; }

    /// <summary>The steps the march along a view takes, 48.</summary>
    public int ViewSteps => SkyLane.ViewSteps;

    /// <summary>
    /// The lowest height in degrees of the light over the horizon the ground's table holds, -24,
    /// under which the sky gives the ground no light.
    /// </summary>
    public float LowestGround => SkyLane.LowestGround;

    /// <summary>
    /// The altitudes in meters the ground's table holds a row at, from 0 to 10,000, a row's light
    /// read between two of them in its logarithm.
    /// </summary>
    public ReadOnlyMemory<float> GroundAltitudes { get; } = SkyLane.GroundAltitudes;

    /// <summary>The lanes, each three of the module's wavelength bands.</summary>
    public IReadOnlyList<SkyLaneTables> Lanes { get; }
}

/// <summary>
/// One lane of a sky's tables, three of the module's wavelength bands, each table's values three
/// floats a cell, one a band, row after row.
/// </summary>
public sealed class SkyLaneTables
{
    private readonly SkyLane _lane;
    private readonly Spectrum _bands;
    private readonly int _index;

    internal SkyLaneTables(SkyLane lane, Spectrum bands, int index)
    {
        (_lane, _bands, _index) = (lane, bands, index);
        var air = lane.Air;
        Sun = lane.SunLight;
        RayleighScattering = air.RayleighScattering;
        AerosolExtinction = air.AerosolsAt(0);
        AerosolAlbedo = air.AerosolAlbedo;
        OzoneAbsorption = air.OzoneAbsorption;
        GroundAlbedo = air.GroundAlbedo;
        // The matrix's columns are each band's light alone turned into linear sRGB.
        var (r, g, b) = (bands.ToLinearSrgb(index, Vector3.UnitX), bands.ToLinearSrgb(index, Vector3.UnitY), bands.ToLinearSrgb(index, Vector3.UnitZ));
        ToLinearSrgb = new float[] { r.X, g.X, b.X, r.Y, g.Y, b.Y, r.Z, g.Z, b.Z };
        Transmittance = Flat(lane.TransmittanceTable);
        Scattered = Flat(lane.ScatteredTable);
    }

    /// <summary>
    /// The sun's light outside the air in the lane's three bands, watts a square meter a nanometer.
    /// </summary>
    public Vector3 Sun { get; }

    /// <summary>
    /// The moon's light outside the air in the lane's three bands at a phase angle of
    /// <paramref name="phaseAngle"/> degrees, the sun's reflected by the lunar disc, which lights
    /// the sky in place of <see cref="Sun"/>.
    /// </summary>
    public Vector3 Moon(float phaseAngle) => _bands.Lane(_index, Sky.Moon.Outside(phaseAngle));

    /// <summary>The molecules' scattering a meter at sea level in each band.</summary>
    public Vector3 RayleighScattering { get; }

    /// <summary>The aerosols' extinction a meter at sea level in each band.</summary>
    public Vector3 AerosolExtinction { get; }

    /// <summary>The share of the aerosols' extinction they scatter in each band.</summary>
    public Vector3 AerosolAlbedo { get; }

    /// <summary>The ozone's absorption a meter in the middle of its layer in each band.</summary>
    public Vector3 OzoneAbsorption { get; }

    /// <summary>The ground's albedo in each band.</summary>
    public Vector3 GroundAlbedo { get; }

    /// <summary>
    /// The matrix that turns a light in the lane's three bands into linear sRGB whose luminance is
    /// its lux, or its candelas a square meter for a radiance, nine floats row by row.
    /// </summary>
    public ReadOnlyMemory<float> ToLinearSrgb { get; }

    /// <summary>The transmittance table's rows, its altitudes, 32.</summary>
    public int TransmittanceHeights => SkyLane.TransmittanceHeights;

    /// <summary>The transmittance table's columns, its cosines of the zenith angle, 256.</summary>
    public int TransmittanceAngles => SkyLane.TransmittanceAngles;

    /// <summary>
    /// The share of the light that crosses the air to its top from an altitude toward a way, each
    /// band's as it is, row <c>h</c> at the altitude Top (h / (rows - 1))^2 and column <c>a</c> at
    /// the cosine -1 + 2 a / (columns - 1), none where the way meets the ground.
    /// </summary>
    public ReadOnlyMemory<float> Transmittance { get; }

    /// <summary>The scattering table's rows, its altitudes, 32.</summary>
    public int ScatterHeights => SkyLane.ScatterHeights;

    /// <summary>The scattering table's columns, the sun's zenith cosines, 128.</summary>
    public int ScatterAngles => SkyLane.ScatterAngles;

    /// <summary>
    /// The natural logarithm of the light scattered more than once a steradian a meter of the
    /// scattering coefficient, per unit of the sun's light outside the air, held at no less than
    /// the logarithm of 1e-30, rows and columns mapped as <see cref="Transmittance"/>'s with the
    /// sun's cosine in place of the way's.
    /// </summary>
    public ReadOnlyMemory<float> Scattered { get; }

    /// <summary>
    /// The ground's table at the altitude of <see cref="SkyTables.GroundAltitudes"/>'s
    /// <paramref name="row"/>, made where it was not yet: the heights in degrees of the light
    /// over the horizon the row holds, rising, and the natural logarithm of the sky's light on a
    /// level surface at each in the lane's three bands, read between two heights in its logarithm.
    /// </summary>
    public (ReadOnlyMemory<float> Heights, ReadOnlyMemory<float> LogLight) Ground(int row)
    {
        var (heights, light) = _lane.GroundRow(row);
        return (heights, Flat(light));
    }

    private static float[] Flat(Vector3[] cells)
    {
        var flat = new float[cells.Length * 3];
        for (var i = 0; i < cells.Length; i++)
            (flat[3 * i], flat[3 * i + 1], flat[3 * i + 2]) = (cells[i].X, cells[i].Y, cells[i].Z);
        return flat;
    }
}
