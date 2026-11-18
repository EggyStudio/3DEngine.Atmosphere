using System.Numerics;

namespace Sky;

/// <summary>
/// The light of the sun or the moon where it reaches a place, its illuminance on a surface facing
/// it, its color and that color's temperature.
/// </summary>
/// <param name="Illuminance">Lux on a surface facing the light, 0 where it is below the
/// horizon.</param>
/// <param name="Color">The light's color in linear sRGB at a luminance of 1, so the illuminance
/// carries all of its strength.</param>
/// <param name="Temperature">The color's correlated color temperature in kelvin, 0 where the light
/// is out.</param>
public readonly record struct CelestialLight(float Illuminance, Vector3 Color, float Temperature);

/// <summary>The sun's light outside the air and where it reaches the ground through it.</summary>
/// <remarks>
/// Outside the air the sun gives the same light at every height over the horizon, 1,361 watts a
/// square meter at about 94 lumens a watt, 128,000 lux, in the color of a black body at the sun's
/// effective temperature. Through the air the molecules take more of its blue than of its red and
/// the way through grows long as the sun sinks, so the light that reaches the ground dims and warms
/// toward the horizon.
/// </remarks>
public static class Sun
{
    /// <summary>The sun's illuminance outside the air in lux on a surface facing it.</summary>
    public const float OutsideIlluminance = 128_000;

    /// <summary>
    /// The sun's effective temperature in kelvin, the black body's whose color its light outside
    /// the air is taken to have.
    /// </summary>
    public const float Temperature = 5_772;

    /// <summary>The sun's light outside the air in linear sRGB, its luminance in lux.</summary>
    public static Vector3 Outside { get; } = Colorimetry.FromTemperature(Temperature) * OutsideIlluminance;

    /// <summary>
    /// How wide the sun's disc looks from the earth, in degrees, its mean of 0.533.
    /// </summary>
    public const float AngularDiameter = 0.533f;

    /// <summary>The solid angle of the sun's disc, in steradians, 6.8e-5.</summary>
    public static float SolidAngle { get; } = 2 * MathF.PI * (1 - MathF.Cos(float.DegreesToRadians(AngularDiameter / 2)));

    /// <summary>
    /// The sun's light at <paramref name="altitude"/> meters through <paramref name="air"/>, with
    /// the sun <paramref name="elevation"/> degrees over the horizon there.
    /// </summary>
    /// <remarks>
    /// The sun is taken as a point, so it is out from the moment its middle meets the ground's
    /// horizon, and the light scattered toward the place by the sky above is the sky's
    /// (<see cref="SkyLight"/>), none of it counted here. An engine that lets the light go as the
    /// disc sets multiplies it by <see cref="DiscShare"/>.
    /// </remarks>
    public static CelestialLight At(Atmosphere air, float elevation, float altitude = 0) => Through(air, Outside, elevation, altitude);

    /// <summary>
    /// The luminance of the sun's disc in candelas a square meter at <paramref name="altitude"/>
    /// meters through <paramref name="air"/>, the sun <paramref name="elevation"/> degrees over the
    /// horizon, in linear sRGB whose luminance is the disc's, about 1.6e9 under a high sun.
    /// </summary>
    /// <remarks>
    /// The light of <see cref="At"/> spread over <see cref="SolidAngle"/>, alike over the disc.
    /// While the disc sets, its part still up is taken through the air along the horizon, and the
    /// part below is the engine's to hide by <see cref="DiscShare"/>; once all of it is down it is
    /// none.
    /// </remarks>
    public static Vector3 DiscLuminance(Atmosphere air, float elevation, float altitude = 0)
    {
        if (DiscShare(elevation) <= 0) return Vector3.Zero;
        var light = Through(air, Outside, MathF.Max(elevation, 0), altitude);
        return light.Color * (light.Illuminance / SolidAngle);
    }

    /// <summary>
    /// The share of the sun's disc above the horizon with its middle <paramref name="elevation"/>
    /// degrees over it, from none once its upper edge is down to all once its lower edge is up.
    /// </summary>
    public static float DiscShare(float elevation) => ShareAbove(elevation, AngularDiameter);

    /// <summary>
    /// A light of <paramref name="outside"/> in linear sRGB outside the air, whose luminance is its
    /// lux, through <paramref name="air"/> to <paramref name="altitude"/> meters from
    /// <paramref name="elevation"/> degrees over the horizon.
    /// </summary>
    /// <remarks>
    /// For a sun an engine gives another strength or color than <see cref="Outside"/>, and for the
    /// moon's light, which <see cref="Moon.At"/> sends through here. The air takes the same share
    /// of any strength, so the light that comes through is in proportion and its color the same.
    /// </remarks>
    public static CelestialLight Through(Atmosphere air, Vector3 outside, float elevation, float altitude = 0)
    {
        var through = outside * air.Transmittance(altitude, MathF.Sin(float.DegreesToRadians(elevation)));
        var lux = Colorimetry.Luminance(through);
        return lux <= 0 ? new CelestialLight(0, Vector3.Zero, 0) : new CelestialLight(lux, through / lux, Colorimetry.TemperatureOf(through));
    }

    /// <summary>
    /// The share of a disc <paramref name="angularDiameter"/> degrees wide above the horizon with
    /// its middle <paramref name="elevation"/> degrees over it, the circle's segment above the
    /// horizon's line over the whole, the horizon taken as straight across so small a disc.
    /// </summary>
    internal static float ShareAbove(float elevation, float angularDiameter)
    {
        // The ends by comparison before the arithmetic, since acos and the division by pi round
        // apart on some processors, arm64's giving 0.99999994 for a disc wholly up.
        var radius = angularDiameter / 2;
        if (elevation >= radius) return 1;
        if (elevation <= -radius) return 0;
        var x = elevation / radius;
        return (MathF.Acos(-x) + x * MathF.Sqrt(1 - x * x)) / MathF.PI;
    }
}
