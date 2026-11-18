using System.Globalization;
using System.Numerics;

namespace Sky;

/// <summary>
/// The visible light from 380 to 780 nm in bands of equal width, the sun's light outside the air
/// and ozone's absorption in each, and the conversion of a light by band to linear sRGB through the
/// CIE 1931 standard observer.
/// </summary>
/// <remarks>
/// <para>
/// A band holds the mean of the 5 nm rows of <c>Data/Spectra.csv</c> that fall in it, and the
/// observer's color matching functions integrated over it, so a light given as watts a square meter
/// a nanometer in each band turns into XYZ by the sum of each band's light times its integrals and
/// 683 lumens a watt, and into linear sRGB by XYZ's matrix, its luminance the light's lux.
/// </para>
/// <para>
/// The color matching functions are Wyman, Sloan and Shirley's analytic fit of 2013 to CIE 1931's
/// two-degree observer, each a sum of Gaussians with one width below its peak and another above.
/// </para>
/// </remarks>
internal sealed class Spectrum
{
    /// <summary>The shortest wavelength the bands cover, in nanometers.</summary>
    public const float Shortest = 380;

    /// <summary>The longest wavelength the bands cover, in nanometers.</summary>
    public const float Longest = 780;

    // Lumens a watt at 555 nm, by which the observer's Y is a light's lux.
    private const float Efficacy = 683;

    private static readonly Lazy<(float Nm, float Sun, float Ozone)[]> Rows = new(Read);
    private static readonly Dictionary<int, Spectrum> Made = [];

    private Spectrum(int count)
    {
        Count = count;
        Width = (Longest - Shortest) / count;
        Centers = new float[count];
        Sun = new float[count];
        Ozone = new float[count];
        Observer = new Vector3[count];
        for (var b = 0; b < count; b++)
        {
            var (from, to) = (Shortest + b * Width, Shortest + (b + 1) * Width);
            Centers[b] = (from + to) / 2;
            var rows = Rows.Value.Where(r => r.Nm >= from && r.Nm < to || b == count - 1 && r.Nm == to).ToList();
            Sun[b] = rows.Average(r => r.Sun);
            Ozone[b] = rows.Average(r => r.Ozone);
            // The observer integrated over the band a tenth of a nanometer at a time.
            var sum = Vector3.Zero;
            for (var nm = from + 0.05f; nm < to; nm += 0.1f)
                sum += Observe(nm) * 0.1f;
            Observer[b] = sum;
        }
    }

    /// <summary>How many bands the light is in.</summary>
    public int Count { get; }

    /// <summary>Each band's width in nanometers.</summary>
    public float Width { get; }

    /// <summary>Each band's middle wavelength in nanometers.</summary>
    public float[] Centers { get; }

    /// <summary>
    /// The sun's light outside the air in each band, watts a square meter a nanometer.
    /// </summary>
    public float[] Sun { get; }

    /// <summary>
    /// Ozone's absorption cross-section in each band, square centimeters a molecule.
    /// </summary>
    public float[] Ozone { get; }

    /// <summary>The observer's X, Y and Z integrated over each band, in nanometers.</summary>
    public Vector3[] Observer { get; }

    /// <summary>The light in <paramref name="count"/> bands, made once a count.</summary>
    public static Spectrum Of(int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        lock (Made)
        {
            if (!Made.TryGetValue(count, out var spectrum))
                Made[count] = spectrum = new Spectrum(count);
            return spectrum;
        }
    }

    /// <summary>
    /// A light given as watts a square meter a nanometer in each band, in linear sRGB whose
    /// luminance is its lux.
    /// </summary>
    public Vector3 ToLinearSrgb(ReadOnlySpan<float> light)
    {
        var xyz = Vector3.Zero;
        for (var b = 0; b < Count; b++)
            xyz += Observer[b] * light[b];
        return Colorimetry.FromXyz(xyz * Efficacy);
    }

    /// <summary>
    /// The CIE 1931 observer's color matching functions at <paramref name="nm"/>.
    /// </summary>
    public static Vector3 Observe(float nm) => new(
        1.056f * Lobe(nm, 599.8f, 37.9f, 31.0f) + 0.362f * Lobe(nm, 442.0f, 16.0f, 26.7f) - 0.065f * Lobe(nm, 501.1f, 20.4f, 26.2f),
        0.821f * Lobe(nm, 568.8f, 46.9f, 40.5f) + 0.286f * Lobe(nm, 530.9f, 16.3f, 31.1f),
        1.217f * Lobe(nm, 437.0f, 11.8f, 36.0f) + 0.681f * Lobe(nm, 459.0f, 26.0f, 13.8f));

    // A Gaussian of one width below its peak and another above.
    private static float Lobe(float x, float peak, float below, float above)
    {
        var t = (x - peak) / (x < peak ? below : above);
        return MathF.Exp(-0.5f * t * t);
    }

    private static (float Nm, float Sun, float Ozone)[] Read()
    {
        using var stream = typeof(Spectrum).Assembly.GetManifestResourceStream("Sky.Spectra.csv")
            ?? throw new InvalidOperationException("The library carries no Sky.Spectra.csv.");
        using var reader = new StreamReader(stream);
        var rows = new List<(float, float, float)>();
        while (reader.ReadLine() is { } line)
        {
            if (line.Length == 0 || line[0] == '#') continue;
            var parts = line.Split(',');
            rows.Add((float.Parse(parts[0], CultureInfo.InvariantCulture), float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture)));
        }
        return [.. rows];
    }
}
