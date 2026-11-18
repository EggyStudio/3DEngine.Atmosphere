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
        Assert.InRange(Colorimetry.Luminance(zenith), 3_000, 5_500);
        var horizon = Sky.Luminance(new Vector3(-1, 0.02f, 0), sun);
        // An ordinary clear day's haze makes the horizon 1.3 times as bright as the zenith.
        Assert.True(Colorimetry.Luminance(horizon) > 1.2f * Colorimetry.Luminance(zenith), $"horizon {horizon}, zenith {zenith}");
    }

    [Fact]
    public void The_Sky_Lights_A_Level_Surface_With_A_Quarter_Of_What_The_Sun_Does()
    {
        // 19,300 lux at 54 degrees against the sun's 73,700 on the level, an ordinary clear day's
        // haze scattering a share of the sun's light into the sky.
        var sun = Sun.At(Atmosphere.Earth, 54);
        var level = sun.Illuminance * MathF.Sin(float.DegreesToRadians(54));
        var sky = Colorimetry.Luminance(Sky.Illuminance(SunAt(54)));
        Assert.InRange(level, 73_000, 74_500);
        Assert.InRange(sky, 18_950, 19_700);
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
    public void The_Ground_Table_Holds_Within_A_Hundredth_Of_The_Sum_From_24_Degrees_Down_To_The_Zenith()
    {
        // Read between the table's rows at heights none of them stands at, from the deep
        // twilight to the zenith, each channel within 1% of itself.
        for (var elevation = -23.9f; elevation <= 90; elevation += 0.9f)
        {
            var (table, sum) = (Sky.GroundIlluminance(SunAt(elevation)), Sky.Illuminance(SunAt(elevation)));
            for (var c = 0; c < 3; c++)
                Assert.True(MathF.Abs(table[c] - sum[c]) < 0.01f * sum[c], $"at {elevation} degrees the table gives {table} for {sum}");
        }
    }

    [Fact]
    public void Below_24_Degrees_Down_The_Ground_Table_Gives_None_Where_The_Sum_Is_Under_A_Thousandth_Of_The_Night()
    {
        foreach (var elevation in new[] { -24.5f, -30, -60, -90 })
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

    [Fact]
    public void Below_The_Horizon_The_Ground_Shows_The_Suns_And_The_Skys_Light_It_Sends_Back()
    {
        // From the ground every way below the horizon meets the ground at once, which sends back
        // its albedo of the light on it alike every way, the sun's on a level surface and the
        // sky's, over pi, so at sunset, with no sun on it, the ground still shows the sky's light.
        foreach (var elevation in new[] { 54f, 10, 0, -3 })
        {
            var sun = SunAt(elevation);
            var direct = Sun.At(Atmosphere.Earth, elevation).Color * (Sun.At(Atmosphere.Earth, elevation).Illuminance * MathF.Max(sun.Y, 0));
            var expected = Atmosphere.Earth.GroundAlbedo * (direct + Sky.GroundIlluminance(sun)) / MathF.PI;
            foreach (var down in new[] { new Vector3(1, -0.2f, 0), new Vector3(0, -1, 0), new Vector3(-0.5f, -0.5f, 0.7f) })
            {
                var ground = Sky.Luminance(down, sun);
                for (var c = 0; c < 3; c++)
                    Assert.True(MathF.Abs(ground[c] - expected[c]) <= 0.01f * expected[c], $"at {elevation} degrees the ground along {down} shows {ground} for {expected}");
            }
        }
    }

    [Fact]
    public void The_Twilight_Zenith_Is_Blue_And_The_Ozone_Makes_It_Bluer()
    {
        // The twilight's zenith is blue, which Hulburt (1953) put down to the ozone's absorption
        // of the yellow and the green high in the air, and the light outdoors turns to shorter
        // waves as the sun sinks through the twilight (Spitschan and others, 2016). So from 4 to
        // 10 degrees down the zenith is bluer than it is red, and at 6, 8 and 10 bluer by a
        // quarter at least than in the same air with no ozone, the Earth's column making it 1.34
        // to 1.39 times as blue (dotnet test Sky.Tests).
        var bare = new SkyLight(Atmosphere.Earth with { OzoneAbsorption = Vector3.Zero });
        foreach (var elevation in new[] { -4f, -6, -8, -10 })
        {
            var zenith = Sky.Luminance(Vector3.UnitY, SunAt(elevation));
            Assert.True(zenith.Z > zenith.X, $"at {elevation} degrees the zenith is {zenith}");
            if (elevation > -5) continue;
            var without = bare.Luminance(Vector3.UnitY, SunAt(elevation));
            Assert.True(zenith.Z / zenith.X > 1.25f * (without.Z / without.X), $"at {elevation} degrees the ozone takes the zenith from {without} to {zenith}");
        }
    }

    [Fact]
    public void The_Days_Figures_Stand_At_The_Reference_Scenes_Height_Within_Half_A_Percent()
    {
        // The sun 54 degrees high at the ground, the sky's light on a level surface and its
        // luminance at the zenith, through the air carried to 111 km with the Earth's real column
        // of ozone and an ordinary clear day's haze, held within 0.5% (dotnet test Sky.Tests).
        var sun = Sun.At(Atmosphere.Earth, 54);
        Assert.Equal(91_138f, sun.Illuminance, 91_138f * 0.005f);
        Assert.Equal(5_025f, sun.Temperature, 5_025f * 0.005f);
        Assert.Equal(19_336f, Colorimetry.Luminance(Sky.Illuminance(SunAt(54))), 19_336f * 0.005f);
        Assert.Equal(4_316f, Colorimetry.Luminance(Sky.Luminance(Vector3.UnitY, SunAt(54))), 4_316f * 0.005f);
    }

    [Theory]
    [InlineData(1.1f)]
    [InlineData(2f)]
    [InlineData(3f)]
    [InlineData(4f)]
    [InlineData(6.95f)]
    public void The_Twilight_Darkens_Without_A_Bend_Under_Every_Haze(float turbidity)
    {
        // Under any haze from the clearest air to a city's, OPAC's urban type a turbidity of 6.95,
        // the sky's light on a level surface falls through the twilight with its logarithm's slope
        // moving by under 0.1 every quarter degree down to 16 degrees, so a table read too coarsely
        // for some haze shows here first.
        var sky = new SkyLight(Atmosphere.Earth.WithTurbidity(turbidity));
        float Lux(float elevation) => Colorimetry.Luminance(sky.Illuminance(SunAt(elevation)));
        var (before, last) = (float.NaN, Lux(0));
        for (var elevation = -0.25f; elevation >= -16; elevation -= 0.25f)
        {
            var lux = Lux(elevation);
            var slope = MathF.Log(lux / last);
            Assert.True(float.IsNaN(before) || MathF.Abs(slope - before) < 0.1f, $"at a turbidity of {turbidity}, {elevation} degrees down, the slope moves from {before} to {slope}");
            (before, last) = (slope, lux);
        }
    }
}
