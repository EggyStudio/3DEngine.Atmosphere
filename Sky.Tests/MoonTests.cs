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
    public void The_Moons_Light_Outside_The_Air_Is_The_Suns_Times_The_Discs_Reflectance()
    {
        // Kieffer and Stone's ROLO model (2005) gives the lunar disc's reflectance in its bands, a
        // line in its logarithm from 0.09 at 400 nm to 0.15 at 700, so the moon's light in each
        // band is the sun's times that line, scaled to the moon's illuminance by its phase.
        var spectrum = Spectrum.Light;
        foreach (var phaseAngle in new[] { 0f, 60f })
        {
            var moon = Moon.Outside(phaseAngle);
            Assert.Equal(Moon.OutsideIlluminance(phaseAngle), Colorimetry.Luminance(spectrum.ToLinearSrgb(moon)), Moon.OutsideIlluminance(phaseAngle) * 1e-4f);
            var (red, blue) = (spectrum.Count - 2, 2);
            var expected = MathF.Exp(0.1634f * (spectrum.Centers[red] - spectrum.Centers[blue]) / 100);
            Assert.Equal(expected, moon[red] / spectrum.SunLight[red] / (moon[blue] / spectrum.SunLight[blue]), expected * 0.01f);
        }
    }

    [Theory]
    [InlineData(10f)]
    [InlineData(30f)]
    [InlineData(90f)]
    public void The_Moon_Dims_Through_The_Air_As_The_Sun_Does_And_Is_Redder(float elevation)
    {
        // The air takes the moon's light band by band as it takes the sun's, so its share differs
        // from the sun's by the moon's redder light alone, which the air lets through more of, 1.6%
        // more at 10 degrees and 0.3% overhead, and the moon stays the redder, 3,026 K against the
        // sun's 3,378 at 10 degrees and 4,406 against 5,166 overhead (dotnet test Sky.Tests).
        var moon = Moon.At(Atmosphere.Earth, elevation, 0);
        var sun = Sun.At(Atmosphere.Earth, elevation);
        var (moonShare, sunShare) = (moon.Illuminance / Moon.OutsideIlluminance(0), sun.Illuminance / Sun.OutsideIlluminance);
        Assert.InRange(moonShare / sunShare, 1, 1.03f);
        Assert.True(moon.Temperature < sun.Temperature - 300, $"at {elevation} degrees the moon is {moon.Temperature} K and the sun {sun.Temperature}");
    }

    [Fact]
    public void The_Moonlit_Sky_Is_The_Sunlit_Skys_Times_The_Moons_Light_Over_The_Suns_And_Redder()
    {
        // The sky's light lies in the blue bands, where the moon's light is the less against the
        // sun's, so the moonlit sky is 0.992 of the sunlit sky's by the moon's lux over the sun's
        // and 6,971 K where the sunlit is 9,020, the light 29 degrees high (dotnet test Sky.Tests).
        var sky = new SkyLight(Atmosphere.Earth);
        var toward = new System.Numerics.Vector3(MathF.Cos(0.5f), MathF.Sin(0.5f), 0);
        var (sunlit, moonlit) = (sky.GroundIlluminance(toward), sky.GroundIlluminanceBy(Moon.Outside(0), toward, 0));
        var share = Colorimetry.Luminance(moonlit) / Colorimetry.Luminance(sunlit) / (Moon.OutsideIlluminance(0) / Sun.OutsideIlluminance);
        Assert.InRange(share, 0.98f, 1);
        Assert.True(Colorimetry.TemperatureOf(moonlit) < Colorimetry.TemperatureOf(sunlit) - 1_000);
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
