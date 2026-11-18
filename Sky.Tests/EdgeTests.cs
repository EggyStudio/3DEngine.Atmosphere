using System.Numerics;
using Xunit;

namespace Sky.Tests;

/// <summary>
/// The edges a binding meets first, each answered with a number or a plain exception naming what
/// was wrong, never a NaN from a mistake nor a crash from inside the library.
/// </summary>
public class EdgeTests
{
    private static readonly SkyLight Sky = new(Atmosphere.Earth);

    public static TheoryData<Vector3> Wayless => new() { Vector3.Zero, new Vector3(float.NaN, 1, 0), new Vector3(0, float.PositiveInfinity, 0) };

    [Theory]
    [MemberData(nameof(Wayless))]
    public void A_Way_Of_No_Length_Or_Not_A_Number_Is_Refused_By_Name(Vector3 way)
    {
        // A default vector is the usual slip, and normalizing it gives a NaN that would light a
        // scene with nothing, so the sky refuses it by the parameter's name.
        Assert.Equal("view", Assert.Throws<ArgumentException>(() => Sky.Luminance(way, Vector3.UnitY)).ParamName);
        Assert.Equal("sunToward", Assert.Throws<ArgumentException>(() => Sky.Luminance(Vector3.UnitY, way)).ParamName);
        Assert.Equal("sunToward", Assert.Throws<ArgumentException>(() => Sky.Illuminance(way)).ParamName);
        Assert.Equal("lightToward", Assert.Throws<ArgumentException>(() => Sky.GroundIlluminance(way)).ParamName);
    }

    [Theory]
    [InlineData(90.5f)]
    [InlineData(-91f)]
    [InlineData(float.NaN)]
    public void A_Latitude_Past_A_Pole_Is_Refused(float latitude)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SkyClock { Latitude = latitude });
    }

    [Theory]
    [InlineData(90f)]
    [InlineData(-90f)]
    public void A_Clock_At_A_Pole_Gives_Finite_Ways(float latitude)
    {
        foreach (var refracted in new[] { false, true })
        {
            var clock = new SkyClock { Latitude = latitude, DayOfYear = 172, Hour = 3, Refracted = refracted };
            foreach (var way in new[] { clock.SunDirection, clock.MoonDirection, clock.Pole, clock.TowardSky(1, 0.5f) })
            {
                Assert.True(float.IsFinite(way.X) && float.IsFinite(way.Y) && float.IsFinite(way.Z), $"at {latitude} a way is {way}");
                Assert.Equal(1, way.Length(), 1e-4f);
            }
            Assert.True(float.IsFinite(SceneLight.At(clock, Sky).LevelIlluminance));
        }
    }

    [Fact]
    public void A_Cloud_Cover_Not_A_Number_Is_Refused()
    {
        var clear = SceneLight.At(new SkyClock(), Sky);
        Assert.Throws<ArgumentOutOfRangeException>(() => clear.Under(new WeatherConditions { CloudCover = float.NaN }));
    }

    [Fact]
    public void A_Clock_Or_A_Weather_Stepped_Back_Gives_Numbers()
    {
        var clock = new SkyClock { DayOfYear = 0, Hour = 1 };
        clock.Advance(-30);
        Assert.Equal(363, clock.DayOfYear, 1e-3f);
        Assert.Equal(19, clock.Hour, 1e-3f);
        var weather = new WeatherState { Forecast = true, Clock = clock };
        weather.Advance(-30);
        Assert.True(float.IsFinite(weather.Wind.X) && float.IsFinite(weather.Current.CloudCover));
    }
}
