using System.Numerics;
using Xunit;

namespace Sky.Tests;

public class AtmosphereTests
{
    private static readonly Atmosphere Earth = Atmosphere.Earth;

    private static Vector3 Depth(Vector3 transmittance) => new(-MathF.Log(transmittance.X), -MathF.Log(transmittance.Y), -MathF.Log(transmittance.Z));

    [Fact]
    public void The_Optical_Depth_Straight_Up_Is_Each_Layers_Coefficient_Times_Its_Depth()
    {
        // Each exponential layer holds its sea level's coefficient over its scale height, cut at
        // the top, and the ozone's layer its middle's over half its width, in each band of every
        // lane the air is read in.
        var spectrum = Spectrum.Light;
        for (var lane = 0; lane < spectrum.Lanes; lane++)
        {
            var air = Earth.Lane(spectrum, lane);
            var rayleigh = air.RayleighScattering * air.RayleighScaleHeight * (1 - MathF.Exp(-air.Top / air.RayleighScaleHeight));
            var aerosol = air.AerosolsAt(0) * air.AerosolScaleHeight * (1 - MathF.Exp(-air.Top / air.AerosolScaleHeight));
            var expected = rayleigh + aerosol + air.OzoneAbsorption * air.OzoneWidth / 2;
            var depth = Depth(air.Transmittance(0, 1));
            for (int i = 0; i < 3; i++)
                Assert.InRange(depth[i] / expected[i], 0.995f, 1.005f);
        }
    }

    [Fact]
    public void The_Sun_At_54_Degrees_Comes_Through_As_Chapmans_Function_Lets_It()
    {
        // Chapman's function for the two exponential layers and the ozone's sum gives 0.925, 0.849
        // and 0.710 from the ground with the sun 53.97 degrees high, for a thin air of the Earth's
        // molecules, a third of its column of ozone from 20 to 40 km and a haze of 1e-5 a meter at
        // the ground over 1,200 m, read as one lane of three bands with those coefficients.
        var sin = MathF.Sin(float.DegreesToRadians(53.97f));
        var thin = Earth with
        {
            OzoneAbsorption = new Vector3(0.65e-6f, 1.88e-6f, 0.08e-6f) * 0.6f, OzoneBottom = 20_000, OzoneWidth = 20_000,
            AerosolExtinction = 10e-6f, AerosolScaleHeight = 1_200, AerosolAlbedo = new Vector3(0.9f), AerosolAnisotropy = 0.8f,
            LaneAerosol = new Vector3(10e-6f),
        };
        var through = thin.Transmittance(0, sin);
        Assert.Equal(0.925f, through.X, 0.004f);
        Assert.Equal(0.849f, through.Y, 0.004f);
        Assert.Equal(0.710f, through.Z, 0.004f);
        // The Earth's own air, its column of 300 Dobson units and a clear day's haze, lets through
        // 0.878, 0.832 and 0.714 of the sun's light in each channel, taken by band, and an ordinary
        // day's 0.766, 0.697 and 0.561 (dotnet test Sky.Tests).
        var earth = Earth.Transmittance(0, sin);
        Assert.Equal(0.878f, earth.X, 0.002f);
        Assert.Equal(0.832f, earth.Y, 0.002f);
        Assert.Equal(0.714f, earth.Z, 0.002f);
        var ordinary = Atmosphere.ContinentalAverage.Transmittance(0, sin);
        Assert.Equal(0.766f, ordinary.X, 0.002f);
        Assert.Equal(0.697f, ordinary.Y, 0.002f);
        Assert.Equal(0.561f, ordinary.Z, 0.002f);
    }

    [Fact]
    public void A_Way_Into_The_Ground_Lets_Nothing_Through_And_One_Over_The_Horizon_From_Above_Does()
    {
        Assert.Equal(Vector3.Zero, Earth.Transmittance(0, -0.05f));
        // From 10 km the horizon lies 3.2 degrees below level, so a way 1 degree below leaves the
        // air.
        var below = Earth.Transmittance(10_000, MathF.Sin(float.DegreesToRadians(-1)));
        Assert.True(below.X > 0.1f, $"red through {below.X}");
    }

    [Fact]
    public void Turbidity_Sets_The_Aerosols_And_Reads_Back()
    {
        Assert.Equal(1.093f, Earth.Turbidity, 0.001f);
        Assert.Equal(2.392f, Atmosphere.ContinentalAverage.Turbidity, 0.001f);
        Assert.Equal(3, Earth.WithTurbidity(3).Turbidity, 0.0001f);
        Assert.Equal(1, Earth.WithTurbidity(0.5f).Turbidity, 0.0001f);
        Assert.Equal(0, Earth.WithTurbidity(1).AerosolExtinction);
    }
}
