using System.Numerics;

namespace Atmosphere;

/// <summary>
/// The moon's light outside the air by its phase and where it reaches the ground through it.
/// </summary>
/// <remarks>
/// The moon's illuminance outside the air follows Krisciunas and Schaefer's fit of the moon's
/// brightness by its phase angle, the angle at the moon between the sun and the eye, 0 when full
/// and 180 when new, which falls faster than the lit share of the disc since the moon's rough
/// surface throws shadows that the full moon alone shows none of. Its light is the sun's reflected
/// wavelength by wavelength by the lunar disc's reflectance in Kieffer and Stone's ROLO model
/// (2005), which rises from 0.09 at 400 nm to 0.15 at 700, so the moon is redder than the sun. The
/// further reddening ROLO finds away from the full moon is left out.
/// </remarks>
public static class Moon
{
    /// <summary>
    /// How wide the moon's disc looks from the earth, in degrees, its mean of 0.518.
    /// </summary>
    public const float AngularDiameter = 0.518f;

    /// <summary>The solid angle of the moon's disc, in steradians, 6.4e-5.</summary>
    public static float SolidAngle => MathF.PI * MathF.Pow(AngularDiameter * 0.5f * MathF.PI / 180f, 2);

    /// <summary>
    /// The share of the moon's disc above the horizon with its middle <paramref name="elevation"/>
    /// degrees over it, from none once its upper edge is down to all once its lower edge is up.
    /// </summary>
    public static float DiscShare(float elevation) => Sun.ShareAbove(elevation, AngularDiameter);

    /// <summary>
    /// The share of the moon's disc the sun lights as seen from the earth, at a phase angle of
    /// <paramref name="phaseAngle"/> degrees, all of it when full and none when new.
    /// </summary>
    public static float LitShare(float phaseAngle) => (1f + MathF.Cos(phaseAngle * MathF.PI / 180f)) / 2f;

    /// <summary>
    /// The luminance in candelas a square meter of the moon's sunlit part outside the air, at a
    /// phase angle of <paramref name="phaseAngle"/> degrees, its illuminance spread over the lit
    /// share of its disc, 4,900 when full.
    /// </summary>
    public static float LitLuminance(float phaseAngle) => OutsideIlluminance(phaseAngle) / (SolidAngle * MathF.Max(LitShare(phaseAngle), 1e-3f));

    /// <summary>
    /// The luminance in candelas a square meter of the moon's dark part outside the air, which the
    /// earth's sunlit side lights, at the moon's phase angle of <paramref name="phaseAngle"/>
    /// degrees, the most at a new moon, when the moon sees the earth full, and none when full.
    /// </summary>
    /// <remarks>
    /// A full earth seen from the moon is a disc of 8.6e-4 steradians sending back some 0.3 of the
    /// sunlight it takes, a luminance near 12,800 candelas a square meter against the 133,800 lux
    /// of the sun, so it lights the moon with some 11 lux, 8e-5 of the sun's light there, and the
    /// moon's dark part shows that share of the full moon's luminance, falling as the earth's lit
    /// share, one less the moon's, falls.
    /// </remarks>
    public static float EarthshineLuminance(float phaseAngle) => EarthshineShare * LitLuminance(0) * (1f - LitShare(phaseAngle));

    /// <summary>
    /// The share of the full moon's luminance the moon's dark part shows by a full earth's light.
    /// </summary>
    public const float EarthshineShare = 8e-5f;

    /// <summary>
    /// The moon's illuminance outside the air in lux on a surface facing it, at a phase angle of
    /// <paramref name="phaseAngle"/> degrees, about 0.31 when full.
    /// </summary>
    public static float OutsideIlluminance(float phaseAngle)
    {
        // Their fit gives foot-candles, 10.764 lux each.
        var angle = Math.Clamp(MathF.Abs(phaseAngle), 0, 180);
        return 10.764f * MathF.Pow(10, -0.4f * (3.84f + 0.026f * angle + 4e-9f * MathF.Pow(angle, 4)));
    }

    /// <summary>
    /// The moon's light at <paramref name="altitude"/> meters through <paramref name="air"/>, the
    /// moon <paramref name="elevation"/> degrees over the horizon there at a phase angle of
    /// <paramref name="phaseAngle"/> degrees.
    /// </summary>
    /// <remarks>
    /// The air takes the moon's light band by band, so the moon's light at the ground is its light
    /// outside the air in each band through the air, rather than <see cref="Sun.Through"/> of its
    /// color, which takes a light of the sun's spectrum.
    /// </remarks>
    public static CelestialLight At(Air air, float elevation, float phaseAngle, float altitude = 0)
    {
        ArgumentNullException.ThrowIfNull(air);
        return Sun.Of(air.Through(Outside(phaseAngle), altitude, MathF.Sin(float.DegreesToRadians(elevation))));
    }

    // The line in its logarithm the lunar disc's reflectance follows through ROLO's constant term,
    // its 15 bands from 405 to 775 nm in Kieffer and Stone's Table 4 (version 311g), the oxygen
    // band at 763.7 nm left out, within 0.038 rms, the band-to-band excursions the paper corrects
    // against a soil's spectrum.
    internal static float Reflectance(float nm) => MathF.Exp(-2.147f + 0.1634f * (nm - 550) / 100);

    // The moon's light outside the air in each of the module's bands, watts a square meter a
    // nanometer, its lux the moon's at its phase angle.
    internal static float[] Outside(float phaseAngle)
    {
        var lux = OutsideIlluminance(phaseAngle);
        return [.. Shape.Value.Select(light => light * lux)];
    }

    // The sun's light times the disc's reflectance in each band, at a lux of 1.
    private static readonly Lazy<float[]> Shape = new(() =>
    {
        var spectrum = Spectrum.Light;
        var light = Enumerable.Range(0, spectrum.Count).Select(b => spectrum.SunLight[b] * spectrum.Mean(b, Reflectance)).ToArray();
        var lux = Colorimetry.Luminance(spectrum.ToLinearSrgb(light));
        return [.. light.Select(band => band / lux)];
    });
}

/// <summary>
/// The light of a clear night with no moon, the stars' and the air's own glow, away from a town's.
/// </summary>
public static class Night
{
    /// <summary>
    /// The sky's luminance in candelas a square meter, 2.26e-4, a dark site's sky of 21.7
    /// magnitudes a square arcsecond in the visual band.
    /// </summary>
    /// <remarks>
    /// A sky of m magnitudes a square arcsecond has a luminance of 108,000 times 10 to the power of
    /// -0.4 m candelas a square meter.
    /// </remarks>
    public const float SkyLuminance = 2.26e-4f;

    /// <summary>
    /// The illuminance the sky gives a level surface in lux, pi times <see cref="SkyLuminance"/>,
    /// the sky taken as alike over its whole dome.
    /// </summary>
    public const float SkyIlluminance = MathF.PI * SkyLuminance;

    /// <summary>
    /// The color of a moonless sky's own glow in linear sRGB at a luminance of 1, a dark sky's B-V
    /// of 0.8.
    /// </summary>
    /// <remarks>
    /// Benn and Ellison (1998) measured the dark sky over La Palma at B 22.7 and V 21.9 magnitudes
    /// a square arcsecond, redder than the sun's 0.65 by the airglow's lines of oxygen and sodium
    /// and its bands of hydroxyl, and the color of that B-V is read as a star's is
    /// (<see cref="Stars.Color"/>).
    /// </remarks>
    public static Vector3 SkyColor { get; } = Stars.Color(0.8f);

    /// <summary>
    /// The luminance in candelas a square meter of the Milky Way's starlight along a way given by
    /// its right ascension and declination in radians for the equinox of 2000, outside the air,
    /// which a dark sky shows over <see cref="SkyLuminance"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A simple profile of the band in galactic coordinates. In the galactic plane it holds 100 S10
    /// toward the anticenter and 1,000 toward the center, an S10 being a tenth-magnitude star in
    /// each square degree, 8.3e-7 candelas a square meter, the integrated starlight of the order
    /// Leinert and others' reference of the night sky's brightness (1998) gives along the plane, so
    /// the band's brightest stretch toward Sagittarius is near 8.3e-4 and nearly four times the
    /// moonless sky. Away from the plane it falls to a third each 8 degrees of galactic latitude.
    /// </para>
    /// <para>
    /// The galactic coordinates take the north galactic pole at 192.859 degrees of right ascension
    /// and 27.128 of declination and the north celestial pole at 122.932 degrees of galactic
    /// longitude, the IAU's for the equinox of 2000.
    /// </para>
    /// </remarks>
    public static float GalaxyLuminance(float rightAscension, float declination)
    {
        var (poleRa, poleDec, celestialPoleLongitude) = (Radians(192.859f), Radians(27.128f), Radians(122.932f));
        var sinB = (MathF.Sin(poleDec) * MathF.Sin(declination)) + (MathF.Cos(poleDec) * MathF.Cos(declination) * MathF.Cos(rightAscension - poleRa));
        var b = MathF.Asin(Math.Clamp(sinB, -1f, 1f));
        var y = MathF.Cos(declination) * MathF.Sin(rightAscension - poleRa);
        var x = (MathF.Cos(poleDec) * MathF.Sin(declination)) - (MathF.Sin(poleDec) * MathF.Cos(declination) * MathF.Cos(rightAscension - poleRa));
        var l = celestialPoleLongitude - MathF.Atan2(y, x);
        var toCenter = MathF.Pow((1f + MathF.Cos(l)) / 2f, 3f);
        var plane = 100f + (900f * toCenter);
        return plane * S10 * MathF.Exp(-MathF.Abs(b) / Radians(8f));
    }

    /// <summary>
    /// The luminance in candelas a square meter of a tenth-magnitude star in each square degree,
    /// the unit S10 the Milky Way's light is given in.
    /// </summary>
    public static float S10 => Stars.Illuminance(10f) / MathF.Pow(MathF.PI / 180f, 2);

    private static float Radians(float degrees) => degrees * MathF.PI / 180f;
}
