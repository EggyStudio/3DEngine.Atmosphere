using System.Numerics;
using Xunit;

namespace Sky.Tests;

/// <summary>
/// The README's smallest binding, run against a host that stands for an engine and records what
/// the binding gives it, so the build and the suite prove the guide.
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
    }

    private static Host Bind(float seconds)
    {
        var host = new Host();
        // The README's block from here to the end marker, word for word.
        // Once, as the world loads and off the frame, the air, its sky and the sky's ground table.
        var sky = new SkyLight(Atmosphere.Earth);
        sky.GroundIlluminance(Vector3.UnitY);
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
    }

    [Fact]
    public void The_Readmes_Block_Stands_Word_For_Word_In_This_Host()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "Sky.slnx")))
            root = root.Parent;
        Assert.NotNull(root);
        var readme = File.ReadAllText(Path.Combine(root.FullName, "README.md")).ReplaceLineEndings("\n");
        var start = readme.IndexOf("## Binding it in your engine", StringComparison.Ordinal);
        Assert.True(start >= 0, "the README has no section Binding it in your engine");
        var open = readme.IndexOf("```csharp\n", start, StringComparison.Ordinal) + "```csharp\n".Length;
        var block = readme[open..readme.IndexOf("```\n", open, StringComparison.Ordinal)];
        var source = File.ReadAllText(Path.Combine(root.FullName, "Sky.Tests", "BindingTests.cs")).ReplaceLineEndings("\n");
        var lines = source.Split('\n');
        var from = Array.FindIndex(lines, l => l.Contains("// The README's block from here", StringComparison.Ordinal)) + 1;
        var to = Array.FindIndex(lines, l => l.Contains("// The end marker.", StringComparison.Ordinal));
        var hosted = string.Join("\n", lines[from..to].Select(l => l.Length >= 8 ? l[8..] : l.TrimStart())) + "\n";
        Assert.Equal(block, hosted);
    }
}
