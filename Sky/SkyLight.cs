using System.Numerics;

namespace Sky;

/// <summary>
/// The light the sky gives, the sun's light scattered by the air toward a place, as luminance along
/// each way from it and as illuminance on a level surface there, for the sun outside the air of
/// <see cref="Sun.Outside"/>.
/// </summary>
/// <remarks>
/// <para>
/// Along a way from the eye the air scatters the sun's light toward the eye once, by the
/// molecules' phase function and Cornette and Shanks's for the aerosols, each part dimmed by the
/// air between the sun and the point and between the point and the eye. The light scattered more
/// than once is Hillaire's approximation of 2020, which takes the light at a point scattered twice
/// or more as alike from every way, the second order's luminance divided by one less the share the
/// air sends on at each order. A way that meets the ground takes the ground's light, the sun's
/// light it returns by its albedo, and the second order carries the ground's light too.
/// </para>
/// <para>
/// The light's strength scales with the sun's outside the air, so an engine whose sun is given
/// another illuminance multiplies what this returns by its sun's over <see
/// cref="Sun.OutsideIlluminance"/>.
/// The two tables the light is read from, the transmittance by height and zenith and the light
/// scattered more than once by height and the sun's zenith, are made when the sky is.
/// </para>
/// </remarks>
public sealed class SkyLight
{
    // The spectrum's bands the sky is read in, and the sky in each lane of three of them.
    private readonly Spectrum _bands;
    private readonly SkyLane[] _lanes;

    /// <summary>
    /// Makes the sky of <paramref name="air"/>, its tables made at once over the processor's cores,
    /// a row of heights to a core, the light scattered more than once read from the
    /// transmittance's.
    /// </summary>
    public SkyLight(Atmosphere air)
    {
        ArgumentNullException.ThrowIfNull(air);
        Air = air;
        _bands = Spectrum.Light;
        _lanes = [.. air.Lanes().Select((lane, i) => new SkyLane(lane, _bands.LaneSun(i)))];
    }

    /// <summary>The air the sky is made of.</summary>
    public Atmosphere Air { get; }

    /// <summary>
    /// The sky's luminance in candelas a square meter along <paramref name="view"/> from
    /// <paramref name="altitude"/> meters, with the sun toward <paramref name="sunToward"/>, both
    /// with +Y up, in linear sRGB whose luminance is the sky's.
    /// </summary>
    /// <remarks>
    /// The sun's own disc is left out, which an engine draws by its angular size from
    /// <see cref="Sun.At"/>. A way below the horizon ends at the ground, which shows the light the
    /// sun and the sky give it times its albedo over pi, with the air between, so the lower half of
    /// the sky an engine lights by is the ground's under both.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// <paramref name="view"/> or <paramref name="sunToward"/> has no length or is not a number.
    /// </exception>
    public Vector3 Luminance(Vector3 view, Vector3 sunToward, float altitude = 0) =>
        Combine(lane => lane.Luminance(view, sunToward, altitude));

    /// <summary>
    /// The illuminance in lux the sky gives a level surface at <paramref name="altitude"/> meters,
    /// with the sun toward <paramref name="sunToward"/>, in linear sRGB whose luminance is the lux,
    /// the sun's own light apart.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// <paramref name="sunToward"/> has no length or is not a number.
    /// </exception>
    public Vector3 Illuminance(Vector3 sunToward, float altitude = 0) =>
        Combine(lane => lane.Illuminance(sunToward, altitude));

    /// <summary>
    /// The illuminance in lux the sky gives a level surface <paramref name="altitude"/> meters
    /// above the ground with the light that lights it toward <paramref name="lightToward"/>, as
    /// <see cref="Illuminance"/> gives it within 1%, read from a table in microseconds where the
    /// sum takes hundredths of a second. The first reading at an altitude builds the table's rows
    /// about it, a fifth of a second to most of a second each by the processor's cores, so a
    /// binding reads it at the altitudes its camera keeps while it loads, off its frame.
    /// </summary>
    /// <remarks>
    /// The sky's light on a level surface depends only on how high the light stands and how high
    /// the surface is, so a table holds it in rows at altitudes from the ground to 10 km, 500 m
    /// apart to 2 km and a kilometer apart above, each by the light's height, a row a degree and
    /// more where the light's logarithm bends, the light between rows and between heights read by
    /// its logarithm. Each row is made once, at its first reading, across the processor's cores,
    /// and above 10 km the sum is read. With the light 24 degrees down or lower it gives none,
    /// since the sky lit so gives under 1.6e-7 lux at the ground, a five-thousandth of
    /// <see cref="Night.SkyIlluminance"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// <paramref name="lightToward"/> has no length or is not a number.
    /// </exception>
    public Vector3 GroundIlluminance(Vector3 lightToward, float altitude = 0) =>
        Combine(lane => lane.GroundIlluminance(lightToward, altitude));

    // The sky's light on a level surface lit by a light of outside in each band outside the air in
    // place of the sun's, as the moon's, the sky's arithmetic being the same in each band for any
    // strength of the light that lights it.
    internal Vector3 GroundIlluminanceBy(float[] outside, Vector3 lightToward, float altitude) =>
        CombineBy(outside, lane => lane.GroundIlluminance(lightToward, altitude));

    // The sky's luminance along view lit by a light of outside in each band in place of the sun's.
    internal Vector3 LuminanceBy(float[] outside, Vector3 view, Vector3 lightToward, float altitude = 0) =>
        CombineBy(outside, lane => lane.Luminance(view, lightToward, altitude));

    private Vector3 CombineBy(float[] outside, Func<SkyLane, Vector3> read)
    {
        var sum = Vector3.Zero;
        for (var lane = 0; lane < _lanes.Length; lane++)
        {
            var (light, sun) = (_bands.Lane(lane, outside), _bands.LaneSun(lane));
            var share = new Vector3(Over(light.X, sun.X), Over(light.Y, sun.Y), Over(light.Z, sun.Z));
            sum += _bands.ToLinearSrgb(lane, read(_lanes[lane]) * share);
        }
        return sum;

        static float Over(float light, float sun) => sun > 0 ? light / sun : 0;
    }

    /// <summary>
    /// Makes the ground table's rows about the altitudes from <paramref name="lowest"/> to
    /// <paramref name="highest"/> meters, so a reading in that span afterward makes none, a fifth
    /// of a second to most of a second a row by the processor's cores.
    /// </summary>
    /// <remarks>
    /// A binding calls it while it loads, off its frame, over the altitudes its camera keeps, since
    /// a row made at a first reading stalls the frame that reads it. Above 10 km no row is made,
    /// the sum being read there.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Either altitude is not a number, or <paramref name="highest"/> is under
    /// <paramref name="lowest"/>.
    /// </exception>
    public void PrepareGround(float lowest, float highest)
    {
        foreach (var lane in _lanes)
            lane.PrepareGround(lowest, highest);
    }

    /// <summary>
    /// How many of the ground table's 13 rows are made, which <see cref="PrepareGround"/> and a
    /// first reading near an altitude make.
    /// </summary>
    public int GroundRowsMade => _lanes[0].GroundRowsMade;

    /// <summary>
    /// The sky's tables as a read-only view, for an engine that draws the sky on a GPU and repeats
    /// <see cref="Luminance"/>'s arithmetic there, its README section "Drawing the sky on a GPU"
    /// saying how each is read.
    /// </summary>
    public SkyTables Tables => _tables ??= new SkyTables(Air, _bands, _lanes);

    private SkyTables? _tables;

    // A reading of every lane, its bands turned into color and summed.
    private Vector3 Combine(Func<SkyLane, Vector3> read)
    {
        var sum = Vector3.Zero;
        for (var lane = 0; lane < _lanes.Length; lane++)
            sum += _bands.ToLinearSrgb(lane, read(_lanes[lane]));
        return sum;
    }
}
