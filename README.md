# Sky

Sky is a library both engines, 3DEngine and BevyCSharp, take as a git submodule, for the sky's
clock, the sun's and the sky's light in physical units, the atmosphere's parameters, the weather and
the looks by the sun's height. Both engines light from its numbers while each draws its own sky. It
references no engine, and each engine implements the interfaces it declares where it binds the
module.

## What it holds

| Type | Gives |
|---|---|
| `Atmosphere` | The air as data, the planet's radius, the molecules' scattering, the aerosols' extinction and the ozone's absorption with their heights, the ground's albedo, and the transmittance along a way through it. `Atmosphere.Earth` is the Earth of Unity's HDRP, and `WithTurbidity` sets its haze |
| `Sun` | The sun's light outside the air, 128,000 lux at 5,772 K, and `Sun.At`, its lux, color and color temperature at the ground for its height over the horizon, with its disc's size, its luminance and the share of it above the horizon, and `Sun.Through` for a light of another strength or color |
| `SkyLight` | The sky's luminance along a way and its illuminance on a level surface, the sun's light scattered once and more than once, in candelas a square meter and lux |
| `SceneLight` | The light a scene takes at an hour of a `SkyClock`, the sun's and the moon's at the ground by the share of their discs above the horizon, the sky's lit by each and the night's own on a level surface, the key light's way, and the EV100 an incident meter gives for all of it |
| `Moon` and `Night` | The moon's light by its phase and height, its disc's size and luminance and the earthshine on its dark part, and a moonless sky's luminance and illuminance with the Milky Way's band over it |
| `Stars` and `Star` | The Bright Star Catalogue's 9,096 stars, their places, magnitudes and colors, and a star's illuminance and luminance by its magnitude |
| `Colorimetry` | Linear sRGB's luminance and a black body's color, to and from its temperature |
| `SkyClock` | Where the sun, the moon and the stars stand at an hour of a day of the year for a latitude, by spherical astronomy, with the moon's phase, how far the stars have turned about the pole, and the part of the day, a `DayPhase`, the sun's height makes it |
| `WeatherState` | The weather as it goes, its `WeatherConditions` easing toward a `WeatherPreset` or toward a forecast a `WeatherClimate` writes from the clock, the wind gusting and the ground wetting in rain and whitening under snow |
| `LookCycle` | A camera's look by the sun's height, the `LookSettings` kept at heights of the sun and worked out between the two about any other, its curve a `LookCurve` |

Read against HDRP's Earth, the sun 54 degrees high reaches the ground at 110,000 lux and 5,026 K
and the sky lights a level surface with 7,860 lux beside the sun's 88,900 (`dotnet test Sky.Tests`).

## Taking it into an engine

An engine adds the repository as a git submodule under `modules/` and references its project, here
from a project one folder below the engine's root.

```bash
git submodule add https://github.com/EggyStudio/Sky.git modules/Sky
```

```xml
<ProjectReference Include="..\modules\Sky\Sky\Sky.csproj" />
```

A clone of the engine fetches the module with `git clone --recurse-submodules`, or with
`git submodule update --init` in a clone made without it, and a workflow checks it out with
`submodules: true` on `actions/checkout`. A change the engine uses is committed and pushed here
before the engine's commit that moves its pointer, so the engine's history never names a commit
this repository does not have.

## Building and testing

With the .NET 10 SDK:

```bash
dotnet build Sky.slnx
dotnet test Sky.Tests
```

## License

The Mozilla Public License 2.0, in [LICENSE](LICENSE).
