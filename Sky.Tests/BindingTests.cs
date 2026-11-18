using System.Numerics;
using Xunit;

namespace Sky.Tests;

/// <summary>
/// The README's smallest binding, run against a host that stands for an engine and records what
/// the binding gives it, so the build and the suite prove the guide, the block read against the
/// README by <see cref="ModuleTests"/>.
/// </summary>
public class BindingTests
{
    /// <summary>
    /// An engine's calls as the README names them, recording what each was given.
    /// </summary>
    private sealed class Host
    {
        public (Vector3 Direction, Vector3 Light) Directional { get; private set; }
        public Vector3 AmbientLuminance { get; private set; }
        public float Exposure { get; private set; } = float.NaN;
        public Vector3 FogExtinction { get; private set; }

        public void SetDirectionalLight(Vector3 direction, Vector3 light) => Directional = (direction, light);
        public void SetAmbientLuminance(Vector3 luminance) => AmbientLuminance = luminance;
        public void SetExposure(float ev100) => Exposure = ev100;
        public void SetFogExtinction(Vector3 extinction) => FogExtinction = extinction;

        public SkyTables? Tables { get; private set; }

        public void UploadSkyTables(SkyTables tables) => Tables = tables;
    }

    private static Host Bind(float seconds)
    {
        var host = new Host();
        // The README's block from here to the end marker, word for word.
        // Once, as the world loads and off the frame, the air, its sky and the sky's ground table.
        var sky = new SkyLight(Atmosphere.Earth);
        sky.PrepareGround(0, 2_000);
        host.UploadSkyTables(sky.Tables);
        var clock = new SkyClock { Latitude = 51, DayOfYear = 172, Hour = 15 };
        var weather = new WeatherState { Clock = clock, Forecast = true };

        // Each step of the world's clock, seconds long.
        clock.Advance(seconds / 3600);
        weather.Advance(seconds);
        var light = SceneLight.At(clock, sky).Under(weather.Current);
        var key = light.MoonIsKey ? light.Moon : light.Sun;
        host.SetDirectionalLight(-light.KeyToward, key.Color * key.Illuminance);
        host.SetAmbientLuminance(light.SkyIlluminance / MathF.PI);
        host.SetExposure(light.Exposure);
        host.SetFogExtinction(Haze.Extinction(sky.Air, weather.Current));
        // The end marker.
        return host;
    }

    [Fact]
    public void The_Readmes_Binding_Lights_A_Summer_Afternoon()
    {
        // 15:00 on 21 June at 51 degrees north, the sun 46 degrees high in the southwest, so its
        // light shines down and toward the northeast, and the scene is lit and exposed as a day.
        var host = Bind(1 / 60f);
        var (direction, light) = host.Directional;
        Assert.Equal(1, direction.Length(), 1e-4f);
        Assert.True(direction.Y < -0.5f && direction.X > 0 && direction.Z < 0, $"the light shines along {direction}");
        Assert.InRange(Colorimetry.Luminance(light), 1_000, 110_000);
        Assert.InRange(Colorimetry.Luminance(host.AmbientLuminance), 500, 12_000);
        Assert.InRange(host.Exposure, 12, 16);
        Assert.True(host.FogExtinction.Y > 0 && host.FogExtinction.Z > host.FogExtinction.X);
        Assert.Equal(4, host.Tables!.Lanes.Count);
    }
}
