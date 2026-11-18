using System.Numerics;
using Xunit;

namespace Sky.Tests;

/// <summary>
/// Where <see cref="SkyClock"/> puts the sun, the moon and the stars, against the spherical
/// astronomy it follows.
/// </summary>
public sealed class SkyClockTests
{
    [Fact]
    public void AtTheEquinoxNoonsSunStandsAsHighAsTheLatitudeLets()
    {
        var clock = new SkyClock { Latitude = 45f, DayOfYear = 79f, Hour = 12f };
        Assert.InRange(clock.SunAltitude, 44f, 46f);
        clock.Hour = 0f;
        Assert.InRange(clock.SunAltitude, -46f, -44f);
    }

    [Fact]
    public void WithNoTiltTheSunRisesInTheEastAtSixAndSetsAtEighteen()
    {
        var clock = new SkyClock { Latitude = 30f, AxialTilt = 0f, DayOfYear = 200f, Hour = 6f };
        Assert.InRange(clock.SunAltitude, -1f, 1f);
        Assert.True(clock.SunDirection.X > 0.9f, $"the rising sun stands along {clock.SunDirection}");
        clock.Hour = 18f;
        Assert.InRange(clock.SunAltitude, -1f, 1f);
        Assert.True(clock.SunDirection.X < -0.9f, $"the setting sun stands along {clock.SunDirection}");
    }

    [Fact]
    public void TheSolsticesNoonsStandAsHighAsTheTiltSays()
    {
        var summer = new SkyClock { Latitude = 45f, DayOfYear = 171f, Hour = 12f };
        var winter = new SkyClock { Latitude = 45f, DayOfYear = 354f, Hour = 12f };
        Assert.InRange(summer.SunAltitude, 67.4f, 69.4f);
        Assert.InRange(winter.SunAltitude, 20.6f, 22.6f);
    }

    [Fact]
    public void TheFullMoonStandsOppositeTheSunAndTheNewBesideIt()
    {
        var full = new SkyClock { DayOfYear = 79f, Hour = 0f, MoonAgeAtStart = (SkyClock.SynodicMonth / 2f) - 79f };
        Assert.True(Vector3.Dot(full.MoonDirection, full.SunDirection) < -0.95f, $"the full moon stands along {full.MoonDirection}, the sun along {full.SunDirection}");
        Assert.True(full.MoonLit > 0.99f && full.MoonAltitude > 0f, $"the full moon is lit {full.MoonLit} at {full.MoonAltitude} degrees at midnight");

        var fresh = new SkyClock { DayOfYear = 79f, Hour = 12f, MoonAgeAtStart = -79.5f };
        Assert.True(Vector3.Dot(fresh.MoonDirection, fresh.SunDirection) > 0.95f, $"the new moon stands along {fresh.MoonDirection}, the sun along {fresh.SunDirection}");
        Assert.True(fresh.MoonLit < 0.01f, $"the new moon is lit {fresh.MoonLit}");
    }

    [Fact]
    public void AdvancingPastMidnightTurnsTheDayOver()
    {
        var clock = new SkyClock { DayOfYear = 364.5f, Hour = 22f };
        clock.Advance(5f);
        Assert.InRange(clock.Hour, 2.99f, 3.01f);
        Assert.InRange(clock.DayOfYear, 0.49f, 0.51f);
        clock.Advance(-4f);
        Assert.InRange(clock.Hour, 22.99f, 23.01f);
        Assert.InRange(clock.DayOfYear, 364.49f, 364.51f);
    }

    [Fact]
    public void ThePoleStarStandsStillAsHighAsTheLatitude()
    {
        var clock = new SkyClock { Latitude = 52f };
        foreach (var hour in new[] { 0f, 7f, 15.5f })
        {
            clock.Hour = hour;
            var pole = clock.TowardSky(1.3f, MathF.PI / 2f);
            Assert.True(Vector3.Dot(pole, clock.Pole) > 0.9999f, $"at {hour} the pole star stands along {pole} where the pole is {clock.Pole}");
        }
        Assert.InRange(MathF.Asin(clock.Pole.Y) * 180f / MathF.PI, 51.9f, 52.1f);
    }

    [Fact]
    public void AStarComesBackAfterASiderealDay()
    {
        var clock = new SkyClock { Latitude = 40f, DayOfYear = 100f, Hour = 21f };
        var before = clock.TowardSky(2.1f, 0.4f);
        clock.Advance(23.9345f);
        var after = clock.TowardSky(2.1f, 0.4f);
        Assert.True(Vector3.Dot(before, after) > 0.99995f, $"the star stood along {before} and a sidereal day on along {after}");
    }

    [Fact]
    public void ThePartsOfTheDayFollowTheSun()
    {
        var clock = new SkyClock { Latitude = 45f, DayOfYear = 79f };
        (float Hour, DayPhase Phase)[] expected = [(0f, DayPhase.Night), (5.5f, DayPhase.Dawn), (12f, DayPhase.Day), (18.5f, DayPhase.Dusk), (22f, DayPhase.Night)];
        foreach (var (hour, phase) in expected)
        {
            clock.Hour = hour;
            Assert.Equal(phase, clock.Phase);
        }
    }

    [Fact]
    public void The_Moons_Phase_Angle_Is_180_Degrees_New_90_At_Each_Quarter_And_0_Full()
    {
        // The angle at the moon between the sun and the earth, 180 less the elongation, which
        // Moon.At takes, its lit share the clock's own MoonLit at every age.
        var clock = new SkyClock { DayOfYear = 0, Hour = 0 };
        foreach (var (age, angle) in new[] { (0f, 180f), (0.25f, 90f), (0.5f, 0f), (0.75f, 90f) })
        {
            clock.MoonAgeAtStart = age * SkyClock.SynodicMonth;
            Assert.Equal(angle, clock.MoonPhaseAngle, 0.01f);
        }
        for (var age = 0f; age < SkyClock.SynodicMonth; age += 0.7f)
        {
            clock.MoonAgeAtStart = age;
            Assert.Equal(clock.MoonLit, Moon.LitShare(clock.MoonPhaseAngle), 1e-4f);
        }
    }

    // Bennett's refraction (1982) in minutes of arc from the apparent height, as Meeus's
    // Astronomical Algorithms gives it, chapter 16, a check on Saemundsson's from the true.
    private static float Bennett(float apparent) =>
        1 / MathF.Tan(float.DegreesToRadians(apparent + 7.31f / (apparent + 4.4f)));

    [Fact]
    public void A_Sun_At_The_Horizon_Is_Lifted_34_Minutes_Of_Arc_By_The_Air()
    {
        // The sun's middle seen at the horizon stands truly some 34 minutes of arc below it,
        // which Saemundsson's formula from the true height gives as Bennett's from the apparent
        // does, both within a tenth of a minute from the horizon to the zenith.
        var horizon = -34.5f / 60;
        Assert.Equal(0, SkyClock.ApparentAltitude(horizon), 0.01f);
        for (var apparent = 0f; apparent <= 90; apparent += 1.5f)
        {
            var truly = apparent - Bennett(apparent) / 60;
            Assert.Equal(apparent, SkyClock.ApparentAltitude(truly), 0.1f / 60);
        }
        // A minute of arc at 45 degrees, none at the zenith, and none for a sun two degrees down
        // or lower, which no one sees.
        Assert.Equal(45 + 1f / 60, SkyClock.ApparentAltitude(45), 0.05f / 60);
        Assert.Equal(90, SkyClock.ApparentAltitude(90), 1e-4f);
        Assert.Equal(-2, SkyClock.ApparentAltitude(-2));
        Assert.Equal(-10, SkyClock.ApparentAltitude(-10));
    }

    [Fact]
    public void Cold_Dense_Air_Bends_More_And_Warm_Thin_Air_Less()
    {
        // Meeus's correction, the refraction times the pressure over 1,010 hectopascals and 283
        // kelvin over the air's temperature.
        var standard = SkyClock.ApparentAltitude(5) - 5;
        Assert.Equal(standard * 283 / 253f, SkyClock.ApparentAltitude(5, celsius: -20) - 5, 1e-5f);
        Assert.Equal(standard * 0.9f, SkyClock.ApparentAltitude(5, hectopascals: 909) - 5, 1e-4f);
    }

    [Fact]
    public void A_Refracted_Clock_Lifts_The_Sun_And_An_Unrefracted_One_Gives_Its_True_Height()
    {
        var truly = new SkyClock { Latitude = 0, DayOfYear = 79, Hour = 18f + 0.5746f / 15 };
        var seen = new SkyClock { Latitude = 0, DayOfYear = 79, Hour = 18f + 0.5746f / 15, Refracted = true };
        Assert.False(truly.Refracted);
        Assert.Equal(-0.5746f, truly.SunAltitude, 0.01f);
        Assert.Equal(0, seen.SunAltitude, 0.02f);
        // The lift is toward the zenith, the sun's bearing the same.
        var (a, b) = (truly.SunDirection, seen.SunDirection);
        Assert.Equal(MathF.Atan2(a.Z, a.X), MathF.Atan2(b.Z, b.X), 1e-4f);
        Assert.Equal(1, b.Length(), 1e-5f);
    }
}
