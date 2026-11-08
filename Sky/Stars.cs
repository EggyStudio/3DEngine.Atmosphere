using System.Globalization;
using System.Numerics;

namespace Sky;

/// <summary>
/// The bright stars, to about magnitude 6.5, with their places and their light, which an engine
/// draws at its camera's exposure by <see cref="SkyClock.TowardSky"/>.
/// </summary>
/// <remarks>
/// <para>
/// The catalog is the Bright Star Catalogue's 9,096 stars with a place, named in the head of
/// <c>Data/BrightStars.csv</c>, which the library carries as a resource. A star's place is fixed
/// for the equinox of 2000, and the precession since, a third of a degree by 2026, is left out.
/// </para>
/// <para>
/// A star's illuminance follows the magnitude scale from <see cref="ZeroIlluminance"/>, five
/// magnitudes a hundredth of the light, its zero point the one <see cref="Night.SkyLuminance"/>
/// is given by, 108,000 candelas a square meter for a sky of magnitude 0 a square arcsecond, so a
/// star of magnitude 0 lights a surface facing it with 108,000 times a square arcsecond's solid
/// angle, 2.54e-6 lux. Its color is a black body's at the temperature Ballesteros's formula gives
/// its B-V index, 10,100 K for Vega's 0 and 5,800 K for the sun's 0.65.
/// </para>
/// </remarks>
public static class Stars
{
    /// <summary>The illuminance in lux of a star of magnitude 0 on a surface facing it, outside the air.</summary>
    public const float ZeroIlluminance = 2.538e-6f;

    /// <summary>The catalog's stars in its order, by right ascension, read once.</summary>
    public static IReadOnlyList<Star> Catalog => Read.Value;

    private static readonly Lazy<Star[]> Read = new(Load);

    /// <summary>The illuminance in lux of a star of <paramref name="magnitude"/> on a surface facing it, outside the air.</summary>
    public static float Illuminance(float magnitude) => ZeroIlluminance * MathF.Pow(10f, -0.4f * magnitude);

    /// <summary>
    /// The luminance in candelas a square meter of a star of <paramref name="magnitude"/> spread
    /// over <paramref name="solidAngle"/> steradians, as a pixel it falls in shows it, outside the air.
    /// </summary>
    public static float Luminance(float magnitude, float solidAngle) => Illuminance(magnitude) / solidAngle;

    /// <summary>The temperature in kelvin of a star of B-V color index <paramref name="colorIndex"/>, by Ballesteros's formula.</summary>
    public static float Temperature(float colorIndex) =>
        4_600f * ((1f / (0.92f * colorIndex + 1.7f)) + (1f / (0.92f * colorIndex + 0.62f)));

    /// <summary>The color in linear sRGB at a luminance of 1 of a star of B-V color index <paramref name="colorIndex"/>.</summary>
    public static Vector3 Color(float colorIndex) => Colorimetry.FromTemperature(Temperature(colorIndex));

    // The catalog's lines, its head's lines of comment left out, each a right ascension in hours,
    // a declination in degrees, a visual magnitude and a color index, which may be empty.
    private static Star[] Load()
    {
        using var stream = typeof(Stars).Assembly.GetManifestResourceStream("Sky.BrightStars.csv")
            ?? throw new InvalidOperationException("The library carries no Sky.BrightStars.csv.");
        using var reader = new StreamReader(stream);
        var stars = new List<Star>(9_100);
        while (reader.ReadLine() is { } line)
        {
            if (line.Length == 0 || line[0] == '#') continue;
            var parts = line.Split(',');
            float Number(int i) => float.Parse(parts[i], CultureInfo.InvariantCulture);
            stars.Add(new Star(Number(0) * MathF.PI / 12f, Number(1) * MathF.PI / 180f, Number(2), parts[3].Length > 0 ? Number(3) : 0.65f));
        }
        return [.. stars];
    }
}
