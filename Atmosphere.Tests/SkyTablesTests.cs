using System.Numerics;
using Xunit;

namespace Atmosphere.Tests;

/// <summary>
/// The sky's tables as an engine uploads them, read by the README's section "Drawing the sky on a
/// GPU" alone and held to the sky's own luminance.
/// </summary>
public class SkyTablesTests
{
    [Theory]
    [InlineData(60f, 0f)]
    [InlineData(10f, 0f)]
    [InlineData(-3f, 0f)]
    [InlineData(-8f, 0f)]
    [InlineData(25f, 1_500f)]
    [InlineData(-2f, 8_000f)]
    public void A_Reader_Of_The_Tables_By_The_Readme_Gives_The_Skys_Luminance_Within_Half_A_Percent(float sunHeight, float altitude)
    {
        var sky = Skies.Earth;
        var r = float.DegreesToRadians(sunHeight);
        var sun = new Vector3(MathF.Cos(r), MathF.Sin(r), 0);
        foreach (var (height, turn) in new[] { (90f, 0f), (45f, 30f), (20f, 170f), (5f, 0f), (2f, 90f), (-10f, 200f), (-40f, 10f) })
        {
            var (e, t) = (float.DegreesToRadians(height), float.DegreesToRadians(turn));
            var view = new Vector3(MathF.Cos(e) * MathF.Cos(t), MathF.Sin(e), MathF.Cos(e) * MathF.Sin(t));
            var (expected, read) = (sky.Luminance(view, sun, altitude), Read(sky.Tables, view, sun, altitude));
            for (var c = 0; c < 3; c++)
                Assert.True(MathF.Abs(read[c] - expected[c]) <= 0.005f * MathF.Abs(expected[c]) + 1e-12f,
                    $"the sun {sunHeight} degrees up at {altitude} m, the way {height} up at {turn} around, reads {read} for {expected}");
        }
    }

    [Theory]
    [InlineData(60f, 0f)]
    [InlineData(15f, 60f)]
    [InlineData(-2f, 0f)]
    public void A_Reader_Of_The_Tables_By_The_Readme_Gives_The_Moonlit_Skys_Luminance_Within_Half_A_Percent(float moonHeight, float phaseAngle)
    {
        var sky = Skies.Earth;
        var r = float.DegreesToRadians(moonHeight);
        var moon = new Vector3(MathF.Cos(r), MathF.Sin(r), 0);
        foreach (var (height, turn) in new[] { (90f, 0f), (30f, 150f), (5f, 10f), (-10f, 200f) })
        {
            var (e, t) = (float.DegreesToRadians(height), float.DegreesToRadians(turn));
            var view = new Vector3(MathF.Cos(e) * MathF.Cos(t), MathF.Sin(e), MathF.Cos(e) * MathF.Sin(t));
            var expected = sky.LuminanceBy(Moon.Outside(phaseAngle), view, moon);
            var read = Read(sky.Tables, view, moon, 0, lane => lane.Moon(phaseAngle));
            for (var c = 0; c < 3; c++)
                Assert.True(MathF.Abs(read[c] - expected[c]) <= 0.005f * MathF.Abs(expected[c]) + 1e-15f,
                    $"the moon {moonHeight} degrees up at a phase angle of {phaseAngle}, the way {height} up, reads {read} for {expected}");
        }
    }

    // The README's march, step by step, from the public view alone, lit by the sun or, where a
    // light is given, by that light in each lane's bands in place of the sun's.
    private static Vector3 Read(SkyTables tables, Vector3 v, Vector3 s, float h, Func<SkyLaneTables, Vector3>? by = null)
    {
        var sum = Vector3.Zero;
        foreach (var lane in tables.Lanes)
        {
            var light = Lane(tables, lane, v, s, h, by?.Invoke(lane) ?? lane.Sun);
            var m = lane.ToLinearSrgb.Span;
            sum += new Vector3(m[0] * light.X + m[1] * light.Y + m[2] * light.Z, m[3] * light.X + m[4] * light.Y + m[5] * light.Z, m[6] * light.X + m[7] * light.Y + m[8] * light.Z);
        }
        return sum;
    }

    private static Vector3 Lane(SkyTables t, SkyLaneTables lane, Vector3 v, Vector3 s, float h, Vector3 outside)
    {
        double R = t.PlanetRadius, r = R + Math.Max(h, 0), m = v.Y;
        var inward = r * r * (m * m - 1) + R * R;
        double? ground = m < 0 && inward >= 0 ? -r * m - Math.Sqrt(inward) : null;
        var outward = r * r * (m * m - 1) + (R + t.Top) * (R + t.Top);
        var length = ground ?? (outward > 0 ? Math.Max(0, -r * m + Math.Sqrt(outward)) : 0);
        var c = Vector3.Dot(v, s);
        var g = t.AerosolAnisotropy;
        var rayleighPhase = 3 * (1 + c * c) / (16 * MathF.PI);
        var b = MathF.Max(1 + g * g - 2 * g * c, 1e-6f);
        var aerosolPhase = 3 * (1 - g * g) * (1 + c * c) / (8 * MathF.PI * (2 + g * g) * MathF.Pow(b, 1.5f));
        var (light, through) = (Vector3.Zero, Vector3.One);
        (float Altitude, Vector3 Up) At(double d)
        {
            var p = new Vector3(0, (float)(R + h), 0) + v * (float)d;
            var size = p.Length();
            return (size - (float)R, p / size);
        }
        for (var i = 0; i < t.ViewSteps; i++)
        {
            var (from32, to32) = ((float)i / t.ViewSteps, (float)(i + 1) / t.ViewSteps);
            var (near, far) = (from32 * from32 * (float)length, to32 * to32 * (float)length);
            var (y, up) = At((near + far) / 2);
            var u = Vector3.Dot(up, s);
            var yy = MathF.Max(y, 0);
            var rayleigh = lane.RayleighScattering * MathF.Exp(-yy / t.RayleighScaleHeight);
            var aerosol = lane.AerosolExtinction * MathF.Exp(-yy / t.AerosolScaleHeight);
            var ozone = lane.OzoneAbsorption * MathF.Max(0, 1 - MathF.Abs(2 * (y - t.OzoneBottom) / t.OzoneWidth - 1));
            var extinction = rayleigh + aerosol + ozone;
            var scattering = aerosol * lane.AerosolAlbedo;
            var source = (rayleigh * rayleighPhase + scattering * aerosolPhase) * Table(lane.Transmittance.Span, lane.TransmittanceHeights, lane.TransmittanceAngles, t.Top, y, u)
                + (rayleigh + scattering) * Exp(Table(lane.Scattered.Span, lane.ScatterHeights, lane.ScatterAngles, t.Top, y, u));
            var l = far - near;
            light += through * source * Span(extinction, l);
            through *= Exp(-extinction * l);
        }
        var sky = Vector3.Zero;
        if (ground is { } d)
        {
            var u = Vector3.Dot(At(d).Up, s);
            light += through * Table(lane.Transmittance.Span, lane.TransmittanceHeights, lane.TransmittanceAngles, t.Top, 0, u) * MathF.Max(u, 0) * lane.GroundAlbedo / MathF.PI;
            var degrees = float.RadiansToDegrees(MathF.Asin(Math.Clamp(u, -1, 1)));
            if (degrees >= t.LowestGround)
            {
                var (heights, logs) = lane.Ground(0);
                var (hs, ls) = (heights.ToArray(), logs.ToArray());
                var k = 0;
                while (k < hs.Length - 1 && hs[k + 1] < degrees) k++;
                Vector3 Log(int i) => new(ls[3 * i], ls[3 * i + 1], ls[3 * i + 2]);
                var above = Math.Min(k + 1, hs.Length - 1);
                var f = above == k || hs[above] == hs[k] ? 0 : (degrees - hs[k]) / (hs[above] - hs[k]);
                sky = through * Exp(Vector3.Lerp(Log(k), Log(above), Math.Clamp(f, 0, 1))) * lane.GroundAlbedo / MathF.PI;
            }
        }
        return light * outside + sky * outside / lane.Sun;
    }

    private static Vector3 Table(ReadOnlySpan<float> table, int rows, int columns, float top, float altitude, float cosine)
    {
        var row = MathF.Sqrt(Math.Clamp(altitude / top, 0, 1)) * (rows - 1);
        var column = (Math.Clamp(cosine, -1, 1) + 1) / 2 * (columns - 1);
        var (r0, c0) = ((int)row, (int)column);
        var (r1, c1) = (Math.Min(r0 + 1, rows - 1), Math.Min(c0 + 1, columns - 1));
        var (fr, fc) = (row - r0, column - c0);
        return Vector3.Lerp(Vector3.Lerp(Cell(table, columns, r0, c0), Cell(table, columns, r0, c1), fc), Vector3.Lerp(Cell(table, columns, r1, c0), Cell(table, columns, r1, c1), fc), fr);
    }

    private static Vector3 Cell(ReadOnlySpan<float> table, int columns, int r, int c) =>
        new(table[3 * (r * columns + c)], table[3 * (r * columns + c) + 1], table[3 * (r * columns + c) + 2]);

    private static Vector3 Span(Vector3 x, float l)
    {
        static float One(float x, float l) => x * l < 1e-5f ? l : (1 - MathF.Exp(-x * l)) / x;
        return new Vector3(One(x.X, l), One(x.Y, l), One(x.Z, l));
    }

    private static Vector3 Exp(Vector3 v) => new(MathF.Exp(v.X), MathF.Exp(v.Y), MathF.Exp(v.Z));
}
