using Xunit;

namespace Sky.Tests;

public class MoonTests
{
    [Fact]
    public void The_Full_Moon_Gives_A_Third_Of_A_Lux_And_The_Quarter_A_Tenth_Of_That()
    {
        var full = Moon.OutsideIlluminance(0);
        Assert.Equal(0.313f, full, 0.002f);
        Assert.InRange(Moon.OutsideIlluminance(90) / full, 0.07f, 0.12f);
        Assert.True(Moon.OutsideIlluminance(180) < 1e-3f);
    }

    [Fact]
    public void The_Moon_Dims_Through_The_Air_As_The_Sun_Does()
    {
        var moon = Moon.At(Atmosphere.Earth, 30, 0);
        var sun = Sun.At(Atmosphere.Earth, 30);
        Assert.Equal(sun.Illuminance / Sun.OutsideIlluminance, moon.Illuminance / Moon.OutsideIlluminance(0), 0.0001f);
        Assert.Equal(sun.Temperature, moon.Temperature, 1f);
    }

    [Fact]
    public void The_Full_Moons_Disc_Of_Half_A_Degree_Shows_4_900_Candelas_A_Square_Meter()
    {
        Assert.Equal(6.43e-5f, Moon.SolidAngle, 0.02e-5f);
        Assert.Equal(4_870f, Moon.LitLuminance(0), 50f);
        Assert.Equal(0.5f, Moon.LitShare(90), 1e-5f);
    }

    [Fact]
    public void The_Earth_Lights_The_New_Moons_Dark_Part_By_8e_5_Of_The_Full_Moons_Luminance_And_None_When_Full()
    {
        Assert.Equal(8e-5f, Moon.EarthshineLuminance(180) / Moon.LitLuminance(0), 1e-7f);
        Assert.Equal(4e-5f, Moon.EarthshineLuminance(90) / Moon.LitLuminance(0), 1e-7f);
        Assert.Equal(0f, Moon.EarthshineLuminance(0), 1e-9f);
    }

    [Fact]
    public void A_Moonless_Sky_Gives_Less_Than_A_Thousandth_Of_A_Lux() => Assert.InRange(Night.SkyIlluminance, 5e-4f, 1e-3f);
}
