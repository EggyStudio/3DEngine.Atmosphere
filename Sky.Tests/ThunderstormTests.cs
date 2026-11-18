using System.Numerics;
using Xunit;

namespace Sky.Tests;

/// <summary>
/// A storm's lightning (<see cref="Thunderstorm"/>), when and where it strikes, how its flash
/// flickers and when its thunder is heard.
/// </summary>
public class ThunderstormTests
{
    private static WeatherConditions Storm(float strikesAMinute) =>
        new() { Thunder = strikesAMinute, CloudBase = 800, Temperature = 20 };

    [Fact]
    public void Sound_Crosses_Air_At_343_Meters_A_Second_At_20_Degrees_And_331_At_Freezing()
    {
        // The speed of sound in dry air as an ideal gas, 331.3 times the square root of one plus
        // the temperature over 273.15 kelvin.
        Assert.Equal(343.2f, Thunderstorm.SpeedOfSound(20), 0.1f);
        Assert.Equal(331.3f, Thunderstorm.SpeedOfSound(0), 0.1f);
    }

    [Fact]
    public void A_Storm_Strikes_At_Its_Rate_With_Gaps_Spread_As_A_Poisson_Process_Spreads_Them()
    {
        // At 4 strikes a minute for 500 minutes, 2,000 on average with a spread of 45, and the
        // gaps between them spread exponentially, their spread as large as their mean of 15 s.
        var storm = new Thunderstorm(seed: 7);
        var times = new List<float>();
        for (var step = 0; step < 500 * 60 * 10; step++)
            foreach (var _ in storm.Advance(0.1f, Storm(4)))
                times.Add(step * 0.1f);
        Assert.InRange(times.Count, 2_000 - 135, 2_000 + 135);
        var gaps = times.Zip(times.Skip(1), (a, b) => b - a).ToList();
        var mean = gaps.Average();
        var spread = MathF.Sqrt((float)gaps.Average(g => (g - mean) * (g - mean)));
        Assert.InRange(mean, 14, 16);
        Assert.InRange(spread / mean, 0.9f, 1.1f);
    }

    [Fact]
    public void Strikes_Land_Evenly_Over_The_Ground_Between_The_Nearest_And_The_Farthest()
    {
        // Spread evenly over the ring's area, half land inside the circle that halves it.
        var storm = new Thunderstorm(seed: 3);
        var strikes = Enumerable.Range(0, 4_000).Select(_ => storm.Strike(Storm(1))).ToList();
        Assert.All(strikes, s => Assert.InRange(s.Distance, storm.Nearest, storm.Farthest));
        var halving = MathF.Sqrt((storm.Nearest * storm.Nearest + storm.Farthest * storm.Farthest) / 2);
        Assert.InRange(strikes.Count(s => s.Distance < halving) / 4_000f, 0.47f, 0.53f);
    }

    [Fact]
    public void A_Quarter_Of_The_Strikes_Reach_The_Ground_And_The_Rest_Flash_In_The_Cloud()
    {
        // About three quarters of flashes stay in the cloud (Rakov and Uman, Lightning, 2003), so
        // a quarter strike the ground there; one in the cloud flashes at its base.
        var storm = new Thunderstorm(seed: 11);
        var strikes = Enumerable.Range(0, 4_000).Select(_ => storm.Strike(Storm(1))).ToList();
        Assert.InRange(strikes.Count(s => s.ToGround) / 4_000f, 0.23f, 0.27f);
        Assert.All(strikes, s => Assert.Equal(s.ToGround ? 0 : 800, s.Position.Y));
    }

    [Fact]
    public void A_Strikes_Thunder_Is_Heard_Its_Distance_Over_The_Speed_Of_Sound_Later()
    {
        var strike = new Thunderstorm(seed: 5).Strike(Storm(1), new Vector3(0, 0, -3_432));
        Assert.Equal(3_432, strike.Distance, 0.01f);
        Assert.Equal(10, strike.ThunderDelay, 0.01f);
    }

    [Fact]
    public void A_Flash_Rises_At_Once_Flickers_And_Is_Over_In_Its_Duration()
    {
        Assert.Equal(0, Thunderstorm.Envelope(-0.01f, 1));
        Assert.Equal(0, Thunderstorm.Envelope(Thunderstorm.FlashDuration + 0.01f, 1));
        var samples = Enumerable.Range(0, 551).Select(i => Thunderstorm.Envelope(i / 1000f, 1)).ToList();
        Assert.All(samples, s => Assert.InRange(s, 0, 1));
        Assert.True(samples[22] > 0.4f, $"the flash is at {samples[22]} 22 ms in");
        // The return strokes brighten the flash again after it has begun to fall.
        var rises = samples.Zip(samples.Skip(1), (a, b) => b > a).Skip(30).Count(r => r);
        Assert.True(rises > 0, "the flash never brightens again");
        // And it dies away to none at its end rather than going out from a hundredth of its peak.
        foreach (var seed in new uint[] { 1, 2, 3 })
            Assert.True(Thunderstorm.Envelope(Thunderstorm.FlashDuration - 1e-4f, seed) < 1e-3f, $"seed {seed} ends at {Thunderstorm.Envelope(Thunderstorm.FlashDuration - 1e-4f, seed)}");
    }

    [Fact]
    public void A_Flash_Lights_A_Surface_Facing_It_By_The_Inverse_Square_Of_Its_Distance()
    {
        // The flash's lumens spread over the sphere about it, 4e9 at its peak unless set.
        var storm = new Thunderstorm();
        Assert.Equal(4e9f / (4 * MathF.PI * 1e6f), storm.Illuminance(1_000, 1), 1e-2f);
        Assert.Equal(storm.Illuminance(1_000, 1) / 4, storm.Illuminance(2_000, 1), 1e-3f);
        Assert.Equal(0, storm.Illuminance(1_000, 0));
    }

    [Fact]
    public void The_Same_Seed_Strikes_The_Same_Storm()
    {
        var (a, b) = (new Thunderstorm(seed: 9), new Thunderstorm(seed: 9));
        for (var i = 0; i < 2_000; i++)
            Assert.Equal(a.Advance(0.5f, Storm(6)).ToList(), b.Advance(0.5f, Storm(6)).ToList());
    }

    [Fact]
    public void A_Storm_Easing_In_Strikes_As_Its_Rate_Climbs()
    {
        // The storm spends a share of one strike at the rate it has each step, so a rate that
        // climbs from none strikes as it climbs rather than after a gap drawn at the first rate.
        var storm = new Thunderstorm(seed: 2);
        var first = -1f;
        for (var step = 0; step < 6_000 && first < 0; step++)
            if (storm.Advance(0.1f, Storm(MathF.Min(step * 0.1f / 60, 1) * 12)).Count > 0)
                first = step * 0.1f;
        Assert.InRange(first, 0, 120);
    }
}
