using System.Numerics;

namespace Sky;

/// <summary>
/// Colors in linear sRGB, its primaries and D65's white, and the color temperatures of light, to
/// and from a black body's.
/// </summary>
public static class Colorimetry
{
    /// <summary>
    /// The luminance of a color in linear sRGB, by BT.709's weights, so a color of luminance 1
    /// carries a hue alone.
    /// </summary>
    public static float Luminance(Vector3 rgb) => 0.2126f * rgb.X + 0.7152f * rgb.Y + 0.0722f * rgb.Z;

    /// <summary>
    /// The color of a black body at <paramref name="kelvin"/>, from 1,667 to 25,000, in linear sRGB
    /// at a luminance of 1.
    /// </summary>
    /// <remarks>
    /// The chromaticity is read off the Planckian locus by Kim and others' cubic fit in CIE 1931's
    /// xy, and a channel a red light's would take below 0 is held at 0.
    /// </remarks>
    public static Vector3 FromTemperature(float kelvin)
    {
        double t = Math.Clamp(kelvin, 1_667f, 25_000f);
        var x = t <= 4_000
            ? -0.2661239e9 / (t * t * t) - 0.2343589e6 / (t * t) + 0.8776956e3 / t + 0.179910
            : -3.0258469e9 / (t * t * t) + 2.1070379e6 / (t * t) + 0.2226347e3 / t + 0.240390;
        var y = t <= 2_222 ? -1.1063814 * x * x * x - 1.34811020 * x * x + 2.18555832 * x - 0.20219683
            : t <= 4_000 ? -0.9549476 * x * x * x - 1.37418593 * x * x + 2.09137015 * x - 0.16748867
            : 3.0817580 * x * x * x - 5.87338670 * x * x + 3.75112997 * x - 0.37001483;
        var rgb = Vector3.Max(FromXyz(new Vector3((float)(x / y), 1, (float)((1 - x - y) / y))), Vector3.Zero);
        return rgb / Luminance(rgb);
    }

    /// <summary>
    /// The correlated color temperature of a color in linear sRGB, the black body's whose color is
    /// nearest it in CIE 1960's uv, 0 for no light, <see cref="float.PositiveInfinity"/> for a
    /// light bluer than any black body and 1,000 K for one redder than a black body at 1,000 K.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The nearest point of the Planckian locus, which the CIE defines the correlated color
    /// temperature by, the locus made from Planck's law through the module's observer from 360 to
    /// 830 nm a mired apart, from 1,000 mireds, 1,000 K, to none, the end a black body nears as it
    /// grows infinitely hot, and read between two of its points along the segment that joins them.
    /// The CIE's D50 and D65 and its daylight at 15,000 and 25,000 K read within 0.5% of their
    /// published temperatures and its illuminant A within 1%, 2,829 K for 2,856, since the
    /// module's observer, a fit to the CIE's, stands apart from it in the red
    /// (<c>ColorimetryTests</c>).
    /// </para>
    /// <para>
    /// The nautical twilight's light as measured lies beyond the locus's end, a chromaticity no
    /// black body has, and the module's from some 12 degrees down, and either reads as infinite,
    /// so a figure of it is given by its x and y instead.
    /// </para>
    /// </remarks>
    public static float TemperatureOf(Vector3 rgb)
    {
        var xyz = ToXyz(rgb);
        var sum = xyz.X + xyz.Y + xyz.Z;
        if (sum <= 0) return 0;
        var (point, locus) = (Uv(xyz), Locus.Value);
        // Every fourth point, then the points about the nearest of those, since the distance falls
        // to one least along so smooth an arc.
        var nearest = Nearest(point, locus, 0, locus.Length - 1, 4);
        nearest = Nearest(point, locus, Math.Max(nearest - 4, 0), Math.Min(nearest + 4, locus.Length - 1), 1);
        // The point's foot on the segments either side of the nearest of the locus's points.
        var (mired, best) = ((float)nearest, Vector2.DistanceSquared(point, locus[nearest]));
        foreach (var other in new[] { nearest - 1, nearest + 1 })
        {
            if (other < 0 || other >= locus.Length) continue;
            var along = locus[other] - locus[nearest];
            var t = Math.Clamp(Vector2.Dot(point - locus[nearest], along) / along.LengthSquared(), 0, 1);
            var distance = Vector2.DistanceSquared(point, locus[nearest] + along * t);
            if (distance < best)
                (mired, best) = (nearest + (other - nearest) * t, distance);
        }
        return mired <= 0 ? float.PositiveInfinity : 1e6f / mired;
    }

    // The Planckian locus in CIE 1960's uv a mired apart, from an infinitely hot black body at
    // index 0 to one at 1,000 K at index 1,000, its spectrum's shape at infinity the limit of
    // Planck's law, the wavelength to the power of -4.
    private static readonly Lazy<Vector2[]> Locus = new(() =>
    {
        const double SecondRadiation = 1.438777e-2;
        var locus = new Vector2[1_001];
        for (var mired = 0; mired < locus.Length; mired++)
        {
            var xyz = Vector3.Zero;
            for (var nm = 360; nm <= 830; nm++)
            {
                var meters = nm * 1e-9;
                var power = mired == 0 ? 1 / Math.Pow(nm, 4)
                    : 1 / (Math.Pow(nm, 5) * double.ExpM1(SecondRadiation * mired / 1e6 / meters));
                xyz += Spectrum.Observe(nm) * (float)(power * 1e20);
            }
            locus[mired] = Uv(xyz);
        }
        return locus;
    });

    private static int Nearest(Vector2 point, Vector2[] locus, int from, int to, int step)
    {
        var (nearest, best) = (from, float.MaxValue);
        for (var i = from; i <= to; i += step)
        {
            var distance = Vector2.DistanceSquared(point, locus[i]);
            if (distance < best)
                (nearest, best) = (i, distance);
        }
        return nearest;
    }

    // CIE 1960's uv of a color in XYZ.
    private static Vector2 Uv(Vector3 xyz)
    {
        var d = xyz.X + 15 * xyz.Y + 3 * xyz.Z;
        return new Vector2(4 * xyz.X / d, 6 * xyz.Y / d);
    }

    // Linear sRGB to and from CIE 1931's XYZ, D65 white.
    internal static Vector3 ToXyz(Vector3 c) => new(
        0.4124564f * c.X + 0.3575761f * c.Y + 0.1804375f * c.Z,
        0.2126729f * c.X + 0.7151522f * c.Y + 0.0721750f * c.Z,
        0.0193339f * c.X + 0.1191920f * c.Y + 0.9503041f * c.Z);

    internal static Vector3 FromXyz(Vector3 c) => new(
        3.2404542f * c.X - 1.5371385f * c.Y - 0.4985314f * c.Z,
        -0.9692660f * c.X + 1.8760108f * c.Y + 0.0415560f * c.Z,
        0.0556434f * c.X - 0.2040259f * c.Y + 1.0572252f * c.Z);
}
