using System.Numerics;
using Xunit;

namespace Sky.Tests;

/// <summary>
/// The air between the eye and what it sees (<see cref="Haze"/>), clear and in fog, by
/// Koschmieder's law of visibility.
/// </summary>
public class HazeTests
{
    private static readonly Atmosphere Air = Atmosphere.Earth;

    [Fact]
    public void Clear_Air_Is_Seen_Through_For_166_Kilometers_And_Hazes_A_Thousandth_At_25_Meters()
    {
        // Koschmieder's visibility is 3.912 over the extinction at 550 nm, the distance at which
        // a black thing's contrast falls to 2%. Clear air at the ground takes 13.5e-6 a meter in
        // green by the molecules' scattering and 10e-6 by the aerosols' extinction, 2.35e-5 in
        // all.
        Assert.InRange(Haze.Visibility(Air), 165_000, 168_000);
        var near = Haze.Share(Air, 25);
        var far = Haze.Share(Air, 1_000);
        Assert.InRange(near.Y, 5.5e-4f, 6.3e-4f);
        Assert.InRange(far.Y, 0.022f, 0.025f);
        // The molecules take more of the blue, so the far air is bluer than it is red.
        Assert.True(far.Z > far.Y && far.Y > far.X, $"the haze at a kilometer is {far}");
    }

    [Fact]
    public void Thick_Fog_Is_Seen_Through_For_80_Meters_And_Hides_Seven_Tenths_At_25_Meters()
    {
        // WeatherConditions.Fog at 1 is some eighty meters seen, so 25 meters keeps
        // exp(-3.912 * 25 / 80) of a thing's light, 0.295, the same in every channel to within the
        // clear air's part.
        var fog = new WeatherConditions { Fog = 1 };
        Assert.Equal(80, Haze.Visibility(Air, fog), 0.5f);
        var near = Haze.Share(Air, 25, fog);
        Assert.Equal(0.705f, near.Y, 0.002f);
        Assert.True(MathF.Abs(near.Z - near.X) < 0.002f, $"the fog at 25 meters is {near}");
        Assert.True(Haze.Share(Air, 1_000, fog).Y > 0.9999f);
    }

    [Fact]
    public void Fog_Between_Thins_The_Visibility_Evenly_In_Its_Logarithm()
    {
        // Half way from the clear air's visibility to eighty meters by their logarithms, the
        // geometric mean of the two, as the fog's drawing eases them.
        var clear = Haze.Visibility(Air);
        var half = Haze.Visibility(Air, new WeatherConditions { Fog = 0.5f });
        Assert.Equal(MathF.Sqrt(clear * 80), half, half * 1e-3f);
        Assert.Equal(clear, Haze.Visibility(Air, new WeatherConditions()), clear * 1e-5f);
    }

    [Fact]
    public void What_Comes_Through_And_What_The_Air_Hides_Make_The_Whole()
    {
        var fog = new WeatherConditions { Fog = 0.3f };
        foreach (var distance in new[] { 0f, 25, 400, 5_000 })
            Assert.True(Vector3.Distance(Haze.Transmittance(Air, distance, fog) + Haze.Share(Air, distance, fog), Vector3.One) < 1e-6f);
        Assert.Equal(Vector3.One, Haze.Transmittance(Air, 0));
    }
}
