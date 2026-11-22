using Xunit;

namespace Atmosphere.Tests;

/// <summary>
/// The weather's conditions as numbers a stranger's engine can read without another engine's
/// renderer in mind.
/// </summary>
public class WeatherConditionsTests
{
    // Each preset's cloud density as it stood, a share of a deck whose densest cloud takes 0.04 of
    // the light a meter, the extinction a cloud's drawing was tuned at, which a stratocumulus
    // deck's 0.02 to 0.05 a meter brackets.
    public static TheoryData<WeatherPreset, float> Densities => new()
    {
        { WeatherPreset.Clear, 1f }, { WeatherPreset.FewClouds, 1f }, { WeatherPreset.PartlyCloudy, 1f },
        { WeatherPreset.Overcast, 1.3f }, { WeatherPreset.Hazy, 0.25f }, { WeatherPreset.Drizzle, 1.3f },
        { WeatherPreset.Rain, 1.6f }, { WeatherPreset.Storm, 2f }, { WeatherPreset.Thunderstorm, 2.2f },
        { WeatherPreset.LightSnow, 1f }, { WeatherPreset.Snow, 1.4f }, { WeatherPreset.Blizzard, 1.8f },
        { WeatherPreset.Fog, 0.5f }, { WeatherPreset.MistyMorning, 1f }, { WeatherPreset.Sandstorm, 1f },
    };

    [Theory]
    [MemberData(nameof(Densities))]
    public void A_Presets_Deck_Has_The_Optical_Depth_Its_Density_Gave_It(WeatherPreset preset, float density)
    {
        // The optical depth straight through the deck at its densest, the extinction a meter
        // times the deck's thickness, so an engine that draws by an extinction a meter divides
        // it by the thickness and finds the density it drew at.
        var weather = WeatherConditions.Of(preset);
        var depth = 0.04f * density * (weather.CloudTop - weather.CloudBase);
        Assert.Equal(depth, weather.CloudOpticalDepth, depth * 1e-5f);
    }

    [Fact]
    public void Conditions_Unset_Hold_A_Deck_Of_Optical_Depth_100()
    {
        var weather = new WeatherConditions();
        Assert.Equal(100f, weather.CloudOpticalDepth, 1e-3f);
        Assert.Equal(0.04f, weather.CloudOpticalDepth / (weather.CloudTop - weather.CloudBase), 1e-6f);
    }
}
