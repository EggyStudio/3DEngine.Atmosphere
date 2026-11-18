using System.Numerics;

namespace Sky;

/// <summary>
/// The light a scene takes at an hour of the sky's clock, the sun's and the moon's at the ground,
/// the sky's on a level surface and the exposure an incident meter gives for all of it, in lux and
/// EV100.
/// </summary>
/// <remarks>
/// <para>
/// Day, twilight and night are one sum, so no rule hands the light from one to the next. The sun's
/// and the moon's light come through the air (<see cref="Sun.At"/> and <see cref="Moon.At"/>), each
/// times the share of its disc above the horizon; the sky's is <see cref="SkyLight.Illuminance"/>
/// lit by the sun and again by the moon, scaled by the moon's light against the sun's, so the
/// twilight's air goes dark as the earth's shadow climbs it; and under all of it lies the dark
/// site's sky of <see cref="Night.SkyIlluminance"/>, taken as white.
/// </para>
/// <para>
/// An engine with one directional light aims it along <see cref="KeyToward"/>, the sun's way or the
/// moon's, whichever lights a surface facing it more, and lights the rest by the sky. From the
/// ground to 10 km the sky's light is read from <see cref="SkyLight.GroundIlluminance"/>'s table,
/// so a call takes microseconds once the table's rows about its altitude are made; above 10 km it
/// is summed over the dome, which takes hundredths of a second.
/// </para>
/// </remarks>
/// <param name="Sun">
/// The sun's light at the ground on a surface facing it, by its disc's share.
/// </param>
/// <param name="SunToward">
/// The way toward the sun, +Y up, as <see cref="SkyClock"/> gives it.
/// </param>
/// <param name="Moon">
/// The moon's light at the ground on a surface facing it, by its disc's share.
/// </param>
/// <param name="MoonToward">The way toward the moon, +Y up.</param>
/// <param name="SkyIlluminance">
/// The sky's light on a level surface in linear sRGB whose luminance is its lux, by the sun, by the
/// moon and the night's own, the sun's and the moon's direct light apart.
/// </param>
public sealed record SceneLight(CelestialLight Sun, Vector3 SunToward, CelestialLight Moon, Vector3 MoonToward, Vector3 SkyIlluminance)
{
    // ISO 2720's calibration constant for an incident meter's flat receptor, as the usual meters
    // take it, the standard allowing 240 to 400.
    private const float IncidentCalibration = 250;

    /// <summary>
    /// The light at the clock's hour, day and latitude through <paramref name="sky"/>'s air, at
    /// <paramref name="altitude"/> meters.
    /// </summary>
    public static SceneLight At(SkyClock clock, SkyLight sky, float altitude = 0)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(sky);
        var (sunToward, moonToward) = (clock.SunDirection, clock.MoonDirection);
        var sun = Scaled(Sky.Sun.At(sky.Air, clock.SunAltitude, altitude), Sky.Sun.DiscShare(clock.SunAltitude));
        var phaseAngle = clock.MoonPhaseAngle;
        var moon = Scaled(Sky.Moon.At(sky.Air, clock.MoonAltitude, phaseAngle, altitude), Sky.Moon.DiscShare(clock.MoonAltitude));
        var moonOverSun = Sky.Moon.OutsideIlluminance(phaseAngle) / Sky.Sun.OutsideIlluminance;
        // From the sky's table to 10 km up, in microseconds; above it summed over the dome.
        Vector3 Lit(Vector3 toward) => sky.GroundIlluminance(toward, altitude);
        var skyLight = Lit(sunToward) + Lit(moonToward) * moonOverSun + new Vector3(Night.SkyIlluminance);
        return new SceneLight(sun, sunToward, moon, moonToward, skyLight);
    }

    /// <summary>
    /// The EV100 an incident meter gives for <paramref name="illuminance"/> lux, log2 of the lux
    /// times 100 over ISO 2720's calibration constant of 250, 0 at 2.5 lux.
    /// </summary>
    public static float ExposureFor(float illuminance) => MathF.Log2(illuminance * 100 / IncidentCalibration);

    /// <summary>Whether the moon lights a surface facing it more than the sun does.</summary>
    public bool MoonIsKey => Moon.Illuminance > Sun.Illuminance;

    /// <summary>
    /// The way toward the light that lights most, the moon's where it is the key.
    /// </summary>
    public Vector3 KeyToward => MoonIsKey ? MoonToward : SunToward;

    /// <summary>
    /// The lux on a level surface, the sun's and the moon's by the sine of their height and the
    /// sky's.
    /// </summary>
    public float LevelIlluminance =>
        Sun.Illuminance * MathF.Max(SunToward.Y, 0)
        + Moon.Illuminance * MathF.Max(MoonToward.Y, 0)
        + Colorimetry.Luminance(SkyIlluminance);

    /// <summary>
    /// The EV100 an incident meter held level gives for <see cref="LevelIlluminance"/>, about 15 in
    /// full sun and -12 under a moonless sky.
    /// </summary>
    /// <remarks>
    /// A meter exposes every light to the same middle gray, so a moonless night shown at this
    /// exposure is as bright as noon. The darkness the eye sees at night is the look's to give,
    /// as <see cref="LookCycle"/>'s keys under the horizon give it.
    /// </remarks>
    public float Exposure => ExposureFor(LevelIlluminance);

    /// <summary>
    /// This light under <paramref name="weather"/>'s cloud, the sun's and the moon's direct light
    /// dimmed and moved into the sky's as the cover grows, none of it direct under a full cover.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Kasten and Czeplak's relations of 1980 for the light at the ground under a cover of N
    /// eighths, from Hamburg's ten years of hourly readings, take the global light, direct and
    /// diffuse on a level surface together, as the clear sky's times 1 - 0.75 (N / 8)^3.4, a
    /// quarter of it under a full cover, and the diffuse share of it as 0.3 + 0.7 (N / 8)^2. The
    /// clear share here is the clear sky's own at the light's height, where their 0.3 is a mean
    /// over the day, and it climbs to all of the light by the cover's square as theirs does.
    /// </para>
    /// <para>
    /// The sky keeps its clear part where it is uncovered, and the cloud adds the rest of the
    /// diffuse light in the color of the clear day's whole light, sun and sky, which the cloud
    /// scatters alike in every channel. The moon's light and the night's own are dimmed by the
    /// same relations. The cloud's density and kind are left out, the cover alone setting the
    /// light.
    /// </para>
    /// </remarks>
    public SceneLight Under(WeatherConditions weather)
    {
        ArgumentNullException.ThrowIfNull(weather);
        var cover = Math.Clamp(weather.CloudCover, 0, 1);
        var sun = Sun.Color * (Sun.Illuminance * MathF.Max(SunToward.Y, 0));
        var moon = Moon.Color * (Moon.Illuminance * MathF.Max(MoonToward.Y, 0));
        var (direct, skyLux) = (Colorimetry.Luminance(sun + moon), Colorimetry.Luminance(SkyIlluminance));
        var whole = direct + skyLux;
        if (cover <= 0 || whole <= 0) return this;
        var under = whole * (1 - 0.75f * MathF.Pow(cover, 3.4f));
        var clearShare = skyLux / whole;
        // The direct part as the clear direct share times one less the cover's square, so a full
        // cover leaves exactly none on every processor.
        var directLux = under * (1 - clearShare) * (1 - cover * cover);
        var diffuse = under - directLux;
        var directShare = direct > 0 ? directLux / direct : 0;
        var cloud = diffuse - skyLux * (1 - cover);
        var sky = cloud >= 0
            ? SkyIlluminance * (1 - cover) + (sun + moon + SkyIlluminance) / whole * cloud
            : SkyIlluminance * (diffuse / skyLux);
        return this with { Sun = Scaled(Sun, directShare), Moon = Scaled(Moon, directShare), SkyIlluminance = sky };
    }

    private static CelestialLight Scaled(CelestialLight light, float share) =>
        share <= 0 ? new CelestialLight(0, Vector3.Zero, 0) : light with { Illuminance = light.Illuminance * share };
}
