using System.Numerics;

namespace Sky;

/// <summary>
/// A strike of lightning about a place, where it lands or flashes, how far off, when its thunder
/// is heard there and the seed its flash flickers by.
/// </summary>
/// <param name="Position">
/// Where the bolt lands or the flash stands, from the place on the ground under the eye, +Y up, on
/// the ground for a strike to it and at the clouds' base for a flash in the cloud.
/// </param>
/// <param name="Distance">How far off along the ground, in meters.</param>
/// <param name="ThunderDelay">The seconds from the flash until its thunder is heard.</param>
/// <param name="ToGround">
/// Whether the bolt reaches the ground, or the flash stays in the cloud.
/// </param>
/// <param name="Seed">
/// The seed of the flash's flicker (<see cref="Thunderstorm.Envelope"/>).
/// </param>
public readonly record struct LightningStrike(Vector3 Position, float Distance, float ThunderDelay, bool ToGround, uint Seed);

/// <summary>
/// A storm's lightning about a place, striking at the rate <see cref="WeatherConditions.Thunder"/>
/// gives, each strike placed, timed and flickered from the storm's seed, so the same seed strikes
/// the same storm on every system.
/// </summary>
/// <remarks>
/// <para>
/// The strikes are a Poisson process, as many a minute on average with no beat to them however
/// the rate moves. The storm holds a share of one strike drawn from an exponential spread and
/// spends it at the rate it has each step, striking when it is spent, so a storm the weather eases
/// into strikes as its rate climbs rather than after a gap drawn at the first, smallest rate.
/// </para>
/// <para>
/// A strike lands at a distance spread evenly over the ground between <see cref="Nearest"/> and
/// <see cref="Farthest"/> and in any direction, and <see cref="GroundShare"/> of them reach the
/// ground, the rest flashing in the cloud at its base. The engine draws the bolt and the flash's
/// light by <see cref="Envelope"/> and <see cref="Illuminance"/>, and plays the thunder
/// <see cref="LightningStrike.ThunderDelay"/> after.
/// </para>
/// </remarks>
public sealed class Thunderstorm
{
    /// <summary>
    /// The seconds a flash lasts, its rise, its return strokes and its fall, 0.55.
    /// </summary>
    public const float FlashDuration = 0.55f;

    // The fall's value at the flash's end, e to the minus 4.5.
    private static readonly float EndOfFall = MathF.Exp(-4.5f);

    private uint _state;
    private uint _strikes;
    private float _left = float.NaN;

    /// <summary>Makes a storm whose strikes come from <paramref name="seed"/>.</summary>
    public Thunderstorm(uint seed = 0)
    {
        Seed = seed;
        _state = seed;
    }

    /// <summary>The seed the strikes' times, places and flickers are made from.</summary>
    public uint Seed { get; }

    /// <summary>The nearest a strike lands to the place, in meters, 150 unless set.</summary>
    public float Nearest { get; set; } = 150;

    /// <summary>The farthest a strike lands from the place, in meters, 4,000 unless set.</summary>
    public float Farthest { get; set; } = 4_000;

    /// <summary>
    /// The share of strikes that reach the ground, a quarter unless set, the rest flashing in the
    /// cloud.
    /// </summary>
    /// <remarks>
    /// About three quarters of flashes stay in the cloud, by Rakov and Uman's Lightning (2003). A
    /// binding that shows more bolts than a real storm would raises it.
    /// </remarks>
    public float GroundShare { get; set; } = 0.25f;

    /// <summary>A flash's light at its brightest, in lumens, 4e9 unless set.</summary>
    /// <remarks>
    /// Set by sight rather than from a measurement, so a flash 150 meters off lights the ground
    /// with some 14,000 lux and one at the farthest with some 20.
    /// </remarks>
    public float PeakLumens { get; set; } = 4e9f;

    /// <summary>
    /// A flash's color in linear sRGB at a luminance of 1, a bluish white as the eye sees it, set
    /// by sight.
    /// </summary>
    public static Vector3 FlashColor { get; } = new Vector3(0.82f, 0.88f, 1f) / Colorimetry.Luminance(new Vector3(0.82f, 0.88f, 1f));

    /// <summary>
    /// The speed of sound in meters a second in dry air at <paramref name="celsius"/> degrees, 343
    /// at 20 and 331 at freezing, the air taken as an ideal gas.
    /// </summary>
    public static float SpeedOfSound(float celsius) => 331.3f * MathF.Sqrt(MathF.Max(1 + celsius / 273.15f, 0));

    /// <summary>
    /// Moves the storm on <paramref name="seconds"/> under <paramref name="weather"/>, at its
    /// <see cref="WeatherConditions.Thunder"/> strikes a minute, its clouds' base and its air's
    /// temperature, and gives the strikes that began in that time, the earliest first.
    /// </summary>
    public IReadOnlyList<LightningStrike> Advance(float seconds, WeatherConditions weather)
    {
        ArgumentNullException.ThrowIfNull(weather);
        if (float.IsNaN(_left)) _left = Share();
        var rate = MathF.Max(weather.Thunder, 0);
        if (rate <= 0 || seconds <= 0) return [];
        var spent = rate / 60 * seconds;
        List<LightningStrike>? struck = null;
        while (spent >= _left)
        {
            spent -= _left;
            (struck ??= []).Add(Strike(weather));
            _left = Share();
        }
        _left -= spent;
        return struck ?? (IReadOnlyList<LightningStrike>)[];
    }

    /// <summary>
    /// Strikes at once under <paramref name="weather"/>, at <paramref name="at"/> on the ground
    /// from the place under the eye, or where the storm chooses.
    /// </summary>
    /// <remarks>A strike given a place reaches the ground there, its height left out.</remarks>
    public LightningStrike Strike(WeatherConditions weather, Vector3? at = null)
    {
        ArgumentNullException.ThrowIfNull(weather);
        Vector3 position;
        bool toGround;
        if (at is { } given)
        {
            (position, toGround) = (given with { Y = 0 }, true);
        }
        else
        {
            var (near, far) = (Nearest, Farthest);
            var reach = MathF.Sqrt(near * near + Next() * (far * far - near * near));
            var angle = Next() * MathF.Tau;
            toGround = Next() < GroundShare;
            position = new Vector3(MathF.Cos(angle) * reach, toGround ? 0 : weather.CloudBase, MathF.Sin(angle) * reach);
        }
        var distance = new Vector2(position.X, position.Z).Length();
        var seed = Seed * 2654435761u + _strikes++;
        return new LightningStrike(position, distance, distance / SpeedOfSound(weather.Temperature), toGround, seed);
    }

    /// <summary>
    /// How bright a flash is from 0 to 1, <paramref name="elapsed"/> seconds after it began, a rise
    /// in its first 22 ms and a fall, with two or three return strokes at
    /// <paramref name="seed"/>'s times brightening it again, none before it and after
    /// <see cref="FlashDuration"/>.
    /// </summary>
    public static float Envelope(float elapsed, uint seed)
    {
        if (elapsed < 0 || elapsed > FlashDuration) return 0;
        var t = elapsed / FlashDuration;
        var rise = MathF.Min(t / 0.04f, 1);
        // The fall less its value at the end, so the flash dies away to none there rather than
        // going out from a hundredth of its peak.
        var fall = (MathF.Exp(-4.5f * t) - EndOfFall) / (1 - EndOfFall);
        var flicker = 1f;
        var state = seed;
        for (var stroke = 0; stroke < 3; stroke++)
        {
            var d = (t - (0.12f + Unit(ref state) * 0.55f)) / 0.05f;
            flicker += 0.9f * MathF.Exp(-d * d);
        }
        return Math.Clamp(rise * fall * flicker, 0, 1);
    }

    /// <summary>
    /// The lux a flash gives a surface facing it <paramref name="distance"/> meters off at
    /// <paramref name="envelope"/> of its brightest, its lumens spread over the sphere about it,
    /// the air between left out.
    /// </summary>
    public float Illuminance(float distance, float envelope) =>
        PeakLumens * Math.Clamp(envelope, 0, 1) / (4 * MathF.PI * MathF.Max(distance * distance, 1));

    // A share of one strike from an exponential spread, one on average.
    private float Share() => -MathF.Log(Math.Clamp(Next(), 1e-6f, 1 - 1e-6f));

    private float Next() => Unit(ref _state);

    // A number from 0 to under 1 by SplitMix32's step, the same on every system and runtime.
    private static float Unit(ref uint state)
    {
        var z = state += 0x9E3779B9u;
        z = (z ^ (z >> 16)) * 0x85EBCA6Bu;
        z = (z ^ (z >> 13)) * 0xC2B2AE35u;
        z ^= z >> 16;
        return (z >> 8) * (1f / (1 << 24));
    }
}
