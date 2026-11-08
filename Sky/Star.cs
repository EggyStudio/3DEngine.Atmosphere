namespace Sky;

/// <summary>A star of <see cref="Stars.Catalog"/>, its place on the sky for the equinox of 2000, its visual magnitude and its color index.</summary>
/// <param name="RightAscension">Its right ascension, in radians.</param>
/// <param name="Declination">Its declination, in radians.</param>
/// <param name="Magnitude">Its visual magnitude, smaller the brighter, Sirius's -1.46.</param>
/// <param name="ColorIndex">Its B-V color index, 0.65 where the catalog gives none, the sun's.</param>
public readonly record struct Star(float RightAscension, float Declination, float Magnitude, float ColorIndex);
