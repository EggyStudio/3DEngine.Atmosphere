using System.Numerics;
using System.Runtime.CompilerServices;

namespace Sky;

/// <summary>
/// The air around a planet as a sky is drawn and lit from it, the planet's radius, the molecules'
/// scattering, the aerosols' extinction and the ozone's absorption with the heights each thins
/// over, and the ground's albedo, in meters and per meter at sea level.
/// </summary>
/// <remarks>
/// <para>
/// The molecules and the aerosols thin with height as an exponential of their scale heights, and
/// the ozone lies in a layer from <see cref="OzoneBottom"/> up by <see cref="OzoneWidth"/>, densest
/// in its middle and thinning to nothing at either edge. A coefficient is given at three
/// wavelengths, 680, 550 and 440 nanometers, read as linear sRGB's red, green and blue, so a color
/// a coefficient multiplies is a color of the frame.
/// </para>
/// <para>
/// <see cref="Earth"/> holds the Earth's air, its molecules' scattering Bruneton and Neyret's
/// precomputed atmospheric scattering of 2008 gives, its ozone a column of 300 Dobson units in a
/// layer from 10 to 40 km (<see cref="OzoneAbsorption"/>), a vertical optical depth of 0.028 in
/// green, which turns the twilight's zenith blue, and its haze an ordinary clear day's
/// (<see cref="AerosolExtinction"/>), a turbidity of 2.40.
/// </para>
/// </remarks>
public sealed record Atmosphere
{
    /// <summary>The Earth's air, each term as <see cref="Earth"/> holds it until set.</summary>
    public Atmosphere()
    {
    }

    /// <summary>
    /// The Earth's air on an ordinary clear day, its ozone a column of 300 Dobson units.
    /// </summary>
    public static Atmosphere Earth { get; } = new();

    /// <summary>The planet's radius in meters, 6,378,100 for the Earth at the equator.</summary>
    public float PlanetRadius { get; init; } = 6_378_100;

    /// <summary>
    /// How much of the light the molecules scatter a meter at sea level, at 680, 550 and 440
    /// nanometers, which they take nothing of otherwise.
    /// </summary>
    public Vector3 RayleighScattering { get; init; } = new(5.8e-6f, 13.5e-6f, 33.1e-6f);

    /// <summary>
    /// The height in meters over which the molecules thin to a share of 1/e, 8,000 for the Earth.
    /// </summary>
    public float RayleighScaleHeight { get; init; } = 8_000;

    /// <summary>
    /// How much of the light the aerosols take out a meter at sea level, scattered or absorbed,
    /// alike at every wavelength.
    /// </summary>
    /// <remarks>
    /// <see cref="Turbidity"/> reads the air's haze from it and <see cref="WithTurbidity"/> sets it
    /// from one. Unless set, the haze with the four terms below it is the continental average
    /// aerosol of Hess, Koepke and Schult's OPAC (1998) at a relative humidity of 80%, 0.075 of the
    /// light a kilometer at the ground at 550 nm in a layer an optical depth of 0.151 deep, which
    /// one exponential of a 2,013 m scale height holds both of, scattering 0.925 of what it takes
    /// with an asymmetry of 0.703 and falling with wavelength by an Angstrom exponent of 1.42 from
    /// 500 to 800 nm, an ordinary clear day's of 44 km visibility.
    /// </remarks>
    public float AerosolExtinction { get; init; } = 7.5e-5f;

    /// <summary>
    /// How steeply the aerosols' extinction falls with wavelength, the power of 550 nm over the
    /// wavelength it goes as, 1.42 unless set.
    /// </summary>
    public float AerosolAngstrom { get; init; } = 1.42f;

    // The aerosols' extinction at sea level in each of a lane's three bands, set in a lane alone.
    internal Vector3? LaneAerosol { get; init; }

    /// <summary>
    /// The share of the light the aerosols take out that they scatter, the rest absorbed, at each
    /// wavelength.
    /// </summary>
    public Vector3 AerosolAlbedo { get; init; } = new(0.925f);

    /// <summary>
    /// The height in meters over which the aerosols thin to a share of 1/e, 2,013 for the Earth.
    /// </summary>
    public float AerosolScaleHeight { get; init; } = 2_013;

    /// <summary>
    /// How strongly the aerosols scatter forward, from -1 for all of the light back the way it came
    /// to 1 for all of it on, as Cornette and Shanks's phase function takes it.
    /// </summary>
    public float AerosolAnisotropy { get; init; } = 0.703f;

    /// <summary>
    /// How much of the light the ozone absorbs a meter in the middle of its layer, at each
    /// wavelength, unless set the absorption Bruneton's precomputed atmospheric scattering of 2017
    /// gives a column of 300 Dobson units.
    /// </summary>
    public Vector3 OzoneAbsorption { get; init; } = new(0.650e-6f, 1.881e-6f, 0.085e-6f);

    /// <summary>The height in meters the ozone's layer starts at, 10 km unless set.</summary>
    public float OzoneBottom { get; init; } = 10_000;

    /// <summary>
    /// The ozone layer's depth in meters, densest halfway up it, 30 km unless set.
    /// </summary>
    public float OzoneWidth { get; init; } = 30_000;

    /// <summary>
    /// The share of the light the ground returns at each wavelength, scattered evenly.
    /// </summary>
    public Vector3 GroundAlbedo { get; init; } = new(0.12f, 0.10f, 0.09f);

    /// <summary>
    /// The height in meters past which the air is taken to end, where its thickest layer has
    /// thinned to a millionth or the ozone's layer ends, whichever is higher, 111 km for Earth.
    /// </summary>
    /// <remarks>
    /// The twilight's sky is lit through the air high above the earth's shadow, which stands
    /// overhead at 35 km with the sun 6 degrees down and at 79 km with it 9 down. Air ended at a
    /// thousandth, 55 km, would lie all in that shadow from 8 degrees down, its twilight lit
    /// through the low air toward the sun alone and reddened where it is blue.
    /// </remarks>
    public float Top => MathF.Max(-MathF.Log(1e-6f) * MathF.Max(RayleighScaleHeight, AerosolScaleHeight), OzoneBottom + OzoneWidth);

    /// <summary>
    /// The air's haze, the optical depth of the molecules and the aerosols over that of the
    /// molecules alone straight up at 550 nanometers, 1 for air with no aerosols in it and about
    /// 1.1 for the Earth here.
    /// </summary>
    public float Turbidity => 1 + AerosolExtinction * AerosolScaleHeight / (RayleighScattering.Y * RayleighScaleHeight);

    /// <summary>
    /// This air with the aerosols' extinction set so its <see cref="Turbidity"/> is <paramref
    /// name="turbidity"/>, 1 at least.
    /// </summary>
    public Atmosphere WithTurbidity(float turbidity) =>
        this with { AerosolExtinction = MathF.Max(turbidity - 1, 0) * RayleighScattering.Y * RayleighScaleHeight / AerosolScaleHeight };

    /// <summary>
    /// How much of the light the molecules scatter a meter at <paramref name="altitude"/> meters,
    /// at each wavelength.
    /// </summary>
    public Vector3 RayleighAt(float altitude) => RayleighScattering * MathF.Exp(-MathF.Max(altitude, 0) / RayleighScaleHeight);

    /// <summary>
    /// How much of the light the aerosols take out a meter at <paramref name="altitude"/> meters.
    /// </summary>
    public float AerosolAt(float altitude) => AerosolExtinction * MathF.Exp(-MathF.Max(altitude, 0) / AerosolScaleHeight);

    /// <summary>
    /// How much of the light the ozone absorbs a meter at <paramref name="altitude"/> meters, at
    /// each wavelength.
    /// </summary>
    public Vector3 OzoneAt(float altitude) => OzoneAbsorption * MathF.Max(0, 1 - MathF.Abs(2 * (altitude - OzoneBottom) / OzoneWidth - 1));

    /// <summary>
    /// How much of the light the air takes out a meter at <paramref name="altitude"/> meters,
    /// scattered or absorbed, at each wavelength.
    /// </summary>
    public Vector3 ExtinctionAt(float altitude) => RayleighAt(altitude) + AerosolsAt(altitude) + OzoneAt(altitude);

    // The aerosols' extinction a meter at an altitude in each channel, or each of a lane's bands.
    internal Vector3 AerosolsAt(float altitude) =>
        (LaneAerosol ?? new Vector3(AerosolExtinction)) * MathF.Exp(-MathF.Max(altitude, 0) / AerosolScaleHeight);

    // A light given outside the air in each of the module's bands, through the air from altitude
    // toward a way whose cosine with the zenith is cosZenith, in linear sRGB whose luminance is its
    // lux, the air taking each band as its own.
    internal Vector3 Through(float[] outside, float altitude, float cosZenith)
    {
        var (spectrum, lanes) = (Spectrum.Light, Lanes());
        var through = Vector3.Zero;
        for (var lane = 0; lane < lanes.Length; lane++)
            through += spectrum.ToLinearSrgb(lane, spectrum.Lane(lane, outside) * lanes[lane].Transmittance(altitude, cosZenith));
        return through;
    }

    // The air read in each lane of the module's bands, made once an air and kept by the air
    // itself rather than by its value, so an air made from this one with another term never reads
    // this one's lanes.
    internal Atmosphere[] Lanes() =>
        Made.GetValue(this, air => [.. Enumerable.Range(0, Spectrum.Light.Lanes).Select(lane => air.Lane(Spectrum.Light, lane))]);

    private static readonly ConditionalWeakTable<Atmosphere, Atmosphere[]> Made = new();

    // The air read in three of a spectrum's bands from the lane's first, its molecules' scattering
    // falling as the wavelength to the power of 4.08 from its 550 nm value, the green channel's,
    // its ozone's absorption as the ozone's cross-section against its value at 550, its aerosols'
    // extinction as 550 nm over the wavelength to the power of AerosolAngstrom, and its albedos
    // read between the channels' wavelengths, the sRGB primaries' dominant 611, 549 and 464 nm.
    internal Atmosphere Lane(Spectrum spectrum, int lane)
    {
        Vector3 Bands(Func<int, float> of) => new(of(3 * lane), of(3 * lane + 1), of(3 * lane + 2));
        float Between(Vector3 rgb, float nm) => nm >= 611 ? rgb.X : nm >= 549
            ? float.Lerp(rgb.Y, rgb.X, (nm - 549) / 62) : nm >= 464 ? float.Lerp(rgb.Z, rgb.Y, (nm - 464) / 85) : rgb.Z;
        return this with
        {
            RayleighScattering = Bands(b => RayleighScattering.Y * spectrum.Mean(b, nm => MathF.Pow(550 / nm, 4.08f))),
            OzoneAbsorption = Bands(b => OzoneAbsorption.Y * spectrum.Ozone[b] / Spectrum.OzoneAt550),
            LaneAerosol = Bands(b => AerosolExtinction * spectrum.Mean(b, nm => MathF.Pow(550 / nm, AerosolAngstrom))),
            AerosolAlbedo = Bands(b => Between(AerosolAlbedo, spectrum.Centers[b])),
            GroundAlbedo = Bands(b => Between(GroundAlbedo, spectrum.Centers[b])),
        };
    }

    /// <summary>
    /// The share of the sun's light that crosses the air from <paramref name="altitude"/> meters
    /// toward a way whose cosine with the zenith there is <paramref name="cosZenith"/>, out to the
    /// top of the air, in each channel of linear sRGB, or nothing where the way meets the ground.
    /// </summary>
    /// <remarks>
    /// The air takes the light by wavelength, so the share is the sun's light through it summed by
    /// band and turned into color, over the sun's light outside it, which a light of the sun's
    /// spectrum keeps in each channel.
    /// The planet is round, so a way below the level of the eye still leaves the air where it
    /// passes over the ground's horizon. The optical depth is summed over steps that are short near
    /// the start, where the air is thickest along a way that starts low, and lengthen toward the
    /// top.
    /// </remarks>
    public Vector3 Transmittance(float altitude, float cosZenith)
    {
        if (LaneAerosol is null)
        {
            // The sun's light by band through the air, over the sun's light outside it, in each
            // channel.
            return Through(Spectrum.Light.SunLight, altitude, cosZenith) / Sun.Outside;
        }
        var (toGround, toTop) = Distances(altitude, cosZenith);
        if (toGround is not null) return Vector3.Zero;
        var depth = OpticalDepth(altitude, cosZenith, toTop, 64);
        return new Vector3(MathF.Exp(-depth.X), MathF.Exp(-depth.Y), MathF.Exp(-depth.Z));
    }

    /// <summary>
    /// The optical depth of the air over <paramref name="distance"/> meters from
    /// <paramref name="altitude"/> toward a way whose cosine with the zenith is
    /// <paramref name="cosZenith"/>, summed over <paramref name="steps"/> steps.
    /// </summary>
    internal Vector3 OpticalDepth(float altitude, float cosZenith, float distance, int steps)
    {
        var depth = Vector3.Zero;
        for (int i = 0; i < steps; i++)
        {
            // The steps lengthen as the square of their count, from the start out.
            var (from, to) = ((float)i / steps, (float)(i + 1) / steps);
            var (near, far) = (from * from * distance, to * to * distance);
            depth += ExtinctionAt(AltitudeAlong(altitude, cosZenith, (near + far) / 2)) * (far - near);
        }
        return depth;
    }

    /// <summary>
    /// The altitude in meters <paramref name="distance"/> meters from <paramref name="altitude"/>
    /// toward a way whose cosine with the zenith is <paramref name="cosZenith"/>.
    /// </summary>
    internal float AltitudeAlong(float altitude, float cosZenith, float distance)
    {
        // In doubles, since the radius squared is some 4e13, past which a float keeps half a meter.
        double r = PlanetRadius + (double)altitude, t = distance;
        return (float)(Math.Sqrt(r * r + t * t + 2 * r * t * cosZenith) - PlanetRadius);
    }

    /// <summary>
    /// The cosine with the zenith at <paramref name="distance"/> meters along the way, which turns
    /// as the ground curves under it.
    /// </summary>
    internal float CosZenithAlong(float altitude, float cosZenith, float distance)
    {
        double r = PlanetRadius + (double)altitude, t = distance;
        var at = Math.Sqrt(r * r + t * t + 2 * r * t * cosZenith);
        return (float)((r * cosZenith + t) / at);
    }

    /// <summary>
    /// How far a way from <paramref name="altitude"/> whose cosine with the zenith is
    /// <paramref name="cosZenith"/> runs to the ground, or null where it misses it, and to the top
    /// of the air, 0 where it starts above it and leaves.
    /// </summary>
    internal (float? Ground, float Top) Distances(float altitude, float cosZenith)
    {
        double r = PlanetRadius + (double)MathF.Max(altitude, 0), mu = cosZenith, top = PlanetRadius + (double)Top;
        var outward = r * r * (mu * mu - 1) + top * top;
        var toTop = outward > 0 ? Math.Max(0, -r * mu + Math.Sqrt(outward)) : 0;
        var inward = r * r * (mu * mu - 1) + (double)PlanetRadius * PlanetRadius;
        float? toGround = mu < 0 && inward >= 0 ? (float)Math.Max(0, -r * mu - Math.Sqrt(inward)) : null;
        return (toGround, (float)toTop);
    }
}
