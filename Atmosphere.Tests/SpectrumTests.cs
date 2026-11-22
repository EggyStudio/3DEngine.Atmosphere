using System.Numerics;
using Xunit;

namespace Atmosphere.Tests;

/// <summary>
/// The light by wavelength (<see cref="Spectrum"/>), its bands, the sun's light and ozone's in
/// them, and its conversion through the CIE 1931 observer to linear sRGB.
/// </summary>
public class SpectrumTests
{
    // A black body's light at a temperature, by Planck's law, in watts a square meter a nanometer
    // for any scale, its color the point.
    private static float[] Planck(Spectrum spectrum, float kelvin) =>
        [.. spectrum.Centers.Select(nm => 1 / (MathF.Pow(nm * 1e-9f, 5) * (MathF.Exp(1.4388e-2f / (nm * 1e-9f * kelvin)) - 1)) * 1e-30f)];

    [Theory]
    [InlineData(12)]
    [InlineData(24)]
    public void Light_Alike_At_Every_Wavelength_Is_The_Equal_Energy_White(int count)
    {
        // An equal-energy light sits at x and y of a third, which the observer's fit holds within
        // a few thousandths.
        var spectrum = Spectrum.Of(count);
        var xyz = Colorimetry.ToXyz(spectrum.ToLinearSrgb(Enumerable.Repeat(1f, count).ToArray()));
        var sum = xyz.X + xyz.Y + xyz.Z;
        Assert.Equal(1f / 3, xyz.X / sum, 0.004f);
        Assert.Equal(1f / 3, xyz.Y / sum, 0.004f);
    }

    [Theory]
    [InlineData(4_000f)]
    [InlineData(5_772f)]
    [InlineData(6_500f)]
    public void A_Black_Bodys_Light_Reads_Back_As_Its_Temperature(float kelvin)
    {
        var spectrum = Spectrum.Of(24);
        Assert.Equal(kelvin, Colorimetry.TemperatureOf(spectrum.ToLinearSrgb(Planck(spectrum, kelvin))), kelvin * 0.015f);
    }

    [Fact]
    public void The_Suns_Light_Outside_The_Air_Is_The_Luminous_Solar_Constant()
    {
        // Darula, Kittler and Gueymard (2005) give the sun's illuminance outside the air at the
        // earth's mean distance as 133.8 klux.
        var spectrum = Spectrum.Of(24);
        Assert.Equal(133_800f, Colorimetry.Luminance(spectrum.ToLinearSrgb(spectrum.Sun)), 133_800f * 0.01f);
    }

    [Fact]
    public void Ozones_Absorption_Is_Greatest_In_The_Chappuis_Band_Near_600_Nanometers()
    {
        // Serdyuchenko and Gorshelev's cross-section at 233 K peaks near 600 nm at some 5.1e-21
        // square centimeters and is a hundredth of that at 400.
        var spectrum = Spectrum.Of(40);
        var peak = Array.IndexOf(spectrum.Ozone, spectrum.Ozone.Max());
        Assert.InRange(spectrum.Centers[peak], 585, 615);
        Assert.InRange(spectrum.Ozone.Max(), 4.6e-21f, 5.4e-21f);
        Assert.True(spectrum.Ozone[0] < spectrum.Ozone.Max() / 100);
    }
}
