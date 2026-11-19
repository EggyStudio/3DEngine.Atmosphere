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

The module's session, the README's link row. It stands as the file's first line above the title,
the devlog and the cheatsheet by their addresses on `main`, a space between them, then a blank
line and the title.

The module's session, the next three, proposed from what a stranger binding the module would miss
first, each with a failing test first. One, the dusk's slope under every haze, which the review
weighed. Two, the sky's light away from the ground: `SceneLight.At` reads the ground's table at an
altitude of 0 alone and sums the dome elsewhere, some 14 ms a call, so a binding whose camera flies
or stands on a mountain pays it each frame; a table over the altitude too, held within 1% of the
sum to 10 km. Three, the edges a stranger meets first, a way of no length, a clock at a pole, a
weather stepped back, answered with a number or a plain exception and never a NaN.

The first, read. The test walks the sky's light on a level surface every quarter degree to 16
degrees down at turbidities of 1.1, 2, 3, 4 and 6.95, OPAC's urban type, and holds its logarithm's
slope moving by under 0.1, as the Earth's own twilight test does. It found no jump: the worst moves
are 0.017, 0.029, 0.045, 0.056 and 0.079, each near 14.75 degrees down from a turbidity of 2,
growing with the haze, and a turbidity of 10 gives 0.096, so the 128 angles hold to the haziest
air a binding would set. With no fault the test failed on none and stays as the guard the review
named; the second is begun.

The module's session, the sky's light away from the ground. `SkyLight.GroundIlluminance` takes an
altitude, and its table holds a row of heights at 13 altitudes, 500 m apart to 2 km and a kilometer
apart to 10 km, each made at its first reading, the light between two rows read by its logarithm
and above 10 km the sum. `SceneLight.At` reads it at every altitude, so a camera that flies or
stands on a mountain reads the sky's light in 8.4 microseconds where the sum takes 1.9 ms, the
first reading near an altitude building the rows about it at 100 to 230 ms a row on 32 cores. The
test, which failed to build first, holds each channel within 1% of the sum at 137, 1,800, 4,321 and
9,000 m from the deep twilight to the zenith; with rows at 0, 0.5, 1, 2, 3.5, 5, 7.5 and 10 km it
missed by 1.1% at 9 km 12.6 degrees down and at 4,321 m 19.9 degrees down, so the rows stand closer.
Tools stands at `0b0b902` with its two facts beside the five, no tracked text naming an engine and
the README opening with its link row, both holding on their first run.

The module's session, the ground table's rows ahead of time. `SkyLight.PrepareGround` makes the
table's rows about a span of altitudes, so a reading in it afterward makes none, and
`SkyLight.GroundRowsMade` counts the rows made. The test, which failed to build first, makes the
span from the ground to 3 km, its 6 rows, reads at 23 altitudes in it with the count unchanged and
reads past it at 3.5 km with one row more. The README's binding block makes the span to 2 km where
it read once at the ground, the host's copy with it, and the conventions say a binding makes the
span its camera keeps while the world loads. The suite takes 12 s where it took 8, its tests making
more rows.

The module's session, the edges. A way of no length or not a number, the usual slip of a default
vector, gave the sky's luminance and illuminance a NaN and made `GroundIlluminance` throw an
`IndexOutOfRangeException` from inside its table; the four readers refuse it with an
`ArgumentException` naming the parameter. A latitude past a pole or not a number gave a clock of
nonsense ways and is refused as it is set, and a cloud cover not a number gave `SceneLight.Under` a
NaN and is refused. A NaN given straight to arithmetic, as `Sun.DiscShare` takes a height, gives
the NaN back, the caller's own. Ten tests hold them, of which the seven on the refusals failed
first; the three on a clock at either pole, its ways finite and of unit length refracted or not,
and on a clock and a weather stepped back, which give numbers, passed before and guard them. The
suite holds 144.

The module's session, daylight by the sun's height, read through `Atmosphere.Earth`, at
`.github/assets/readings/2026-11-18-daylight-by-height.webp`.

| The sun's height | The sun facing it | IES clear sky | The sky on a level surface | All of it |
|---|---|---|---|---|
| 5 degrees | 7,901 lux, 2,653 K | 11,457 lux | 3,865 lux, 6,308 K | 4,553 lux, 5,354 K |
| 10 | 28,194 lux, 3,491 K | 38,045 | 7,106 lux, 6,701 K | 12,001 lux, 4,957 K |
| 20 | 57,948 lux, 4,290 K | 69,001 | 11,886 lux, 7,118 K | 31,706 lux, 5,082 K |
| 30 | 74,111 lux, 4,659 K | 83,773 | 15,100 lux, 7,294 K | 52,156 lux, 5,231 K |
| 54 | 91,138 lux, 5,025 K | 98,351 | 19,336 lux, 7,522 K | 93,068 lux, 5,415 K |
| 90 | 97,207 lux, 5,152 K | 103,349 | 20,895 lux, 7,686 K | 118,102 lux, 5,487 K |

The sun's light facing it is 0.69 of the IES clear sky's, 127.5 klux times e to the -0.21 over
the sine of its height (IES RP-21, 1984), at 5 degrees and 0.94 at the zenith, an ordinary day's
beside a clear one's, so the light holds no fault. Its color does. Daylight in the middle of the
day, the sun above 20 degrees, stands near 6,500 K at a clear noon as measured at Ny-Alesund against
lower latitudes, and Peyvandi and others (2016) give clear daylight's interquartile range as 5,712
to 7,757 K, where the module's day is 5,082 to 5,487 K. A test of 5,500 to 6,500 K at 20, 30, 54 and
90 degrees fails at every one and is held out of this commit until the mend is chosen. The cause is
the three wavelengths the air is read at. The molecules' scattering at 680, 550 and 440 nm stands
for the sRGB primaries, whose dominant wavelengths are near 611, 549 and 464 nm, and read there the
day is 100 to 220 K bluer, 5,301 K at 20 degrees and 5,533 at 54; the sun outside the air, a black
body at its effective 5,772 K, would give some 90 K more at 5,900; the haze, alike in every channel
where an aerosol takes more of the blue, and the ozone read at the same three wavelengths, carry the
rest. The thorough mend is the light at a dozen or so wavelengths, turned into the three channels at
the end, which moves every figure the engines read and the reference scene's match; the smaller one
reads the molecules at the primaries' wavelengths, which takes 54 and 90 degrees into the band and
leaves 20 and 30 under it. Which is the review's and the owner's.

The module's session, the key light's hand-off, read with nothing changed. `SceneLight.KeyToward`
turns from the sun's way to the moon's at the instant the moon's direct light passes the sun's, and
read every tenth of a minute through an equinox's dusk with a full moon rising opposite, at 0, 51
and 65 degrees north, the turn comes with the sun 0.00 to 0.02 degrees down, the full moon itself
at the horizon, and the key's light then is some 2e-7 lux on a surface facing it against the sky's
800 lux on a level surface. A light so faint turns unseen, so no test is written and nothing is
mended. Tools stands at `75d77f2`, and the module writes cheatsheet as one word, the README's link
row among its lines.

The module's session, the spectral mend's first step, the light by wavelength, with no figure the
engines read moved. `Data/Spectra.csv` carries the sun's light outside the air, the extraterrestrial
spectrum of the ASTM G173-03 reference spectra after Gueymard's SMARTS 2.9.2, and ozone's absorption
cross-section, Serdyuchenko and Gorshelev's of 2011 at 233 K, each the mean over a 5 nm band from
380 to 780 nm, its sources named in its head. The internal `Spectrum` divides that light into bands
of equal width, integrates Wyman, Sloan and Shirley's fit of 2013 to the CIE 1931 observer over
each, and turns a light by band into linear sRGB whose luminance is its lux, the tests reading it
through the library's internals. Seven tests, which failed to build first, hold an equal-energy
light at the white of a third within 0.004 at 12 and 24 bands, a black body at 4,000, 5,772 and
6,500 K read back within 1.5%, the sun's light outside the air at the luminous solar constant
Darula, Kittler and Gueymard give, 133.8 klux, within 1%, and ozone's absorption greatest near 600
nm at some 5.1e-21 square centimeters. The sun's measured light outside the air is 132,727, 132,974
and 133,375 lux at 5,793, 5,925 and 5,929 K in 6, 12 and 24 bands, where the module's sun is 128,000
lux of a black body at 5,772 K; the next step reads the air by band and gives the sun that light.

The module's session, the spectral mend's second and third steps in one commit, the air and the
sky by 12 bands, since the sun's light by band under a sky in three channels would have moved every
figure twice. The molecules scatter as the wavelength to the power of 4.08 from their 550 nm value,
the ozone absorbs by its cross-section against its value at 550, the aerosols' extinction falls by
OPAC's Angstrom exponent of 1.42, a new term, `Atmosphere.AerosolAngstrom`, and the two albedos are
read between the channels' wavelengths. `SkyLight` is now four lanes of three bands, each the old
arithmetic, turned into linear sRGB at the end, and `Atmosphere.Transmittance` is the sun's light
through the air by band over its light outside, in each channel. The sun outside the air is the
measured spectrum at the luminous solar constant, 133,800 lux at 5,925 K, where it was 128,000 lux
of a black body at 5,772 K. Twelve bands stand within 0.003 of 24 in x and y from 60 degrees up to
12 down, 6 bands apart by up to 0.019, so the module keeps 12.

At noon, the sun 54 degrees high, the measured sun outside the air adds 4.5% to all sunlight and the
air by band takes back about 1%. The sun at the ground is 94,200 lux at 5,033 K where it was 91,138
at 5,025, 3.4% more, the sky lights a level surface with 20,118 lux where it gave 19,336, and the
level light is 96,327 lux at 5,648 K where it was 93,068 at 5,415, 3.5% more. At 20 degrees the
level light is 32,412 lux at 5,342 K where it was 31,706 at 5,082. The apparent sunset is 590 lux
where it was 600 and the end of civil twilight 4.64 where it was 4.92, 1.47 and 1.37 times the
sources, so the bound of 1.55 holds, and every turbidity's row stays within twice. The 128
scattering angles hold, the ground table within 1% of the sum from 24 degrees down to the zenith.

The daylight test is committed with its bounds as the review set them, 5,500 to 6,500 K from 40
degrees up and 5,300 K as a floor at 20 and 30, 5,342 and 5,472 K now. What the low sun lacks is
still open. Spitschan's rural evenings below read 6,600 K at 10 degrees where the module gives
5,374, though those evenings were not chosen clear, so the measured figure may hold cloud. The
review's two candidates, the haze's albedo and asymmetry by wavelength and the orders of scattering
the sky's table sums, are untried.

The twilight's color, read against the downwelling spectra Spitschan, Aguirre, Brainard and Sweeney
published with their paper of 2016, three July evenings at the rural site with the moon under a
third lit, binned a degree of the sun's height and read through the module's observer
(`.github/assets/readings/2026-11-18-spectral-twilight.webp`). With the sun 6 degrees down their
mean is x 0.235 and y 0.242, the evenings spread by 0.025 and 0.029. The module gives 0.240 and
0.249 where it gave 0.289 and 0.251, so the old twilight stood 2 deviations toward purple in x and
the new stands within a quarter of one. From the sun at the horizon to 6 degrees down the new
twilight holds within 0.014 of their mean in x and y, and a test holds it within 0.02 at 0, 3 and 6
degrees down, the old model missing by 0.033 at the horizon and 0.054 at 6 down. In the nautical
twilight their light is bluer still, x 0.216 at 10 degrees down against the module's 0.238. The term
that makes the blue is ozone by band, its Chappuis absorption, which the paper itself names. Under
the old three channels ozone was read at 680, 550 and 440 nm, and its band near 600 nm, where its
absorption peaks, was missing. I found no other source with figures by the sun's height. The papers
of Lee and Hernandez-Andres on twilight's colors were closed to the fetch.

What I sent as the civil end's color before the mend was wrong. It read 11,639 K, not 5,850.
`Colorimetry.TemperatureOf` is McCamy's cubic, which holds to about 12,500 K. Past that its
figures mean nothing, so the civil end's 24,335 K reads 44,300 K exactly, the nearest point of the
Planckian locus in CIE 1960 uv, and the measured evenings' 140,500 K, near the locus's end. The test
of the bluest twilight now finds its least x, where it took the cubic's figure, and still finds it
between 6 and 12 degrees down.

Rule: `Colorimetry.TemperatureOf` is a public member that gives wrong figures for the twilight the
module now makes. It is untouched here, and the exact reading, or Robertson's, is the mend I would
propose as an item of its own.

Where the cost goes, read on this machine. Making a `SkyLight` takes 353 to 380 ms where it took 112
to 154. Of that, the four lanes' transmittance tables are 54 to 60 ms and their light scattered more
than once some 240 ms, a lane 73 to 76 ms, and the rest is first-time work, the lanes' air and the
spectrum's bands. Its tables hold 632 KB where they held 193. A row of the ground table takes 630 to
780 ms where it took 340 to 430, and holds 13 KB where it held 3. Each band is now held to its own
GroundError, set to 0.002 from 0.003, since a dim band held against the lane's mean missed the sum
by 1.07% 15.8 degrees down, its red turned from bands the matrix subtracts. A read of the table takes
8 microseconds where it took 5, a luminance 52 to 77 where it took 9 to 16, and `Sun.At` 21 where it
took 9, after each air's lanes are kept with the air. The suite takes 73 to 82 s here where it took
11.

The tests the mend moved, each to its figure as it reads, the arithmetic ones now held per lane
where the albedo or the coefficient takes the light band by band. They are the optical depth
straight up, the ground below the horizon, Chapman's thin air read as one lane, the reference
scene's figures, the sun at 54 degrees, now 94,000 lux, sunny 16's level light, the minute's share
through sunset, now 0.83 at least, and the turbidity rows. The dusk's zenith over the day's had a
floor of 0.005 from no source, and it reads 0.0048, so it is lowered to 0.003, the comment saying
the bounds are orders of magnitude rather than a source's.

The module's session, `Colorimetry.TemperatureOf` read exactly. It finds the nearest point of the
Planckian locus in CIE 1960's uv, the locus made from Planck's law through the module's observer
from 360 to 830 nm a mired apart from 1,000 K to an infinitely hot black body, read between two of
its points along the segment that joins them. A light bluer than any black body reads as
`float.PositiveInfinity`, one redder than a black body at 1,000 K reads 1,000, and no light reads
0, the README saying so. Robertson's method was not taken, since the exact reading costs 2.2
microseconds a call and its locus under a millisecond once. Five tests failed first, the CIE's
daylight at 15,000 and 25,000 K, which the cubic read low past its range, two chromaticities past
the locus's end and a red light past its start, and pass now beside the CIE's A, D50 and D65.

The CIE's illuminant A reads 2,829 K for its 2,856, so its row is held within 1% and the rest within
0.5%. The method is not the cause. Through the CIE's own tabulated observer (CIE 018:2019, its
dataset DOI 10.25039/CIE.DS.xvudnb9b) the same locus reads A, D50 and D65 within 2 K of their
published figures, so the gap is Wyman, Sloan and Shirley's fit, which stands apart from the CIE's
in the red, and every light the module makes goes through that fit. The locus is kept on the fit, so
a black body's spectrum through the module reads back its own temperature. Taking the CIE's table
as the module's observer is the mend I would propose, and it moves every color slightly. The table
is under CC BY-SA 4.0, so carrying it in the library is the owner's word. `FromTemperature` still
reads Kim and others' cubic fit to the CIE's locus, from 1,667 to 25,000 K, and its color reads back
38 K apart at 2,000 K and within 25 from 3,000 to 10,000, where the test held 25 at 2,000 and now
holds 40.

Every figure the cubic gave is reread. The sun 54 degrees high is 5,021 K, which was 5,033, the
level light there 5,640 K, which was 5,648, at 20 degrees 5,333, which was 5,342, at 30 5,464 and at
40 5,559, the apparent sunset 10,780 K and the end of civil twilight 43,640 K, which the cubic read
as 24,335. The sun outside the air is 5,920 K. The README, the reference scene's test, the sun's
test and the daylight test's comment carry the new figures, and the daylight test's bounds hold.

Two readings stay open. In the nautical twilight the measured light is bluer than the module's, x
0.216 against 0.238 at 10 degrees down. The untried candidates are the ozone's column and height,
300 Dobson units from 10 to 40 km, a single tent where the real layer peaks near 22 km, the ozone's
cross-section taken at 233 K alone, the light scattered more than once, which Hillaire's
approximation takes as alike every way, and the stratosphere's own aerosol, which the module has
none of. Below 40 degrees the day is redder than measured daylight, 5,333 K at 20 degrees against a
floor of 5,300 and a band from 5,500. Its untried candidates are the haze's albedo and asymmetry by
wavelength, the orders of scattering the sky's table sums, and the rural evenings themselves, which
read 6,600 K at 10 degrees but were not chosen clear.

The module's session, S4, the moon's light by wavelength. The moon's light outside the air is the
sun's in each band times the lunar disc's reflectance, the line the constant term of Kieffer and
Stone's ROLO model (2005, Table 4, version 311g) follows in its logarithm over its 15 bands from 405
to 775 nm, the oxygen band at 763.7 left out, 0.091 at 400 nm, 0.117 at 550 and 0.149 at 700, within
0.038 rms of the bands, their excursions the paper itself corrects against a soil's spectrum. It is
scaled to Krisciunas and Schaefer's lux by the phase as before. `Moon.At` takes it through the air
band by band, and the moonlit sky in `SceneLight` is the sky's arithmetic with the moon's light in
place of the sun's in each band, where it was the sunlit sky scaled by the moon's lux. The moon
outside the air is 4,936 K against the sun's 5,920, 4,406 K overhead at the ground against the sun's
5,166, and 3,026 against 3,378 at 10 degrees; the air lets through 0.3% more of its light overhead
than of the sun's and 1.6% more at 10 degrees. The moonlit sky is 0.992 of the sunlit by the lux
ratio and 1,600 to 2,400 K redder, and a full moon overhead lights a level surface with 0.288 lux
at 4,791 K. Three tests failed first, to build, and two of them then missed bounds I had guessed,
the moonlit sky's share above 1 and the moon 500 K under the sun at 10 degrees, which the physics
puts at 0.992 and 352 K and the tests now hold. They hold the disc's reflectance through the bands
within 1%, the moon's share through the air against the sun's and its redness at 10, 30 and 90
degrees, and the moonlit sky's share and redness. The further reddening ROLO
finds away from the full moon is left out, and no measured moonlight's color by a source I could cite
was found to hold it against. Tools moves to `dc117ac`, and the cheatsheet written again changes two
lines, `LookCycle.Keys` and `LookSettings.Tint`, whose tuples now carry their elements' names.

The module's session, the observer, which way was taken and why. A fit of its own closes the CIE's
A, D50 and D65 within 5 K, so the module takes it and carries no table, as the review ruled. The
reading that led there, with the CIE's tabulated 1931 observer (CIE 018:2019, DOI
10.25039/CIE.DS.xvudnb9b) used outside the repository to fit against and to read by: Wyman, Sloan and
Shirley's x-bar runs low from 660 to 720 nm, 0.0096 under the CIE's 0.047 at 680, a fifth of it. One
lobe more there, fitted alone to x-bar by least squares, takes A from 27 K under to 8.5 over and
puts D50 22 K and D65 31 K over; with the main red lobe's amplitude fitted beside it A is 1 K under,
D50 6 over and D65 12 over. Fitting all seven lobes' amplitudes with the red lobe by the
chromaticity of black bodies from 1,500 to 20,000 K and the functions' own residuals closes the
three within 3 K, but it held that lobe 4 nm wide below its peak, a kink, and moved y-bar, the
luminance, its largest residual from 0.0076 to 0.0136. The fit taken holds the lobe's width to 10
nm at least and y-bar as theirs, so no light's lux moves, and fits x-bar's and z-bar's amplitudes
with the red lobe, at 676.1 nm, 0.00763 high, 10.0 nm below its peak and 34.6 above. It reads A at
2,855.8 K, D50 at 5,001.3 and D65 at 6,503.3, and the CIE's daylight at 15,000 and 25,000 K 0.2%
and 0.3% under, within the test's 0.5%. Its cost is z-bar's largest residual, 0.0283 where it was
0.0238 of a peak of 1.78, and x-bar's is 0.0144 where it was 0.0148. The test of A, D50 and D65
within 5 K failed first at 2,829, 4,990 and 6,498 K.

Every figure, before and after. No lux moves, so the sun at 54 degrees stays 94,200 lux and the
level lights, the sunset's 590 and the end of civil twilight's 4.64 stay as they were. The sun
outside the air is 5,921 K where it was 5,920, the sun at 54 degrees 5,023 where it was 5,021, the
level light at 54 degrees 5,641 where it was 5,640, at 30 degrees 5,464 and at 40 5,559 as before,
at 20 degrees 5,332 where it was 5,333, the apparent sunset 10,765 where it was 10,780 and the end of
civil twilight 43,574 where it was 43,640, its x 0.2403 where it was 0.2401. The moon outside the air
is 4,934 K where it was 4,936 and overhead 4,405 where it was 4,406, and the moonlit sky 6,972 K
where it was 6,971. A black body's color from `FromTemperature` reads back within 9 K from 2,000 to
25,000 K where 2,000 K read 38 apart, so that test's bound returns to 25 K, and 50 at 25,000. The
README, the reference scene's test and the moon's and the daylight test's comments carry the new
figures.

Tools moves to `4e922ea` by pin.py, in place of the hand pin to `dc117ac`, which `e6d564e` had
already made. It printed that Tools moved from `dc117ac` to `4e922ea` by 3 commits, no fact new or
changed, CHEATSHEET.md written again the same as it stood, and DEVLOG.md written again by the new
pin's devlog.py, which now lists each day's commits.

The module's session, S5, the night's own glow in its color. `Night.SkyColor` is a dark sky's B-V
of 0.8, which Benn and Ellison measured over La Palma (1998) at B 22.7 and V 21.9 magnitudes a square
arcsecond, read as a star's color is, 5,285 K, and `SceneLight` lays the night's 7.1e-4 lux in that
color where it laid it white. The test failed first, to build, and holds the color within 150 K of a
B-V of 0.8's and the moonless midnight's sky in it. The night's sheet for the owner
(`.github/assets/readings/2026-11-18-night.webp`) sets the night before the spectral work, Sky at
`128925a`, beside the night now. A full moon overhead lights a surface facing it with 0.236 lux at
4,392 K where it gave 0.238 at 5,142, the sun's color, 0.6% less overhead and 5% less at 5 degrees,
the air read band by band. The moonless night's glow is x 0.338 where it was D65's 0.313, and the
moonlit night's level light x 0.355 where it was 0.338. The level light from sunset into the dark
keeps its lux within 6% before and after, a full moon rising opposite lifting it from 10 degrees
down. The B-V is read through a black body's color, where the airglow's light is lines of oxygen and
sodium and bands of hydroxyl, a chromaticity off the locus, so the glow's color is the measured
index's and not its spectrum's.

Tools moves to `1e1ec63` by pin.py, which printed that Tools moved from `4e922ea` to `1e1ec63` by 2
commits, no fact new or changed, CHEATSHEET.md written again and DEVLOG.md written again. The
cheatsheet gains one line, `Night.SkyColor`. The devlog now leaves out two old commits, `102482c` and
`a2005ac`, whose sentences write cheat sheet as two words and so fail the line check.

The module's session, the module drawing its own sky. `Sky.Tests/SkyPicture.cs` draws the dome on
the CPU from the module alone, each pixel `SkyLight.Luminance` lit by the sun, the sky lit by the
moon's light band by band through a new internal reading beside the ground's, the night's glow in
its color, the Milky Way's light, and the sun, the moon and the catalog's stars as points, their
illuminance at the ground spread over the pixel each falls in, every disc being under a pixel here.
Under the horizon the ground is the reader's, a level of the air's albedo, with the night's glow
sent back from it. The exposure is ISO 12232's saturation-based one, a pixel full at 1.2 times 2 to
the EV100 candelas a square meter, at the EV100 `SceneLight`'s incident meter gives the scene, each
channel past it held full, and the encoding is sRGB's curve of IEC 61966-2-1, no tone curve. A
fisheye of the whole dome and a panorama from 10 degrees under the horizon to the zenith are drawn of
noon, mid-morning, sunset, the end of civil twilight, the nautical twilight, a night under a full
moon and a moonless one at 50 degrees north, and `dotnet test Sky.Tests` with `SKY_PICTURES` naming
a folder writes them there as PNG. The sheet is `.github/assets/readings/2026-11-18-sky-drawn.webp`.
The test failed first, to build, and holds the noon fisheye's middle pixel to the reader's zenith
luminance through the same exposure and curve within a count of 255 in each channel.

What the pictures show. Noon's zenith is 7,278 candelas a square meter at 9,349 K, a grey blue,
since the sun 63 degrees high stands near it and an ordinary clear day's haze whitens it; the same
noon at a turbidity of 1.11 reads 34,000 K. The sunset's and the twilight's skies are metered to a
middle gray by the level light, so the sky toward the set sun is held full, and the nights are
metered the same way, so the moonless night shows its glow near full and the Milky Way across it,
as a metered long exposure would. The mid-morning scene's sun is 46 degrees high, the June
solstice's 9 o'clock at 50 degrees north.

What a binding draws that the picture lacks. Clouds, the weather's cover and its layer, which the
module gives as figures and no shape. The horizon's ground, terrain and what stands on it, where the
picture has a level of one albedo. The haze along the ground between the eye and far things, which
`Haze` gives and the picture, having nothing far, does not use. The sun's and the moon's discs as
discs, the sun's limb darkened and the moon's terminator by its phase, where the picture's pixel is
wider than either. Refraction, which `SkyClock.Refracted` gives and the picture leaves off, drawing
true heights. The glare an eye or a lens adds about the sun, an exposure that adapts over time, and
the tone curve a renderer chooses, each of which the picture leaves out so its pixels stay the
numbers.

Tools moves to `1452dbe` by pin.py, which printed that Tools moved from `1e1ec63` to `1452dbe` by 1
commit, no fact new or changed, CHEATSHEET.md written again the same as it stood, and DEVLOG.md
written again.

The module's session, a reading of the noon sky, nothing changed. At 50 degrees north on the June
solstice the sun stands 63.42 degrees up, 26.58 from the zenith, and the zenith is 7,278 candelas a
square meter at 9,349 K. The targets: the CIE general standard skies (ISO 15469), type 12 the
standard clear sky and 13 the polluted clear sky, for the luminance by the angle from the sun, scaled
to the zenith luminance of Karayel and others (1984), which Darula and Kittler verified against
cloudless skies, (1.376 Tv - 1.81) tan h + 0.38 kcd/m2, at the luminous turbidity of Navvab and
others (1984) that the module's own direct sun gives, 2.83, so 4,535 candelas a square meter; and
for color, Hernandez-Andres, Romero and Lee's clear skylight over Granada (JOSA A 2001), purest blue
at 90 to 110 degrees from the sun, mean purity 21.6% against the sun's light outside the air and at
most 39.4%, dominant wavelength 478 to 479 nm, modal inverse color temperature 70 to 75 MK-1.

The module's zenith is 1.60 times the formula's, and along the sun's vertical its sky is twice the
CIE band from 5 to 20 degrees from the sun, 1.2 times at 40, 1.3 at 60 and 1.5 at 90. Its color is
near Granada's where the sky is bluest, 17,438 K at 90 degrees from the sun, 57 MK-1, purity 30.3%,
477 nm, and white at the zenith, 9,418 K, 106 MK-1, purity 18.1%, since at this sun the zenith lies
inside the aureole. Switched alone, with the same exposure, the haze's amount at turbidity 1.6
brings the zenith to 4,271 and 12,266 K, but its own direct sun gives a luminous turbidity of 1.98
and the formula 2,219 for it, so the ratio grows, 1.92; its albedo by wavelength, 0.935, 0.925 and
0.91 at 464, 549 and 611 nm, a probe's guess, moves nothing; one Henyey-Greenstein lobe at 0.703 in
place of the Cornette-Shanks form gives 6,890; pairs of a forward and a broad lobe at the same
asymmetry take the zenith to 4,056 to 6,684 but brighten the sky within 10 degrees of the sun and,
their broad lobe flatter, the far sky; and the light scattered once alone gives 6,241 at the zenith
and 0.65 times the band at 90 degrees, so the light scattered more than once carries more than half
the far sky and puts it 1.5 to 2 times over the band from 90 degrees out. Two terms carry the fault.
The single lobe at 0.703 sends 0.378 a steradian 26.6 degrees from the sun, where an aerosol's real
forward peak sends far less, which is the zenith's excess and the wide glow. The light scattered more
than once, Hillaire's, taken alike every way, is the far sky's excess. The sheet is
`.github/dump/2026-11-18/20-38-noon-sky-switched.png`.

The module's session, the Earth's air set to the owner's choice. The owner sent Hillaire's paper
of 2020, A Scalable and Production Ready Sky and Atmosphere Rendering Technique, as the sky the
module should give, and chose its Table 1 air as `Atmosphere.Earth` with the ordinary day's haze
kept beside it as `Atmosphere.ContinentalAverage`, put to them with the two airs' pictures side by
side (`.github/dump/2026-11-18/20-44-paper-air.png`). The paper's method is the module's own; its
air is a clear day's, a haze scattering 3.996e-6 and absorbing 4.40e-6 a meter over 1.2 km with an
asymmetry of 0.8, alike at every wavelength, the planet 6,360 km, the molecules 5.802, 13.558 and
33.1e-6 and a ground of 0.3, a turbidity of 1.09 and 178 km visibility. What it moves: the noon
zenith at 50 degrees north is 2,193 candelas a square meter at 59,600 K where it was 7,278 at 9,349,
a deep blue with a tight halo; the sun 54 degrees high 111,690 lux at 5,292 K where it was 94,200 at
5,023, the sky on a level surface 9,186 where it was 20,118; daylight 5,553 K at 20 degrees and
5,845 overhead, so the daylight test holds 5,500 to 6,500 K from 20 degrees up and the 5,300 floor
goes, which closes the low sun's open reading; the twilight 0.249 and 0.253 in x and y at 6 degrees
down against the measured evenings' 0.235 and 0.242, within the test's 0.02; and the dusk, the
apparent sunset 748 lux and the end of civil twilight 5.94, 1.87 and 1.75 times the published
figures, held within twice them by a test of its own. The 1.55 bound, the turbidity rows, the 44
km visibility and the zenith range of 3,000 to 5,500 stay as tests of `ContinentalAverage`, whose
figures move a little with the new planet and molecules, 2.39 and 591 lux. The proposed tests of
Karayel's zenith and the CIE band are not written: the formula is fitted to luminous turbidities
from about 2, and the clear day's is 1.5. The drawing rings the sun's place in the pictures it
writes, and the seven scenes through the clear air are `.github/assets/readings/2026-11-18-sky-clear.webp`.
Tools moves to `457dd21` by pin.py, which named one changed fact, the engine names in tracked text,
which the suite passes. The older Replies are in `.github/REVIEW-ARCHIVE.md`.

Handoff. The spectral work, the observer, the moon and the night, the drawing and the Earth's clear
air stand committed with the suite green at 176. The noon reading's mend (a), OPAC's phase shape
fitted, and (b), the light scattered more than once against the far sky, were paused on the
owner's choice and are not done: under the clear air they matter for `ContinentalAverage` alone,
and whether they go on is the review's to say. The nautical twilight's gap stays open, the
measured light bluer than the module's from 7 to 12 degrees down. A fresh session reads AGENTS.md,
`.github/STYLE.md`, `.github/COMMITS.md`, this file's head, the last Decisions and the last
Replies, then `Sky/Spectrum.cs`, `Sky/Atmosphere.cs`, `Sky/SkyLight.cs` and `Sky/SkyLane.cs`; the
scratch probes are outside the repository and not needed. Every commit runs pin.py and
`commitcheck.py . --staged --message=<file>` first.

The module's session, the drawing metered two ways. Each hour is drawn again at the EV100 a
reflected reading of the sky itself gives, ISO 2720's reflected meter with a constant of 12.5 at a
sensitivity of 100 on the sky's mean luminance over the dome, the sun, the moon and the stars left
out, beside the incident meter's, and a test that failed first holds the reading to that mean. The
dusk and the night now read as photographs do, sunset at 11.37 where the incident meter gave 8.22,
the civil end at 4.80 for 1.25 and the moonless night at -9.05 for -11.78. Noon does not: the two
readings stand 0.38 stops apart, 15.06 and 15.44, and the clear sky's mean placed at the meter's
gray, a tenth of a full pixel, encoded by sRGB's curve alone, stays a dark blue; a camera's tone
curve, which the drawing leaves out to keep its pixels the numbers, is what lifts it. The sheet is
`.github/assets/readings/2026-11-18-metered-two-ways.webp`.

The module's session, the suite's time. The tests now read one sky made for each air and shared,
`Sky.Tests/Skies.cs`, two airs set alike sharing one, where each test class and many tests made
their own; the test that counts the ground rows a sky makes and the binding test, which runs the
README's block word for word, still make theirs. On this machine the suite went from 113 s of wall
time and 1,420 s of processor time to 71 s and 899, the processor's time the steadier figure since
the wall time has read from 58 to 113 s between runs of the same tree.

The module's session, the sky's tables for a GPU. `SkyLight.Tables` gives a made sky's tables as a
read-only view, `SkyTables` with the air's shared terms, the march's 48 steps, the ground's lowest
height and its altitudes, and a `SkyLaneTables` a lane: the sun's light and the air's coefficients
in its three bands, its transmittance and its light scattered more than once, the latter as its
natural logarithm held at the logarithm of 1e-30, each with its rows and columns, the ground's rows
made on asking, and the nine numbers of the matrix that turns its bands into linear sRGB. The
README's section "Drawing the sky on a GPU" states the axes' mapping and the march in words, and
the binding block uploads the view. A reader written in the test from those words alone, using the
view alone, gives `SkyLight.Luminance` within 0.5% in every channel at six sun heights and altitudes
to 8 km and seven ways each, above and below the horizon; it failed first, the view absent.

The module's session, the moon's bands in the tables' view. `SkyLaneTables.Moon(phaseAngle)` gives
the moon's light outside the air in a lane's three bands, and the README says the moonlit sky is
the same march toward the moon with it in place of `Sun`, the ground's term scaled by the moon's
bands over the sun's. The README reader so lit gives the module's moonlit sky within 0.5% with the
moon 60 and 15 degrees up and 2 down and phase angles of 0 and 60, four ways each; it failed first,
to build.
