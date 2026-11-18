using System.Numerics;
using Xunit;

namespace Sky.Tests;

public class SkyLightTests
{
    private static readonly SkyLight Sky = new(Atmosphere.Earth);

    private static Vector3 SunAt(float elevation) =>
        new(MathF.Cos(float.DegreesToRadians(elevation)), MathF.Sin(float.DegreesToRadians(elevation)), 0);

    [Fact]
    public void The_Clear_Sky_Is_Blue_Overhead_And_Brighter_Toward_The_Horizon()
    {
        var sun = SunAt(54);
        var zenith = Sky.Luminance(Vector3.UnitY, sun);
        Assert.True(zenith.Z > zenith.Y && zenith.Y > zenith.X, $"zenith {zenith}");
        Assert.InRange(Colorimetry.Luminance(zenith), 1_000, 3_000);
        var horizon = Sky.Luminance(new Vector3(-1, 0.02f, 0), sun);
        Assert.True(Colorimetry.Luminance(horizon) > 2 * Colorimetry.Luminance(zenith), $"horizon {horizon}, zenith {zenith}");
    }

    [Fact]
    public void The_Sky_Lights_A_Level_Surface_With_A_Tenth_Of_What_The_Sun_Does()
    {
        // 7,860 lux at 54 degrees against the sun's 88,900 on the level, the air of HDRP's Earth
        // holding little haze.
        var sun = Sun.At(Atmosphere.Earth, 54);
        var level = sun.Illuminance * MathF.Sin(float.DegreesToRadians(54));
        var sky = Colorimetry.Luminance(Sky.Illuminance(SunAt(54)));
        Assert.InRange(level, 88_000, 89_800);
        Assert.InRange(sky, 7_700, 8_020);
    }

    [Fact]
    public void The_Sky_Darkens_Past_Sunset()
    {
        var day = Colorimetry.Luminance(Sky.Luminance(Vector3.UnitY, SunAt(54)));
        var dusk = Colorimetry.Luminance(Sky.Luminance(Vector3.UnitY, SunAt(-3)));
        var night = Colorimetry.Luminance(Sky.Luminance(Vector3.UnitY, SunAt(-18)));
        Assert.InRange(dusk / day, 0.005f, 0.05f);
        Assert.True(night < 0.01f * dusk, $"night {night}, dusk {dusk}");
    }

    [Fact]
    public void Haze_Brightens_The_Sky_As_It_Dims_The_Sun()
    {
        var hazy = new SkyLight(Atmosphere.Earth.WithTurbidity(3));
        var clear = Colorimetry.Luminance(Sky.Illuminance(SunAt(30)));
        Assert.True(Colorimetry.Luminance(hazy.Illuminance(SunAt(30))) > clear);
    }

    [Fact]
    public void The_Twilight_Darkens_Without_A_Bend_From_Sunset_To_16_Degrees_Down()
    {
        // The sky's light on a level surface falls by orders of magnitude through the twilight,
        // so its logarithm's slope changes slowly; every quarter degree down to 16 degrees under
        // the horizon the slope moves by under 0.1, where a table of the light scattered more
        // than once read between its rows as a straight line moved it by 0.51 at 9 degrees down
        // and lit 8 degrees down three times too brightly (dotnet test Sky.Tests).
        static float Lux(float elevation) => Colorimetry.Luminance(Sky.Illuminance(SunAt(elevation)));
        var (before, last) = (float.NaN, Lux(0));
        for (var elevation = -0.25f; elevation >= -16; elevation -= 0.25f)
        {
            var lux = Lux(elevation);
            var slope = MathF.Log(lux / last);
            Assert.True(float.IsNaN(before) || MathF.Abs(slope - before) < 0.1f, $"at {elevation} degrees the slope moves from {before} to {slope}");
            (before, last) = (slope, lux);
        }
    }

    [Fact]
    public void The_Ground_Table_Holds_Within_A_Hundredth_Of_The_Sum_From_20_Degrees_Down_To_The_Zenith()
    {
        // Read between the table's rows at heights none of them stands at, from the deep
        // twilight to the zenith, each channel within 1% of itself or of the sum's lux, whichever
        // is larger. A dim channel is held to the lux, since in the deep twilight the sum's blue, a
        // thousandth of the night sky's light, wavers from one tenth of a degree to the next, its
        // logarithm's slope moving from -0.139 to -0.068 a fortieth of a degree at 12.8 degrees
        // down, which no table read between its rows follows (dotnet test Sky.Tests).
        for (var elevation = -19.9f; elevation <= 90; elevation += 0.9f)
        {
            var (table, sum) = (Sky.GroundIlluminance(SunAt(elevation)), Sky.Illuminance(SunAt(elevation)));
            var lux = Colorimetry.Luminance(sum);
            for (var c = 0; c < 3; c++)
                Assert.True(MathF.Abs(table[c] - sum[c]) < 0.01f * MathF.Max(sum[c], lux), $"at {elevation} degrees the table gives {table} for {sum}");
        }
    }

    [Fact]
    public void Below_20_Degrees_Down_The_Ground_Table_Gives_None_Where_The_Sum_Is_Under_A_Thousandth_Of_The_Night()
    {
        foreach (var elevation in new[] { -20.5f, -30, -60, -90 })
        {
            Assert.Equal(Vector3.Zero, Sky.GroundIlluminance(SunAt(elevation)));
            Assert.True(Colorimetry.Luminance(Sky.Illuminance(SunAt(elevation))) < Night.SkyIlluminance * 1e-3f);
        }
    }

    [Fact]
    public void The_Sky_Lights_A_Level_Surface_Alike_Whatever_Way_The_Sun_Stands_Around()
    {
        // The light on a level surface depends on the sun's height alone, so a sun turned about
        // the zenith gives the same lux to 1e-5 of the larger of each channel and the lux, where
        // ways read at fixed turns moved it by 0.6% as the sun turned between them.
        foreach (var elevation in new[] { 54f, 10, -4 })
        {
            var r = float.DegreesToRadians(elevation);
            var at = Sky.Illuminance(SunAt(elevation));
            var lux = Colorimetry.Luminance(at);
            for (var turn = 0.1f; turn < MathF.Tau; turn += 0.7f)
            {
                var turned = Sky.Illuminance(new Vector3(MathF.Cos(r) * MathF.Cos(turn), MathF.Sin(r), MathF.Cos(r) * MathF.Sin(turn)));
                for (var c = 0; c < 3; c++)
                    Assert.True(MathF.Abs(turned[c] - at[c]) <= 1e-5f * MathF.Max(at[c], lux), $"at {elevation} degrees turned {turn} the sky gives {turned} for {at}");
            }
        }
    }
}
