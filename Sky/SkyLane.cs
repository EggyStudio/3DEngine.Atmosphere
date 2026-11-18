using System.Numerics;

namespace Sky;

// The sky's light in three of the spectrum's bands, which SkyLight reads in lanes and turns into
// color. Each channel of its vectors is a band, the sun's light outside the air in each given, and
// the arithmetic is the light scattered once along a way and Hillaire's light scattered more than
// once, each band alike, so a lane of the module's air read in its bands gives its share of the
// sky's light in watts a square meter a nanometer a steradian, or a nanometer on a level surface.
internal sealed class SkyLane
{
    // The sun's light outside the air in the lane's three bands.
    private readonly Vector3 _sun;

    // The tables' sizes, heights along one axis and the cosine of the zenith along the other. The
    // transmittance's heights crowd near the ground as the square of their count, where the sky is
    // seen from, and its angles are fine enough that the horizon's edge spans half a degree. The
    // light scattered more than once falls by orders of magnitude as the sun goes down through the
    // twilight, so its table holds the light's logarithm, which falls nearly in a straight line
    // and is read between rows as one, where the light itself read so lit the twilight three
    // times too brightly 8 degrees down. Its heights crowd near the ground as the transmittance's
    // do, so through 111 km of air the light near the ground, where the aerosols lie, is read with
    // its lowest rows 115 meters apart, and its 128 angles stand under a degree apart, fine enough
    // for the bend a haze 2 km deep gives the twilight 14 degrees down, where 64 jumped there.
    private const int TransmittanceHeights = 32, TransmittanceAngles = 256, ScatterHeights = 32, ScatterAngles = 128;

    // The steps along a way, from the eye and over the 64 ways the light scattered more than once
    // is gathered from.
    private const int ViewSteps = 48, ScatterSteps = 20, ScatterWays = 8;

    private readonly Vector3[] _transmittance = new Vector3[TransmittanceHeights * TransmittanceAngles];
    // The logarithm of the light scattered more than once.
    private readonly Vector3[] _scattered = new Vector3[ScatterHeights * ScatterAngles];

    // The ground's table starts with a row a degree from LowestGround to the zenith, and a span
    // is halved while the light at its middle and the table's reading there differ by more than
    // GroundError of the band, down to a 64th of a degree. The light
    // 7 to 9 degrees down bends sharply as the earth's shadow overhead passes the top of the air,
    // where the molecules that scatter the most blue lie, and the halving finds that bend.
    private const float LowestGround = -24, GroundError = 0.002f;
    private const int GroundHalvings = 6;

    // The altitudes in meters the ground's table holds a row of heights at, the light between two
    // read as a straight line in its logarithm, and above the highest summed over the dome.
    private static readonly float[] GroundAltitudes = [0, 500, 1_000, 1_500, 2_000, 3_000, 4_000, 5_000, 6_000, 7_000, 8_000, 9_000, 10_000];

    // The ground's table, a row at each of those altitudes made at its first reading, each the
    // light's heights in degrees rising and the logarithm of the sky's light on a level surface at
    // each.
    private readonly Lazy<(float[] Heights, Vector3[] Light)>[] _ground;

    // Makes the sky of air, its tables made at once over the processor's cores,
    // a row of heights to a core, the light scattered more than once read from the
    // transmittance's.
    public SkyLane(Atmosphere air, Vector3 sun)
    {
        Air = air;
        _sun = sun;
        _ground = [.. GroundAltitudes.Select(altitude => new Lazy<(float[], Vector3[])>(() => GroundTable(altitude)))];
        Parallel.For(0, TransmittanceHeights, h =>
        {
            for (int a = 0; a < TransmittanceAngles; a++)
                _transmittance[h * TransmittanceAngles + a] = air.Transmittance(TransmittanceHeight(h), -1 + 2f * a / (TransmittanceAngles - 1));
        });
        Parallel.For(0, ScatterHeights, h =>
        {
            for (int a = 0; a < ScatterAngles; a++)
                _scattered[h * ScatterAngles + a] = Log(Scattered(ScatterHeight(h), -1 + 2f * a / (ScatterAngles - 1)));
        });
    }

    // The air the sky is made of.
    public Atmosphere Air { get; }

    // The sky's luminance in candelas a square meter along view from
    // altitude meters, with the sun toward sunToward, both
    // with +Y up, in linear sRGB whose luminance is the sky's.
    // The sun's own disc is left out, which an engine draws by its angular size from
    // Sun.At. A way below the horizon ends at the ground, which shows the light the
    // sun and the sky give it times its albedo over pi, with the air between, so the lower half of
    // the sky an engine lights by is the ground's under both.
    // view or sunToward has no length or is not a number.
    public Vector3 Luminance(Vector3 view, Vector3 sunToward, float altitude = 0)
    {
        view = Way(view, nameof(view));
        sunToward = Way(sunToward, nameof(sunToward));
        var (toGround, toTop) = Air.Distances(altitude, view.Y);
        var distance = toGround ?? toTop;
        var cosAngle = Vector3.Dot(view, sunToward);
        var (rayleighPhase, aerosolPhase) = (RayleighPhase(cosAngle), AerosolPhase(Air.AerosolAnisotropy, cosAngle));
        var (light, through) = (Vector3.Zero, Vector3.One);
        for (int i = 0; i < ViewSteps; i++)
        {
            var (from, to) = ((float)i / ViewSteps, (float)(i + 1) / ViewSteps);
            var (near, far) = (from * from * distance, to * to * distance);
            var (height, up) = At(altitude, view, (near + far) / 2);
            var sunZenith = Vector3.Dot(up, sunToward);
            var (rayleigh, aerosol) = (Air.RayleighAt(height), Air.AerosolsAt(height) * Air.AerosolAlbedo);
            var source = (rayleigh * rayleighPhase + aerosol * aerosolPhase) * TransmittanceAt(height, sunZenith)
                + (rayleigh + aerosol) * ScatteredAt(height, sunZenith);
            (light, through) = Step(light, through, source, Air.ExtinctionAt(height), far - near);
        }
        var sky = Vector3.Zero;
        if (toGround is { } ground)
        {
            // The ground sends back its albedo of the light on it alike every way, the sun's on a
            // level surface and the sky's, which its table gives in lux, so a way below the
            // horizon at dusk shows the ground the sky lights, not black.
            var (_, up) = At(altitude, view, ground);
            var sunZenith = Vector3.Dot(up, sunToward);
            light += through * TransmittanceAt(0, sunZenith) * MathF.Max(sunZenith, 0) * Air.GroundAlbedo / MathF.PI;
            var lit = new Vector3(MathF.Sqrt(MathF.Max(0, 1 - sunZenith * sunZenith)), sunZenith, 0);
            sky = through * GroundIlluminance(lit) * Air.GroundAlbedo / MathF.PI;
        }
        return light * _sun + sky;
    }

    // The illuminance in lux the sky gives a level surface at altitude meters,
    // with the sun toward sunToward, in linear sRGB whose luminance is the lux,
    // the sun's own light apart.
    // sunToward has no length or is not a number.
    public Vector3 Illuminance(Vector3 sunToward, float altitude = 0)
    {
        // The dome in bands of the cosine with the zenith, each the same solid angle, and around.
        // The sky is alike either side of the sun's vertical plane, so with the sun turned to
        // stand at the turn of 0 the ways on one side are read and each counted twice, the way
        // at a turn and the one at the turn less it seeing the same sky.
        const int Bands = 24, Around = 48;
        sunToward = Way(sunToward, nameof(sunToward));
        var sun = new Vector3(MathF.Sqrt(MathF.Max(0, 1 - sunToward.Y * sunToward.Y)), sunToward.Y, 0);
        var sum = Vector3.Zero;
        for (int b = 0; b < Bands; b++)
        {
            var cos = (b + 0.5f) / Bands;
            var sin = MathF.Sqrt(1 - cos * cos);
            for (int a = 0; a < Around / 2; a++)
            {
                var turn = 2 * MathF.PI * (a + 0.5f) / Around;
                sum += Luminance(new Vector3(sin * MathF.Cos(turn), cos, sin * MathF.Sin(turn)), sun, altitude) * cos;
            }
        }
        return sum * (2 * 2 * MathF.PI / (Bands * Around));
    }

    // The illuminance in lux the sky gives a level surface altitude meters
    // above the ground with the light that lights it toward lightToward, as
    // Illuminance gives it within 1%, read from a table in microseconds where the
    // sum takes hundredths of a second. The first reading at an altitude builds the table's rows
    // about it, a fifth of a second to most of a second each by the processor's cores, so a
    // binding reads it at the altitudes its camera keeps while it loads, off its frame.
    // The sky's light on a level surface depends only on how high the light stands and how high
    // the surface is, so a table holds it in rows at altitudes from the ground to 10 km, 500 m
    // apart to 2 km and a kilometer apart above, each by the light's height, a row a degree and
    // more where the light's logarithm bends, the light between rows and between heights read by
    // its logarithm. Each row is made once, at its first reading, across the processor's cores,
    // and above 10 km the sum is read. With the light 24 degrees down or lower it gives none,
    // since the sky lit so gives under 1.6e-7 lux at the ground, a five-thousandth of
    // Night.SkyIlluminance.
    // lightToward has no length or is not a number.
    public Vector3 GroundIlluminance(Vector3 lightToward, float altitude = 0)
    {
        if (altitude > GroundAltitudes[^1]) return Illuminance(lightToward, altitude);
        var elevation = float.RadiansToDegrees(MathF.Asin(Math.Clamp(Way(lightToward, nameof(lightToward)).Y, -1, 1)));
        if (elevation < LowestGround) return Vector3.Zero;
        altitude = MathF.Max(altitude, 0);
        var row = Array.BinarySearch(GroundAltitudes, altitude);
        if (row >= 0) return Exp(GroundRead(row, elevation));
        var (lower, upper) = (~row - 1, ~row);
        var t = (altitude - GroundAltitudes[lower]) / (GroundAltitudes[upper] - GroundAltitudes[lower]);
        return Exp(Vector3.Lerp(GroundRead(lower, elevation), GroundRead(upper, elevation), t));
    }

    // Makes the ground table's rows about the altitudes from lowest to
    // highest meters, so a reading in that span afterward makes none, a fifth
    // of a second to most of a second a row by the processor's cores.
    // A binding calls it while it loads, off its frame, over the altitudes its camera keeps, since
    // a row made at a first reading stalls the frame that reads it. Above 10 km no row is made,
    // the sum being read there.
    // Either altitude is not a number, or highest is under
    // lowest.
    public void PrepareGround(float lowest, float highest)
    {
        if (float.IsNaN(lowest) || float.IsNaN(highest) || highest < lowest)
            throw new ArgumentOutOfRangeException(
                nameof(highest), "The span of altitudes runs from a number up to one no lower.");
        var top = GroundAltitudes[^1];
        var (low, high) = (Math.Clamp(lowest, 0, top), Math.Clamp(highest, 0, top));
        var first = Array.BinarySearch(GroundAltitudes, low) is var a && a >= 0 ? a : ~a - 1;
        var last = Array.BinarySearch(GroundAltitudes, high) is var b && b >= 0 ? b : ~b;
        for (var row = first; row <= last; row++)
            _ = _ground[row].Value;
    }

    // How many of the ground table's 13 rows are made, which PrepareGround and a
    // first reading near an altitude make.
    public int GroundRowsMade => _ground.Count(row => row.IsValueCreated);

    // The logarithm of the sky's light a row of the ground's table gives with the light at a
    // height, read between the row's heights as a straight line.
    private Vector3 GroundRead(int row, float elevation)
    {
        var (heights, light) = _ground[row].Value;
        var i = Array.BinarySearch(heights, elevation);
        if (i >= 0) return light[i];
        var above = Math.Min(~i, heights.Length - 1);
        var below = above - 1;
        var t = (elevation - heights[below]) / (heights[above] - heights[below]);
        return Vector3.Lerp(light[below], light[above], t);
    }

    // The share of the light that crosses the air from altitude toward a way of
    // cosine cosZenith with the zenith, from the table.
    internal Vector3 TransmittanceAt(float altitude, float cosZenith)
    {
        var row = MathF.Sqrt(Math.Clamp(altitude / Air.Top, 0, 1)) * (TransmittanceHeights - 1);
        var column = (Math.Clamp(cosZenith, -1, 1) + 1) / 2 * (TransmittanceAngles - 1);
        return Bilinear(_transmittance, TransmittanceAngles, row, column);
    }

    // The light scattered twice or more at altitude with the sun at a cosine of
    // cosSunZenith with the zenith, for a sun of 1 outside the air, from the
    // table.
    internal Vector3 ScatteredAt(float altitude, float cosSunZenith)
    {
        var row = MathF.Sqrt(Math.Clamp(altitude / Air.Top, 0, 1)) * (ScatterHeights - 1);
        var column = (Math.Clamp(cosSunZenith, -1, 1) + 1) / 2 * (ScatterAngles - 1);
        return Exp(Bilinear(_scattered, ScatterAngles, row, column));
    }

    // The ground's table, its rows a degree apart first, then each span between two of them
    // halved as it needs, the two halves of a span made at once.
    private (float[] Heights, Vector3[] Light) GroundTable(float altitude)
    {
        var spans = (int)(90 - LowestGround);
        var ends = new Vector3[spans + 1];
        Parallel.For(0, spans + 1, i => ends[i] = GroundLog(LowestGround + i, altitude));
        var rows = new List<(float Height, Vector3 Light)>[spans];
        Parallel.For(0, spans, i => rows[i] = Halve(LowestGround + i, ends[i], LowestGround + i + 1, ends[i + 1], GroundHalvings, altitude));
        (float Height, Vector3 Light)[] all = [.. rows.SelectMany(r => r), (90f, ends[spans])];
        return (all.Select(r => r.Height).ToArray(), all.Select(r => r.Light).ToArray());
    }

    // The rows of a span from one height up to the next, the higher end left to the next span.
    private List<(float Height, Vector3 Light)> Halve(float from, Vector3 low, float to, Vector3 high, int halvings, float altitude)
    {
        var middle = (from + to) / 2;
        var light = GroundLog(middle, altitude);
        var (read, sum) = (Exp(Vector3.Lerp(low, high, 0.5f)), Exp(light));
        // Each band to itself, since a dim band held to the lane's mean misses by more.
        var off = Vector3.Abs(read - sum) - sum * GroundError;
        if (halvings == 0 || MathF.Max(off.X, MathF.Max(off.Y, off.Z)) <= 0)
            return [(from, low)];
        var halves = new List<(float Height, Vector3 Light)>[2];
        Parallel.Invoke(
            () => halves[0] = Halve(from, low, middle, light, halvings - 1, altitude),
            () => halves[1] = Halve(middle, light, to, high, halvings - 1, altitude));
        return [.. halves[0], .. halves[1]];
    }

    // The logarithm of the sky's light on a level surface at the ground with the light at a
    // height in degrees.
    private Vector3 GroundLog(float elevation, float altitude)
    {
        var r = float.DegreesToRadians(elevation);
        return Log(Illuminance(new Vector3(MathF.Cos(r), MathF.Sin(r), 0), altitude));
    }

    // The scattering table's height at a row, crowded near the ground as the transmittance's.
    private float ScatterHeight(int row)
    {
        var along = (float)row / (ScatterHeights - 1);
        return Air.Top * along * along;
    }

    // The transmittance table's height at a row, crowded near the ground.
    private float TransmittanceHeight(int row)
    {
        var along = (float)row / (TransmittanceHeights - 1);
        return Air.Top * along * along;
    }

    // Hillaire's light scattered twice or more at a height with the sun at a cosine with the
    // zenith, gathered over ways spread evenly over the sphere, each marched in even steps.
    private Vector3 Scattered(float altitude, float cosSunZenith)
    {
        var sunToward = new Vector3(MathF.Sqrt(MathF.Max(0, 1 - cosSunZenith * cosSunZenith)), cosSunZenith, 0);
        const float Isotropic = 1 / (4 * MathF.PI);
        var (second, sent) = (Vector3.Zero, Vector3.Zero);
        for (int i = 0; i < ScatterWays; i++)
            for (int j = 0; j < ScatterWays; j++)
            {
                var cos = 1 - 2 * (i + 0.5f) / ScatterWays;
                var sin = MathF.Sqrt(1 - cos * cos);
                var turn = 2 * MathF.PI * (j + 0.5f) / ScatterWays;
                var way = new Vector3(sin * MathF.Cos(turn), cos, sin * MathF.Sin(turn));
                var (toGround, toTop) = Air.Distances(altitude, cos);
                var step = (toGround ?? toTop) / ScatterSteps;
                var (light, onward, through) = (Vector3.Zero, Vector3.Zero, Vector3.One);
                for (int s = 0; s < ScatterSteps; s++)
                {
                    var (height, up) = At(altitude, way, (s + 0.5f) * step);
                    var scattering = Air.RayleighAt(height) + Air.AerosolsAt(height) * Air.AerosolAlbedo;
                    var extinction = Air.ExtinctionAt(height);
                    var span = Span(extinction, step);
                    light += through * scattering * TransmittanceAt(height, Vector3.Dot(up, sunToward)) * Isotropic * span;
                    onward += through * scattering * span;
                    through *= Exp(-extinction * step);
                }
                if (toGround is { } ground)
                {
                    var (_, up) = At(altitude, way, ground);
                    var sunZenith = Vector3.Dot(up, sunToward);
                    light += through * TransmittanceAt(0, sunZenith) * MathF.Max(sunZenith, 0) * Air.GroundAlbedo / MathF.PI;
                }
                // Each way's share of the sphere times the phase function alike everywhere, 1 / N.
                second += light / (ScatterWays * ScatterWays);
                sent += onward / (ScatterWays * ScatterWays);
            }
        return second / (Vector3.One - sent);
    }

    // The height and the zenith at a distance along a way from a height, the ground's curve turning
    // the zenith under it.
    private (float Height, Vector3 Up) At(float altitude, Vector3 way, float distance)
    {
        var point = new Vector3(0, Air.PlanetRadius + altitude, 0) + way * distance;
        var radius = point.Length();
        return (radius - Air.PlanetRadius, point / radius);
    }

    // A step of a march, the light the step's source adds over its length dimmed by the air within
    // it, as Hillaire integrates a step.
    private static (Vector3 Light, Vector3 Through) Step(Vector3 light, Vector3 through, Vector3 source, Vector3 extinction, float length) =>
        (light + through * source * Span(extinction, length), through * Exp(-extinction * length));

    // The integral of the transmittance over a step, (1 - e^(-σ l)) / σ, or the length where the
    // air there takes nearly nothing.
    private static Vector3 Span(Vector3 extinction, float length)
    {
        static float One(float sigma, float length) => sigma * length < 1e-5f ? length : (1 - MathF.Exp(-sigma * length)) / sigma;
        return new Vector3(One(extinction.X, length), One(extinction.Y, length), One(extinction.Z, length));
    }

    private static Vector3 Exp(Vector3 v) => new(MathF.Exp(v.X), MathF.Exp(v.Y), MathF.Exp(v.Z));

    // A way given, of unit length, or the caller's mistake named by its parameter, since a way of
    // no length or not a number normalizes to a NaN that lights a scene with nothing.
    private static Vector3 Way(Vector3 way, string name)
    {
        var length = way.Length();
        if (!float.IsFinite(length) || length < 1e-12f)
            throw new ArgumentException($"The way {way} has no length or is not a number; give one with a direction.", name);
        return way / length;
    }

    // The logarithm of a light, held above 1e-30 so a way the light never reaches stays finite.
    private static Vector3 Log(Vector3 v) =>
        new(MathF.Log(MathF.Max(v.X, 1e-30f)), MathF.Log(MathF.Max(v.Y, 1e-30f)), MathF.Log(MathF.Max(v.Z, 1e-30f)));

    // The molecules' phase function, 3 / (16 pi) (1 + cos^2).
    private static float RayleighPhase(float cos) => 3 / (16 * MathF.PI) * (1 + cos * cos);

    // Cornette and Shanks's phase function for the aerosols, Henyey and Greenstein's form with the
    // molecules' 1 + cos^2 in it, normalized again.
    private static float AerosolPhase(float g, float cos) =>
        3 / (8 * MathF.PI) * (1 - g * g) * (1 + cos * cos) / ((2 + g * g) * MathF.Pow(MathF.Max(1 + g * g - 2 * g * cos, 1e-6f), 1.5f));

    private static Vector3 Bilinear(Vector3[] table, int width, float row, float column)
    {
        var (r0, c0) = ((int)row, (int)column);
        var (r1, c1) = (Math.Min(r0 + 1, table.Length / width - 1), Math.Min(c0 + 1, width - 1));
        var (fr, fc) = (row - r0, column - c0);
        var top = Vector3.Lerp(table[r0 * width + c0], table[r0 * width + c1], fc);
        var bottom = Vector3.Lerp(table[r1 * width + c0], table[r1 * width + c1], fc);
        return Vector3.Lerp(top, bottom, fr);
    }
}
