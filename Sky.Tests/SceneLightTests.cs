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
        // of log2(256 / 0.01), 14.64. The sun 54 degrees high gives 88,900 lux on a level surface
        // and the sky 7,860.
        var light = SceneLight.At(Clock(12 - 36 / 15f, 0), Sky);
        Assert.InRange(light.LevelIlluminance, 95_000, 98_500);
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
        // moves slowly with the sky's, 0.91 to 0.85 a minute, and by 0.900 on either side of the
        // disc's going under, so a step would show as one minute's share apart from its
        // neighbor's; no two neighbors differ by more than 0.01 (dotnet test Sky.Tests).
        var (last, lastShare) = (0f, 0f);
        for (var minute = 0; minute <= 16; minute++)
        {
            var lux = SceneLight.At(Clock(17.8667f + minute / 60f, 0), Sky).LevelIlluminance;
            if (last > 0)
            {
                var share = lux / last;
                Assert.InRange(share, 0.84f, 0.92f);
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
            var lux = Colorimetry.Luminance(sum);
            for (var c = 0; c < 3; c++)
                Assert.True(MathF.Abs(table[c] - sum[c]) < 0.01f * MathF.Max(sum[c], lux), $"at {hour} the table gives {table} for {sum}");
        }
    }
}
