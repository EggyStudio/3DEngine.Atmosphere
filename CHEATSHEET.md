# 3DEngine.Sky cheatsheet

Every public type of the Sky module in the order the README first names it, with its public
members a line each, the declaration and the first sentence of its summary, for a binding to find
a member by. The conventions the numbers stand on, meters, +Y up with north along -Z, linear sRGB
and light in lux and candelas a square meter, are in the README's section Binding it in your
engine. A positional record's constructor stands bare, its type's line saying what it holds. The
list is written by `Cheatsheet.Write` from the built library, and this text by hand.

<!-- written by Cheatsheet.Write from the built library -->
```csharp
record Atmosphere                                        // The air around a planet as a sky is drawn and lit from it, the planet's radius, the molecules' scattering, the aerosols' extinction and the ozone's absorption with the heights each thins over, and the ground's albedo, in meters and per meter at sea level
Atmosphere();                                            // The Earth's air, each term as Earth holds it until set
Atmosphere Earth { get; }                                // The Earth's air on a clear day, its ozone a column of 300 Dobson units, as Hillaire (2020) sets it
Atmosphere ContinentalAverage { get; }                   // The Earth's air on an ordinary day, its haze OPAC's continental average aerosol at a relative humidity of 80%, a turbidity of 2.39 and 44 km visibility
float PlanetRadius { get; init; }                        // The planet's radius in meters, 6,360,000 for the Earth
Vector3 RayleighScattering { get; init; }                // How much of the light the molecules scatter a meter at sea level, at 680, 550 and 440 nanometers, which they take nothing of otherwise
float RayleighScaleHeight { get; init; }                 // The height in meters over which the molecules thin to a share of 1/e, 8,000 for the Earth
float AerosolExtinction { get; init; }                   // How much of the light the aerosols take out a meter at sea level, scattered or absorbed, alike at every wavelength
float AerosolAngstrom { get; init; }                     // How steeply the aerosols' extinction falls with wavelength, the power of 550 nm over the wavelength it goes as, 0 unless set, the clear day's haze taking every wavelength alike
Vector3 AerosolAlbedo { get; init; }                     // The share of the light the aerosols take out that they scatter, the rest absorbed, at each wavelength
float AerosolScaleHeight { get; init; }                  // The height in meters over which the aerosols thin to a share of 1/e, 1,200 for the Earth
float AerosolAnisotropy { get; init; }                   // How strongly the aerosols scatter forward, from -1 for all of the light back the way it came to 1 for all of it on, as Cornette and Shanks's phase function takes it
Vector3 OzoneAbsorption { get; init; }                   // How much of the light the ozone absorbs a meter in the middle of its layer, at each wavelength, unless set the absorption Bruneton's precomputed atmospheric scattering of 2017 gives a column of 300 Dobson units
float OzoneBottom { get; init; }                         // The height in meters the ozone's layer starts at, 10 km unless set
float OzoneWidth { get; init; }                          // The ozone layer's depth in meters, densest halfway up it, 30 km unless set
Vector3 GroundAlbedo { get; init; }                      // The share of the light the ground returns at each wavelength, scattered evenly
float Top { get; }                                       // The height in meters past which the air is taken to end, where its thickest layer has thinned to a millionth or the ozone's layer ends, whichever is higher, 111 km for Earth
float Turbidity { get; }                                 // The air's haze, the optical depth of the molecules and the aerosols over that of the molecules alone straight up at 550 nanometers, 1 for air with no aerosols in it, 1.09 for Earth's clear day and 2.39 for ContinentalAverage's ordinary one
Atmosphere WithTurbidity(float turbidity);               // This air with the aerosols' extinction set so its Turbidity is turbidity, 1 at least
Vector3 RayleighAt(float altitude);                      // How much of the light the molecules scatter a meter at altitude meters, at each wavelength
float AerosolAt(float altitude);                         // How much of the light the aerosols take out a meter at altitude meters
Vector3 OzoneAt(float altitude);                         // How much of the light the ozone absorbs a meter at altitude meters, at each wavelength
Vector3 ExtinctionAt(float altitude);                    // How much of the light the air takes out a meter at altitude meters, scattered or absorbed, at each wavelength
Vector3 Transmittance(float altitude, float cosZenith);  // The share of the sun's light that crosses the air from altitude meters toward a way whose cosine with the zenith there is cosZenith, out to the top of the air, in each channel of linear sRGB, or nothing where the way meets the ground

class Sun                                                // The sun's light outside the air and where it reaches the ground through it
float OutsideIlluminance = 133800f;                      // The sun's illuminance outside the air in lux on a surface facing it
float Temperature = 5772f;                               // The sun's effective temperature in kelvin, the black body's that sends out as much power a square meter as it does, its light's color being its spectrum's
float AngularDiameter = 0.533f;                          // How wide the sun's disc looks from the earth, in degrees, its mean of 0.533
Vector3 Outside { get; }                                 // The sun's light outside the air in linear sRGB, its luminance in lux, its color its measured spectrum's
float SolidAngle { get; }                                // The solid angle of the sun's disc, in steradians, 6.8e-5
CelestialLight At(Atmosphere air, float elevation, float altitude = 0f);  // The sun's light at altitude meters through air, with the sun elevation degrees over the horizon there
Vector3 DiscLuminance(Atmosphere air, float elevation, float altitude = 0f);  // The luminance of the sun's disc in candelas a square meter at altitude meters through air, the sun elevation degrees over the horizon, in linear sRGB whose luminance is the disc's, about 1.3e9 under a high sun
float DiscShare(float elevation);                        // The share of the sun's disc above the horizon with its middle elevation degrees over it, from none once its upper edge is down to all once its lower edge is up
CelestialLight Through(Atmosphere air, Vector3 outside, float elevation, float altitude = 0f);  // A light of outside in linear sRGB outside the air, whose luminance is its lux, through air to altitude meters from elevation degrees over the horizon

class SkyLight                                           // The light the sky gives, the sun's light scattered by the air toward a place, as luminance along each way from it and as illuminance on a level surface there, for the sun outside the air of Outside
SkyLight(Atmosphere air);                                // Makes the sky of air, its tables made at once over the processor's cores, a row of heights to a core, the light scattered more than once read from the transmittance's
Atmosphere Air { get; }                                  // The air the sky is made of
int GroundRowsMade { get; }                              // How many of the ground table's 13 rows are made, which PrepareGround and a first reading near an altitude make
Vector3 Luminance(Vector3 view, Vector3 sunToward, float altitude = 0f);  // The sky's luminance in candelas a square meter along view from altitude meters, with the sun toward sunToward, both with +Y up, in linear sRGB whose luminance is the sky's
Vector3 Illuminance(Vector3 sunToward, float altitude = 0f);  // The illuminance in lux the sky gives a level surface at altitude meters, with the sun toward sunToward, in linear sRGB whose luminance is the lux, the sun's own light apart
Vector3 GroundIlluminance(Vector3 lightToward, float altitude = 0f);  // The illuminance in lux the sky gives a level surface altitude meters above the ground with the light that lights it toward lightToward, as Illuminance gives it within 1%, read from a table in microseconds where the sum takes hundredths of a second
void PrepareGround(float lowest, float highest);         // Makes the ground table's rows about the altitudes from lowest to highest meters, so a reading in that span afterward makes none, a fifth of a second to most of a second a row by the processor's cores

record SceneLight                                        // The light a scene takes at an hour of the sky's clock, the sun's and the moon's at the ground, the sky's on a level surface and the exposure an incident meter gives for all of it, in lux and EV100
SceneLight(CelestialLight Sun, Vector3 SunToward, CelestialLight Moon, Vector3 MoonToward, Vector3 SkyIlluminance);
CelestialLight Sun { get; init; }                        // The sun's light at the ground on a surface facing it, by its disc's share
Vector3 SunToward { get; init; }                         // The way toward the sun, +Y up, as SkyClock gives it
CelestialLight Moon { get; init; }                       // The moon's light at the ground on a surface facing it, by its disc's share
Vector3 MoonToward { get; init; }                        // The way toward the moon, +Y up
Vector3 SkyIlluminance { get; init; }                    // The sky's light on a level surface in linear sRGB whose luminance is its lux, by the sun, by the moon and the night's own, the sun's and the moon's direct light apart
bool MoonIsKey { get; }                                  // Whether the moon lights a surface facing it more than the sun does
Vector3 KeyToward { get; }                               // The way toward the light that lights most, the moon's where it is the key
float LevelIlluminance { get; }                          // The lux on a level surface, the sun's and the moon's by the sine of their height and the sky's
float Exposure { get; }                                  // The EV100 an incident meter held level gives for LevelIlluminance, about 15 in full sun and -12 under a moonless sky
SceneLight At(SkyClock clock, SkyLight sky, float altitude = 0f);  // The light at the clock's hour, day and latitude through sky's air, at altitude meters
float ExposureFor(float illuminance);                    // The EV100 an incident meter gives for illuminance lux, log2 of the lux times 100 over ISO 2720's calibration constant of 250, 0 at 2.5 lux
SceneLight Under(WeatherConditions weather);             // This light under weather's cloud, the sun's and the moon's direct light dimmed and moved into the sky's as the cover grows, none of it direct under a full cover

class SkyClock                                           // The sky's clock, where the sun, the moon and the stars stand at an hour of a day of the year for a latitude, worked out by the usual spherical astronomy
float SynodicMonth = 29.530588f;                         // The days of a synodic month, from one new moon to the next
SkyClock();                                              // A clock at noon on the March equinox at 45 degrees north, the moon new on day 0, each until set
float Hour { get; set; }                                 // The local solar time, from midnight at 0 to 24, noon unless set
float DayOfYear { get; set; }                            // The day of the year, from 0 on the first of January to 365, the March equinox's, 79, unless set
float Latitude { get; set; }                             // The latitude, in degrees north of the equator, 45 unless set
float AxialTilt { get; set; }                            // The tilt of the earth's axis, in degrees, earth's 23.44 unless set, none keeping every day the equinox
float MoonAgeAtStart { get; set; }                       // The moon's age on day 0 of the year, the days since a new moon, 0 unless set
float MoonAge { get; }                                   // The moon's age now, the days since the last new moon, from 0 to a synodic month
float MoonPhase { get; }                                 // The moon's phase, from 0 at a new moon through 0.5 when full back to 1
float MoonLit { get; }                                   // The share of the moon's face the sun lights as seen from here, from none new to all full
float MoonPhaseAngle { get; }                            // The moon's phase angle in degrees, the angle at the moon between the sun and the earth, 180 when new, 90 at each quarter and 0 when full, which At takes
bool Refracted { get; set; }                             // Whether the sun, the moon and the stars stand at their apparent heights, lifted by the air's refraction as an eye sees them, or at their true ones, true unless set
Vector3 SunDirection { get; }                            // The way toward the sun
Vector3 MoonDirection { get; }                           // The way toward the moon
float SunAltitude { get; }                               // How high the sun stands above the horizon, in degrees, under it below 0
float MoonAltitude { get; }                              // How high the moon stands above the horizon, in degrees
Vector3 Pole { get; }                                    // The way toward the pole star, north and as high as the latitude
float SkyTurn { get; }                                   // How far the stars have turned about the pole, the local sidereal time, in radians
DayPhase Phase { get; }                                  // Which part of the day the sun's height makes it
void Advance(float hours);                               // Moves the clock on by hours, the day of the year with it
Vector3 TowardSky(float rightAscension, float declination);  // The way toward a point of the sky given in its own coordinates, its right ascension and declination in radians, as the stars stand at this hour
float ApparentAltitude(float trueAltitude, float celsius = 10f, float hectopascals = 1010f);  // The height in degrees a body trueAltitude degrees over the horizon is seen at through air at celsius degrees and hectopascals, lifted by the air's refraction, 34 minutes of arc at the horizon, one at 45 degrees and none at the zenith

class WeatherConditions                                  // The weather at a moment, how much cloud and of what kind, how hard it rains or snows, how thick the fog is and how the wind blows, which a WeatherState eases toward and an engine gives a camera's clouds and fog
WeatherConditions();                                     // Clear conditions, no cloud, rain, snow, fog or thunder and a light wind, each until set
float CloudCover { get; set; }                           // How much of the sky has cloud, from none at 0 to all at 1
float Stratus { get; set; }                              // How much of the cover is a flat deck rather than heaps, from heaps at 0 to a deck at 1
float CloudBase { get; set; }                            // The clouds' base above the ground, in meters
float CloudTop { get; set; }                             // The clouds' top, in meters
float CloudOpticalDepth { get; set; }                    // The deck's optical depth straight through it where its cloud is densest, how much of the light crossing it the cloud takes as e to the minus this, 100 unless set
float Rain { get; set; }                                 // How hard it rains, from none at 0 to a downpour at 1
float Snow { get; set; }                                 // How hard it snows, from none at 0 to a blizzard at 1
float Fog { get; set; }                                  // How thick the fog is, from clear air at 0 to some eighty meters seen at 1, the visibility between thinning evenly in its logarithm (Haze)
float Thunder { get; set; }                              // How many strikes of lightning a minute
float WindSpeed { get; set; }                            // How fast the wind blows, in meters a second
float WindFrom { get; set; }                             // Where the wind comes from, in degrees clockwise from north, a north wind at 0 blowing south
float Gusts { get; set; }                                // How much the wind gusts, from steady at 0 to its speed swinging by more than half at 1
float Temperature { get; set; }                          // The air's temperature, in degrees Celsius, under which rain falls as snow
WeatherConditions Copy();                                // A copy, to change without changing these
WeatherConditions Lerp(WeatherConditions from, WeatherConditions to, float share);  // The weather part of the way from one set of conditions to another, each number that far between the two
WeatherConditions Of(WeatherPreset preset);              // The conditions a preset names

class Haze                                               // The air near the ground between the eye and what it sees, clear and in fog, as the share of a thing's light that comes through a distance and the share the air hides, in linear sRGB, and as the visibility in meters
float Contrast = 3.912f;                                 // The optical depth at which a black thing's contrast against the horizon falls to 2%, the eye's threshold, 3.912, by which Koschmieder's law turns an extinction into a visibility
float FogVisibility = 80f;                               // The visibility in meters of the thickest fog, at Fog 1
float Visibility(Atmosphere air, WeatherConditions? weather = null);  // How far a black thing can be told from the horizon through air at the ground in weather's fog, in meters, 178 km in the Earth's clear air, 44 km in an ordinary day's and 80 m in the thickest fog
Vector3 Extinction(Atmosphere air, WeatherConditions? weather = null);  // The share of a thing's light the air at the ground takes in a meter, in each channel, the clear air's with weather's fog added alike in all three
Vector3 Transmittance(Atmosphere air, float distance, WeatherConditions? weather = null);  // The share of a thing's light that comes through distance meters of air along the ground in weather's fog, in each channel
Vector3 Share(Atmosphere air, float distance, WeatherConditions? weather = null);  // The share of a thing's light the air hides over distance meters along the ground in weather's fog, in each channel, which the air's own light fills, in green 0.00055 at 25 meters and 0.022 at a kilometer in the Earth's clear air and 0.0022 and 0.085 in an ordinary day's

class Moon                                               // The moon's light outside the air by its phase and where it reaches the ground through it
float AngularDiameter = 0.518f;                          // How wide the moon's disc looks from the earth, in degrees, its mean of 0.518
float EarthshineShare = 8E-05f;                          // The share of the full moon's luminance the moon's dark part shows by a full earth's light
float SolidAngle { get; }                                // The solid angle of the moon's disc, in steradians, 6.4e-5
float DiscShare(float elevation);                        // The share of the moon's disc above the horizon with its middle elevation degrees over it, from none once its upper edge is down to all once its lower edge is up
float LitShare(float phaseAngle);                        // The share of the moon's disc the sun lights as seen from the earth, at a phase angle of phaseAngle degrees, all of it when full and none when new
float LitLuminance(float phaseAngle);                    // The luminance in candelas a square meter of the moon's sunlit part outside the air, at a phase angle of phaseAngle degrees, its illuminance spread over the lit share of its disc, 4,900 when full
float EarthshineLuminance(float phaseAngle);             // The luminance in candelas a square meter of the moon's dark part outside the air, which the earth's sunlit side lights, at the moon's phase angle of phaseAngle degrees, the most at a new moon, when the moon sees the earth full, and none when full
float OutsideIlluminance(float phaseAngle);              // The moon's illuminance outside the air in lux on a surface facing it, at a phase angle of phaseAngle degrees, about 0.31 when full
CelestialLight At(Atmosphere air, float elevation, float phaseAngle, float altitude = 0f);  // The moon's light at altitude meters through air, the moon elevation degrees over the horizon there at a phase angle of phaseAngle degrees

class Night                                              // The light of a clear night with no moon, the stars' and the air's own glow, away from a town's
float SkyLuminance = 0.000226f;                          // The sky's luminance in candelas a square meter, 2.26e-4, a dark site's sky of 21.7 magnitudes a square arcsecond in the visual band
float SkyIlluminance = 0.00071f;                         // The illuminance the sky gives a level surface in lux, pi times SkyLuminance, the sky taken as alike over its whole dome
Vector3 SkyColor { get; }                                // The color of a moonless sky's own glow in linear sRGB at a luminance of 1, a dark sky's B-V of 0.8
float S10 { get; }                                       // The luminance in candelas a square meter of a tenth-magnitude star in each square degree, the unit S10 the Milky Way's light is given in
float GalaxyLuminance(float rightAscension, float declination);  // The luminance in candelas a square meter of the Milky Way's starlight along a way given by its right ascension and declination in radians for the equinox of 2000, outside the air, which a dark sky shows over SkyLuminance

class Stars                                              // The bright stars, to about magnitude 6.5, with their places and their light, which an engine draws at its camera's exposure by TowardSky
float ZeroIlluminance = 2.538E-06f;                      // The illuminance in lux of a star of magnitude 0 on a surface facing it, outside the air
IReadOnlyList<Star> Catalog { get; }                     // The catalog's stars in its order, by right ascension, read once
float Illuminance(float magnitude);                      // The illuminance in lux of a star of magnitude on a surface facing it, outside the air
float Luminance(float magnitude, float solidAngle);      // The luminance in candelas a square meter of a star of magnitude spread over solidAngle steradians, as a pixel it falls in shows it, outside the air
float Temperature(float colorIndex);                     // The temperature in kelvin of a star of B-V color index colorIndex, by Ballesteros's formula
Vector3 Color(float colorIndex);                         // The color in linear sRGB at a luminance of 1 of a star of B-V color index colorIndex

record struct Star                                       // A star of Catalog, its place on the sky for the equinox of 2000, its visual magnitude and its color index
Star(float RightAscension, float Declination, float Magnitude, float ColorIndex);
float RightAscension { get; init; }                      // Its right ascension, in radians
float Declination { get; init; }                         // Its declination, in radians
float Magnitude { get; init; }                           // Its visual magnitude, smaller the brighter, Sirius's -1.46
float ColorIndex { get; init; }                          // Its B-V color index, 0.65 where the catalog gives none, the sun's

class Thunderstorm                                       // A storm's lightning about a place, striking at the rate Thunder gives, each strike placed, timed and flickered from the storm's seed, so the same seed strikes the same storm on every system
float FlashDuration = 0.55f;                             // The seconds a flash lasts, its rise, its return strokes and its fall, 0.55
Thunderstorm(uint seed = 0);                             // Makes a storm whose strikes come from seed
uint Seed { get; }                                       // The seed the strikes' times, places and flickers are made from
float Nearest { get; set; }                              // The nearest a strike lands to the place, in meters, 150 unless set
float Farthest { get; set; }                             // The farthest a strike lands from the place, in meters, 4,000 unless set
float GroundShare { get; set; }                          // The share of strikes that reach the ground, a quarter unless set, the rest flashing in the cloud
float PeakLumens { get; set; }                           // A flash's light at its brightest, in lumens, 4e9 unless set
Vector3 FlashColor { get; }                              // A flash's color in linear sRGB at a luminance of 1, a bluish white as the eye sees it, set by sight
float SpeedOfSound(float celsius);                       // The speed of sound in meters a second in dry air at celsius degrees, 343 at 20 and 331 at freezing, the air taken as an ideal gas
IReadOnlyList<LightningStrike> Advance(float seconds, WeatherConditions weather);  // Moves the storm on seconds under weather, at its Thunder strikes a minute, its clouds' base and its air's temperature, and gives the strikes that began in that time, the earliest first
LightningStrike Strike(WeatherConditions weather, Vector3? at = null);  // Strikes at once under weather, at at on the ground from the place under the eye, or where the storm chooses
float Envelope(float elapsed, uint seed);                // How bright a flash is from 0 to 1, elapsed seconds after it began, a rise in its first 22 ms and a fall, with two or three return strokes at seed's times brightening it again, scaled so its brightest is 1, none before it and after FlashDuration
float Illuminance(float distance, float envelope);       // The lux a flash gives a surface facing it distance meters off at envelope of its brightest, its lumens spread over the sphere about it, the air between left out

record struct LightningStrike                            // A strike of lightning about a place, where it lands or flashes, how far off, when its thunder is heard there and the seed its flash flickers by
LightningStrike(Vector3 Position, float Distance, float ThunderDelay, bool ToGround, uint Seed);
Vector3 Position { get; init; }                          // Where the bolt lands or the flash stands, from the place on the ground under the eye, +Y up, on the ground for a strike to it and at the clouds' base for a flash in the cloud
float Distance { get; init; }                            // How far off along the ground, in meters
float ThunderDelay { get; init; }                        // The seconds from the flash until its thunder is heard
bool ToGround { get; init; }                             // Whether the bolt reaches the ground, or the flash stays in the cloud
uint Seed { get; init; }                                 // The seed of the flash's flicker (Envelope)

struct LightningFlash                                    // The flicker of one strike's flash, made from its seed once as the strike begins and read through the flash, from 0 to 1, as Envelope gives it
LightningFlash(uint seed);                               // Makes the flash of seed, a strike's own
float Brightness(float elapsed);                         // How bright the flash is from 0 to 1, elapsed seconds after it began, none before it and after FlashDuration

class Colorimetry                                        // Colors in linear sRGB, its primaries and D65's white, and the color temperatures of light, to and from a black body's
float Luminance(Vector3 rgb);                            // The luminance of a color in linear sRGB, by BT.709's weights, so a color of luminance 1 carries a hue alone
Vector3 FromTemperature(float kelvin);                   // The color of a black body at kelvin, from 1,667 to 25,000, in linear sRGB at a luminance of 1
float TemperatureOf(Vector3 rgb);                        // The correlated color temperature of a color in linear sRGB, the black body's whose color is nearest it in CIE 1960's uv, 0 for no light, PositiveInfinity for a light bluer than any black body and 1,000 K for one redder than a black body at 1,000 K

enum DayPhase                                            // Which part of the day the sun's height makes it
Night = 0,                                               // The sun more than twelve degrees under the horizon, the sky dark
Dawn = 1,                                                // The sun between twelve degrees under the horizon and six above it, in the morning
Day = 2,                                                 // The sun more than six degrees above the horizon
Dusk = 3,                                                // The sun between six degrees above the horizon and twelve under it, in the evening

class WeatherState                                       // The weather as it goes, its conditions easing toward the ones it is given or a forecast writes, the wind gusting, the ground wetting in rain and whitening under snow
WeatherState();                                          // Clear weather easing toward clear, its forecast off and its seed 0, each until set
WeatherConditions Current { get; }                       // The conditions now
WeatherConditions Target { get; }                        // The conditions being eased toward
float HalfLife { get; set; }                             // How many seconds of the world's clock the conditions take to come half the way to the target, 30 unless set
bool Forecast { get; set; }                              // Whether a forecast writes the target from the clock every step, off unless set
WeatherClimate Climate { get; set; }                     // What weather the forecast brings, Temperate unless set
float SystemsPerDay { get; set; }                        // How many weather systems pass a day in the forecast, 1.5 unless set
uint Seed { get; set; }                                  // The seed the forecast is made from, so two worlds' weathers differ, 0 unless set
SkyClock? Clock { get; set; }                            // The clock the forecast reads the day and the hour from, or none to keep its own
Vector3 Wind { get; }                                    // The wind's velocity now, in meters a second, gusts and all
Vector3 Carried { get; }                                 // How far the wind has carried what it carries since the state began, in meters
float Wetness { get; }                                   // How wet the ground is, from dry at 0 to soaked at 1
float SnowCover { get; }                                 // How much of the ground lies under snow, from none at 0 to all at 1
void Set(WeatherPreset preset, bool immediately = false);  // Gives the weather a preset to ease toward, or to take at once
void Set(WeatherConditions target, bool immediately = false);  // Gives the weather conditions to ease toward, or to take at once
void Advance(float seconds);                             // Moves the weather on, its conditions, its wind and the ground
WeatherConditions ForecastAt(float dayOfYear, float hour);  // The conditions the forecast gives a day of the year and an hour

enum WeatherPreset                                       // A set of conditions the weather eases into (Set), each as Of gives it
Clear = 0,                                               // Cloudless, dry, a light breeze
FewClouds = 1,                                           // A few fair-weather cumulus
PartlyCloudy = 2,                                        // Scattered cumulus over about half the sky
Overcast = 3,                                            // A solid gray deck of stratus, with no rain
Hazy = 4,                                                // Thin high cirrus over a mostly blue sky
Drizzle = 5,                                             // A light, steady drizzle
Rain = 6,                                                // Rain under a thick deck
Storm = 7,                                               // Heavy rain, a strong wind and a low cloud base
Thunderstorm = 8,                                        // A storm with frequent lightning and towering cloud
LightSnow = 9,                                           // A gentle snowfall in still air
Snow = 10,                                               // Steady snow under an overcast sky
Blizzard = 11,                                           // Heavy snow in a gale, with next to nothing to be seen
Fog = 12,                                                // Dense fog on the ground under a flat sky
MistyMorning = 13,                                       // A low mist at dawn, clearing above
Sandstorm = 14,                                          // A dry, dusty haze in a hot wind

enum WeatherClimate                                      // What weather a WeatherState's forecast brings, how stormy and how cold
Temperate = 0,                                           // Fair spells and rainy ones in turn, cold enough in winter for snow now and then
Maritime = 1,                                            // Often cloudy and wet off a sea, mild all year
Arid = 2,                                                // Mostly clear and dry and hot, a rare storm
Tropical = 3,                                            // Warm and humid, the cloud building and the rain coming in heavy bursts
Polar = 4,                                               // Cold all year, its storms snow

class LookCycle                                          // A look that changes with the sun's height, a timecycle of looks kept at heights of the sun, the look at any height worked out between the two kept about it
LookCycle();                                             // A cycle with no looks kept, which Add fills
IReadOnlyList<(float Degrees, LookSettings Look)> Keys { get; }  // The looks kept, by the sun's height in degrees, lowest first
LookCycle Add(float degrees, LookSettings look);         // Keeps a look at a height of the sun, replacing one kept there
LookSettings At(float degrees);                          // The look at a height of the sun, between the two kept about it
LookCycle Day();                                         // A day's cycle on Vivid, the vivid look by day, warmer and deeper as the sun goes down to the horizon, bluer and calmer through the twilight, and cool at night

class LookSettings                                       // How a camera turns its linear picture into the one the screen shows, which an engine runs in place of its tonemapper
LookSettings();                                          // The look that changes nothing under the knee, each number until set
LookCurve Curve { get; set; }                            // The curve that brings light past white under it, Knee unless set
float Exposure { get; set; }                             // What the light is multiplied by before the curve, 1 unless set
float Knee { get; set; }                                 // The level past which Knee bends each channel toward white on its own, 0.8 unless set
float KneeStrength { get; set; }                         // How far Knee lets each channel past the knee go toward white on its own, from 0, where a color keeps its hue however bright, to 1, unless set
float CurveExposure { get; set; }                        // What Numbered multiplies the light by first, 1.4 unless set
float CurveWhite { get; set; }                           // How sharply Numbered brings light under white, 2 unless set, higher holding more of it near white
float CurveLower { get; set; }                           // The power Numbered raises the dark end by, 1.2 unless set, higher darkening the shadows
float CurveUpper { get; set; }                           // The power Numbered raises the bright end by, 1.5 unless set, higher darkening the highlights
float Saturation { get; set; }                           // How far each color is moved from the gray of its luminance in linear light, after the curve, 1 unless set and 0 gray
(float R, float G, float B) Tint { get; set; }           // What each channel is multiplied by after the saturation, linear, white unless set
float Contrast { get; set; }                             // How far each level shown is pushed from the middle once encoded for the screen, 1 unless set
bool Dither { get; set; }                                // Whether the picture shown is dithered by half a level, so a gradient does not band, on unless set
LookSettings Lerp(LookSettings from, LookSettings to, float share);  // A look part of the way from one to another, each number that far between the two, for a look that changes as a day goes (LookCycle)
LookSettings Vivid();                                    // A vivid look, Numbered at an exposure of 0.9, a white of 1.3 and powers of 1.05 and 1, the light raised by 1.6 before it, a saturation of 1.4 and a contrast of 1.12

enum LookCurve                                           // The curve a camera's look brings light past white under by
Knee = 0,                                                // The brightest channel left alone up to 0.9 and bent smoothly toward white past it, the other two scaled with it, so a bright orange stays orange where a clamp per channel turns it white, with each channel past Knee let go toward white on its own by KneeStrength
Numbered = 1,                                            // A curve set by four numbers, the light times CurveExposure, brought under 1 by a white curve of CurveWhite, then raised by a power running from CurveLower in the dark to CurveUpper in the light, and shown at a gamma of 2.2

record struct CelestialLight                             // The light of the sun or the moon where it reaches a place, its illuminance on a surface facing it, its color and that color's temperature
CelestialLight(float Illuminance, Vector3 Color, float Temperature);
float Illuminance { get; init; }                         // Lux on a surface facing the light, 0 where it is below the horizon
Vector3 Color { get; init; }                             // The light's color in linear sRGB at a luminance of 1, so the illuminance carries all of its strength
float Temperature { get; init; }                         // The color's correlated color temperature in kelvin, 0 where the light is out
```
