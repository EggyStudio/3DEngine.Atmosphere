using Xunit;

namespace Sky.Tests;

public class SunTests
{
    [Fact]
    public void The_Sun_At_54_Degrees_Reaches_The_Ground_At_110000_Lux_And_5000_Kelvin()
    {
        // The reference scene, 27 June at 15:05 in Cologne, sets its sun 53.97 degrees high to
        // 97,050 lux at 5,633 K outside the air, which HDRP's Earth dims to 73,900 lux at 4,924 K
        // at the ground, the sun's color the same within a hundred kelvin and its lux three
        // quarters of this, since that sun outside the air is 120,000 lux times the sine of its
        // height.
        var sun = Sun.At(Atmosphere.Earth, 53.97f);
        Assert.InRange(sun.Illuminance, 108_900, 111_100);
        Assert.InRange(sun.Temperature, 4_976, 5_076);
        Assert.Equal(1, Colorimetry.Luminance(sun.Color), 0.001f);
    }

    [Fact]
    public void The_Sun_Dims_And_Warms_As_It_Sinks_And_Is_Out_Below_The_Horizon()
    {
        var (lux, kelvin) = (float.MaxValue, float.MaxValue);
        foreach (var elevation in new[] { 90f, 60, 30, 15, 5, 1 })
        {
            var sun = Sun.At(Atmosphere.Earth, elevation);
            Assert.True(sun.Illuminance < lux && sun.Temperature < kelvin, $"at {elevation} degrees, {sun.Illuminance} lux at {sun.Temperature} K");
            (lux, kelvin) = (sun.Illuminance, sun.Temperature);
        }
        Assert.Equal(0, Sun.At(Atmosphere.Earth, -2).Illuminance);
    }

    [Fact]
    public void Haze_Dims_The_Sun()
    {
        var clear = Sun.At(Atmosphere.Earth, 30).Illuminance;
        var hazy = Sun.At(Atmosphere.Earth.WithTurbidity(3), 30).Illuminance;
        Assert.InRange(hazy / clear, 0.5f, 0.8f);
    }

    [Fact]
    public void The_Suns_Disc_Is_Half_A_Degree_Wide_And_1_6e9_Candelas_A_Square_Meter_Bright_At_Noon()
    {
        // The sun's mean diameter seen from the earth is 32 minutes of arc, and its disc at the
        // ground under a high sun is about 1.6e9 candelas a square meter.
        Assert.Equal(0.533f, Sun.AngularDiameter, 0.001f);
        Assert.InRange(Sun.SolidAngle, 6.75e-5f, 6.85e-5f);
        var disc = Colorimetry.Luminance(Sun.DiscLuminance(Atmosphere.Earth, 53.97f));
        Assert.InRange(disc, 1.5e9f, 1.7e9f);
        Assert.Equal(Sun.At(Atmosphere.Earth, 53.97f).Illuminance / Sun.SolidAngle, disc, disc * 1e-4f);
    }

    [Fact]
    public void The_Share_Of_A_Disc_Above_The_Horizon_Goes_From_None_To_All_As_Its_Edge_Rises()
    {
        var radius = Sun.AngularDiameter / 2;
        Assert.Equal(0, Sun.DiscShare(-radius - 0.01f));
        Assert.Equal(0.5f, Sun.DiscShare(0), 1e-4f);
        Assert.Equal(1, Sun.DiscShare(radius + 0.01f));
        // A segment of a circle whose chord stands half the radius above the middle, a share of
        // 0.804.
        Assert.Equal(0.804f, Sun.DiscShare(radius / 2), 0.001f);
        Assert.Equal(0.5f, Moon.DiscShare(0), 1e-4f);
        Assert.Equal(0, Moon.DiscShare(-Moon.AngularDiameter / 2 - 0.01f));
        var share = 0f;
        for (var elevation = -radius; elevation <= radius; elevation += 0.01f)
        {
            Assert.True(Sun.DiscShare(elevation) >= share, $"at {elevation} degrees");
            share = Sun.DiscShare(elevation);
        }
    }

    [Fact]
    public void A_Sun_Of_Another_Strength_Comes_Through_The_Air_In_Proportion_With_The_Same_Color()
    {
        var sun = Sun.At(Atmosphere.Earth, 30);
        var half = Sun.Through(Atmosphere.Earth, Sun.Outside / 2, 30);
        Assert.Equal(sun.Illuminance / 2, half.Illuminance, sun.Illuminance * 1e-5f);
        Assert.Equal(sun.Temperature, half.Temperature, 1f);
    }
}
