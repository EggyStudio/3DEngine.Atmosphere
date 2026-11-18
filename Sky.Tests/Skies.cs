using System.Collections.Concurrent;

namespace Sky.Tests;

/// <summary>
/// The skies the tests read, one made for each air and shared, since making a sky's tables is the
/// most of what a test of it costs. A test that counts what a sky makes makes its own.
/// </summary>
public static class Skies
{
    // Each air by its value, so two airs set alike share one sky.
    private static readonly ConcurrentDictionary<Atmosphere, Lazy<SkyLight>> Made = new();

    /// <summary>The sky of the Earth's own air.</summary>
    public static SkyLight Earth => Of(Atmosphere.Earth);

    /// <summary>The sky of an ordinary day's air.</summary>
    public static SkyLight ContinentalAverage => Of(Atmosphere.ContinentalAverage);

    /// <summary>The sky of <paramref name="air"/>, made the first time any test reads it.</summary>
    public static SkyLight Of(Atmosphere air) => Made.GetOrAdd(air, a => new Lazy<SkyLight>(() => new SkyLight(a))).Value;
}
