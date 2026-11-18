using System.Buffers.Binary;
using System.IO.Compression;
using System.Numerics;

namespace Sky.Tests;

/// <summary>
/// A picture of the sky drawn by the module alone on the CPU, the sky's luminance over the dome
/// with the sun, the moon and the stars, exposed by an EV100 and encoded to 8-bit sRGB.
/// </summary>
/// <remarks>
/// <para>
/// Each pixel is the sky's luminance lit by the sun (<see cref="SkyLight.Luminance"/>) and by the
/// moon's light band by band, the night's own glow in its color and the Milky Way's light beside
/// it, and the sun, the moon and each star of the catalog as a point, its illuminance at the
/// ground spread over the pixel it falls in, since every disc is smaller than a pixel here. Under
/// the horizon the ground is the reader's, a level of the air's albedo lit by the sun, the moon
/// and the night.
/// </para>
/// <para>
/// The exposure is ISO 12232's saturation-based one, a pixel full at 1.2 times 2 to the power of
/// the EV100 candelas a square meter, 78 over the sensitivity of 100 and a lens's transmission of
/// 0.65, and each channel past it is held at full. The encoding is sRGB's own curve of IEC
/// 61966-2-1, a line of 12.92 under 0.0031308 and 1.055 times the 2.4th root less 0.055 above it,
/// with no tone curve.
/// </para>
/// <para>
/// Where asked, a small red ring marks the sun's place while it is above the horizon, so a reader
/// can check a glow against it; the ring is no light of the sky's.
/// </para>
/// </remarks>
public sealed class SkyPicture
{
    private readonly byte[] _pixels;

    private SkyPicture(int width, int height)
    {
        (Width, Height) = (width, height);
        _pixels = new byte[width * height * 3];
    }

    /// <summary>The picture's width in pixels.</summary>
    public int Width { get; }

    /// <summary>The picture's height in pixels.</summary>
    public int Height { get; }

    /// <summary>The luminance in candelas a square meter a pixel is full at by the EV100.</summary>
    public static float Saturation(float exposure) => 1.2f * MathF.Pow(2, exposure);

    /// <summary>The 8-bit sRGB of the pixel at a column and a row.</summary>
    public Vector3 At(int column, int row)
    {
        var i = (row * Width + column) * 3;
        return new Vector3(_pixels[i], _pixels[i + 1], _pixels[i + 2]);
    }

    /// <summary>
    /// The whole dome looking up, the zenith in the middle and the horizon at the circle's edge,
    /// the height over the horizon falling evenly with the distance from the middle, north up and
    /// east to the left as the sky is seen from under it.
    /// </summary>
    public static SkyPicture Fisheye(SkyLight sky, SkyClock clock, float exposure, int size, bool markSun = false)
    {
        var picture = new SkyPicture(size, size);
        var radius = size / 2f;
        var step = MathF.PI / 2 / radius;
        picture.Draw(sky, clock, exposure, (column, row) =>
        {
            var (x, y) = ((column + 0.5f - radius) / radius, (row + 0.5f - radius) / radius);
            var off = MathF.Sqrt(x * x + y * y);
            if (off > 1) return null;
            var zenith = off * MathF.PI / 2;
            var turn = MathF.Atan2(-x, -y);
            var way = new Vector3(MathF.Sin(zenith) * MathF.Sin(turn), MathF.Cos(zenith), -MathF.Sin(zenith) * MathF.Cos(turn));
            var solid = step * step * (zenith > 1e-4f ? MathF.Sin(zenith) / zenith : 1);
            return (way, solid);
        }, way =>
        {
            if (way.Y < 0) return null;
            var zenith = MathF.Acos(Math.Clamp(way.Y, -1, 1));
            var turn = MathF.Atan2(way.X, -way.Z);
            var off = zenith / (MathF.PI / 2) * radius;
            return ((int)(radius - off * MathF.Sin(turn)), (int)(radius - off * MathF.Cos(turn)));
        }, markSun);
        return picture;
    }

    /// <summary>
    /// The sky all the way round from 10 degrees under the horizon to the zenith, north at both
    /// ends and south in the middle, each pixel the same angle across and up.
    /// </summary>
    public static SkyPicture Panorama(SkyLight sky, SkyClock clock, float exposure, int width, int height, bool markSun = false)
    {
        var picture = new SkyPicture(width, height);
        var (across, up) = (2 * MathF.PI / width, float.DegreesToRadians(100) / height);
        picture.Draw(sky, clock, exposure, (column, row) =>
        {
            var turn = (column + 0.5f) * across;
            var elevation = float.DegreesToRadians(90) - (row + 0.5f) * up;
            var way = new Vector3(MathF.Cos(elevation) * MathF.Sin(turn), MathF.Sin(elevation), -MathF.Cos(elevation) * MathF.Cos(turn));
            return (way, across * up * MathF.Cos(elevation));
        }, way =>
        {
            var elevation = MathF.Asin(Math.Clamp(way.Y, -1, 1));
            var turn = MathF.Atan2(way.X, -way.Z);
            if (turn < 0) turn += 2 * MathF.PI;
            var row = (int)((float.DegreesToRadians(90) - elevation) / up);
            return row >= height ? null : ((int)(turn / across) % width, row);
        }, markSun);
        return picture;
    }

    /// <summary>
    /// The scenes drawn at 50 degrees north, the day's and the twilight's on the June solstice
    /// with a new moon and the nights at the December solstice's midnight, the moon full and high
    /// or new.
    /// </summary>
    public static IEnumerable<(string Name, SkyClock Clock)> Scenes()
    {
        SkyClock At(int day, float hour, float moonAge) =>
            new() { Latitude = 50, DayOfYear = day, Hour = hour, MoonAgeAtStart = moonAge - day - hour / 24 };
        SkyClock Evening(float sun)
        {
            var (early, late) = (12f, 23.9f);
            for (var i = 0; i < 40; i++)
            {
                var middle = (early + late) / 2;
                if (At(172, middle, 0).SunAltitude > sun) early = middle; else late = middle;
            }
            return At(172, early, 0);
        }
        yield return ("noon", At(172, 12, 0));
        yield return ("morning", At(172, 9, 0));
        yield return ("sunset", Evening(-34.5f / 60));
        yield return ("civil-end", Evening(-6));
        yield return ("nautical", Evening(-9));
        yield return ("moonlit", At(355, 0, SkyClock.SynodicMonth / 2));
        yield return ("moonless", At(355, 0, 0));
    }

    // Each pixel's way and solid angle from its column and row, or none outside the picture, and
    // the pixel a way falls in, or none, for the points.
    private void Draw(SkyLight sky, SkyClock clock, float exposure, Func<int, int, (Vector3 Way, float Solid)?> pixel, Func<Vector3, (int Column, int Row)?> place, bool markSun)
    {
        var air = sky.Air;
        var (sunToward, moonToward) = (clock.SunDirection, clock.MoonDirection);
        var moonLight = Moon.Outside(clock.MoonPhaseAngle);
        var light = new Vector3[Width * Height];
        var solid = new float[Width * Height];
        // A pixel no way passes through, as the fisheye's corners, is left white.
        Array.Fill(_pixels, (byte)255);
        Parallel.For(0, Height, row =>
        {
            for (var column = 0; column < Width; column++)
            {
                if (pixel(column, row) is not { } p) continue;
                var sum = sky.Luminance(p.Way, sunToward) + sky.LuminanceBy(moonLight, p.Way, moonToward);
                if (p.Way.Y > 0)
                {
                    var (ra, dec) = Celestial(clock, p.Way);
                    sum += Night.SkyColor * Night.SkyLuminance + new Vector3(Night.GalaxyLuminance(ra, dec)) * Share(air, p.Way);
                }
                else
                {
                    // The ground sends back the night's own glow as it does the sky's light, which
                    // the sky's reader leaves to the scene.
                    sum += air.GroundAlbedo * Night.SkyColor * (Night.SkyIlluminance / MathF.PI);
                }
                light[row * Width + column] = sum;
                solid[row * Width + column] = p.Solid;
            }
        });
        // The sun, the moon and the stars as points of their light at the ground.
        void Point(Vector3 way, Vector3 illuminance)
        {
            if (place(way) is not { } at || at.Column < 0 || at.Row < 0 || at.Column >= Width || at.Row >= Height) return;
            var i = at.Row * Width + at.Column;
            if (solid[i] > 0) light[i] += illuminance / solid[i];
        }
        var sun = Sun.At(air, clock.SunAltitude);
        Point(sunToward, sun.Color * sun.Illuminance * Sun.DiscShare(clock.SunAltitude));
        var moon = Moon.At(air, clock.MoonAltitude, clock.MoonPhaseAngle);
        Point(moonToward, moon.Color * moon.Illuminance * Moon.DiscShare(clock.MoonAltitude));
        foreach (var star in Stars.Catalog)
        {
            var way = clock.TowardSky(star.RightAscension, star.Declination);
            if (way.Y > 0)
                Point(way, Stars.Color(star.ColorIndex) * Stars.Illuminance(star.Magnitude) * Share(air, way));
        }
        var full = Saturation(exposure);
        for (var i = 0; i < light.Length; i++)
            if (solid[i] > 0)
                for (var c = 0; c < 3; c++)
                    _pixels[i * 3 + c] = (byte)MathF.Round(Encode(Math.Clamp(light[i][c] / full, 0, 1)) * 255);
        if (markSun && sunToward.Y > 0 && place(sunToward) is { } at)
        {
            var ring = Math.Max(6, Width / 60);
            for (var step = 0; step < 16 * ring; step++)
            {
                var angle = step * 2 * MathF.PI / (16 * ring);
                var (column, row) = (at.Column + (int)MathF.Round(ring * MathF.Cos(angle)), at.Row + (int)MathF.Round(ring * MathF.Sin(angle)));
                if (column >= 0 && row >= 0 && column < Width && row < Height)
                    (_pixels[(row * Width + column) * 3], _pixels[(row * Width + column) * 3 + 1], _pixels[(row * Width + column) * 3 + 2]) = (230, 20, 40);
            }
        }
    }

    // The share of a light's lux the air lets through from the top of the air to the ground along
    // a way, for the stars' and the Milky Way's.
    private static float Share(Atmosphere air, Vector3 way) => Colorimetry.Luminance(air.Transmittance(0, way.Y));

    // sRGB's curve, IEC 61966-2-1.
    private static float Encode(float linear) =>
        linear <= 0.0031308f ? 12.92f * linear : 1.055f * MathF.Pow(linear, 1 / 2.4f) - 0.055f;

    // The right ascension and declination of a way at the clock's hour and latitude, the clock's
    // own placing of the sky turned back.
    private static (float RightAscension, float Declination) Celestial(SkyClock clock, Vector3 way)
    {
        var phi = float.DegreesToRadians(clock.Latitude);
        var (east, up, north) = (way.X, way.Y, -way.Z);
        var declination = MathF.Asin(Math.Clamp(up * MathF.Sin(phi) + north * MathF.Cos(phi), -1, 1));
        var hourAngle = MathF.Atan2(-east, up * MathF.Cos(phi) - north * MathF.Sin(phi));
        return (clock.SkyTurn - hourAngle, declination);
    }

    /// <summary>Writes the picture as an 8-bit RGB PNG.</summary>
    public void WritePng(string path)
    {
        using var file = File.Create(path);
        file.Write([137, 80, 78, 71, 13, 10, 26, 10]);
        var header = new byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header, Width);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4), Height);
        (header[8], header[9]) = (8, 2);
        Chunk(file, "IHDR", header);
        using var packed = new MemoryStream();
        using (var z = new ZLibStream(packed, CompressionLevel.Optimal, true))
            for (var row = 0; row < Height; row++)
            {
                z.WriteByte(0);
                z.Write(_pixels, row * Width * 3, Width * 3);
            }
        Chunk(file, "IDAT", packed.ToArray());
        Chunk(file, "IEND", []);
    }

    private static void Chunk(Stream file, string type, byte[] data)
    {
        Span<byte> length = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(length, data.Length);
        file.Write(length);
        var named = new byte[4 + data.Length];
        System.Text.Encoding.ASCII.GetBytes(type).CopyTo(named, 0);
        data.CopyTo(named, 4);
        file.Write(named);
        BinaryPrimitives.WriteUInt32BigEndian(length, Crc(named));
        file.Write(length);
    }

    private static uint Crc(byte[] data)
    {
        var crc = 0xFFFFFFFFu;
        foreach (var b in data)
        {
            crc ^= b;
            for (var k = 0; k < 8; k++)
                crc = (crc & 1) != 0 ? 0xEDB88320u ^ (crc >> 1) : crc >> 1;
        }
        return ~crc;
    }
}
