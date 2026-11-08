namespace Sky;

/// <summary>
/// The moon's light outside the air by its phase and where it reaches the ground through it.
/// </summary>
/// <remarks>
/// The moon's illuminance outside the air follows Krisciunas and Schaefer's fit of the moon's
/// brightness by its phase angle, the angle at the moon between the sun and the eye, 0 when full
/// and 180 when new, which falls faster than the lit share of the disc since the moon's rough
/// surface throws shadows that the full moon alone shows none of. Its light is the sun's reflected,
/// in the sun's color, the slight redness the moon's albedo gives it left out.
/// </remarks>
public static class Moon
{
    /// <summary>How wide the moon's disc looks from the earth, in degrees, its mean of 0.518.</summary>
    public const float AngularDiameter = 0.518f;

    /// <summary>The solid angle of the moon's disc, in steradians, 6.4e-5.</summary>
    public static float SolidAngle => MathF.PI * MathF.Pow(AngularDiameter * 0.5f * MathF.PI / 180f, 2);

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
    /// sunlight it takes, a luminance near 12,000 candelas a square meter against the 128,000 lux
    /// of the sun, so it lights the moon with some 10 lux, 8e-5 of the sun's light there, and the
    /// moon's dark part shows that share of the full moon's luminance, falling as the earth's lit
    /// share, one less the moon's, falls.
    /// </remarks>
    public static float EarthshineLuminance(float phaseAngle) => EarthshineShare * LitLuminance(0) * (1f - LitShare(phaseAngle));

    /// <summary>The share of the full moon's luminance the moon's dark part shows by a full earth's light.</summary>
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
    public static CelestialLight At(Atmosphere air, float elevation, float phaseAngle, float altitude = 0) =>
        Sun.Through(air, Sun.Outside / Sun.OutsideIlluminance * OutsideIlluminance(phaseAngle), elevation, altitude);
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
    /// The luminance in candelas a square meter of the Milky Way's starlight along a way given by
    /// its right ascension and declination in radians for the equinox of 2000, outside the air,
    /// which a dark sky shows over <see cref="SkyLuminance"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A simple profile of the band in galactic coordinates. In the galactic plane it holds 100 S10
    /// toward the anticenter and 1,000 toward the center, an S10 being a tenth-magnitude star in each
    /// square degree, 8.3e-7 candelas a square meter, the integrated starlight of the order Leinert
    /// and others' reference of the night sky's brightness (1998) gives along the plane, so the band's
    /// brightest stretch toward Sagittarius is near 8.3e-4 and nearly four times the moonless sky.
    /// Away from the plane it falls to a third each 8 degrees of galactic latitude.
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

    /// <summary>The luminance in candelas a square meter of a tenth-magnitude star in each square degree, the unit S10 the Milky Way's light is given in.</summary>
    public static float S10 => Stars.Illuminance(10f) / MathF.Pow(MathF.PI / 180f, 2);

    private static float Radians(float degrees) => degrees * MathF.PI / 180f;
}
