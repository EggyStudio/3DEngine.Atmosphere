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

1. **The disc's share is not exactly one on arm64.** Run 44341456796 of `1fc8aca` at 16:57 on
    2026-11-18 is red on macos-15 alone, Ubuntu and Windows green.
    `SunTests.The_Share_Of_A_Disc_Above_The_Horizon_Goes_From_None_To_All_As_Its_Edge_Rises` expects
    1 and reads 0.99999994, the share's arithmetic rounding apart there. The mend is the function's,
    a disc wholly above the horizon giving 1 and one wholly below giving 0 by a comparison before
    the arithmetic, in the sun's and the moon's share alike, with the test's ends kept exact and the
    values between held to a stated tolerance. `9a096f0` waits unpushed and goes out with the mend.

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

9. **The module is agnostic enough that a stranger binds it in an engine of their own from its
    README alone (2026-11-18 17:28).** It asks a README section, Binding it in your engine, with
    every interface the module declares, the conventions it stands on and the smallest binding that
    works, a small host in the tests that implements those interfaces with no engine and that the
    section's steps are run against, a test that fails where the library references an engine, a
    window or a graphics API, the form in the Tools module's ReferenceTests.cs, and no engine's name
    in a public member or its summary. The two engines appear in the README as examples. A public
    shape that fits the two engines alone is named in a Reply before it changes.

10. **The package id and the assembly are 3DEngine.Sky and the namespace stays Sky (2026-11-18
    17:28).** The prefix gives a unique name. A package is published when the time is right and on
    the owner's word, for a reader who takes the module alone. The engines keep taking the module as
    a git submodule, compiled with them and carried inside each engine's one package. Folders and
    project file names stay.

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

The module's session, the twilight's mend. The table of the light scattered more than once held 32
angles of the sun and was read between them as a straight line, which lit 8 degrees down with 1.02
lux and 12 down with 0.0023 where a table of 512 angles gives 0.31 and 0.00054. It holds 64 angles
and their light's logarithm, read between them as a straight line, which comes within 1% of the
512 at 8 degrees down and within 3% at 12; the day's figures stand, the sky's 7,860 lux at 54
degrees among them. The test reads the slope of the light's logarithm every quarter degree to 16
degrees down, which moved by 0.51 at 9 degrees down and moves by 0.071 at most, failing over 0.1.
Making `SkyLight` takes 36.0 ms where it took 25.3 on 32 cores, and a call of `SceneLight.At` 14
ms before and after, each the mean of a release build's repeated calls in a probe outside the
repository. The table can be shared. The light on a level surface depends only on the height of
the sun or the moon lighting the sky and on the place's altitude, so `SkyLight` can keep a table
of its illuminance by the cosine of the light's zenith at the ground, made with its others, which
`SceneLight.At` reads in microseconds, an altitude other than the ground's summed as it is. That
commit is proposed next, before part 4, with a test that the table's reading holds within 1% of
the sum at every height of the sun.

The module's session, the ground's table. `SkyLight.GroundIlluminance` reads the sky's light on a
level surface at the ground from a table by the height of the light that lights it, and
`SceneLight.At` reads it at the ground, a call taking 10 microseconds where it took 14 ms. The
table starts with a row a degree from 20 degrees down to the zenith and halves a span while the
light at its middle and the table's reading there differ by more than 0.3% of the larger of the
channel and the lux, 189 rows in all, crowding to a 32nd of a degree 8.2 degrees down. There the
sum's blue bends sharply as the earth's shadow overhead passes the top of the air, its logarithm's
slope doubling within a quarter degree, which a finer transmittance table sharpens rather than
removes, so the bend is the model's; a table of even rows an eighth of a degree apart missed it by
3.3%. Under 20 degrees down it gives none, the sum being under 2e-7 lux there. Two tests hold it
within 1% of the sum, each channel against the larger of itself and the lux, at heights between
the rows from 19.9 degrees down to the zenith and through a day and a night of the scene with a full
moon; a channel is held to the lux where it is dim, since the sum's blue in the deep twilight, a
thousandth of the night sky's light, wavers by a few percent from one tenth of a degree to the
next. The table is made at its first reading, 206 ms on 32 cores and 797 ms on 4 in a warm process,
once a `SkyLight`, and making a `SkyLight` stays at 36 ms; the suite takes 5 s where it took 1. The
sum itself could halve its time by the mirror of the sky about the light's vertical plane, which is
left for a commit of its own.

The module's session, part 4, the weather in the light. `Haze` gives the air near the ground
between the eye and a thing, the share of its light that comes through a distance and the share the
air hides, which the air's own light fills, and the visibility. Clear air takes the molecules' and
the aerosols' extinction of HDRP's Earth at the ground, 2.35e-5 a meter in green, a visibility of
166 km by Koschmieder's 3.912, and fog adds an extinction alike in the three channels that thins the
visibility evenly in its logarithm toward 80 m at `Fog` 1, as BevyCSharp's fog eases it. The haze
the module gives, in green, is 0.059% at 25 m and 2.3% at a kilometer in clear air, 0.4% and 15%
at `Fog` 0.25, 2.7% and 66% at 0.5, and 71% at 25 m in the thickest fog; so an atrium 25 m across
in clear air is hazed by six ten-thousandths, and a gray read there is no air of the module's.
`SceneLight.Under` gives the scene's light under the cloud cover by Kasten and Czeplak's relations
of 1980, the global light a quarter of the clear day's under a full cover, two stops down, and its
diffuse share climbing from the clear sky's own by the cover's square, the direct part exactly none
under a full cover on every processor; the moon's light and the night's own dim alike. Rain and snow
add nothing to the haze, since the presets carry their fog with them, the cloud's density and kind
are left out, and the turbidity the proposal named for the haze is the fog's extinction in its
place. Eight tests, each figure's source in its comment, and a sheet of the haze by distance and the
light by cover at `.github/assets/readings/2026-11-18-haze-and-cloud.webp`.

The module's session, the public shapes that fit only the two engines, read for the owner's
direction that a stranger can take the module into an engine of their own from its README alone.
The module declares no interface, since it asks nothing of an engine, so the README's section says
so and the tests' host is a minimal caller. Three shapes fit our engines more than any engine.
`WeatherConditions.CloudDensity` is a share of the density an engine draws its clouds at, a number
with no physical meaning outside BevyCSharp's clouds, and is proposed as an extinction in a meter
or the deck's optical depth. `WeatherConditions.Fog` runs from the camera's own fog at 0, the
engine's, where `Haze` reads 0 as clear air, and its summary is proposed to say clear air. And
`LookSettings` with `LookCurve` is a tonemap in the order both engines run it, exposure, curve,
saturation and tint in linear light, contrast once encoded and a dither, which an engine with a
tonemapper of its own cannot take without writing that pass; it is proposed to stay as it is, its
summary saying it is a look an engine runs in place of its tonemapper, as it says. None of the
three is changed before the review answers.

The module's session, part 5, the lightning. `Thunderstorm` holds what BevyCSharp's storm timed and
placed on its own, no engine's type in it. Strikes are a Poisson process at
`WeatherConditions.Thunder` strikes a minute, a share of one strike drawn from an exponential
spread spent at the rate each step, so a storm easing in strikes as its rate climbs; they land
evenly over the ground between 150 m and 4 km; a flash has BevyCSharp's envelope, a rise in 22 ms,
a fall and three return strokes at its seed's times, over in 0.55 s; its light falls as the inverse
square of its distance; and its thunder comes the distance over the speed of sound, 331.3 m/s times
the root of one plus the air's temperature over 273.15 K, 343 m/s at 20 degrees. Three departures
from the bridge's. A quarter of strikes reach the ground by default, as Rakov and Uman give three
quarters of flashes staying in the cloud, where the bridge sent two thirds, so a binding that shows
more bolts sets `GroundShare`. The flash's fall has its end value taken away, so it dies away to
none at 0.55 s where the bridge's went out from a hundredth of its peak, some 140 lux at 150 m in
one frame, which a test holds. And the storm draws its numbers by SplitMix32 rather than .NET's
`Random`, so a seed strikes the same storm on every system and runtime. The flash's 4e9 lumens at
its peak and its bluish white are the bridge's, set by sight and said so in their summaries. Nine
tests, the suite 114, and a sheet of three flashes and a storm's strikes from above at
`.github/assets/readings/2026-11-18-lightning.webp`.

The module's session, the flash's peak. A flash's shape, its rise, fall and return strokes, is
divided by its own brightest, found at 513 points through the flash, so every flash reaches 1 once
and comes to a point there; a product passing 1 was cut flat at it, seed 2's for some 30 ms, and
one that never reached 1 left a flash dimmer than `PeakLumens` says, seed 1's at 0.95. Finding
the brightest takes 513 readings of the shape, so it is made once a flash in `LightningFlash`, a
value of the strike's seed that a binding makes as the strike begins and reads every frame, and
`Thunderstorm.Envelope` makes one a call. The test reads 1,000 seeds every millisecond, each
reaching within 0.005 of 1 and none holding within 1e-5 of its brightest for more than one step,
and it failed on the cut flashes before the mend. The sheet is
`.github/assets/readings/2026-11-18-lightning-peak.webp`.

The module's session, the sum by the sky's mirror. `SkyLight.Illuminance` turns the sun to stand
at the turn of 0 and reads the dome on one side of its vertical plane, each way counted twice,
since the sky is alike either side of it. A call takes 3.78 ms where it took 6.57, and making a
`SkyLight` with its ground's table 129 ms where it took 204, in a release build's probe outside the
repository on 32 cores, the mean of 220 calls. The results are not equal to the last bit, since the
sum adds in another order. For a sun toward +X, the plane the sum stood in before, they hold within
2.2e-6 of the larger of the channel and the lux. For a sun turned about the zenith they move by
up to 0.6%, the old sum's own error, which read the dome at 48 fixed turns, so its lux moved as
the sun turned between them where the light on a level surface depends on the sun's height alone.
The new sum reads the same ways about the sun at every turn, and a test holds the lux alike within
1e-5 at any turn, which failed on the old sum by 2.7e-5 at 10 degrees high.

The module's session, the public surface against the binding section. Of the 23 public types, 22
were named in the section or a table and `LightningStrike` in neither, which joins the
`Thunderstorm` row. The read found three things the section's conventions left out, and it says
them: a compass direction is in degrees clockwise from north, as the wind's, whose vector keeps the
section's axes, a north wind blowing toward +Z; a `SkyLight` makes its tables on .NET's thread pool;
and a `Thunderstorm` belongs to one caller as the clock does, where a `LightningFlash` is read from
any thread. A test fails where a public type is named whole in neither the section nor a row, a
name as `Star` and not the start of `Stars`, and it failed on `LightningStrike` first.

The module's session, the sky's lower half, read from 3DEngine at `ee7ac712`. Its SHARED.md asks
that the sky below the horizon be drawn in the fog's color blended over a few degrees above it while
the image's lower half still lights the scene, with a horizon color a program asks for without fog
(line 139), and its lighting takes a surface's view below the horizon as the ground's color times
the sky's light on it and each directional light's on a floor (RENDERING.md, line 2264). Its sky
pass, `Shaders/atmosphere_sky.slang`, writes every texel as `SkyLight.Luminance` gives it from the
ground, so a way below the horizon there meets the ground at once and shows the ground's light. Sky
gave that light as the sun's on a level surface alone, times the albedo over pi, and left out the
sky's light on the ground, so the lower half lacked the sky's share by day, some 8% at 54 degrees,
and was black from sunset through the twilight, when the sky gives the ground 1,287 lux at the
horizon and no sun reaches it. `SkyLight.Luminance` below the horizon gives the ground under the
sun's light and the sky's, the second read from the ground's table, with the air between where the
eye stands above the ground, and a test holds it within 1% of the albedo times both over pi at 54,
10, 0 and -3 degrees, which failed at 54 degrees by the missing 8%. The engine's pass computes the
ground's term on its own, `atmosphere_sky.slang` line 59, so its binding adds the sky's light there
to match. The horizon color without fog is `SkyLight.Luminance` along the horizon, as it is; the
fog's blend over it stays the engine's drawing.

The module's session, the bridge's binding, read from BevyCSharp at its head with nothing changed
there. Every number it computes or sets itself for the sky that the module could give, by its file
and line.

| Where | What the bridge computes or sets | What Sky gives |
|---|---|---|
| `games/FeatureTest/Behaviors/DayNight.cs:63` | The sun above the air, 12,500 lux in (1, 0.98, 0.95) | `Sun.Outside`, 128,000 lux at 5,772 K, and `Sun.At` at the ground |
| `DayNight.cs:66-78` | The sun's lux and color by the hour, 0 to 12,500 | `Sun.At` by `SkyClock.SunAltitude`, or `SceneLight.At` |
| `DayNight.cs:351` | The sun's and the moon's light faded over 0.05 of height at the horizon | `Sun.DiscShare` and `Moon.DiscShare` |
| `DayNight.cs:85-93`, `:242` | The moon at 900 lux in day terms, a fixed blue, times `MoonLit` | `Moon.At` by `SkyClock.MoonPhaseAngle` |
| `DayNight.cs:104-137` | The ambient and the sky's ambient light by the hour, 20 to 1,000 | `SceneLight.SkyIlluminance`, by sun, moon and night |
| `DayNight.cs:155-171`, `Applied.cs:164-165` | EV100 9.7 by day, a night lens of -0.12, switched at 18:45 and 5:30 | `SceneLight.Exposure`, by the light rather than the hour |
| `BevyCSharp/Sky/NightSkySettings.cs:30`, `Kernels/night_sky.slang:130-144` | Stars of 600 nits, hashed one cell in five | `Stars.Catalog` with `Stars.Luminance` |
| `night_sky.slang:143` | A star's tint picked between two at random | `Stars.Color` by its B-V |
| `NightSkySettings.cs:39`, `night_sky.slang:154-161` | The Milky Way at 25 nits about a hard-coded pole and center | `Night.GalaxyLuminance` |
| `night_sky.slang:187` | Stars faded in from 3 to 9 degrees under the horizon | `SkyLight.Luminance` over `Night.SkyLuminance` |
| `NightSkySettings.cs:42`, `:48` | The full moon at 4,000 nits, earthshine 0.012 of it | `Moon.LitLuminance`, 4,870, and `Moon.EarthshineLuminance`, 8e-5 |
| `NightSkySettings.cs:45`, `Applied.cs:211` | The moon 0.7 degrees wide, 1.5 in FeatureTest | `Moon.AngularDiameter`, 0.518 |
| `BevyCSharp/Rendering/SunDisk.cs:21` | The sun's disc of 0.00931 radians at an intensity of 1 | `Sun.AngularDiameter`, 0.533 degrees, and `Sun.DiscLuminance` |
| `ScatteringTerm.cs:93-112` | Rayleigh, Mie and ozone of its own Earth | `Atmosphere.Earth` |
| `native/bevy_csharp/src/render/post/atmosphere.rs:108-126`, `RenderSky.Clouds.cs:92` | A planet of 6,360,000 m, and Bevy's ground albedo | `Atmosphere.PlanetRadius`, 6,378,100 m, and `GroundAlbedo` |
| `BevyCSharp/Sky/RenderSky.cs:126` | The clouds' density times `CloudDensity` | `CloudOpticalDepth` over the deck's thickness |
| `RenderSky.cs:137`, `Kernels/sky_fog.slang:96` | The fog's visibility eased toward 80 m, its extinction 3 over it | `Haze.Visibility` and `Haze.Extinction` by Koschmieder's 3.912 |
| `SkyFogSettings.cs:59`, `Applied.cs:207` | The fog at 5,000 nits and the dusk's backdrop at 600 | `SkyLight.Luminance` along the horizon |
| `RenderSky.Thunder.cs:75-79`, `:210-250` | The flash's life, envelope, color and the strikes' Poisson process and places | `Thunderstorm` and `LightningFlash` |
| `RenderSky.Thunder.cs:76` | Sound at 343 m/s | `Thunderstorm.SpeedOfSound` by the weather's temperature |
| `RenderSky.Thunder.cs:217` | Two thirds of strikes to the ground | `Thunderstorm.GroundShare`, a quarter |

Three numbers have no counterpart in Sky, the second Earth of the map's sky
(`ScatteringTerm.cs:142`), the clouds' phase lobes and ambient (`RenderSky.Clouds.cs:248`) and the
bolt's core of 1e7 nits (`RenderSky.Thunder.cs:78`). One looks a fault of the bridge's own.
`ScatteringTerm.cs:100-101` gives its aerosols an absorption of 3.996e-6 and a scattering of
0.444e-6 a meter, a single scattering albedo of 0.1, where Hillaire's Earth, whose Rayleigh terms
the file takes, scatters 3.996e-6 in an extinction of 4.44e-6, an albedo of 0.9, so the two look
swapped and the bridge's haze takes light it should scatter.

The module's session, the dusk into night, `SceneLight.At` at 51 degrees north on the equinox from
the sun 10 degrees up to 24 down, with no moon and with a full moon rising opposite the sunset, at
`.github/assets/readings/2026-11-18-dusk-to-night.webp`. Against the table of Wikipedia's article
Lux the module gives 1,391 lux at sunset for its 400, 3.5 times as bright; 6.5 lux at the end of
civil twilight for its 3.4; 0.10 lux under the full moon 22 degrees up, inside its 0.05 to 0.3; and
7.1e-4 lux on a moonless night for its 0.002 with airglow, a third of it, since `Night.SkyLuminance`
is a dark site's 21.7 magnitudes a square arcsecond. The moonlit light is 5,100 K for the 4,100 of
Wikipedia's article Color temperature, the moon's light taken as the sun's through the air. The
fault the sheet shows is the twilight's color. The light reddens from 5,800 K at sunset to 1,962 K
at 10.5 degrees down, 2,837 K at 8.8 with the moon, where the twilight sky is blue, which Hulburt
(1953) explained by the ozone's absorption of the yellow and the green high in the air. The air of
`Atmosphere.Earth` ends at 55 km, the height where the molecules fall to a thousandth, and the
earth's shadow overhead stands above that from about 8 degrees down, so the air left lit is the low
air toward the sun, its light through long ways that redden it, which is the bend at 8.2 degrees
the ground's table met. The mend proposed next, a failing test first, carries the air higher and
reads the twilight's color against Hulburt's ozone, the moonless night's lux left as the dark
site's by its own source.

The module's session, the twilight's air. `Atmosphere.Top` ends the air where its molecules thin to
a millionth, 111 km, where it was a thousandth, 55 km, and the scattering table's 32 heights crowd
near the ground as the transmittance's do, its lowest rows 115 m apart. The earth's shadow overhead
stands at 35 km with the sun 6 degrees down and 79 km with it 9 down, so the old air lay all in
shadow from 8 degrees down and the twilight was lit through the low air toward the sun alone. The
twilight's zenith is blue from 4 to 10 degrees down, its blue over its red 1.26 at 6, 1.98 at 8 and
2.92 at 10 where it was 0.67 and 0 at 6 and 10, and the light on a level surface stays above
4,530 K through the twilight where it fell to 1,962 K, at
`.github/assets/readings/2026-11-18-twilight-air.webp` beside the old. A test holds the zenith
bluer than red from 4 to 10 degrees down and bluer by a twentieth at least than ozone-free air
gives at 6, 8 and 10, as Hulburt (1953) has it and as Spitschan and others (2016) measured the
outdoor light turning to shorter waves through the twilight, and it failed on the old air at 4
degrees down. The ozone's share is a tenth of the blue over red, since `Atmosphere.Earth`'s ozone,
HDRP's scaled by 0.6 over a layer 20 km thick, has a vertical optical depth near 0.011 in green
where the Earth's 300 Dobson units give some 0.03; raising it moves the day's sun, which HDRP's
figures pin, so it waits for the review. The day's figures stand by a test within 0.5% of what they
read before, the sun 54 degrees high 109,957 lux and 5,025 K for 109,968 and 5,026, the sky 7,851
lux for 7,857 and its zenith 1,630 candelas for 1,631. The ground's table starts at 24 degrees down
where it started at 20, since the higher air lights the sky with 1.1% of the night's own light at
20 and 2e-4 at 24, and its two tests hold each channel within 1% of itself, the wavering of the
dim blue that called for the lux's bound being a fifth of what it was. Making a `SkyLight` takes
51 ms where it took 22, and its ground's table 144 ms where it took 110, the means of 20 in a
release build's probe on 32 cores, at the same tables' sizes and steps, the taller air's thin
layers multiplying very small numbers in the twilight.

The sunset's 1,391 lux against the Lux article's 400. A second source, the twilight illuminance of
Krisciunas and Schaefer (1991) after Brown (1952) as a plot of it in Wikimedia Commons lists it,
gives some 442 lux at sunset, so the two agree and the module is three times either. The higher air
does not account for it, moving the sunset's light from 1,391 to 1,399 lux, and the reference's
conditions account for a part, since `Atmosphere.Earth` is a turbidity of 1.11, a very clear air,
and a turbidity of 2 gives 1,067 lux at sunset, 3 gives 926 and 4 gives 818, still twice the
sources'. The rest stands on the sky's own light at a low sun, 1,266 of the 1,399 lux, where the
light scattered more than once is the first to read; it is proposed as the next reading, with the
end of civil twilight, which the higher air takes from 6.5 to 9.1 lux against 3.4.

The module's session, the light scattered more than once at a low sun, read with nothing changed.
No fault is in it. Of the sky's 1,268 lux on a level surface with the sun at the horizon the light
scattered once gives 1,066, and the term scattered more than once adds 16%, 19% at 6 degrees down,
so the sunset's light stands on the single scattering. The gap to the two sources is the
conditions, three of them. `SkyClock` gives the sun's true height, where a sunset is the apparent
one, the sun's middle at the horizon when it stands truly 0.57 degrees down by the horizon's
refraction of 34 minutes of arc, which takes the module's sunset from 1,268 to 991 lux. The air of
`Atmosphere.Earth` is a turbidity of 1.11, a very clear one, and its ozone 40% of a real column.
With a turbidity of 3 and 300 Dobson units together the apparent sunset gives 588 lux against the
400 and 442 of the Lux article and of Krisciunas and Schaefer after Brown, and the end of civil
twilight, which is the sun's geometric height by its definition, 4.93 lux against 3.4, each within
half as much again where it was three times. So no test is written; a binding that compares a
sunset with a published one takes the sun 0.57 degrees lower than the clock's height there, and
the air's haze is the weather's.

The ozone, a reading with nothing committed, at
`.github/assets/readings/2026-11-18-ozone-300du.webp`. Bruneton's precomputed atmospheric
scattering of 2017 spreads 300 Dobson units over a tent from 10 to 40 km at a peak absorption of
(0.650, 1.881, 0.085) millionths a meter, the values HDRP takes before it scales them by 0.6 over a
layer 20 to 40 km, so the real column's vertical optical depth in green is 0.028 where the Earth's
as set is 0.011. With it the sun 54 degrees high gives 108,177 lux at 5,025 K for 109,957 at 5,025,
the sky 7,708 lux at 20,330 K for 7,851 at 19,412, all of it 1.6% less; at 10 degrees the sun 62,173
lux at 3,491 K for 66,488 at 3,571, the sky 4,559 at 13,057 K for 4,888 at 11,826, all of it 6.6%
less; the apparent sunset 776 lux for 991 and the end of civil twilight 6.53 for 9.45; and the
twilight zenith's blue over red 1.51, 2.40 and 3.60 at 6, 8 and 10 degrees down for 1.26, 1.98 and
2.92. The day's figures held to the reference scene move by 1.6% at 54 degrees, which is the
owner's to weigh.

A `SkyLight` made each frame. No one makes one. Neither engine makes a `SkyLight` outside its
tests, 3DEngine computing its sky on the GPU from `Atmosphere`'s data and BevyCSharp drawing
Bevy's atmosphere, and `MakeSkyLight` in 3DEngine's `E3D.DaySky.cs` makes a directional light. A
moving sun pays nothing, since a `SkyLight` holds the air alone and takes the sun's way at each
call. A binding pays the 51 ms and the ground table's 144 when its air changes, as a weather that
moves the turbidity through `Atmosphere.WithTurbidity` would, so such a binding makes the new one
off its frame and swaps it in when it is made.

The module's session, the Earth's ozone, for the review's Decision. By the owner's choice of
2026-11-18, `Atmosphere.Earth` holds a real column of 300 Dobson units and no second preset, the
ozone's absorption Bruneton's (0.650, 1.881, 0.085) millionths a meter at its densest in a layer
from 10 to 40 km, a vertical optical depth of 0.028 in green, where it held HDRP's, that
absorption times 0.6 from 20 to 40 km, 0.011. The rest of the air stays HDRP's. The day's figures
the tests hold moved by that choice. The sun 54 degrees high reaches the ground at 108,177 lux and
5,025 K for 109,957 and 5,025, 1.6% less; the sky lights a level surface with 7,708 lux for 7,851,
1.8% less, and its zenith shows 1,602 candelas a square meter for 1,630, 1.7% less; the
transmittance at 53.97 degrees is 0.919, 0.833 and 0.709 for 0.926, 0.850 and 0.710, the test
holding HDRP's own figures with HDRP's ozone set in the test and the Earth's beside them; and the
sun on a level surface at 54 degrees is 87,500 lux for 88,900. The twilight test's margin is reread,
the ozone making the zenith's blue over red 1.34 to 1.39 times what ozone-free air gives at 6, 8
and 10 degrees down where it made it 1.11, so the test holds a quarter where it held a twentieth.
The ground's table keeps its shape and its tests; its figures follow the sky's. The sheets whose
stated numbers change are the scene's light, 96,800 lux and an EV100 of 15.24 at 54 degrees, which
read 95,200 and 15.22; the haze and the cloud, the clear 96,800 and the overcast 24,200 lux, which
read 95,200 and 23,800; the twilight's mend, 0.31 lux 8 degrees down, which reads 0.58 with the
higher air and the ozone together; the twilight's air and the dusk into night, the light 9.1 lux 6
degrees down and 1,399 at the horizon, which read 6.3 and 1,127, its color 5,850 K 6 degrees down,
which reads 7,400, bluer as Spitschan and others measured the twilight; each sheet stands as it was
measured on its day. The sheet of this change is
`.github/assets/readings/2026-11-18-ozone-dusk.webp`.

Each engine's ozone. BevyCSharp holds the same column, `ScatteringTerm.cs:109-112` giving Bevy's
ozone (0.65, 1.881, 0.085) millionths a meter densest at 25 km in a layer 30 km wide, Bruneton's 300
Dobson units as Hillaire takes them, so its ozone and the module's agree and nothing moves there.
3DEngine draws its sky from the module's `Atmosphere`, `AirSky.cs:37-39` packing its ozone's
absorption, bottom and width and its top for the pass, and its examples' air is `Atmosphere.Earth`
with the aerosols and the ground's albedo changed alone (`ScenesSponza.cs:129`,
`ScenesBistro.cs:107`), so its sky takes the real column and the 111 km top with its pointer, its
day 1.6% dimmer at 54 degrees and its twilight bluer, which its readings of the reference scene
will show.

The module's session, the apparent height. `SkyClock.ApparentAltitude` gives a body's height as an
eye sees it from its true height by Saemundsson's formula of 1986 with Meeus's correction for the
air's temperature and pressure, 34.5 minutes of arc at the horizon, one at 45 degrees and none at
the zenith, falling to none from a degree under the horizon to two, where the formula's denominator
nears none and no one sees the body. `SkyClock.Refracted`, off unless set, lifts the sun, the moon
and the stars to it with their bearings kept, so the heights the engines read stand as they did,
and the README's conventions say a height is the true one unless it is set. Three tests, which
failed to build before the members, hold the horizon's 34.5 minutes, Saemundsson's from the true
height against Bennett's of 1982 from the apparent within a tenth of a minute from the horizon to
the zenith, the correction's air, and a refracted clock's sun at the horizon where an unrefracted
one stands 0.57 degrees under it.

The module's session, the turbidity as the caller's. It is the caller's already,
`Atmosphere.WithTurbidity` giving an air of any turbidity and `Atmosphere.Turbidity` reading it, so
nothing is added to the library and the reading becomes a table of tests. With the real column of
ozone, the apparent sunset and the end of civil twilight give 774 and 6.53 lux at the Earth's 1.11,
658 and 5.52 at 2, 586 and 4.93 at 3 and 534 and 4.48 at 4, against the Lux article's 400 and 3.4
and the 442 of Krisciunas and Schaefer after Brown: 1.94, 1.65, 1.47 and 1.33 times the 400, 1.75,
1.49, 1.33 and 1.21 times the 442, and 1.92, 1.62, 1.45 and 1.32 times the 3.4. The table holds each
row within 2% of what it reads, every row within twice the sources and the rows from a turbidity of
3, a clear day's, within half as much again, and a second test holds the light falling as the haze
grows. The tests pin a reading rather than mend a fault, so none failed first.

The module's session, Tools' shared checks, taken as Tools' README section A module's checks says.
The submodule stands under `modules/Tools` at `f92df1c`, referenced by the test project alone with
the target `NeedsTools`, which stops a build where it is not fetched with the sentence naming the
command, read with the submodule moved aside; `ModuleTests.cs` holds the README's four facts with
`Library` 3DEngine.Sky, `Root` and `Host` the binding's test set above them; the workflow checks the
submodule out; and the host's marker lines stand as Tools reads them. The four facts found nothing
on their first run, the binding's fact naming the line where a word of the README's block was
changed to read it. Out went the copies of the same checks, Tools' old `ReferenceTests.cs` and the
binding's word-for-word and public-type tests, so the suite holds 118 where it held 128. Two checks
Tools does not hold stay. `IndependenceTests` allows .NET's own assemblies alone, refuses a call
into native code and reads the project file, and `SourceTests` reads every line of the library's
sources for an engine's name, its private members and comments among them, where Tools reads the
public names and their documentation, beside its width of 100 columns. The library and its package
take nothing of Tools', which `IndependenceTests` and the reference fact hold. AGENTS.md gains the
row for `modules`, and the README's building section says how a clone fetches the submodule.

The module's session, the sunset's gap by its candidates, read with nothing changed, at
`.github/assets/readings/2026-11-18-sunset-candidates.webp`. From a clear day's air of turbidity 3
the apparent sunset gives 586 lux on a level surface, the dome summed in quarter-degree bands by a
probe that reads `SkyLight.Luminance`, which gives the module's own 586. Each candidate switched
alone moves it so. The haze's height profile moves it most, its scale height of 1.2 km at 0.6 km
giving 1.087 of it, at 2 km 0.919 and at 3 km 0.850, its depth kept. A haze that absorbs a fifth of
what it takes, its albedo 0.8 for 0.9, gives 0.961. Its phase gives 1.005 and 1.008 at an anisotropy
of 0.7 and 0.6 for 0.8. The ground's albedo gives nothing at none and at 0.3, since a sun under the
horizon lights no ground. A horizon hidden to 2, 4 and 6 degrees, as trees and buildings hide a
meter's, gives 0.999, 0.997 and 0.990, the level surface weighing the low sky by the cosine. The
haze 2 km high and absorbing gives 515 lux, and the same at a turbidity of 4 gives 447 against the
442 of Krisciunas and Schaefer after Brown and the 400 of the Lux article, both measured on a level
surface as the module's is. So none of it is a fault of the module's arithmetic. The gap stands on
the air `Atmosphere.Earth` holds, HDRP's haze, thin and low over a very clear day and absorbing
little, where the sources read the air of their sites on days they called clear, which held more
haze, higher and absorbing more; that is the sources' weather, which a binding gives the module by
`Atmosphere.WithTurbidity` and the haze's own terms. Whether `Atmosphere.Earth`'s haze should stand
nearer the sources' days is the owner's, as its ozone was.

The module's session, its devlog. DEVLOG.md stands at the root, written by Tools'
`scripts/devlog.py` from the submodule, `python3 -I modules/Tools/scripts/devlog.py . Sky`, the same
script as Tools' own head, and reads as the head does, three days and twelve entries, the ten
pictures of 2026-11-18 and items 3 and 5 under Done. The README links it from a top row under its
title, and AGENTS.md's row for it names the command and when it runs.

The module's session, what a binding pays to follow a moving sun, read with nothing changed, at
`.github/assets/readings/2026-11-18-resolution.webp`. A moving sun pays a call of `SceneLight.At`, 6
to 10 microseconds at every resolution read, since a `SkyLight`'s tables hold the air alone and the
sun's way is given to each call; the tables are made again only when the air changes. Making a
`SkyLight` takes 10.9, 27.5, 37.4, 58.0, 98.2, 135.4 and 455.6 ms with its tables at a quarter, a
half, three quarters, one, one and a half, two and four times their rows and angles, and its ground
table 220, 215, 138, 113, 95 and 106 ms with its spans halved to an error of 0.0005, 0.001, 0.003,
0.01, 0.03 and 0.1, in 361 to 115 rows, the means of six in a release build on 32 cores with the
sizes scaled by an edit not kept. The noon's light on a level surface, 95,225 lux with the sun 54
degrees high, holds within a thousandth of a percent at every one. The apparent sunset's, against
the finest, is 1.43% over at a quarter of the tables, 0.45% under at a half and 0.23% over at
today's one, and 0.01% under at today's error of 0.003, 0.14% at 0.01 and 1.54% at 0.03. So the
smallest within 1% at noon and at sunset are the tables at half their rows and angles, 27.5 ms,
and the ground's error at 0.01, 113 ms, together 0.69% under the finest at sunset; today's are
within 0.23%. The ground table's cost moves little with its rows, its 111 rows a degree apart taking
most of it, so halving the tables saves 30 ms of a build of some 200 and nothing a call. Whether to
take the smaller tables is the review's; nothing is changed.

The module's session, the Earth's haze, for the review's Decision. Decision line: by the owner's
choice of 2026-11-18, `Atmosphere.Earth` holds an ordinary clear day's haze, OPAC's continental
average aerosol of Hess, Koepke and Schult (1998) at a relative humidity of 80%, and no second
preset; the rest of its air stays as it was. OPAC gives the type 0.075 of the light a kilometer at
550 nm at the ground, an optical depth of 0.151 over a mixing layer 2 km deep, a single-scattering
albedo of 0.925 and an asymmetry of 0.703 (its Tables 3 and 5), and one exponential of a 2,013 m
scale height holds both its ground extinction and its depth, so the haze is 7.5e-5 a meter over
2,013 m scattering 0.925 with an asymmetry of 0.703, a turbidity of 2.40 and a visibility of 44 km,
where HDRP's was 1e-5 over 1,200 m scattering 0.9 with 0.8, 1.11 and 166 km. Of the three
candidates read, A was taken. B, OPAC's continental polluted, a turbidity of 4.03, holds the
sources within 1.21 times but is a polluted day, and C, the reading's fit, holds them within 1.12
times and has no source. The scattering table takes 128 angles where it took 64, since the new haze
bends the twilight 14 degrees down and 64 angles jumped there, the slope moving by 0.309 where the
test holds 0.1 and by 0.036 with 128. The apparent sunset converges to 601 lux as the angles grow,
600.8, 600.9 and 601.4 at 128, 256 and 512, 1.50 times the Lux article's 400 and 1.36 times
Krisciunas and Schaefer's 442, and the end of civil twilight to 4.93 lux, 1.45 times the 3.4, so the
tests hold 1.55 times all three, and the turbidity table's rows from the Earth's own 2.40 likewise.
The numbers both engines' daylight and the Sponza match are reread by, at 54 degrees: the sun at
the ground 91,138 lux at 5,025 K where it was 108,177 at 5,025, 16% less, the sky on a level
surface 19,336 lux where it was 7,708, and the level light in all 93,068 lux at 5,415 K where it was
95,225 at 5,433, 2.3% less and 18 K warmer, the sun's disc 1.34e9 candelas a square meter where it
was 1.59e9 and the sunny 16 exposure an EV100 of 15.18 where it was 15.22. The transmittance at
53.97 degrees is 0.775, 0.702 and 0.598, the test holding HDRP's own with HDRP's ozone and haze set
in it; the twilight's light reaches 13,600 K 10.5 degrees down, bluer, as Spitschan and others
measured it. 3DEngine reads `Atmosphere.Earth`'s haze with its pointer, its examples changing the
aerosols' extinction and scale height and the ground's albedo themselves, so Sponza and Bistro keep
the haze they set and take the new albedo of 0.925 and asymmetry of 0.703; BevyCSharp draws Bevy's
own haze and reads none of the module's. The sheet is
`.github/assets/readings/2026-11-18-haze-opac.webp`.

The module's session, the cheat sheet. Tools stands at `3720852`, the fifth fact beside the four,
and CHEATSHEET.md is written by `Cheatsheet.Write` with `CHEATSHEET_WRITE=1`, its opening by hand,
linked from the README's top row beside the devlog and named in AGENTS.md's documents table. It
lists 23 types and 208 members. Ten constructors stood with no summary. Six were the parameterless
constructors the compiler makes, of `Atmosphere`, `SkyClock`, `WeatherConditions`, `WeatherState`,
`LookCycle` and `LookSettings`, which the source gives explicitly with summaries of what each
holds until set; four are the positional records' own, of `SceneLight`, `Star`, `LightningStrike`
and `CelestialLight`, which the writer leaves bare where their type's line says what they hold.

The module's session, the sweep for the engines' names. The search the rule gives found 5 lines and
finds none. The README's and AGENTS.md's openings say what an engine does with the module rather
than naming two, the README's binding section says how an engine that draws its sky in its own
pass and one whose renderer holds an atmosphere of its own each read the module, and the comments
of the workflow, the props file and the library's project say an engine where they said both
engines or the engines. `SourceTests` reads the library's source lines through
`ModuleChecks.EngineNames`, so the test holds no engine's name of its own and finds one by the
names Tools' checks refuse, which a name planted in `Sun.cs` showed. The weather's half-life and
step are seconds of the world's clock, where they said the game's, and CHEATSHEET.md and DEVLOG.md
are written again.
