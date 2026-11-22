using System.Numerics;
using Xunit;

namespace Atmosphere.Tests;

/// <summary>
/// The sky drawn by the module alone, held to the numbers its reader gives.
/// </summary>
public class SkyPictureTests
{
    private static readonly SkyLight Sky = Skies.Earth;

    // The EV100 each scene's plain picture is taken at, as a photographer would set it for the
    // time of day: the day's and the twilight's put the dome's mean luminance at 0.13 to 0.19 of
    // full, which sRGB's curve encodes near the middle of its range, and a night's at 0.02, so it
    // lands dark with the stars, the moon and the Milky Way standing out. The disc is let clip.
    // The incident meter's exposure is a stop or more over these in the day, which leaves the
    // Earth's clear air a dark blue, and as far under them at night, where it shows a gray.
    private static readonly Dictionary<string, float> Plain = new()
    {
        ["noon"] = 14.5f, ["morning"] = 14.5f, ["sunset"] = 10.5f, ["civil-end"] = 4.5f,
        ["nautical"] = 0f, ["moonlit"] = -1.5f, ["moonless"] = -6.5f,
    };

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
    public void A_Reflected_Reading_Of_The_Sky_Puts_Its_Mean_Luminance_At_The_Meters_Gray()
    {
        // ISO 2720's reflected meter, K 12.5, gives EV100 = log2(L * 100 / 12.5) for a mean
        // luminance L, so the sky's mean over the dome is exposed at 12.5 / 100 of 2 to the EV100,
        // a tenth of a pixel's full 1.2 times that, wherever the sun stands.
        foreach (var hour in new[] { 12f, 20.5f })
        {
            var clock = new SkyClock { Latitude = 50, DayOfYear = 172, Hour = hour };
            var mean = SkyPicture.MeanLuminance(Sky, clock);
            var exposure = SkyPicture.ReflectedExposure(Sky, clock);
            Assert.Equal(MathF.Log2(mean * 100 / 12.5f), exposure, 1e-3f);
            Assert.Equal(12.5f / 100 / 1.2f, mean / SkyPicture.Saturation(exposure), 1e-4f);
        }
    }

    [Fact]
    public void Each_Plain_Pictures_Exposure_Puts_The_Domes_Mean_Where_Its_Time_Of_Day_Wants_It()
    {
        // The mean over the dome as a share of the full a pixel takes, 0.13 to 0.19 by day and in
        // the twilights, 0.02 by night, where the stars and the Milky Way are to show.
        foreach (var (name, clock) in SkyPicture.Scenes())
        {
            var share = SkyPicture.MeanLuminance(Sky, clock) / SkyPicture.Saturation(Plain[name]);
            if (name.StartsWith("moon"))
                Assert.InRange(share, 0.015f, 0.03f);
            else
                Assert.InRange(share, 0.12f, 0.2f);
        }
    }

    [Fact]
    public void The_Pictures_Are_Written_Where_ATMOSPHERE_PICTURES_Names_A_Folder()
    {
        // dotnet test Atmosphere.Tests with ATMOSPHERE_PICTURES set to a folder writes the fisheye
        // and the panorama of each scene there as PNG, the sun's place ringed, each at the scene's
        // metered exposure and again at a reflected reading of the sky, with scenes.csv naming each
        // by its sun's height and the three exposures, for a sheet to set them on, and the two
        // again at the exposure of the plain table with no ring, the plain pictures of the sky
        // itself.
        var folder = Environment.GetEnvironmentVariable("ATMOSPHERE_PICTURES");
        if (string.IsNullOrEmpty(folder)) return;
        Directory.CreateDirectory(folder);
        using var list = new StreamWriter(Path.Combine(folder, "scenes.csv"));
        list.WriteLine("name,sun,moon,exposure,reflected,plain");
        foreach (var (name, clock) in SkyPicture.Scenes())
        {
            var exposure = SceneLight.At(clock, Sky).Exposure;
            var reflected = SkyPicture.ReflectedExposure(Sky, clock);
            foreach (var (tag, ev) in new[] { ("", exposure), ("-reflected", reflected) })
            {
                SkyPicture.Fisheye(Sky, clock, ev, 400, markSun: true).WritePng(Path.Combine(folder, $"{name}-fisheye{tag}.png"));
                SkyPicture.Panorama(Sky, clock, ev, 1080, 330, markSun: true).WritePng(Path.Combine(folder, $"{name}-panorama{tag}.png"));
            }
            SkyPicture.Fisheye(Sky, clock, Plain[name], 400).WritePng(Path.Combine(folder, $"{name}-fisheye-plain.png"));
            SkyPicture.Panorama(Sky, clock, Plain[name], 1080, 330).WritePng(Path.Combine(folder, $"{name}-panorama-plain.png"));
            list.WriteLine(FormattableString.Invariant($"{name},{clock.SunAltitude:F2},{clock.MoonAltitude:F2},{exposure:F2},{reflected:F2},{Plain[name]:F1}"));
        }
    }
}
