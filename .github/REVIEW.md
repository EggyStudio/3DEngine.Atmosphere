# Review

Reviewed up to `a9573d6`, the common noun's spelling, catalog, in the sources, the catalog's head
and a test's name, the Bright Star Catalogue keeping its name, pushed at 13:30 on 2026-11-08, its
run green at 13:31 on the three systems in under a minute each; it keeps the form. Nothing is pushed
after it. Before it `ff8f9df`, item 5, the night's model by the 3DEngine session, pushed at 12:31 on
2026-11-08, its run green at 12:32 on the three systems in under a minute each; it keeps the form,
carries the catalog as a resource with its source in the file's head and references no engine.
Before it `0e5864f` and `65b663d`, the review's files alone, pushed at 12:34 on 2026-11-05 and 14:51
on 2026-11-03, with no run since the build passes over the review's files; they keep the form.
Before it `fbc385e`, item 3's first part by the BevyCSharp session, pushed at 20:42 on 2026-11-02,
its run green at 20:44 on the three systems in about a minute each; it keeps the form and references
no engine. Before it `a5e474b`, item 3's second part by the 3DEngine session, pushed at 23:34 on
2026-11-01, its run green at 23:36 on the three systems in about a minute each; it keeps the form
and references no engine. Before it `bff9e47`, item 2, pushed at 14:15 on 2026-11-01, its run green
at 14:18 on the three systems in about a minute each, the repository's first, the skeleton as
Vegetation's with the name changed. Before it `c5ee6a9`, item 1, the documents as the other modules
keep them with the opening paragraph saying what this module is for; the commit keeps the form, the
three-mark subject, one sentence, the owner's identity as author and committer, no trailer (N 7.2
and N 7.6 kept at the first commit). Before it `cad5537`, this review and the norm, and `17aeab0`,
the owner's first commit alone, made at 13:47.

## Now

The order the owner set on 2026-10-31 for the modules, this one holding the sky's clock, the sun's
and the sky's light in physical units, the atmosphere's parameters, the weather and the looks by the
sun's height, the model both engines light from while each draws its own sky. The documents first,
then the rest as the other projects have it, then the engines taking the module. Items 1 and 2 are
the game's session's, as for the six before; item 3 is in two parts by the session that owns each;
item 4 is each engine's session's. This module comes when 3DEngine's item 170 starts, which writes
its model here from the first commit rather than moving it later.

1. **The documents, one commit.** AGENTS.md on the game's shape (one paragraph saying what the
    module is and that no engine is referenced, N 2.1; the documents table with STYLE, COMMITS,
    REVIEW, NORM and DEVLOG; building and testing; Where things are with a row a folder; the
    conventions, the commit form among them), README.md (what the module is, how an engine takes it
    as a submodule, `dotnet build` and `dotnet test`), `.github/STYLE.md`, `COMMITS.md` and
    `CODE_OF_CONDUCT.md` as the other modules have them, `.gitignore` of the engines' shape with
    `/.github/dump/` in it, `.gitattributes`. Vegetation's documents are the models; nothing is
    written that the module does not have yet. Read by the reviewing session before it is pushed.

2. **The skeleton, one commit after item 1.** `Sky.slnx`, `Sky/Sky.csproj` as a class library on
    net10.0 with `Nullable` and `ImplicitUsings` in `Directory.Build.props` and
    ImportDirectoryBuildTargets false, `Sky.Tests/` on xunit with one test that the library loads,
    `build/version.txt` at `0.1` and `build/version.sh` as the engines', and
    `.github/workflows/build.yml` as Vegetation's, `test (os)` on ubuntu-24.04, windows-latest and
    macos-15 with `dotnet build Sky.slnx -warnaserror` and `dotnet test Sky.Tests --no-build`, a
    time limit on every job with its reason, `permissions` read and the engines' concurrency group.
    Green on the three systems before item 3.

3. **The model, in two parts.** The first part is BevyCSharp's SkyClock, the sun, the moon and the
    stars by spherical astronomy for an hour, a day and a latitude, WeatherState, the conditions
    easing toward a target or a forecast, and LookCycle, the looks keyed by the sun's height, which
    reference no engine and move as they are with their tests, by the BevyCSharp session after its
    items 90 and 68. The second part is what 3DEngine's item 170 needs and writes here from the
    start, by the 3DEngine session, the sun's illuminance in lux and color temperature by its
    elevation and the air's turbidity, the sky's luminance on the same scale, the moon's and the
    night's, and the atmosphere's parameters as data (the planet's radii, Rayleigh, Mie and ozone),
    read against the reference scene's figures, 97,050 lux at 5,633 K with the sun 54 degrees high.
    Each engine draws its own sky from the module's numbers, BevyCSharp through Bevy's atmosphere,
    3DEngine through item 170's pass, so the two light from one sun and one sky. One commit a part,
    the strict build and the suite green, a Shared line in the engines' REVIEW.md when a part lands.
    The second part is done at `a5e474b` on 2026-11-01, read at 23:34: Atmosphere as HDRP's Earth in
    data with turbidity from the aerosol's and Rayleigh's depths at 550 nm and a transmittance over
    a round planet, Sun.At in lux, color and kelvin, SkyLight as single scattering with Hillaire's
    multiple-scattering term in candela a square meter and lux, Moon after Krisciunas and Schaefer,
    Night and Colorimetry, 20 tests, the strict build clean, no engine referenced. The transmittance
    at 53.97 degrees 0.926, 0.850 and 0.710 against HDRP's 0.925, 0.849 and 0.710; the sun at the
    ground 109,960 lux at 5,026 K; the reference scene's light of 97,050 lux at 5,633 K stands
    outside the air and HDRP's light loop multiplies it by that transmittance, 73,900 lux at 4,924 K
    at the ground, within 100 K of the module, the lux two thirds of it since the scene's time of
    day sets 120,000 times the sine of the height; the scene's sky at intensity mode Multiplier at
    1, its 20,000 lux never read, the sky 7,860 lux on a level surface at 54 degrees, which
    3DEngine's item 170 takes in place of the example's 20,000. The first part, BevyCSharp's clock,
    weather and looks, follows its items 90 and 68. The first part is done at `fbc385e` on
    2026-11-02, read at 20:42, SkyClock, WeatherState and LookCycle with the six types they stand
    on, DayPhase, WeatherConditions, WeatherPreset, WeatherClimate, LookSettings and LookCurve,
    under the namespace Sky with Vector3 from System.Numerics, the crefs into the engine as prose
    and two summaries written engine-neutral; SkyClockTests and WeatherStateTests whole and five of
    LookCycleTests, 40 tests where 20, the strict build clean; the README's table gaining the three.
    Both parts landed, the item is done; BevyCSharp takes the module under its item 91 and 3DEngine
    has it since `bd081fa6`. Two doc comments in LookSettings.cs name 3DEngine in prose, its frame's
    order and its block game's look; no type or package is referenced, so N 2.1 holds, and the two
    sentences are written engine-neutral with the next commit that touches the file.

4. **The engines take the module.** Each engine adds this repository as a git submodule under
    `modules/` and references its project, its workflows checking submodules out, its own clock,
    light and weather code becoming the binding of the module's model (N 2.1) and the rest deleted;
    a change used by an engine is committed here first and pushed before the engine's commit that
    moves the pointer. One commit an engine, after item 3's part that engine uses. 3DEngine takes
    the module at its `bd081fa6` (item 170's first commit), read at 00:14 on 2026-11-02, the
    submodule under modules/ at `a5e474b`, referenced with PrivateAssets all so its package carries
    the module's assembly and documentation, every building job checking submodules out;
    BevyCSharp's turn comes with its item 91. BevyCSharp takes the module at its `1ea6d865` (item
    91), read at 21:35 on 2026-11-02, the submodule under modules/ at `fbc385e`, referenced with
    PrivateAssets all, its nine files out and its users reading the module. Both engines take the
    module; the item is done.

5. **The night's model.** From 3DEngine's item 221 on 2026-11-08, the owner having found Sponza's
    night sky a field of white blobs with a disc many times the moon's size. The module holds the
    moon's illuminance and Night's luminance and no stars, no earthshine and no Milky Way, so the
    night's model comes here, written by the engine's session through the submodule as the
    vegetation was, a catalog of the bright stars to about magnitude 6.5, their places and visual
    magnitudes with a color index, as a compact data file with its source named in a comment, read
    into star places by SkyClock's sidereal time for a latitude and a longitude as the sun and the
    moon are placed; the moon's earthshine as the dark side's luminance by the earth's phase in
    Moon; the Milky Way's band as a luminance along the galactic plane by a simple profile, with its
    figures from the literature in the comment; and a star's magnitude to a luminance and an
    illuminance so an engine can draw it at a camera's exposure. The drawing stays each engine's
    (Decision 6). Tests hold a known star's place at a known hour against an almanac, the moon's
    earthshine against its figure and the magnitude scale at 0 and at 6. After item 4. Done at
    `ff8f9df`, its run green at 12:32 on 2026-11-08, the module's tests 49; the common noun's
    spelling, catalog, in `a9573d6`, pushed at 13:30.

## Verdicts

None yet.

## Decisions

1. **The repository's birth.** The owner made this repository on 2026-11-01 at 13:47, the seventh
    module beside Modeling, Vegetation, Levels, Landscape, RadianceCascades and Tools, after asking
    at 13:4x whether the sky, the atmosphere and the weather belong in a repository of their own so
    both engines use them, or whether BevyCSharp keeps Bevy's sky and 3DEngine builds its own, and
    reading the answer that the model is shared and the drawing is each engine's. The order is the
    modules', the documents first, then the rest as the other projects have it, then the engines
    taking the module.

2. **A git submodule, not a package.** The owner chose on 2026-10-31 that the engines take a module
    as a git submodule built as a project of their own, since a package's upload, wait, test and
    pack is slower to iterate than a commit. A change is committed here first; the reviewing session
    pushes this repository before the engine's commit that moves its pointer; the workflows check
    submodules out. The game keeps using the engine through the engine's package.

3. **The module knows no engine.** N 2.1. The module references no engine, package or bridge; what
    it needs of a renderer, a world, a scheduler or a file system it takes through interfaces it
    declares, which each engine implements where it binds the module. A type of an engine's in this
    repository is a fault.

4. **The repository's terms are the owner's.** The repository was made by the owner with its terms
    file in place, which stands as the owner set it and is spoken of nowhere else.

5. **Who writes here.** The game's session writes items 1 and 2, as for the six modules before; the
    BevyCSharp session moves its clock, weather and looks (item 3's first part) and the 3DEngine
    session writes the sun's and the sky's light and the atmosphere's parameters here as its item
    170 begins (item 3's second part); each engine's session binds the module (item 4). Every
    session reads REVIEW.md's head, its items and the Replies before a batch, as N 7.7 says, and the
    reviewing session pushes.

6. **The model is shared, the drawing is each engine's.** The sky's clock, the sun's and the sky's
    light in lux and kelvin, the atmosphere's parameters, the weather and the looks by the sun's
    height are arithmetic and data no engine owns, so both engines light from one set of numbers and
    the reference scene's figures come out of the same astronomy in both. The drawing is not shared,
    since Bevy's atmosphere is a component of its renderer in WGSL and 3DEngine's pass is Slang over
    Vulkan, and nothing is gained by moving either; BevyCSharp keeps Bevy's atmosphere, fog and
    clouds, 3DEngine builds its pass under item 170, and each feeds its sky from the module. Clouds'
    drawing and rain stay with each engine and come after the global illumination items (3DEngine
    Decision 79).

7. **The night comes to the model.** The owner wrote in the engine's session on 2026-11-08 that the
   night sky looks really bad, never updated from the voxel game's, while the day looks super good,
   so the night's model, the stars, the moon's earthshine and the Milky Way, comes to this module
   (item 5), the engines drawing it as they draw the day, Decision 6 standing; the engine's session
   writes it through the submodule, the review pushing the module before the engine that points at
   it.

8. **The module is worked by a session of its own.** Decided at 16:43 on 2026-11-18 for the seven
    modules at once. From its first commit the module's session alone writes the module's source,
    tests and documents, and the engines' sessions write here no longer; an engine's coder that
    needs something of the module asks the module's session in a message and takes the commit by
    moving its submodule's pointer. Every open item above that names an engine's or the game's
    session as its worker is the module's session's, apart from the binding inside an engine, which
    stays that engine's. The session's first Reply reads this file against the tree and says what is
    done, what is open and the order it proposes; the review answers, and that order stands. The
    rules are NORM.md's as before, among them no engine referenced (N 2.1), tests that need no
    window and no GPU, the commit's form, a failing test before a mend, a picture on a white ground
    where a thing is seen, and any GPU program through the lock of N 7.9 with a waiting file. The
    session reports each checked commit to the reviewing session, which reads it, pushes it and
    reads its run; it does not push.

## Replies

Item 3's second part, from 3DEngine's item 170, is in the commit after `bff9e47`. It holds
`Atmosphere` with HDRP's Earth as data and turbidity as 1 plus the aerosols' optical depth over the
molecules' at 550 nm, `Sun.At` (lux, color, kelvin), `SkyLight` (Hillaire's single and multiple
scattering in candelas a square meter and lux), `Moon`, `Night` and `Colorimetry`, with 20 tests.
Read against the scene in HDRP, the transmittance at 53.97 degrees is 0.926, 0.850 and 0.710 here
against 0.925, 0.849 and 0.710 by HDRP's own formula. The sun reaches the ground at 109,960 lux and
5,026 K here, where the scene's light, 97,050 lux at 5,633 K outside the air, reaches 73,900 lux at
4,924 K through HDRP's transmittance, which its light loop multiplies the light by. The color holds
within a hundred kelvin, and the lux is two thirds of the sun's here, since the scene sets 120,000
lux times the sine of the sun's height outside the air. Its sky volume's intensity mode is 2,
Multiplier in HDRP's order of Exposure, Lux and Multiplier, at 1, so its 20,000 lux is never read,
and its sky is the atmosphere's lit by that light, 7,860 lux on a level surface at 54 degrees here
for a sun of 128,000. The scene's TimeOfDay puts the sun at 16.55, 51.40 and 53.97 degrees at 8:00,
12:00 and 15:05 on 27 June of its year 75.

Item 3's first part, from BevyCSharp's item 91, in the commit carrying this line. SkyClock,
WeatherState and LookCycle move from BevyCSharp as they are, with the six types they stand on,
DayPhase, WeatherConditions, WeatherPreset, WeatherClimate, LookSettings and LookCurve, none of
which references an engine. The changes on the way are the namespace, Sky, BevyCSharp's Vec3
becoming System.Numerics' Vector3 as Sun and Atmosphere have it, and the five references to
BevyCSharp's Render in the documentation said in words, an engine's night sky, weather and look.
Their tests move with them, SkyClockTests and WeatherStateTests whole and the five of LookCycleTests
that need no engine, the one that draws staying in BevyCSharp beside the look's other drawn tests.
The README's table names the three. The strict build is clean and the tests pass, 40 where there
were 20 (dotnet test Sky.Tests). BevyCSharp's commit that takes the module follows once this one is
pushed.

Item 5 done in the commit after `0e5864f`. `Stars.Catalog` holds the Bright Star Catalogue's 9,096
stars with a place (Hoffleit and Warren, 1991, the Smithsonian Astrophysical Observatory's
bsc5.dat), right ascension and declination for 2000, visual magnitude and B-V, as
`Data/BrightStars.csv`, 246 KB carried as a resource, placed by `SkyClock.TowardSky`. The clock's
hour is the local solar time, so the longitude is in it already and the clock takes none.
`Stars.Illuminance` gives 2.54e-6 lux at magnitude 0 by the zero point `Night.SkyLuminance` is
given by, `Stars.Luminance` a star spread over a pixel's solid angle and `Stars.Color` a black
body's by Ballesteros's temperature. `Moon` holds the disc's 0.518 degrees and 6.4e-5 steradians,
its sunlit part's luminance, 4,870 candelas a square meter full, and the earthshine on its dark
part, 8e-5 of the full moon's at a full earth by the earth's light on it, falling with the earth's
lit share. `Night.GalaxyLuminance` gives the Milky Way's band in galactic coordinates, 100 to 1,000
S10 along the plane from the anticenter to the center, of the order Leinert and others give, a
third each 8 degrees off it. Tests hold Sirius at the almanac's place and crossing the meridian at
midnight on the first of January 28.3 degrees up at 45 north, Polaris within a degree of the pole
at every hour, the magnitude scale at 0, 5 and 6 and the moonless sky's luminance at 21.7
magnitudes a square arcsecond by it, the earthshine against its figure and the band at its center,
anticenter and pole, the module's tests 49 in all.

The module's session, its first Reply under Decision 8, read against the tree at `789f0e1` with
the review's files as they stand in it. Items 1 to 5 are done as their text says, and the tree
holds them, the strict build clean and the 49 tests passing (`dotnet build Sky.slnx -warnaserror`
and `dotnet test Sky.Tests --no-build`), no engine referenced by a type, a package or a project.
Three things are open. The two sentences of item 3's last note name 3DEngine in LookSettings.cs at
lines 12 and 108. DEVLOG.md is in AGENTS.md's table and not in the tree; Tools' `scripts/devlog.py`
writes it with items 3 and 5 alone, since items 1, 2 and 4 name no closing commit in their text.
Sixty-five comment lines run past 100 columns, 53 of them in the nine types moved from BevyCSharp
and 12 in Moon, Stars and Star, and a comment in SkyLight.cs breaks mid-sentence at lines 154 to
156. The summary of `LookSettings.Dither` names Bevy's tonemapper and `LookCurve.Numbered`'s
remarks name the BSL shaders and Minecraft, each where a rule or a number is taken from it, as
STYLE.md allows, and they stay. The order proposed is the two sentences written engine-neutral with
DEVLOG.md generated and the review's files, one commit begun with this Reply; then 3DEngine's
request for the sky's lower half as soon as its coder's message comes, ahead of the rest since an
engine waits on it, a failing test first where it mends; then the comments wrapped to 100 columns
and the broken one joined, no code changed, one commit.

The module's session, what the module lacks for both engines to light from it alike, read from its
public members and from where each engine reads them. 3DEngine reads the air, the moon, the stars
and the night's floor, and computes its own sun path, sun disc, moon place and night switch at
twelve degrees down; BevyCSharp reads the clock, the weather and the looks and sets every light in
its own units by curves of the hour, its sun 12,500 lux outside the air, its moon a fixed blue, its
exposure switched at fixed hours, its stars hashed and its Milky Way hard-coded, the night's model
not yet under its pointer at `fbc385e`. Moving those curves onto the module is each engine's
binding. What the module adds so the two bindings land on one set of numbers is proposed in this
order, one commit each, a failing test before each.
1. The sun's disc, its angular size and solid angle beside the moon's and its luminance at the
   ground, with the light through the air for a sun of another color or strength made public, both
   of which 3DEngine writes for itself in `DaySky.Air.cs`.
2. The moon's phase angle in degrees from `SkyClock`, which `Moon.At` takes and each engine works
   out from the clock's share of the month in its own way.
3. The scene's light by the clock, the sun's and the moon's `CelestialLight`, the sky's illuminance
   by each of them and the night's floor, handed from one to the next through the twilight by one
   rule, with an exposure in EV100 for that light, so a scene at an hour is lit and exposed alike in
   both engines.
4. The weather in the light, the cloud cover and density dimming the sun's direct light and moving
   it into the sky's, the fog as a visibility and an extinction in meters by Koschmieder's 3.912,
   and the haze and the rain as the air's turbidity, where the conditions change only the drawing
   in either engine.
5. The lightning, BevyCSharp's strikes as a Poisson process of `Thunder` with their flash's
   envelope, distance and the thunder's delay at 343 meters a second, which needs no engine and
   which 3DEngine has none of.
3DEngine's request for the sky's lower half goes ahead of all of these when its message comes.
Item 1 is begun.
