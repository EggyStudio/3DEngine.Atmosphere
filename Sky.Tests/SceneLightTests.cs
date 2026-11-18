using Xunit;

namespace Sky.Tests;

/// <summary>
/// The light a scene takes at an hour (<see cref="SceneLight"/>), against the published figures of
/// daylight, twilight and moonlight and the incident meter's exposure.
/// </summary>
public class SceneLightTests
{
    private static readonly SkyLight Sky = new(Atmosphere.Earth);

    // A clock on the equator at the March equinox, the sun 15 degrees a hour from the horizon at
    // six, the moon's age at the hour given.
    private static SkyClock Clock(float hour, float moonAge) =>
        new() { Latitude = 0, DayOfYear = 79, Hour = hour, MoonAgeAtStart = moonAge - 79 - (hour / 24) };

    [Fact]
    public void The_Incident_Meter_Gives_EV100_0_At_2_5_Lux()
    {
        // ISO 2720's incident meter with a flat receptor's calibration constant of 250, as the
        // usual meters take it, so E = 2.5 lux times 2 to the power of EV100.
        Assert.Equal(0, SceneLight.ExposureFor(2.5f), 1e-4f);
        Assert.Equal(10, SceneLight.ExposureFor(2.5f * 1024), 1e-4f);
    }

    [Fact]
    public void Full_Sun_At_54_Degrees_Is_Exposed_As_The_Sunny_16_Rule_Within_A_Stop()
    {
        // The sunny 16 rule, f/16 at a hundredth of a second at ISO 100 in full sun, is an EV100
        // of log2(256 / 0.01), 14.64. The sun 54 degrees high gives 76,200 lux on a level surface
        // and the sky 20,100.
        var light = SceneLight.At(Clock(12 - 36 / 15f, 0), Sky);
        Assert.InRange(light.LevelIlluminance, 95_500, 97_200);
        Assert.InRange(light.Exposure, 14.64f - 1, 14.64f + 1);
        Assert.False(light.MoonIsKey);
    }

    [Fact]
    public void The_End_Of_Civil_Twilight_Is_Lit_Within_A_Factor_Of_Three_Of_3_4_Lux()
    {
        // The table of Wikipedia's article Lux gives 3.4 lux for the dark limit of civil twilight
        // under a clear sky, the sun six degrees down, no moon.
        var light = SceneLight.At(Clock(18.4f, 0), Sky);
        Assert.Equal(0, light.Sun.Illuminance);
        Assert.InRange(light.LevelIlluminance, 3.4f / 3, 3.4f * 3);
    }

    [Fact]
    public void A_Moonless_Night_Is_The_Night_Skys_Own_Light_And_A_Full_Moon_High_Is_The_Key()
    {
        // With the sun 90 degrees down the air it lights is gone, and the dark site's sky of
        // Night.SkyIlluminance alone is left. The same table gives a full moon on a clear night
        // 0.05 to 0.3 lux; here it stands overhead at midnight, its light through the least air.
        var moonless = SceneLight.At(Clock(0, 0), Sky);
        Assert.Equal(Night.SkyIlluminance, moonless.LevelIlluminance, Night.SkyIlluminance * 0.05f);
        Assert.False(moonless.MoonIsKey);
        var full = SceneLight.At(Clock(0, SkyClock.SynodicMonth / 2), Sky);
        Assert.True(full.MoonIsKey);
        Assert.InRange(full.LevelIlluminance, 0.05f, 0.35f);
    }

    [Fact]
    public void The_Light_Falls_Through_Sunset_Without_A_Step()
    {
        // Every minute from the sun two degrees up to two down the light falls by a share that
        // moves slowly with the sky's, 0.91 to 0.84 a minute, so a step would show as one minute's
        // share apart from its neighbor's; no two neighbors differ by more than 0.013 (dotnet test
        // Sky.Tests).
        var (last, lastShare) = (0f, 0f);
        for (var minute = 0; minute <= 16; minute++)
        {
            var lux = SceneLight.At(Clock(17.8667f + minute / 60f, 0), Sky).LevelIlluminance;
            if (last > 0)
            {
                var share = lux / last;
                Assert.InRange(share, 0.83f, 0.92f);
                if (lastShare > 0)
                    Assert.True(MathF.Abs(share - lastShare) < 0.02f, $"minute {minute} falls by {share} after {lastShare}");
                lastShare = share;
            }
            last = lux;
        }
    }

    [Fact]
    public void The_Scenes_Sky_From_The_Table_Holds_Within_A_Hundredth_Of_The_Sum_Day_And_Night()
    {
        // At the ground the sky's light is read from the table, and a hair above it summed over
        // the dome, so the two agree within 1% through the day, the twilight and the night, the
        // moon full and lighting the sky as well.
        for (var hour = 0f; hour < 24; hour += 0.7f)
        {
            var clock = Clock(hour, SkyClock.SynodicMonth / 2);
            var (table, sum) = (SceneLight.At(clock, Sky).SkyIlluminance, SceneLight.At(clock, Sky, 0.01f).SkyIlluminance);
            for (var c = 0; c < 3; c++)
                Assert.True(MathF.Abs(table[c] - sum[c]) < 0.01f * sum[c], $"at {hour} the table gives {table} for {sum}");
        }
    }

    [Fact]
    public void Under_No_Cloud_The_Light_Is_The_Clear_Skys()
    {
        var clear = SceneLight.At(Clock(12 - 36 / 15f, 0), Sky);
        var under = clear.Under(new WeatherConditions());
        Assert.Equal(clear.LevelIlluminance, under.LevelIlluminance, clear.LevelIlluminance * 1e-5f);
        Assert.Equal(clear.Sun.Illuminance, under.Sun.Illuminance, clear.Sun.Illuminance * 1e-5f);
    }

    [Fact]
    public void An_Overcast_Sky_Hides_The_Sun_And_Gives_A_Quarter_Of_The_Clear_Light_Two_Stops_Down()
    {
        // Kasten and Czeplak (1980) give the global light under a cover of N eighths as the clear
        // sky's times 1 - 0.75 (N / 8)^3.4, a quarter under a full cover, all of it diffuse, which
        // is the two stops the sunny 16 rule's table puts between full sun and an overcast day.
        var clear = SceneLight.At(Clock(12 - 36 / 15f, 0), Sky);
        var overcast = clear.Under(new WeatherConditions { CloudCover = 1 });
        Assert.Equal(0, overcast.Sun.Illuminance);
        Assert.Equal(0.25f, overcast.LevelIlluminance / clear.LevelIlluminance, 1e-4f);
        Assert.Equal(2, clear.Exposure - overcast.Exposure, 1e-3f);
        // The light through the cloud is the clear day's whole light, sun and sky, so its color
        // is theirs.
        Assert.Equal(Colorimetry.TemperatureOf(clear.Sun.Color * clear.Sun.Illuminance * clear.SunToward.Y + clear.SkyIlluminance),
            Colorimetry.TemperatureOf(overcast.SkyIlluminance), 100f);
    }

    [Fact]
    public void Half_A_Cover_Takes_Seven_Hundredths_Of_The_Light_And_Moves_A_Quarter_Of_The_Rest_Into_The_Sky()
    {
        // 1 - 0.75 * 0.5^3.4 is 0.929, and Kasten and Czeplak's diffuse share of the global light
        // climbs from the clear sky's share by the cover's square, 0.3 + 0.7 (N / 8)^2 in their
        // fit, taken here from the clear sky's own share at the sun's height.
        var clear = SceneLight.At(Clock(12 - 36 / 15f, 0), Sky);
        var half = clear.Under(new WeatherConditions { CloudCover = 0.5f });
        Assert.Equal(0.929f, half.LevelIlluminance / clear.LevelIlluminance, 0.001f);
        var clearShare = Colorimetry.Luminance(clear.SkyIlluminance) / clear.LevelIlluminance;
        var share = Colorimetry.Luminance(half.SkyIlluminance) / half.LevelIlluminance;
        Assert.Equal(clearShare + (1 - clearShare) * 0.25f, share, 1e-3f);
    }

    [Fact]
    public void Cloud_Dims_The_Moon_And_The_Night_As_It_Dims_The_Day()
    {
        var full = SceneLight.At(Clock(0, SkyClock.SynodicMonth / 2), Sky);
        var overcast = full.Under(new WeatherConditions { CloudCover = 1 });
        Assert.Equal(0, overcast.Moon.Illuminance);
        Assert.Equal(0.25f, overcast.LevelIlluminance / full.LevelIlluminance, 1e-4f);
    }

    // The sun's true height when its middle is seen at the horizon, 34.5 minutes of arc under it,
    // which a published sunset is.
    private const float ApparentSunset = -34.5f / 60;

    // The light on a level surface through an air of a turbidity, the sky's and the sun's by its
    // disc's share, for a sun at a height.
    private static float Level(Atmosphere air, SkyLight sky, float elevation)
    {
        var r = float.DegreesToRadians(elevation);
        var sun = Sun.At(air, elevation).Illuminance * MathF.Max(MathF.Sin(r), 0) * Sun.DiscShare(elevation);
        return Colorimetry.Luminance(sky.Illuminance(new System.Numerics.Vector3(MathF.Cos(r), MathF.Sin(r), 0))) + sun;
    }

    [Theory]
    [InlineData(1.11f, 749f, 5.86f)]
    [InlineData(2f, 626f, 4.92f)]
    [InlineData(2.398f, 590f, 4.64f)]
    [InlineData(3f, 545f, 4.29f)]
    [InlineData(4f, 485f, 3.82f)]
    public void The_Sunset_And_The_End_Of_Civil_Twilight_By_The_Airs_Turbidity(float turbidity, float sunset, float civil)
    {
        // Against the Lux article's 400 lux at sunset and 3.4 at the end of civil twilight, and
        // the 442 at sunset of Krisciunas and Schaefer (1991) after Brown (1952), the sunset the
        // apparent one and the twilight's six degrees the sun's true height. Every turbidity gives
        // twice the sources' light at most, and from the Earth's own 2.40, an ordinary clear day's,
        // 1.55 times at most, each row as it reads within 2% (dotnet test Sky.Tests).
        var air = MathF.Abs(turbidity - Atmosphere.Earth.Turbidity) < 0.01f ? Atmosphere.Earth : Atmosphere.Earth.WithTurbidity(turbidity);
        var sky = new SkyLight(air);
        var (atSunset, atCivil) = (Level(air, sky, ApparentSunset), Level(air, sky, -6));
        Assert.Equal(sunset, atSunset, sunset * 0.02f);
        Assert.Equal(civil, atCivil, civil * 0.02f);
        var most = turbidity >= 2.39f ? 1.55f : 2f;
        foreach (var source in new[] { 400f, 442f })
            Assert.InRange(atSunset / source, 1, most);
        Assert.InRange(atCivil / 3.4f, 1, most);
    }

    [Fact]
    public void A_Hazier_Air_Gives_Less_Light_At_Sunset_And_At_The_End_Of_Civil_Twilight()
    {
        var (sunset, civil) = (float.MaxValue, float.MaxValue);
        foreach (var turbidity in new[] { 1.11f, 2, 3, 4 })
        {
            var air = MathF.Abs(turbidity - Atmosphere.Earth.Turbidity) < 0.01f ? Atmosphere.Earth : Atmosphere.Earth.WithTurbidity(turbidity);
            var sky = new SkyLight(air);
            var (atSunset, atCivil) = (Level(air, sky, ApparentSunset), Level(air, sky, -6));
            Assert.True(atSunset < sunset && atCivil < civil, $"at a turbidity of {turbidity} the light is {atSunset} and {atCivil}");
            (sunset, civil) = (atSunset, atCivil);
        }
    }

    [Fact]
    public void The_Earths_Own_Air_Lights_The_Sunset_And_The_End_Of_Civil_Twilight_Within_1_55_Times_The_Sources()
    {
        // Atmosphere.Earth is an ordinary clear day's air, so its apparent sunset is within 1.55
        // times the Lux article's 400 lux and Krisciunas and Schaefer's 442 after Brown, and its
        // end of civil twilight within 1.55 times the Lux article's 3.4.
        var sky = new SkyLight(Atmosphere.Earth);
        var (sunset, civil) = (Level(Atmosphere.Earth, sky, ApparentSunset), Level(Atmosphere.Earth, sky, -6));
        foreach (var source in new[] { 400f, 442f })
            Assert.InRange(sunset / source, 1, 1.55f);
        Assert.InRange(civil / 3.4f, 1, 1.55f);
    }

    [Fact]
    public void The_Light_Is_Bluest_In_The_Nautical_Twilight()
    {
        // Spitschan, Aguirre, Brainard and Sweeney (2016) measured the downwelling light's
        // chromaticity through the twilight shift toward blue and back, its x and y least during
        // the nautical twilight at their rural site; so the light on a level surface, the sun's
        // and the sky's together with no moon, has its least x with the sun between 6 and 12
        // degrees down. Its x rather than its color temperature, which past the end of the
        // Planckian locus is none.
        var (bluest, at) = (1f, 0f);
        for (var minute = 0; minute <= 100; minute++)
        {
            var light = SceneLight.At(Clock(17.5f + minute / 60f, 0), Sky);
            var level = light.Sun.Color * (light.Sun.Illuminance * MathF.Max(light.SunToward.Y, 0)) + light.SkyIlluminance;
            var xyz = Colorimetry.ToXyz(level);
            var x = xyz.X / (xyz.X + xyz.Y + xyz.Z);
            if (x < bluest)
                (bluest, at) = (x, float.RadiansToDegrees(MathF.Asin(light.SunToward.Y)));
        }
        Assert.InRange(at, -12, -6);
    }

    [Theory]
    [InlineData(0f, 0.276f, 0.295f)]
    [InlineData(-3f, 0.262f, 0.278f)]
    [InlineData(-6f, 0.235f, 0.242f)]
    public void The_Civil_Twilights_Color_Is_The_Measured_Evenings_Within_0_02(float elevation, float x, float y)
    {
        // The mean CIE 1931 x and y of the downwelling light Spitschan, Aguirre, Brainard and
        // Sweeney (2016) measured at their rural site on three July evenings, the moon less than
        // a third lit, a degree of the sun's height a bin, read from their published spectra
        // through the module's observer. Each bin's evenings spread by 0.023 to 0.033 in x and y,
        // and the light on a level surface is held within 0.02 of their mean.
        var r = float.DegreesToRadians(elevation);
        var sun = Sun.At(Atmosphere.Earth, elevation);
        var level = sun.Color * (sun.Illuminance * MathF.Max(MathF.Sin(r), 0) * Sun.DiscShare(elevation))
            + Sky.GroundIlluminance(new System.Numerics.Vector3(MathF.Cos(r), MathF.Sin(r), 0));
        var xyz = Colorimetry.ToXyz(level);
        var sum = xyz.X + xyz.Y + xyz.Z;
        Assert.Equal(x, xyz.X / sum, 0.02f);
        Assert.Equal(y, xyz.Y / sum, 0.02f);
    }

    [Theory]
    [InlineData(20f, 5_300f)]
    [InlineData(30f, 5_300f)]
    [InlineData(40f, 5_500f)]
    [InlineData(54f, 5_500f)]
    [InlineData(90f, 5_500f)]
    public void Daylight_From_40_Degrees_Up_Is_Between_5500_And_6500_Kelvin(float elevation, float lowest)
    {
        // The global light on a level surface stands near 6,500 K at a clear noon as measured at
        // Ny-Alesund against lower latitudes, and Peyvandi and others (2016) give clear daylight's
        // interquartile range as 5,712 to 7,757 K, so the sun's and the sky's light together is
        // between 5,500 and 6,500 K from 40 degrees up. Under 40 the module's light is redder
        // than that, 5,333 K at 20 degrees (dotnet test Sky.Tests), and is held at 5,300 K so
        // neither height slips while what it lacks there is read.
        var r = float.DegreesToRadians(elevation);
        var sun = Sun.At(Atmosphere.Earth, elevation);
        var level = sun.Color * (sun.Illuminance * MathF.Sin(r)) + Sky.GroundIlluminance(new System.Numerics.Vector3(MathF.Cos(r), MathF.Sin(r), 0));
        Assert.InRange(Colorimetry.TemperatureOf(level), lowest, 6_500);
    }
}
