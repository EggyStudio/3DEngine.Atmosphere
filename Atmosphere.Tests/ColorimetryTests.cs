using System.Numerics;
using Xunit;

namespace Atmosphere.Tests;

public class ColorimetryTests
{
    [Theory]
    [InlineData(2_000, 25)]
    [InlineData(3_000, 25)]
    [InlineData(5_000, 25)]
    [InlineData(6_500, 25)]
    [InlineData(10_000, 25)]
    [InlineData(25_000, 50)]
    public void A_Black_Bodys_Color_Reads_Back_As_Its_Temperature(float kelvin, float within)
    {
        // The color is Kim and others' fit to the CIE's locus and the reading the module's own,
        // through the module's fit to the CIE's observer, the two within 9 K from 2,000 to 25,000
        // (dotnet test Atmosphere.Tests).
        var color = Colorimetry.FromTemperature(kelvin);
        Assert.Equal(1, Colorimetry.Luminance(color), 0.001f);
        Assert.Equal(kelvin, Colorimetry.TemperatureOf(color), within);
    }

    // The CIE's illuminants by their published chromaticities and correlated color temperatures,
    // A at 2,856 K, D50 at 5,003 and D65 at 6,504, each read within 5 K, and the CIE daylight
    // locus's formula at a nominal 15,000 and 25,000 K, 15,009 and 25,014 by today's second
    // radiation constant, within 0.5%, that locus lying off the Planckian one.
    [Theory]
    [InlineData(0.44757f, 0.40745f, 2_856f, 5f)]
    [InlineData(0.34567f, 0.35850f, 5_003f, 5f)]
    [InlineData(0.31271f, 0.32902f, 6_504f, 5f)]
    [InlineData(0.26140f, 0.27022f, 15_009f, 75f)]
    [InlineData(0.24985f, 0.25480f, 25_014f, 125f)]
    public void A_Chromaticity_Reads_As_The_CIEs_Correlated_Color_Temperature(float x, float y, float kelvin, float within)
    {
        Assert.Equal(kelvin, Colorimetry.TemperatureOf(Of(x, y)), within);
    }

    [Theory]
    [InlineData(0.217f, 0.220f)]
    [InlineData(0.228f, 0.232f)]
    public void A_Chromaticity_Past_The_Locuss_End_Reads_As_Infinite(float x, float y)
    {
        // A light bluer than any black body, as the nautical twilight's is, its nearest point of
        // the Planckian locus the end a black body nears as it grows infinitely hot.
        Assert.Equal(float.PositiveInfinity, Colorimetry.TemperatureOf(Of(x, y)));
    }

    [Fact]
    public void A_Light_Redder_Than_The_Locuss_Start_Reads_As_1000_Kelvin_And_Darkness_As_0()
    {
        Assert.Equal(1_000f, Colorimetry.TemperatureOf(Of(0.70f, 0.29f)));
        Assert.Equal(0f, Colorimetry.TemperatureOf(Vector3.Zero));
    }

    private static Vector3 Of(float x, float y) => Colorimetry.FromXyz(new Vector3(x / y, 1, (1 - x - y) / y));
}
