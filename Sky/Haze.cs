using System.Numerics;

namespace Sky;

/// <summary>
/// The air near the ground between the eye and what it sees, clear and in fog, as the share of a
/// thing's light that comes through a distance and the share the air hides, in linear sRGB, and as
/// the visibility in meters.
/// </summary>
/// <remarks>
/// <para>
/// Clear air takes its molecules' scattering and its aerosols' extinction at the ground
/// (<see cref="Atmosphere.ExtinctionAt"/>), more of the blue than of the red, so far hills go blue.
/// Fog adds an extinction alike in every channel, set by the visibility it leaves, which goes from
/// the clear air's toward <see cref="FogVisibility"/> evenly in its logarithm as
/// <see cref="WeatherConditions.Fog"/> goes from 0 to 1. Koschmieder's law gives the visibility as
/// <see cref="Contrast"/> over the extinction at 550 nm, the green channel's.
/// </para>
/// <para>
/// A thing at a distance is seen with its light times <see cref="Transmittance"/>, and the share
/// the air hides, <see cref="Share"/>, is filled by the air's own light, which an engine takes from
/// its sky toward the horizon. The figures are for a way along the ground; a way that climbs
/// through the air is <see cref="Atmosphere.Transmittance"/>'s. Rain and snow add nothing here,
/// since the weather's presets carry their fog with them.
/// </para>
/// </remarks>
public static class Haze
{
    /// <summary>
    /// The optical depth at which a black thing's contrast against the horizon falls to 2%, the
    /// eye's threshold, 3.912, by which Koschmieder's law turns an extinction into a visibility.
    /// </summary>
    public const float Contrast = 3.912f;

    /// <summary>
    /// The visibility in meters of the thickest fog, at <see cref="WeatherConditions.Fog"/> 1.
    /// </summary>
    public const float FogVisibility = 80;

    /// <summary>
    /// How far a black thing can be told from the horizon through <paramref name="air"/> at the
    /// ground in <paramref name="weather"/>'s fog, in meters, 178 km in the Earth's clear air, 44
    /// km in an ordinary day's and 80 m in the thickest fog.
    /// </summary>
    public static float Visibility(Atmosphere air, WeatherConditions? weather = null)
    {
        ArgumentNullException.ThrowIfNull(air);
        var clear = Contrast / air.ExtinctionAt(0).Y;
        var fog = Math.Clamp(weather?.Fog ?? 0, 0, 1);
        return MathF.Exp(float.Lerp(MathF.Log(clear), MathF.Log(FogVisibility), fog));
    }

    /// <summary>
    /// The share of a thing's light the air at the ground takes in a meter, in each channel, the
    /// clear air's with <paramref name="weather"/>'s fog added alike in all three.
    /// </summary>
    public static Vector3 Extinction(Atmosphere air, WeatherConditions? weather = null)
    {
        ArgumentNullException.ThrowIfNull(air);
        var clear = air.ExtinctionAt(0);
        var fog = MathF.Max(Contrast / Visibility(air, weather) - clear.Y, 0);
        return clear + new Vector3(fog);
    }

    /// <summary>
    /// The share of a thing's light that comes through <paramref name="distance"/> meters of air
    /// along the ground in <paramref name="weather"/>'s fog, in each channel.
    /// </summary>
    public static Vector3 Transmittance(Atmosphere air, float distance, WeatherConditions? weather = null)
    {
        var depth = Extinction(air, weather) * MathF.Max(distance, 0);
        return new Vector3(MathF.Exp(-depth.X), MathF.Exp(-depth.Y), MathF.Exp(-depth.Z));
    }

    /// <summary>
    /// The share of a thing's light the air hides over <paramref name="distance"/> meters along the
    /// ground in <paramref name="weather"/>'s fog, in each channel, which the air's own light
    /// fills, in green 0.00055 at 25 meters and 0.022 at a kilometer in the Earth's clear air and
    /// 0.0022 and 0.085 in an ordinary day's.
    /// </summary>
    public static Vector3 Share(Atmosphere air, float distance, WeatherConditions? weather = null) =>
        Vector3.One - Transmittance(air, distance, weather);
}
