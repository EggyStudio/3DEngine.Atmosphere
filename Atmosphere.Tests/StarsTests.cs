using Xunit;

namespace Atmosphere.Tests;

public class StarsTests
{
    private static float Radians(float degrees) => degrees * MathF.PI / 180f;

    private static float Degrees(float radians) => radians * 180f / MathF.PI;

    [Fact]
    public void A_Star_Of_Magnitude_0_Gives_2_54e_6_Lux_And_One_Of_6_A_251st_Of_That()
    {
        Assert.Equal(2.538e-6f, Stars.Illuminance(0), 0.01e-6f);
        Assert.Equal(100f, Stars.Illuminance(0) / Stars.Illuminance(5), 0.01f);
        Assert.Equal(251.19f, Stars.Illuminance(0) / Stars.Illuminance(6), 0.05f);
    }

    [Fact]
    public void The_Magnitude_Scale_Gives_The_Moonless_Skys_Luminance_At_21_7_Magnitudes_A_Square_Arcsecond()
    {
        var arcsecond = MathF.Pow(MathF.PI / 648_000f, 2);
        Assert.Equal(Night.SkyLuminance, Stars.Luminance(21.7f, arcsecond), Night.SkyLuminance * 0.01f);
    }

    [Fact]
    public void The_Catalog_Holds_The_Bright_Stars_Sirius_The_Brightest_Where_The_Almanac_Puts_It()
    {
        var catalog = Stars.Catalog;
        Assert.Equal(9_096, catalog.Count);
        var sirius = catalog.MinBy(star => star.Magnitude);
        Assert.Equal(-1.46f, sirius.Magnitude);
        // The almanac's place for 2000, 6h 45m 8.9s and -16 degrees 42' 58".
        Assert.Equal(6f + 45f / 60f + 8.9f / 3_600f, Degrees(sirius.RightAscension) / 15f, 0.001f);
        Assert.Equal(-(16f + 42f / 60f + 58f / 3_600f), Degrees(sirius.Declination), 0.01f);
        Assert.Equal(0.00f, sirius.ColorIndex);
    }

    [Fact]
    public void Polaris_Stands_Within_A_Degree_Of_The_Pole_At_Every_Hour()
    {
        var polaris = Stars.Catalog.Where(star => Degrees(star.Declination) > 89f).MinBy(star => star.Magnitude);
        Assert.InRange(polaris.Magnitude, 1.9f, 2.1f);
        var clock = new SkyClock { Latitude = 45f };
        for (var hour = 0f; hour < 24f; hour += 3f)
        {
            clock.Hour = hour;
            var angle = Degrees(MathF.Acos(Math.Clamp(System.Numerics.Vector3.Dot(clock.TowardSky(polaris.RightAscension, polaris.Declination), clock.Pole), -1f, 1f)));
            Assert.InRange(angle, 0.6f, 0.9f);
        }
    }

    [Fact]
    public void Sirius_Stands_South_At_Midnight_On_The_First_Of_January_As_The_Almanac_Has_It()
    {
        // Sirius crosses the meridian at local midnight about the first of January, as high as 90
        // degrees less the latitude plus its declination, 28.3 at 45 north.
        var sirius = Stars.Catalog.MinBy(star => star.Magnitude);
        var clock = new SkyClock { Latitude = 45f, DayOfYear = 0f, Hour = 0f };
        var way = clock.TowardSky(sirius.RightAscension, sirius.Declination);
        Assert.Equal(28.28f, Degrees(MathF.Asin(way.Y)), 1f);
        Assert.True(way.Z > 0.8f, "south, along positive Z");
        Assert.InRange(way.X, -0.05f, 0.05f);
    }

    [Fact]
    public void A_Stars_Color_Follows_Its_Index_From_Blue_To_Red()
    {
        Assert.Equal(10_100f, Stars.Temperature(0f), 50f);
        Assert.Equal(5_780f, Stars.Temperature(0.65f), 50f);
        Assert.True(Stars.Color(-0.2f).Z > Stars.Color(1.5f).Z);
        Assert.True(Stars.Color(1.5f).X > Stars.Color(-0.2f).X);
    }

    [Fact]
    public void The_Milky_Way_Is_A_Thousand_S10_Toward_Its_Center_And_Next_To_Nothing_At_Its_Pole()
    {
        Assert.Equal(8.33e-7f, Night.S10, 0.02e-7f);
        var center = Night.GalaxyLuminance(Radians(266.405f), Radians(-28.936f));
        Assert.Equal(1_000f, center / Night.S10, 50f);
        var anticenter = Night.GalaxyLuminance(Radians(86.405f), Radians(28.936f));
        Assert.Equal(100f, anticenter / Night.S10, 10f);
        var pole = Night.GalaxyLuminance(Radians(192.859f), Radians(27.128f));
        Assert.True(pole / Night.S10 < 1f);
    }
}
