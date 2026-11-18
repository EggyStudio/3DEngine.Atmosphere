# Sky

[DEVLOG.md](DEVLOG.md) holds what landed by day, with the pictures taken on the way, and
[CHEATSHEET.md](CHEATSHEET.md) every public type and member a line each.

Sky is a library both engines, 3DEngine and BevyCSharp, take as a git submodule, for the sky's
clock, the sun's and the sky's light in physical units, the atmosphere's parameters, the weather and
the looks by the sun's height. Both engines light from its numbers while each draws its own sky. It
references no engine, and each engine implements the interfaces it declares where it binds the
module.

## What it holds

| Type | Gives |
|---|---|
| `Atmosphere` | The air as data, the planet's radius, the molecules' scattering, the aerosols' extinction and the ozone's absorption with their heights, the ground's albedo, and the transmittance along a way through it. `Atmosphere.Earth` is the Earth of Unity's HDRP with a real column of ozone, 300 Dobson units, and an ordinary clear day's haze, and `WithTurbidity` sets its haze |
| `Sun` | The sun's light outside the air, 128,000 lux at 5,772 K, and `Sun.At`, its lux, color and color temperature at the ground for its height over the horizon, with its disc's size, its luminance and the share of it above the horizon, and `Sun.Through` for a light of another strength or color |
| `SkyLight` | The sky's luminance along a way and its illuminance on a level surface, the sun's light scattered once and more than once, in candelas a square meter and lux, with the illuminance at the ground read from a table in microseconds |
| `SceneLight` | The light a scene takes at an hour of a `SkyClock`, the sun's and the moon's at the ground by the share of their discs above the horizon, the sky's lit by each and the night's own on a level surface, the key light's way, and the EV100 an incident meter gives for all of it, and the same light under a `WeatherConditions`'s cloud |
| `Haze` | The air near the ground between the eye and what it sees, the share of a thing's light that comes through a distance and the share hidden, clear and in a `WeatherConditions`'s fog, and the visibility by Koschmieder's law |
| `Moon` and `Night` | The moon's light by its phase and height, its disc's size and luminance and the earthshine on its dark part, and a moonless sky's luminance and illuminance with the Milky Way's band over it |
| `Stars` and `Star` | The Bright Star Catalogue's 9,096 stars, their places, magnitudes and colors, and a star's illuminance and luminance by its magnitude |
| `Thunderstorm` | A storm's lightning about a place at a `WeatherConditions`'s rate of strikes, each placed, timed and flickered from the storm's seed, with the flash's light at a distance and the thunder's delay by the air's temperature, each strike a `LightningStrike`, and a `LightningFlash`, one strike's flicker made once from its seed |
| `Colorimetry` | Linear sRGB's luminance and a black body's color, to and from its temperature |
| `SkyClock` | Where the sun, the moon and the stars stand at an hour of a day of the year for a latitude, by spherical astronomy, with the moon's phase, how far the stars have turned about the pole, and the part of the day, a `DayPhase`, the sun's height makes it |
| `WeatherState` | The weather as it goes, its `WeatherConditions` easing toward a `WeatherPreset` or toward a forecast a `WeatherClimate` writes from the clock, the wind gusting and the ground wetting in rain and whitening under snow |
| `LookCycle` | A camera's look by the sun's height, the `LookSettings` kept at heights of the sun and worked out between the two about any other, its curve a `LookCurve` |

Read against HDRP's Earth with the Earth's own ozone and an ordinary clear day's haze, the sun 54
degrees high reaches the ground at 91,100 lux and 5,025 K and the sky lights a level surface with
19,300 lux beside the sun's 73,700 (`dotnet test Sky.Tests`).

## Taking it into an engine

An engine adds the repository as a git submodule under `modules/` and references its project, here
from a project one folder below the engine's root.

```bash
git submodule add https://github.com/EggyStudio/Sky.git modules/Sky
```

```xml
<ProjectReference Include="..\modules\Sky\Sky\Sky.csproj" />
```

The project builds the assembly `3DEngine.Sky`, whose types are in the namespace `Sky`.

A clone of the engine fetches the module with `git clone --recurse-submodules`, or with
`git submodule update --init` in a clone made without it, and a workflow checks it out with
`submodules: true` on `actions/checkout`. A change the engine uses is committed and pushed here
before the engine's commit that moves its pointer, so the engine's history never names a commit
this repository does not have.

## Binding it in your engine

The module declares no interface, since it asks nothing of an engine. It draws nothing, opens no
window, reads no file (the star catalog is carried in its assembly) and keeps no thread of its own,
making a `SkyLight`'s tables on .NET's thread pool, so a binding is the engine's code that calls
it and gives the numbers it returns to the engine's lights, sky, fog and camera. The library
references nothing past .NET's own assemblies, which a test of its own holds.

It stands on these conventions.

- Lengths are in meters, and an altitude is meters above the ground.
- +Y is up, -Z north and +X east, so the axes are right-handed with +Z south. A way toward a body
  points from the place to it, and a directional light from that body shines along its negative.
- A height over the horizon, a latitude, a disc's angular size and the moon's phase angle are in
  degrees, and so is a compass direction, as `WeatherConditions.WindFrom`, clockwise from north. A
  right ascension, a declination and `SkyClock.SkyTurn` are in radians.
- A height is the true one, unbent by the air, unless `SkyClock.Refracted` is set, which
  lifts the sun, the moon and the stars to the heights an eye sees them at, 34 minutes of arc at
  the horizon by `SkyClock.ApparentAltitude`. A published sunset is the apparent one, and a
  twilight's degrees are the sun's true height.
- `SkyClock.Hour` is the local solar time from 0 to 24, noon when the sun stands highest, and
  `DayOfYear` counts from 0 on the first of January in a year of 365 days. `WeatherState` advances
  by seconds.
- A color is linear sRGB, the BT.709 primaries with a D65 white and no transfer curve. Where a
  color carries a light, its luminance is the light's lux or candelas a square meter; a
  `CelestialLight`'s color has a luminance of 1 and its illuminance carries the strength.
- `Atmosphere.Earth` is an ordinary clear day's air, a visibility of 44 km and a turbidity of
  2.40, its haze OPAC's continental average, and its apparent sunset and end of civil twilight
  within 1.55 times the published figures. A binding with another day's air sets it through
  `Atmosphere.WithTurbidity` and the haze's terms.
- Illuminance is in lux, luminance in candelas a square meter, a color temperature in kelvin and an
  exposure in EV100 by an incident meter's calibration of 250. The sun gives 128,000 lux outside the
  air, and an engine whose sun is another strength scales what the module returns by its sun's over
  `Sun.OutsideIlluminance`.
- A `SkyLight` does not change once made and is read from any thread, as a `LightningFlash` is.
  A `SkyClock`, a `WeatherState` and a `Thunderstorm` change as they advance and belong to one
  caller.

The smallest binding makes the sky once and reads the scene's light each step of the world's
clock, in a file that references the assembly `3DEngine.Sky` and opens the namespaces `Sky` and
`System.Numerics`. `host` stands for the engine's own calls, whatever it names them.

```csharp
// Once, as the world loads and off the frame, the air, its sky and the sky's ground table.
var sky = new SkyLight(Atmosphere.Earth);
sky.GroundIlluminance(Vector3.UnitY);
var clock = new SkyClock { Latitude = 51, DayOfYear = 172, Hour = 15 };
var weather = new WeatherState { Clock = clock, Forecast = true };

// Each step of the world's clock, seconds long.
clock.Advance(seconds / 3600);
weather.Advance(seconds);
var light = SceneLight.At(clock, sky).Under(weather.Current);
var key = light.MoonIsKey ? light.Moon : light.Sun;
host.SetDirectionalLight(-light.KeyToward, key.Color * key.Illuminance);
host.SetAmbientLuminance(light.SkyIlluminance / MathF.PI);
host.SetExposure(light.Exposure);
host.SetFogExtinction(Haze.Extinction(sky.Air, weather.Current));
```

The ambient luminance is the sky's illuminance over pi, the luminance of a sky alike over its
dome that lights a level surface as this one does. An engine that draws its own sky reads
`SkyLight.Luminance` along each way, the sun's disc from `Sun.DiscLuminance`, the moon's from
`Moon`, and the stars from `Stars.Catalog` placed by `SkyClock.TowardSky`. 3DEngine and BevyCSharp
are two such bindings, the first drawing its sky in a pass of its own from `SkyLight`'s numbers and
the second feeding Bevy's atmosphere and clouds from the clock and the weather.

## Building and testing

The tests take the checks every module keeps from the Tools module, a git submodule under
`modules/Tools`, so a clone fetches it with `git clone --recurse-submodules`, or with
`git submodule update --init` in a clone made without it. With the .NET 10 SDK:

```bash
dotnet build Sky.slnx
dotnet test Sky.Tests
```

## License

The Mozilla Public License 2.0, in [LICENSE](LICENSE).
