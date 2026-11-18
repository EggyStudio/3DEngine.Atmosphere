using System.Numerics;
using Xunit;

namespace Sky.Tests;

/// <summary>
/// The sky drawn by the module alone, held to the numbers its reader gives.
/// </summary>
public class SkyPictureTests
{
    private static readonly SkyLight Sky = new(Atmosphere.Earth);

    [Fact]
    public void The_Fisheyes_Middle_Pixel_At_Noon_Is_The_Zeniths_Luminance_Exposed_And_Encoded()
    {
        // The middle of a fisheye looking up is the zenith, so its pixel is the reader's luminance
        // there over the luminance that saturates at the scene's EV100, encoded by sRGB's curve,
        // each channel within one count of 255.
        var clock = new SkyClock { Latitude = 50, DayOfYear = 172, Hour = 12 };
        var exposure = SceneLight.At(clock, Sky).Exposure;
        var picture = SkyPicture.Fisheye(Sky, clock, exposure, 33);
        var zenith = Sky.Luminance(Vector3.UnitY, clock.SunDirection) / SkyPicture.Saturation(exposure);
        var middle = picture.At(16, 16);
        for (var c = 0; c < 3; c++)
        {
            var linear = Math.Clamp(zenith[c], 0, 1);
            var expected = linear <= 0.0031308f ? 12.92f * linear : 1.055f * MathF.Pow(linear, 1 / 2.4f) - 0.055f;
            Assert.Equal(MathF.Round(expected * 255), middle[c], 1f);
        }
    }

    [Fact]
    public void The_Pictures_Are_Written_Where_SKY_PICTURES_Names_A_Folder()
    {
        // dotnet test Sky.Tests with SKY_PICTURES set to a folder writes the fisheye and the
        // panorama of each scene there as PNG, the sun's place ringed, with scenes.csv naming each
        // by its sun's height and its exposure, for a sheet to set them on.
        var folder = Environment.GetEnvironmentVariable("SKY_PICTURES");
        if (string.IsNullOrEmpty(folder)) return;
        Directory.CreateDirectory(folder);
        using var list = new StreamWriter(Path.Combine(folder, "scenes.csv"));
        list.WriteLine("name,sun,moon,exposure");
        foreach (var (name, clock) in SkyPicture.Scenes())
        {
            var exposure = SceneLight.At(clock, Sky).Exposure;
            SkyPicture.Fisheye(Sky, clock, exposure, 400, markSun: true).WritePng(Path.Combine(folder, $"{name}-fisheye.png"));
            SkyPicture.Panorama(Sky, clock, exposure, 1080, 330, markSun: true).WritePng(Path.Combine(folder, $"{name}-panorama.png"));
            list.WriteLine(FormattableString.Invariant($"{name},{clock.SunAltitude:F2},{clock.MoonAltitude:F2},{exposure:F2}"));
        }
    }
}
