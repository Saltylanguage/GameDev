# Working state

This file is the stable doorway into current collaboration context. It should not
become a master changelog.

## Current focus

**Population Reinforcements phase Mutation: 2026-09-23.** The active
experimental offer now uses its third slot for a repeatable `+1` individual of
the player species. A seeded placement chooses an unoccupied, passable cell for
the next phase, respects the population cap, and is retained if the expedition
restarts. This is one selection per phase decision; no balance batch has been
run. The option inherits the current legacy choice cost of 5 Data. The existing
S3-04 product notes describe Mutations as free, so the broader economy
discrepancy remains unresolved. See the
[Population Reinforcements handoff](handoffs/2026-09-23-codex-population-reinforcement-mutation.md).

**Fox hunting and mating energy: 2026-09-23.** Foxes now prioritize
hunting when below 75% of maximum energy and prey is available; that priority
interrupts the short Mating state and defers a reproduction outcome while the
Fox is hunting, eating, or attacking. They can seek a mate at 25% maximum
energy. Each reproduction attempt has a 45% success chance. A successful birth
costs both Fox parents 15% of maximum energy per
offspring (36 each at the authored 240-energy cap); offspring still start with
the authored 48 energy. A pair at exactly 25% when it enters Mating can still
resolve that attempt when metabolism runs first during the tick. The thresholds
and cost scale when maximum energy changes. Focused Fox tests passed 10/10.
The full EditMode suite passed 248/249; the remaining failure is the existing
`EfficientDigestionAccumulatesFractionalEnergyDeterministically` test
(expected 23, got 0), also seen before this change. No matched Forest Edge
balance batch has been run, so treat these as provisional values. See the
[Fox tuning change](handoffs/2026-09-23-2054-codex-fox-hunt-mating-energy.md).

**Forest Edge hare energy behavior: 2026-09-23.** Hare energy loss now occurs
once every 10 simulation ticks. Reserve feeding uses a 6-energy trigger and a
24-energy refill target: dropping below 6 starts refilling; after reaching 24,
the hare stops eating until it falls below 6 again. Other species retain their
existing per-tick metabolism. Focused tests cover the refill cycle, metabolism
cadence, and maximum-energy upgrade behavior. Unity test verification is
pending; the user is currently running an expedition, so avoid interrupting it.
See the [hare feeding and mating handoff](handoffs/2026-09-23-codex-hare-full-energy-mate-seeking.md).

**Field observation board pan and zoom: 2026-09-22.** The custom board now
handles captured left-drag panning and cursor-anchored wheel zoom (0.75x–4x),
with bounds based on the visible board area. This moves the behavior into
`SpeciesSimulationBoard` so shell XAML rewrites cannot silently drop it again.
Static diff checks and a scratch C# compile passed (0 errors; one external System.Net.Http version warning). Unity's live Pipeline connection was unavailable, so Editor compilation and runtime interaction still need confirmation. See the
[pan and zoom handoff](handoffs/2026-09-22-2355-codex-board-pan-zoom.md).

**Forest Edge current authored values: 2026-09-22 diagnostic batches.** Two
Clean CellSim runs used seeds 10100-10119 at 600 and 1,200 ticks on commit
`c6b3282`, with the authored 20x20 / 0.2-second Forest Edge setup, opposed-roll
combat, natural attack opportunities, and no upgrades. Foxes ended extinct in
20/20 seeds by tick 600; Hares ended extinct in 10/20 at both horizons and
averaged 0.80 / 0.85 final individuals. Mean Fox combat kills and starvation
deaths were 15.65 and 15.85 per run, while mean Hare births were 0.60 / 0.80.
Bevin reports an exploratory two-stage concept with Salty: species survival to
earn data first, where collapse without intervention is expected, then data
investment in upgrades to build a healthy, collapse-resistant environment.
These no-upgrade runs are only a first-stage pressure baseline; they do not test
upgraded play or the second-stage goal. This concept is not a finalized spec or
success gate. See the [diagnostic batch
handoff](handoffs/2026-09-22-2200-codex-forest-edge-current-values-diagnostic-batches.md)
and its raw reports and summaries. The September 21 42x20 Fox-6 comparison and
mating-fix evidence remain in the [first balance handoff](handoffs/2026-09-21-codex-forest-edge-first-balance-pass.md).
**Forest Edge balance iteration: 2026-09-21.** The first matched 600-tick
Hare/Fox pass provisionally moves Forest Edge's explicit starting Fox population
from 4 to 6. Across seeds 10100–10119 this increased direct Fox pressure: mean
Fox kills rose from 20.35 to 24.25 and Hare combat deaths rose from 20.35 to
24.25, with no Hare extinction. Keep Fox 6 as the working comparison baseline.
Fox/Hare population equality and final population are descriptive only, not the
balance score. The current direct measures are predation encounters and kills,
Hare post-contact survival, starvation pressure, and whether phase 3 remains
weakened. A paired 300-tick continuation also found the existing Tough Hide →
Threat Exposure path improved Hare post-contact survival (`pAVI` 0.39→0.46)
while increasing phase-3 starvation pressure (31.4→35.5 deaths/run). Evidence
and the rejected Hare starting-energy trial are recorded in the [first balance
handoff](handoffs/2026-09-21-codex-forest-edge-first-balance-pass.md).
The same handoff records the subsequent Fox mating-state fix: mutually ready
adjacent Foxes take mating priority over foraging and do not move while mating.
The six-phase follow-up reduced phase-3 `Mating↔Wandering` transitions from
4.8/5.2 per run to 0.2/0.2, with the live EditMode suite passing 236/236.
The latest follow-up adds a shared 24-tick reproduction cooldown after an
eligible attempt and prevents full reproduction groups from staying in
`Mating`. A 2026-09-22 review found that the cooldown blocked state selection
and vision-based mate pursuit, but a local movement fallback still pulled
cooldown or low-energy Foxes toward each other. The current correction gates
both mate-pursuit paths on both animals being eligible. A focused regression
was added and passed; the full live EditMode suite now passes 240/240. Earlier
mating coverage passed 4/4 focused and 237/237 before this correction. The
latest live five-seed
continuation recorded three Fox births, but zero aggregate `Mating` state
ticks; that telemetry/eligibility discrepancy remains a follow-up, not a
Fox/Hare population-equality target.

**Hare starvation follow-up: 2026-09-22.** The grass reserve increase from
10 to 11.5 did not change matched Hare starvation deaths, so it was not kept.
Using the same Forest Edge scenario, seeds 1–20, 600 ticks, and the current
Hare bite value of 5, grass reproduction `0.0015` produced 283 Hare
starvation deaths. Grass reproduction `0.0115` produced 236, a provisional
16.61% reduction. Keep `0.0115` as the working value; the result is a
starvation-pressure target, not a population-matching claim. Evidence is
recorded in the [grass-starvation candidate](../artifacts/cellular-experiment-20260922-075959/report.json)
and [matched control](../artifacts/cellular-experiment-20260922-074858/report.json).

**Fox hunt / Hare escape follow-up: 2026-09-22.** Hungry Foxes now use their
visible Hare target for vision-based pursuit while in `Hunting`, instead of
falling back to wandering unless prey is already adjacent. Herbivores now enter
`Threatened` when a predator is visible, including a stationary Fox outside
attack range, and the existing escape movement chooses an available cell that
increases distance from that Fox. Focused EditMode checks cover visible-prey
hunting, stationary-threat perception, stationary-Fox escape, and the existing
Fox-pursuit fixture. This is a capability fix, not a claim that Fox and Hare
populations should match; a matched Forest Edge run is still needed to assess
its ecological effect.

**GalapagOS simulation field fit: 2026-09-23.** The desktop starts with the
legacy Lab placeholder collapsed. Forest Edge now uses a 36×21 grid (756 cells,
about 10% fewer than 42×20) while retaining its authored starting animal
counts. The board resolves square cells from its available width and height,
then applies the user's zoom, so the default view fits the whole grid. The C#
test-project build passed and the simulation XAML is well-formed. Visual
PlayMode acceptance is pending because the connected Editor is in Play Mode;
its current camera capture shows Noesis' invalid-license screen.

**Combat default reconciliation: 2026-09-18.** Opposed-roll combat is now the
default across `SpeciesSimulation`, runner construction, checkpoint restore,
the `CellSim` wrappers, and job submission. `RestoreCheckpoint` was the last
runtime API that still defaulted to legacy fixed damage. Legacy fixed damage
remains available when explicitly requested for compatibility or historical
replay. Checkpoints do not record combat mode, so a historical legacy run must
pass that mode when restoring. The focused EditMode regression passed 1/1 in an
isolated project copy; the full suite was not rerun for this correction. See
the [combat default handoff](handoffs/2026-09-18-1458-codex-opposed-roll-checkpoint-default.md)
and [test artifacts](../artifacts/legacy-combat-default-20260918/).

**Unity automation lanes and acceptance: 2026-09-18.** `CellSim` now routes
tests, visual checks, and experiments through `Auto`, `Live`, and `Clean`.
`Live` reuses this project's ready Pipeline Editor for focused feedback;
`Clean` is the reproducible acceptance lane; `Auto` selects between them and
reports busy, Safe Mode, or unreachable locked states without broad process or
lock cleanup. `Doctor` provides fast CLI and license diagnostics. Test filters
support test name, assembly, and the `Core`, `Simulation`, `Graphics`, `UI`,
`Authoring`, and `Tooling` categories. The obsolete 200 tick
`ContinuousSkipPreservesWorldHistoryAndMetricsUntilTheSameAbsoluteTick` test was
removed because it encoded a superseded final-phase decision flow. The exact
full clean command then passed EditMode 234/234 and PlayMode 28/30 with zero
failures and two expected graphics-only skips. Evidence:
[full clean test artifacts](../artifacts/unity-tests-20260918-144956/).

**Unity plugin workflow, Pipeline, and MCP: 2026-09-18.** The project routes
Unity work through [`UNITY_PLUGIN_WORKFLOWS.md`](UNITY_PLUGIN_WORKFLOWS.md).
`com.unity.pipeline` 0.7.0-exp.1 is installed and resolved. Live category tests,
a live seeded experiment, and live visual evidence were exercised; the visual
test passed 1/1 and its 1280x720 screenshot was reviewed. A clean 600 tick
experiment also produced a complete report bundle. The Codex MCP configuration
now uses the installed Unity CLI pinned to this project; the previous
`unity_mcp` user-relay entry was retired. A Codex restart or new task is needed
to load the new MCP server. One warm `UI` category run passed 10/11 while the
same tests passed in the clean full PlayMode suite, so the warm-only failure is
recorded as an Editor-state/test-isolation issue rather than a product failure.
See [`UNITY_MCP_RELAY_OPERATIONS.md`](UNITY_MCP_RELAY_OPERATIONS.md) and the
latest workflow handoff for operational details and artifact paths.

**Island Survivor retirement: 2026-09-18.** The scene/build entry, runtime
slice, dedicated tests and validator, and six Island Chores textures plus Unity
metadata were removed. Historical handoffs remain preserved. Bare-cell resolver
and board-snapshot regressions pass in the current EditMode suite. The repeated
`Terrain_01` sprite-atlas warning also appears in the pre-retirement no-graphics
baseline and is not caused by this cleanup.

**Handoff artifact validation: 2026-09-18.** The previous 102 unavailable
artifact warnings came from legacy handoffs that predate schema 1. Their
original run paths remain as historical provenance; the validator now checks
local Markdown links in those notes without requiring their machine-local
artifacts to remain present. Current schema-1 handoffs still check artifact
availability and pass with zero warnings. See the
[validation handoff](handoffs/2026-09-18-1257-codex-historical-artifact-reference-validation.md).

**Context refresh: 2026-09-18.** The pushed `ProjectMain` baseline includes the
terrain art migration, Bare-cell neighbor-mask correction and regression tests,
and simulation-shell/UI integration. Josh closed S3-01 after integration.
S3-02's player contract is complete. The original S3-03 closeout run passed its
focused End/cancel test 1/1 and no-graphics PlayMode 31/33 with 0 failures and
two expected graphics-only skips. That run also caught and fixed the prototype
scene's stale Noesis resource-dictionary reference. Tests ran in an isolated
copy because editor processes were open; reports are retained under
`artifacts/s3-03-test-results-20260918/`. The later post-retirement results
above supersede the old full-suite totals. See
[`handoffs/2026-09-18-0036-codex-s3-03-flow-recovery.md`](handoffs/2026-09-18-0036-codex-s3-03-flow-recovery.md).
Treat test claims as bounded by the retained evidence below. The S3-03 card is
already complete; see the closeout handoff for the validation record. The
separate three-option Mutation gap is tracked for S3-04, so the Sprint 3 gate
remains open.

**Terrain art standard update: 2026-09-17.** New terrain source tiles are
64x64 pixels at 64 PPU and live under `Assets/Art/Terrain/Blob/64/`; this keeps
each tile one world unit. The terrain atlas retains its scene-referenced GUID.
All 47 Grass masks generated from the 14 authored Forest
representatives are now packed by `Terrain_01.spriteatlasv2` and loaded by the
simulation runtime. `Grass_000` is full dirt and `Grass_255` is full grass.
Desert art is not present yet; its slots remain optional and no Grass art is
used as a substitute. The Island Chores atlases were removed with the retired
Island Survivor slice on 2026-09-18. At that earlier terrain-art checkpoint,
EditMode passed 251/251 and the no-graphics PlayMode run passed 28 with two
expected graphics-only skips. A graphics-capable ForestEdge
board capture also passed 1/1 and shows the Grass/Dirt tiles in context. See
[`handoffs/2026-09-17-codex-terrain-art-standard-64px.md`](handoffs/2026-09-17-codex-terrain-art-standard-64px.md).

Follow-up review found that empty Bare cells were not receiving Grass masks.
Snapshots and both board/paint-preview renderers now resolve a Grass neighbor
mask for Bare cells too, so dirt cells inside a Grass field can display their
Grass vertices. Resolver and snapshot regression tests were added and pass in
the current clean EditMode suite recorded at the top of this file.

**Roadmap v2.2 is active as of 2026-09-17.** M0 is complete and M1 is active.
Sprint 2 closed on 2026-09-17 with Fox telemetry as its sole carry-over. S3
kickoff `S3-KICKOFF-20260917-01` is verified: the committed plan is active for
2026-09-17–2026-09-30. Josh closed S3-01; the latest retained clean validation
records EditMode 234/234 and no-graphics PlayMode 28/30 with 0 failures and two
expected graphics-only skips. The focused phase-decision UI check and the
separate Settings/Collection and Main Menu reviews remain open in Loose Ends
P1-031. Trello S3-01 is in Done with its validation caveat recorded. S3-02's
contract is complete, and Josh has marked its Trello card complete. S3-03 is
complete and Unity-validated with results retained in its handoff. The S3-02
card's acceptance wording may still need a cleanup pass to remove the deferred
board-size and playable-plant decisions.
S3-05 duration/memory measurement remains uncommitted stretch work
and 2h of Sim capacity remains unallocated. S3's priority is a safe game-state
loop with tested recovery and return to the Lab, meaningful and understandable
Mutations, and a bounded visual polish/UI integration pass. Local profile
saving is scheduled for S4. The GalapagOS Desktop is the canonical player home;
the standalone Lab remains a legacy/developer route.
The S3-04 working plan is now recorded. Josh confirmed that Mutation copy will
translate repeatable, predictable Stat-Line impacts into concise qualitative
player guidance, with simpler directional language when the evidence cannot
support a precise claim; raw statistics remain off the player surface. The
approved S3 bridge uses the five existing experimental Hare Mutations as the
rotating offer pool and shows three choices per boundary. The third choice is
now the fixed, repeatable Reinforcements Mutation, which adds one selected
species individual at a deterministic open cell in the following phase.
Selected Mutations can return at later boundaries; Skip has no S3-04 reward.
The population addition and its balance remain provisional pending evidence.

**CF-0 through CF-5 are implemented and verified.** This includes continuation
parity, boundary upgrades, the controlled preview path, phase/final Stat-Lines,
checkpoint replay, the headless schedule, and the accepted EX-010 execution.
CF-6 duration/memory measurement is stretch work rather than an S3 closeout
gate; the Windows player smoke and corrected scenario run are complete, and
graphics acceptance is complete.
The current Desktop acceptance contract directly starts Forest Edge with Hare
when the player opens Simulation. This preserves the merged Bev/Sim simulation
experience. The initial S3-01 baseline on 2026-09-17 had EditMode 247/249 with
two terrain failures and PlayMode 28/29 with one justified graphics-only skip.
The latest retained rerun is `artifacts/unity-tests-20260917-222442/`: EditMode
251/251 passed; no-graphics PlayMode 28 passed with two expected graphics-only
skips. The graphics-capable ForestEdge visual test passed 1/1 and captured the
board under `artifacts/visual-evidence-20260917-222658/`. The original terrain
failures are resolved locally. Josh subsequently closed S3-01 in Trello;
post-fix validation remains tracked separately.
Desert art remains absent. Verifying the end-to-end state/recovery route is S3
work; profile saving is scheduled for S4.
The same-world lifecycle, phase/expedition evidence meaning, initialization-only
upgrade policy, above-cap energy behavior and a versioned fresh-run fixture are
locked in the [consecutive simulation plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md).

The project-owned Noesis/XAML image pipeline is now migrated: every XAML image
and icon consumer uses a `StaticResource` from
[`Assets/UI/ImageResources.xaml`](../Assets/UI/ImageResources.xaml), with
`GlobalResources.xaml` providing the shared merge and Main Menu loading the
dictionary directly. The custom simulation-board sprite atlas remains a
runtime renderer input rather than an XAML image consumer.
The game-design feature sequence is now triaged in
[GAME_FEATURE_ROADMAP_TRIAGE.md](GAME_FEATURE_ROADMAP_TRIAGE.md), starting with
the Expedition Decision Loop. S3-02 is complete: the expedition has six phases,
ten seconds of simulation time per phase, one minute total, and five upgrade
decision moments with three temporary Mutations or Skip at each, one after
each of phases 1–5. Permanent currency purchases in
the Gene Lab are Genome Upgrades that fill a Genome skill tree. Letting the
player skip for extra currency is undecided and non-blocking; if adopted, it
uses the same currency as Genome Upgrades. S3-04 Mutation choices are free and
should be readable at a glance through an icon, identity, and evidence-backed
qualitative direction (for example, “Hunter Lv2 — better tracking and sharper
teeth”); no Stat-Line breakdown is shown. Restart is removed; End abandons
the run after confirmation, forfeits rewards if used before round 6, and Pause
remains available. Round 1 has no upgrade;
rounds 1–5 each lead to a three-Mutation choice or Skip; round 6 ends in results
with no upgrade. Victory is survival to the end of round 6; rewards use a
performance measure currently in development (not simply final population).
Extra bonus-event rewards are possible but undecided. Extinction ends
immediately as a failed run with no rewards. Board size is deferred, and
playable plants (including Fern) are on hold. Older engineering and research
records still contain ten-phase/200-tick values; those are historical
configurations, not the current player contract. The upgrade direction now
separates temporary per-run Species-Simulation **Mutations** from permanent
**Genome Upgrades**, bought with currency in the Gene Lab application and
organized in a species' Genome skill tree, and
configurable **active Genome**. Mutations never enter Biome Simulations. The
active Genome is frozen at launch and applies to every population of that
species, including when it is not player-controlled. Species and Biome
Simulations use different success scorecards. Scalable balance work uses shared
capabilities and provisional Adaptation Value estimates, but requires
mode-appropriate direct-effect, matchup, and ecosystem evidence under
[`SG-005`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md). The seven
existing Hare assets are provisional Mutation candidates. The implemented
Genome foundation carries stable per-species unlocked and active node IDs
through the local profile, launch request, run, checkpoint, and result. Generic
ScriptableObject authoring types resolve metadata into an immutable catalog for
the Gene Lab. The visualization-only five-node Hare and seven-node Fox maps,
their debug selector, and the player-scene provider were removed on 2026-09-17;
the simulation's seven-item Mutation catalog and Bev experimental behavior were
not changed. No production Genome map is currently assigned. Genome effects,
cost validation, economy, buying/activation actions, rule application,
versioned migration, and recovery remain planned. Named Genome loadouts are
deferred and non-blocking, and Species Mastery remains
deferred and non-gating. Skip's possible currency bonus is undecided and
non-blocking; if adopted it uses Genome Upgrade currency, with amount and limits
not yet defined. Provisional scientific-data settlement remains open pending feature-owner
approval.

The integrated branch also contains a Main Menu polish/refinement pass, broader
GalapagOS Desktop/Simulation/Lab XAML and ViewModel updates, and new concept
art for Settings, Species Collection, Expedition Setup, and Field Notes. The
Main Menu handoff is **Needs Review**: the meadow background, CRT-style boot and
desktop handoff, focus behavior, and procedural chime were verified in Unity,
but title/brand and promotion of generated art remain human decisions. The
concept images are references, not approved production assets. A focused
Settings/Collection PlayMode invocation on 2026-09-12 exited without producing
results XML, so it is not acceptance evidence.

The 2026-09-09 artifact-retention audit and cleanup removed only approved,
verified duplicates and old clean logs (about 1.35 GB). Compact summaries and
cited raw evidence remain; five semantic duplicate bundles are still review
candidates under the retention handoff.
The preview now supports phase survivor data, live/legacy upgrade choices,
same-run resume, explicit End, and manual inspection. Continuous terminal
completion is results-only; upgrades are offered at phase boundaries, and a
new expedition is an explicit next action. Continuous phases remain the
default player flow; uninterrupted single-run mode is Developer Mode-only.
Phase result/telemetry windows and ordered acquisition timing are now captured
by the runtime and report serializers. Boundary checkpoints can be copied,
restored, and resumed with deterministic runner output. The opt-in headless
schedule applies cumulative per-phase loadouts and emits the same phase
contract. EX-010 has now executed on the approved ten-phase schedule and was
accepted by Josh and Sim as bounded evidence. P3 is closed under its revised
bounded gate. EX-011 has executed successfully under its preregistered
Open Range/Deer contract; Josh accepted its narrow ordered-combination finding
on 2026-09-17. Reuse remains limited to that tested setup and does not approve
individual-upgrade effects, production balance, or generalized transfer. No
further experiment is selected.
The `bev-experimental` Coupled Hare/Fox response path is enabled in the current
preview: it applies a deterministic free counterpart legacy upgrade at the same
Expedition boundary and records both species' immutable snapshots, origins, and
trigger IDs. Unity EditMode passed 239/239 and the focused same-boundary
PlayMode test passed 1/1 on 2026-09-11. The matched Forest Edge 100-seed
three-arm check passed its direct Fox-hit-conversion gate; the factual result
and the broad PlayMode-suite limitation are recorded in the coupled-response
handoff before any mapping expansion.

Latest retained S3-01 test artifacts are
`artifacts/unity-tests-20260917-174307/EditMode-results.xml` (247 passed, 2
failed, 0 skipped; 249 total) and
`artifacts/unity-tests-20260917-174422/PlayMode-results.xml` (28 passed, 0
failed, 1 skipped; 29 total). Profile persistence and Genome asset-change
tests now pass. In that retained run, both remaining EditMode failures were
terrain checks against the absent `Assets/Art/Terrain/Blob/64` set and atlas.
The Grass family and atlas are now integrated, and focused terrain checks pass
(EditMode 3/3; PlayMode 1/1). The earlier full S3-01 rerun, before the final
population initialization fix, recorded:
EditMode 251/251, no-graphics PlayMode 28 passed with two expected
graphics-only skips, and graphics-capable PlayMode 30/30. The artifact is
`artifacts/unity-tests-20260917-220612/`. PlayMode covers the direct-start
Forest Edge/Hare route, Lab launch/return, and settings-rejection/run-
preservation path. The earlier terrain blocker is resolved locally.
The older paragraph above describes the 2026-09-17 validation snapshot. The
Trello S3-01 card was subsequently marked Done; see the [S3 control
record](Sprints/S3-control-record.md). Existing graphics acceptance evidence
is also retained at 1280×720 and 1920×1080.

The latest retained bundle after the population initialization fix, before the
empty-cell tiling follow-up, is `artifacts/unity-tests-20260917-222442/`:
EditMode 251/251 passed; no-graphics PlayMode 28 passed with two expected
graphics-only skips. The focused graphics-capable ForestEdge scene test passed
1/1 and captured setup, running, rewards, and results under
`artifacts/visual-evidence-20260917-222658/`. A separate full graphics-suite
attempt exited during Unity startup without producing a result file and is
inconclusive.

- Durable product direction: [`PROJECT_CONTEXT.md`](PROJECT_CONTEXT.md)
- Vertical-slice product brief: [`PRODUCT_BRIEF.md`](PRODUCT_BRIEF.md)
- Consecutive simulation phases — review and migration plan: [`CONTINUOUS_SIMULATION_FLOW_PLAN.md`](CONTINUOUS_SIMULATION_FLOW_PLAN.md)
- Stat-Line, predictive AI and telemetry applicability: [`CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md`](CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md)
- Simulation-flow documentation coverage: [`CONTINUOUS_SIMULATION_DOCUMENTATION_AUDIT.md`](CONTINUOUS_SIMULATION_DOCUMENTATION_AUDIT.md)
- Vertical-slice scenario, roster, and builds: [`VERTICAL_SLICE_SELECTION.md`](VERTICAL_SLICE_SELECTION.md)
- Future scientific-data economy: [`SCIENTIFIC_DATA_ECONOMY.md`](SCIENTIFIC_DATA_ECONOMY.md)
- Mutation, Genome, and balance delivery plan: [`UPGRADE_SYSTEM_DIRECTION.md`](UPGRADE_SYSTEM_DIRECTION.md)
- Coupled Hare/Fox implementation goal pack: [`COUPLED_SPECIES_RESPONSE_GOAL_PACK.md`](COUPLED_SPECIES_RESPONSE_GOAL_PACK.md)
- Mutation authoring workflow: [`UPGRADE_AUTHORING_GUIDE.md`](UPGRADE_AUTHORING_GUIDE.md)
- Hare Mutation acceptance matrix: [`UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md`](UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md)
- Official upgrade and ecology balance guideline: [`Studio Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md)
- Official Noesis image resource pipeline: [`Studio Guidelines/SG-006-NOESIS-IMAGE-RESOURCE-PIPELINE.md`](Studio%20Guidelines/SG-006-NOESIS-IMAGE-RESOURCE-PIPELINE.md)
- Upgrade-system planning concerns: [`Planning concerns/upgrade-system.md`](Planning%20concerns/upgrade-system.md)
- Main Menu, Lab, and progression delivery plan: [`MAIN_MENU_LAB_DELIVERY_PLAN.md`](MAIN_MENU_LAB_DELIVERY_PLAN.md)
- GalapagOS desktop art direction: [`../ART_STYLE_GUIDE.md`](../ART_STYLE_GUIDE.md)
- GalapagOS desktop app ecosystem: [`GALAPAGOS_DESKTOP_APP_ECOSYSTEM_PLAN.md`](GALAPAGOS_DESKTOP_APP_ECOSYSTEM_PLAN.md)
- GalapagOS desktop feature set: [`GALAPAGOS_DESKTOP_FEATURE_SET_PLAN.md`](GALAPAGOS_DESKTOP_FEATURE_SET_PLAN.md)
- Unity MVVM and GalapagOS UI architecture: [`UNITY_MVVM_ARCHITECTURE_PLAN.md`](UNITY_MVVM_ARCHITECTURE_PLAN.md)
- Unity MVVM UI contracts: [`UNITY_MVVM_UI_CONTRACTS.md`](UNITY_MVVM_UI_CONTRACTS.md)
- Sprint 0 closeout plan: [`SPRINT_0_CLOSEOUT_PLAN.md`](SPRINT_0_CLOSEOUT_PLAN.md)
- Sprint 1 authoritative execution plan: [`SPRINT_1_PLAN.md`](SPRINT_1_PLAN.md)
- Sprint Kickoff and carry-over workflow: [`SPRINT_KICKOFF_WORKFLOW.md`](SPRINT_KICKOFF_WORKFLOW.md)
- Active production roadmap and sprint plan: [`../ROADMAP.md`](../ROADMAP.md)
- Roadmap v2 review handoff: [`handoffs/2026-09-11-codex-roadmap-v2.md`](handoffs/2026-09-11-codex-roadmap-v2.md)
- Genome contract slice: [`handoffs/2026-09-12-codex-genome-contract-slice.md`](handoffs/2026-09-12-codex-genome-contract-slice.md)
- Genome catalog responsiveness: [`handoffs/2026-09-12-1756-codex-genome-catalog-responsiveness.md`](handoffs/2026-09-12-1756-codex-genome-catalog-responsiveness.md)
- Dummy Genome visualization fixture: [`handoffs/2026-09-12-1847-codex-dummy-genome-visualization-fixture.md`](handoffs/2026-09-12-1847-codex-dummy-genome-visualization-fixture.md)
- Genome tree tile visual pass: [`handoffs/2026-09-12-1938-codex-genome-tree-tile-visual-pass.md`](handoffs/2026-09-12-1938-codex-genome-tree-tile-visual-pass.md)
- Genome map swap debug fixture: [`handoffs/2026-09-12-1959-codex-genome-map-swap-debug.md`](handoffs/2026-09-12-1959-codex-genome-map-swap-debug.md)
- Genome fixture and document reconciliation: [`handoffs/2026-09-17-codex-genome-fixture-and-doc-reconciliation.md`](handoffs/2026-09-17-codex-genome-fixture-and-doc-reconciliation.md)
- Main Menu polish and refinement handoff: [`handoffs/2026-09-09-codex-main-menu-polish-first-pass.md`](handoffs/2026-09-09-codex-main-menu-polish-first-pass.md)
- Artifact retention audit: [`handoffs/2026-09-09-artifact-retention-audit.md`](handoffs/2026-09-09-artifact-retention-audit.md)
- Active Sprint 3 safe game loop and M1 closeout: [`Sprints/S3-control-record.md`](Sprints/S3-control-record.md)
- S3-04 Mutation readability and bounded review plan: [`Sprints/S3-04-mutation-readability-plan.md`](Sprints/S3-04-mutation-readability-plan.md)
- S3-02 expedition contract complete: [`Sprints/S3-02-expedition-contract.md`](Sprints/S3-02-expedition-contract.md)
- S3-03 flow and recovery work: [`handoffs/2026-09-18-0036-codex-s3-03-flow-recovery.md`](handoffs/2026-09-18-0036-codex-s3-03-flow-recovery.md)
- Sprint 3 kickoff handoff: [`handoffs/2026-09-17-1613-codex-sprint-3-kickoff.md`](handoffs/2026-09-17-1613-codex-sprint-3-kickoff.md)
- Stable-but-incomplete feature action plan: [`INCOMPLETE_FEATURES_ACTION_PLAN.md`](INCOMPLETE_FEATURES_ACTION_PLAN.md)
- Proposed next work bucket: [`NEXT_WORK_BUCKET_PLAN.md`](NEXT_WORK_BUCKET_PLAN.md)
- Sprint 1 species stat-line tickets: [`SPRINT_1_SPECIES_STAT_LINE_TICKETS.md`](SPRINT_1_SPECIES_STAT_LINE_TICKETS.md)
- Fox/Rabbit art foundation lane: [`FOX_RABBIT_ART_FOUNDATION_PLAN.md`](FOX_RABBIT_ART_FOUNDATION_PLAN.md)
- Design scratchpad: [`SPECIES_IDEAS_SCRATCHPAD.md`](SPECIES_IDEAS_SCRATCHPAD.md)
- Fun/design values scratchpad: [`WHAT_IS_FUN.md`](WHAT_IS_FUN.md)
- Hunting-strategy ideation: [`Species Design/HUNTING_STRATEGIES_IDEATION.md`](Species%20Design/HUNTING_STRATEGIES_IDEATION.md)
- Reactive species/ecology arms-race plan: [`REACTIVE_SPECIES_ECOLOGY_PLAN.md`](REACTIVE_SPECIES_ECOLOGY_PLAN.md)
- Cellular simulation deferred work: [`CELLULAR_SIM_TODOS.md`](CELLULAR_SIM_TODOS.md)
- Unity simulation execution and experiment tooling: [`UNITY_SIMULATION_TOOLING.md`](UNITY_SIMULATION_TOOLING.md)
- Proposed custom report dashboard developer tooling: [`CUSTOM_REPORT_DASHBOARD_TOOLING_PLAN.md`](CUSTOM_REPORT_DASHBOARD_TOOLING_PLAN.md)
- Cellular sprite sheets and smart-tiling: [`CELLULAR_SPRITE_TILING_PLAN.md`](CELLULAR_SPRITE_TILING_PLAN.md)
- Future AI workflow skills: [`AI_WORKFLOW_SKILLS_PLAN.md`](AI_WORKFLOW_SKILLS_PLAN.md)
- AI-assisted ecology laboratory research plan: [`Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md`](Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md)
- Discord agent collaboration goal: [`DISCORD_AGENT_COLLABORATION_TODOS.md`](DISCORD_AGENT_COLLABORATION_TODOS.md)
- Discord message contract: [`DISCORD_AGENT_COLLABORATION_PROTOCOL.md`](DISCORD_AGENT_COLLABORATION_PROTOCOL.md)
- Legacy prototype audit: [`LEGACY_PROTOTYPE_AUDIT.md`](LEGACY_PROTOTYPE_AUDIT.md)
- Next architecture batch: [`NEXT_ARCHITECTURE_BATCH.md`](NEXT_ARCHITECTURE_BATCH.md)
- One-note-per-task handoff journal: [`handoffs/`](handoffs/)
- Handoff process: [`COLLABORATION_WORKFLOW.md`](COLLABORATION_WORKFLOW.md)
- Loose Ends ledger and review protocol: [`LOOSE_ENDS.md`](LOOSE_ENDS.md)
- Project hygiene ticket summaries: [`PROJECT_HYGIENE_TICKET_SUMMARIES.md`](PROJECT_HYGIENE_TICKET_SUMMARIES.md)

## How to get current

1. Read `PROJECT_CONTEXT.md`.
2. Read the newest handoff notes and any notes relevant to the area being changed.
   Filenames sort chronologically and include the contributor and topic.
3. Confirm the notes against the checked-out branch, `git status`, recent commits,
   code, and tests.

Create a new note instead of editing a running history here:

```powershell
.\tools\New-Handoff.cmd -Owner "your-name" -Topic "short feature name"
```

Each generated note links back here. Notes may be corrected while their work is
still local, but once shared they should normally remain historical records; add
a newer note when status or conclusions change.
