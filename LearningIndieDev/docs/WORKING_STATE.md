# Working state

This file is the stable doorway into current collaboration context. It should not
become a master changelog.

## Current focus

**Chrono species artwork: 2026-10-02, accepted for BevBranch publication.** Both handmade source
sprites now supply the standardized 32/64/128 icons and retained compatibility
exports. Existing asset names and GUIDs remain stable, covering the animal atlas,
Species Catalog and Noesis icon consumers. Both simulation scenes use the Chrono
fox/rabbit board sprites. Rebuild from the originals with **Salty Game > Art >
Rebuild Chrono Species Icons**. Export validation and two focused PlayMode checks
passed; Field Notes and simulation screenshots were inspected. Bevin visually
accepted the integration and authorized pushing it with additive Trello updates.
Separate local prototype population and fox species edits are excluded from the
art checkpoint. See
[art ownership and export instructions](Species%20Design/CHRONO_SPECIES_ART.md)
and the [validation handoff](handoffs/2026-10-02-chrono-species-art-standardization.md).

**S4-01 Hare strategy workbook: 2026-10-01, ready for Salty review.** Bevin
approved starting values, costs and example paths for Trailblazer, Warren
and Gardeners. The workbook is now a repository review artifact; team review,
implementation and balance evidence remain. Read the
[workbook handoff](handoffs/2026-10-01-1711-codex-s4-01-hare-workbook-review.md)
for the decisions and a review prompt for Salty and his agent. The intended
400/20/10, 0.1-second manual S4 fixture differs from committed scenario defaults.

**S4-02 first implementation slice: 2026-10-01, local and ready for review.**
Bevin authorized reversible implementation without waiting for Salty's explicit
approval; team review remains pending. General Movement and Threat Avoidance
now support the Trailblazer path, and the preview Inspector has an in-memory
S4 fixture and paused single-tick controls. Focused validation passed 180
EditMode and two PlayMode tests. Bevin closed the first manual visual-testing
pass on October 1 after checking the controls, Movement acquisition and results.
The seed-5, 400/20/10 run ended in Hare extinction at tick 180; Threat Avoidance
and the full five-pick path were not reached in that manual run. A clipped
Movement card footer and stale Game-view setup display remain UI follow-ups.
Bevin reports preferring 400 Plants / 20 Hares / 10 Foxes during population
exploration. This is the next comparison candidate; the implemented S4 fixture
and authored production defaults have not been changed. Seed Dispersal is now
implemented locally with direct planting counters and a Gardeners offer path;
automated and manual card/results acceptance are complete, while balance
validation remains open. The `CellSim`
wrapper's missing `Resolve-UnityExecutionLane` helper also needs repair before
the larger experiment checkpoint. Read the
[implementation handoff](handoffs/2026-10-01-2216-codex-s4-02-trailblazer-and-editor-manual-testing-first-slice.md)
and the original [six-skill checklist](handoffs/2026-10-01-1947-codex-s4-02-implementation-checklist-and-editor-testing-preflight.md).

**Player slashline display: 2026-10-02, local and ready for review.** The existing
Field Notes button now opens cumulative species metrics without Developer Mode,
and both completed and failed results show the same slashline. Existing domain
calculations, ADD accounting and N/A/INVALID statuses are reused. Live Unity
checks covered Hare/Fox, paused updates, decisions, reinforcement and results;
rendered prototype panels were inspected. Bevin reported the new display looks
good on October 2. The NUnit regression was updated but
not executed, to preserve the open scene's unsaved edits. Read the
[slashline handoff](handoffs/2026-10-01-2358-codex-player-slashline-field-notes-and-results.md).

**Warren crowding energy reduction: 2026-10-02, local and ready for review.**
Bevin selected this next slice. Crowding Tolerance now removes 10% of the
original extra crowding cost per level (cap 10), with a separate fractional
loss accumulator and unchanged ordinary metabolism/crowd eligibility. The
Inspector and reports expose eligible animal-ticks and actual energy lost.
25 focused assertion cases passed in place, compilation passed, and a live
six-tick diagnostic completed the five Warren choices through normal offers;
the offer card was rendered and inspected. This is mechanics/presentation
evidence, not balance or a standard-suite result. The modified-scene dialog
was closed by Bevin; the newly saved prototype scene diff is preserved for
separate review. Read the
[Warren handoff](handoffs/2026-10-02-0143-codex-s4-02-warren-crowding-energy-reduction.md).
Read the [Seed Dispersal handoff](handoffs/2026-10-02-1319-codex-s4-02-seed-dispersal-and-gardeners.md).

The September planning notes below are dated snapshots. Use the
[S4 Trello control card](https://trello.com/c/Zn4UpBYc) for current scope,
ownership and capacity; those older notes are not a fresh kickoff decision.

**S4 active: 2026-10-01.** Sim accepts 20h (14h feature, 3h protected
reserve, and 3h unallocated). Josh confirmed his 20h
availability and responsibilities. Josh chose the active profile plus current
defaults for P1-032 (Forest Edge, Hare, seed 10100), accepted its 6h estimate,
deferred local profile-choice save/restore, and approved 400/25/15 with a
0.1-second step as an experiment variant; the authored production asset
remains 400/55/35 at 0.2 seconds. Missing or invalid profile context must fail
closed to visible profile selection. The control/candidate pilot was attempted
but Unity licensing and Package Manager failures stopped it before simulation;
no experiment result or S4-02/03 re-estimate exists. Josh accepts the current
S4-02/03 estimates as planning assumptions. The pilot remains an early
checkpoint before the full screen, not a kickoff blocker. S4 is Active from
2026-10-01; P1-032 and S4-01/02/03/07 are in Trello Current Work, with the
S4 control card in Roadmap & Milestones. S4-04/05 remain in Backlog. The 8h
reserve is protected.
See the [S4 control record](Sprints/S4-control-record.md), [kickoff handoff](handoffs/2026-10-01-codex-s4-kickoff.md), and [accepted Sim
review](handoffs/2026-09-30-2018-codex-s4-sim-review-and-stat-sheet-strategy-plan.md).

**Loose Ends disposition: 2026-10-01.** P2-005 raw-artifact retention and
P2-022 Terrain Paint diagnostic retention are decided. CF-6 remains optional
Backlog work, and P2-015 is no longer an unbounded cleanup ticket. No raw
reports or Unity assets were removed. See the [ledger](LOOSE_ENDS.md) and
[review handoff](handoffs/2026-10-01-0112-codex-loose-ends-retention-decisions.md).

**P1-029 contract accepted: 2026-10-01.** Josh approved the Fertile Droppings
direction and progression defaults: Gene Lab after three completed runs, a
permanent Rabbit tree license, Rabbit Data-funded active upgrades, a refunded
resettable allocation with a flat Research Data fee, and the existing 8-point
capacity. The node's scent behavior and numeric balance are not implemented or
validated. P1-029 is closed as a design gap; implementation remains future
work. See the [accepted contract](handoffs/2026-10-01-0154-codex-p1-029-first-hare-genome-contract.md)
and [Loose Ends](LOOSE_ENDS.md).

**S4 planning basis: 2026-09-29.** M1 and S3 are closed. The M1 evidence and
cap-aware offer fix are recorded in pushed commits `25642440` and `974827f2`.
Sim's proposed work builds on his recent Forest Edge, Mutation, telemetry, and
report changes. The M1 Trello card is COMPLETE, and S3-05 remains Backlog
stretch work. See the [Sim kickoff handoff](handoffs/2026-09-29-1241-codex-s4-sim-kickoff-approvals.md)
and [M1 closeout](handoffs/2026-09-29-codex-m1-closeout.md).

**Hare purchase stat line: 2026-09-28, shared on BevBranch.** Successful phase-boundary
population additions are now tracked as `ADD` in the run and checkpoint. The
whole-run Herbivore and Predator Stat-Lines include `ADD` in expected FPO and
survival denominators and exclude it from birth-based RFS. The in-game summary,
JSON report, human report, CSV, and independent Hare validator expose or account
for the new field; phase windows still start with their post-purchase opening
population. Static diff inspection and script parsing passed. The locally
installed Unity 6000.4.6f1 Editor compiled the project with zero Console
errors; gameplay behavior remains unverified because no test or live purchase
run was requested. The Editor instance used for compilation was closed.

**Forest Edge population cleanup: 2026-10-01, ready for review on BevBranch.**
See the [cleanup handoff](handoffs/2026-10-01-1916-codex-population-configuration-cleanup.md) for scope and verification. The shared
production default remains 36x20, 400 Plants / 55 Hares / 35 Foxes, zero
starting probabilities and a 0.2-second step. The proposed S4 fixture uses
400/25/15 and an explicit 0.1-second step through experiment inputs; it does
not replace the production asset. `CellSim Run -StartingPopulations` now passes
those inputs through the existing runner. Saved explicit preview presets are
preserved; preview settings without an override follow the selected asset.
Unity 6000.4.6f1 clean focused validation passed 10/10
`PopulationConfigurationTests` EditMode cases and 2/2 existing developer-settings
PlayMode cases. A one-seed, two-tick `CellSim Run` smoke check at 0.1 seconds
recorded 400/25/15 at tick zero; the production asset still matches HEAD.
The checks exercise combined grid/count overrides, invalid inputs, saved v4/v5
presets, defaults following authored counts, and edit/Undo/save/reload.
Local results are under `artifacts/unity-tests-20261001-180224/`,
`artifacts/unity-tests-20261001-180800/` and
`artifacts/cellular-experiment-20261001-180623/`. This is focused functional
validation, not a full-suite, visual Catalog interaction or balance acceptance.

**Historical Forest Edge no-choice run: 2026-09-29, local on BevBranch.** This
run used a locally edited 36x20 scenario with 400 Plants, 25 Hares and 15
Foxes, all starting probabilities zero. It did not establish a shared baseline.
A matched 200-seed no-choice run (10100–10299) through the locally installed
Unity Editor, using the saved desktop 0.1-second step, observed Hare extinction
in 178/200 runs (median tick 211) and Fox extinction in 196/200 (median tick
351); only 4 runs reached tick 600. Plants averaged 402.05 at tick 200, while
Hare combat deaths outnumbered starvation deaths, consistent with predation
being the main early pressure in that configuration. Later checkpoint counts
are censored by runs ending when the selected Fox player species goes extinct. The scenario
asset still authors a 0.2-second step; this evidence is for the live saved
0.1-second setting. See the [shared historical evidence summary](Research/Reports/2026-10-01-historical-forest-edge-populations/report.md).
Raw reports and the original analysis remain in ignored local artifacts.

**Historical composite-Mutation pilot: 2026-09-29, local.** The catalog Mutation,
`warren-guarded-burrow` (block +2, movement speed -0.25), was tested alone
against a same-seed no-Mutation control on seeds 10100-10119 using a local
400/25/15 Forest Edge start, a 0.2-second step and Hare as the player species.
This differs from the 0.1-second, Fox-player historical baseline above.
It was acquired after phase 1 and retained through
the six 100-tick phases. Control had 2/20 Hare extinctions and mean terminal
Hare 119.05; Guarded Burrow had 0/20 and mean terminal Hare 137.10. Mean APS
was 1.35887 versus 1.56838. All 20 slash lines per arm independently validated
with the documented HPS/EHS/ECN raw-event limitations. This is directional pilot
evidence only. Guarded Burrow combines block and a movement penalty; it does
not validate the proposed S4 basic Hide skill. Reconcile the earlier full
200-seed follow-up with the current S4 pilot-first plan before scheduling more
work. The [shared historical summary and per-seed slash lines](Research/Reports/2026-10-01-historical-forest-edge-populations/report.md)
preserve the comparison; raw reports and validators remain local artifacts.

**Previous Forest Edge baseline: 2026-09-29.** The 400 Plants / 55 Hares / 5
Foxes run remains a historical comparison. Its report is local-only at
`artifacts/forest-edge-startpop-screen-20260929/report.md`; it is not part of
the shared evidence package.

**Scenario population authoring: 2026-09-29, local.** The Species Catalog's
Scenario Roster exposes editable starting-population fields on each species
card. Edits use Unity's serialized-object Undo path; click **Save Assets** to
persist them, then start or restart a preview without an explicit population
override to use the selected scenario's authored values. The historical run
loaded the locally edited scenario asset without a starting-population
override and verified 400 Plants / 25
Hares / 15 Foxes at tick zero in all 200 seeds. The screenshot/run values are
recorded locally. This authoring tool does not approve a production balance change.

**S3-08 Fox telemetry clarification: 2026-09-27, validated.** The report
already distinguishes pre-resolution FSM state ticks from resolver outcomes;
the reproduction funnel now also exposes eligible attempts (chance failures,
no-location outcomes, and successful attempts) alongside the blocked gates.
This is a derived metric and does not change Fox behavior. The new Editor test
initially caused Safe Mode because the project's NUnit version does not support
`Assert.Multiple`; separate `Assert.That` calls fixed the compile error. The
direct installed Unity CLI EditMode runs passed the report serializer and
runtime reproduction funnel tests (1/1 each). A five-seed, 600-tick Forest
Edge report on the authored 36x20 grid contains
`reproductionEligibleAttempts`; the generated Markdown includes the Eligible
column. The current Editor log has no compiler errors. The 177 warnings in the
user's screenshot are not the Safe Mode blocker. Artifacts are under
`artifacts/s3-08-reproduction-telemetry/`. See the [S3-08 telemetry
handoff](handoffs/2026-09-27-codex-s3-08-fox-telemetry.md).

**S3-04 Mutation readability and first bounded review: 2026-09-27.** The
phase-choice path now offers three distinct free Hare Mutations plus Skip,
allows repeat picks to increase the level, and keeps the same expedition
running. Tough Hide was approved as the first evidence candidate. A matched
20-seed, six-phase Forest Edge/Hare comparison supports the directional copy
“Block more incoming attacks,” but does not establish a broad population
benefit. The player-facing copy and generic A/B/C markers are accepted for this
slice. Focused EditMode 1/1 and three PlayMode checks passed in the installed
Unity Editor. See the
[S3-04 Tough Hide handoff](handoffs/2026-09-27-codex-s3-04-tough-hide.md).

**Forest Edge board visual pass: 2026-09-27.** The visual pass was merged into
`BevBranch`, based on the birth-poof pilot. The board now
has a pixel canopy edge, low ground details tied to grass/resource state, and
prototype side rock clusters that sit outside the traversable field. Birth
presentation now gathers all children placed in the latest birth tick, enlarges
the poof, and retains the heart and sparkles. A short code-synthesized litter
chime is rate-limited and uses the desktop volume/mute controls. The view model
tracks up to four actual adjacent-cell steps from one Fox that is currently
hunting; their marks fade over nine simulation ticks and persist through pause.
The retained baseline and first changed 1280×720 views are in
[`visual-evidence-20260927-095257`](../artifacts/visual-evidence-20260927-095257/)
and [`visual-evidence-20260927-095849`](../artifacts/visual-evidence-20260927-095849/).
The first changed image shows a clear wooded edge, but the center still reads
as the original grass/bare grid. Later variation and rock placement edits have
not been captured. Unity crashed during a later live visual run; the clean lane
then reported the project lock as unreachable. Doctor also reported the Unity
licensing client and Unity services endpoint unreachable. The merged project
compiled in the directly installed Unity 6000.4.6f1 Editor on 2026-09-27;
full visual acceptance remains open. See the
**Forest Edge Hare metabolism first pass: 2026-09-28.** The production Hare
energy-loss interval is now 8 ticks instead of 10; grass, reproduction, and
other Hare values are unchanged. This is a provisional first pass to intensify
the no-predator famine, not a Fox–Hare equilibrium decision. The matched
no-Fox 20-seed test showed a smaller mean Hare peak (155 vs. 171), grass
depletion about 25 ticks earlier, and extinction by tick 600 in 20/20 runs
(baseline 11/20). The same in-progress Hare dispersal code was present in both
arms, so its effect is not isolated. See the [comparison analysis](../artifacts/cellular-experiment-20260928-034040/ai-analysis-v1.md),
[candidate report](../artifacts/cellular-experiment-20260928-034040/report-summary.md),
and [baseline report](../artifacts/cellular-experiment-20260928-033949/report-summary.md).

**Crowded Hare dispersal: 2026-09-27.** The fallback Hare movement path now
allows an overcrowded Hare to move into an adjacent passable, empty cell above
the local group-size cap when that move reduces nearby Hare density. Normal
mate-seeking, feeding, threat escape, and non-Hare movement retain their
existing priorities. A focused regression test was added, but the project test
runner did not execute it because the connected Unity Editor is currently in
Play Mode. No map-spread or starvation experiment has been run, so this is a
provisional behavior change, not a validated population outcome. See the
[crowded-Hare dispersal handoff](handoffs/2026-09-27-1124-codex-crowded-hare-dispersal.md).

**Forest Edge board visual pass: 2026-09-27–28.** Work continues on
`codex/forest-edge-visual-pass`, based on the birth-poof pilot. The board has a
pixel canopy edge, low ground details tied to grass/resource state, and
prototype side rock clusters outside the traversable field. Birth presentation
gathers all children placed in the latest birth tick, enlarges the poof, and
retains the heart and sparkles. A short code-synthesized litter chime is
rate-limited and uses the desktop volume/mute controls. The board view model
tracks up to four actual adjacent-cell steps from one Fox that is currently
hunting; their marks fade over nine simulation ticks and persist through pause.
The compact phase tracker now gives its active pixel bunny a soft double
heartbeat pulse. A small Field Ledger reaction card appears for a new birth or
Fox hunt, stays hidden between events, uses existing rabbit/fox pixel art, and
lets births take priority over hunt notices. The reviewed 1280×720 and
1920×1080 captures are in
[`visual-evidence-20260928-163621`](../artifacts/visual-evidence-20260928-163621/)
and [`visual-evidence-20260928-163450`](../artifacts/visual-evidence-20260928-163450/).
The center still reads mainly as a grass/bare grid; trees and other habitat
variation remain future work. Unity crashed during a later visual attempt on
2026-09-27, but the Editor reconnected on 2026-09-28 and the focused visual
acceptance test passed at both resolutions. The captures manually trigger the
reaction presentation; a natural birth/hunt through the full event route and a
full suite have not yet been verified. A clean full PlayMode attempt on
2026-09-28 stopped before tests began because the Unity licensing client threw
an `ObjectDisposedException` and Package Manager IPC timed out; it produced no
test verdict. See the
[field-pass handoff](handoffs/2026-09-27-forest-edge-visual-pass.md).

**Crowding and starvation: 2026-09-26.** Production Hares and Foxes use a
2× metabolism multiplier while their local group exceeds its size limit and
tolerance. The crowding pass no longer kills creatures directly; lethal energy
loss is classified as starvation. This is implemented in the
[simulation step](../Assets/Scripts/Game/Simulation/SpeciesSimulation.cs). No
post-change simulation or tests have been run, so the balance effect is
unverified.

**Time-based grass spread: 2026-09-24.** Grass now spreads on a per-tile
simulation-time interval instead of a per-tick chance roll. The provisional
17.4-second interval preserves the previous expected wait at Forest Edge's
0.2-second tick interval. Each plant tile tracks elapsed simulation ticks,
converted using the scenario step duration; the timer pauses while an animal
occupies the tile and resets after successful spread. Initial plant timers are
seeded at staggered points in the cycle to avoid a synchronized first wave.
This changes spreading; passive food-reserve regrowth keeps its current
behavior. No tests or matched balance runs have been run for this change, so
the interval remains unverified. Hare seed-drop chance is zero in the
production definition, diagnostic definition, and scenario generator, so
Hares consume grass locally without reseeding it. See the
[time-based grass growth handoff](handoffs/2026-09-24-codex-time-based-grass-growth.md).

**Hare mate-seeking and reproductive drive: 2026-09-24.** The production Hare
now has a 100% reproduction chance and can seek a mate at 50% of its 48-energy
cap (24 energy), matching the existing 50%-cap mating cost. Once eligible, a
Hare paths toward the nearest same-species individual without a reproduction
cooldown anywhere on the board before foraging; this social search is separate
from vision so Fox-detection range stays unchanged. The local group limit is
raised from 3 to 7 so a pair and its maximum five-offspring litter can use the
same patch; at that time, direct crowding deaths were still active. The
2026-09-26 update replaced them with doubled metabolism under crowding. The
distant-mate and gathered-three regressions passed, and the final full EditMode suite passed
259/259 at
[`unity-tests-20260924-150134`](../artifacts/unity-tests-20260924-150134/).
No matched Forest Edge balance run has measured the ecological effect yet, so
these remain provisional.

**Population Reinforcements phase Mutation (historical snapshot): 2026-09-23.**
At that point the experimental offer used its third slot for a repeatable `+1` individual of
the player species. A seeded placement chooses an unoccupied, passable cell for
the next phase, respects the population cap, and is retained if the expedition
restarts. This is one selection per phase decision. Phase-boundary Mutations
are free; the fixed option's separate legacy catalog cost is not charged. No
balance batch has been run. See the
[Population Reinforcements handoff](handoffs/2026-09-23-codex-population-reinforcement-mutation.md)
and its [2026-09-24 free-choice follow-up](handoffs/2026-09-24-1341-codex-free-mutations-and-near-term-desktop-migration.md).

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
The full Unity suite passed EditMode 256/256 and PlayMode 33/33 on 2026-09-24;
see [retained results](../artifacts/unity-tests-20260924-134934/). No matched
Forest Edge balance batch has been run, so treat these as provisional values. See the
[Fox tuning change](handoffs/2026-09-23-2054-codex-fox-hunt-mating-energy.md).

**Forest Edge hare energy behavior: 2026-09-23.** Hare energy loss now occurs
once every 10 simulation ticks. Reserve feeding uses a 6-energy trigger and a
24-energy refill target: dropping below 6 starts refilling; after reaching 24,
the hare stops eating until it falls below 6 again. Other species retain their
existing per-tick metabolism. Focused tests cover the refill cycle, metabolism
cadence, and maximum-energy upgrade behavior. The full Unity suite passed
256/256 EditMode and 33/33 PlayMode on 2026-09-24; see
[retained results](../artifacts/unity-tests-20260924-134934/).
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

**Historical Hare starvation comparison: 2026-09-22.** The grass reserve increase from
10 to 11.5 did not change matched Hare starvation deaths, so it was not kept.
Using the same Forest Edge scenario, seeds 1–20, 600 ticks, and the current
Hare bite value of 5, grass reproduction `0.0015` produced 283 Hare
starvation deaths. Grass reproduction `0.0115` produced 236, a provisional
16.61% reduction. This chance-based value was superseded by the time-based
spread interval on 2026-09-24; the measured result remains historical evidence,
not a validation of the new interval. Evidence is recorded in the
[grass-starvation candidate](../artifacts/cellular-experiment-20260922-075959/report.json)
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
legacy Lab placeholder collapsed. Forest Edge's production scenario now uses
36×20 (720 cells, about 14.3% fewer than 42×20) while retaining its authored
starting animal counts. The production asset is the gameplay source of truth;
the editor generator writes to the separate legacy `CellularSimulation`
scenario path, so changing that generated asset did not change gameplay. The
board resolves square cells from its available width and height, then applies
the user's zoom, so the default view fits the whole grid. The existing visual
acceptance assertions now expect 36×20. Unity PlayMode acceptance was not
rerun for this change.

**Saved-grid override follow-up: 2026-09-24.** The connected Editor had a
64×64 v3 saved default, which was overriding the Forest Edge production asset
and matching the square board in the screenshot. Startup now migrates older
saved settings once, preserving their other values and replacing only width
and height with the selected scenario's authored dimensions. Newer saved
defaults still retain custom dimensions. The connected Editor is not in Play
Mode, so the visual result awaits the next runtime launch.

**Saved-grid migration follow-up: 2026-09-26.** The v4 migration only handled
v3 keys; an existing v4 saved default could still override the 36×20 scenario.
Startup now migrates both v3 and v4 dimensions into v5 from the selected
scenario while preserving other saved values. A v5 custom grid remains intact.
The v4 64×64 migration regression passed 1/1 in the locally installed Unity
6000.4.6f1 Editor. The Forest Edge visual acceptance test also passed 1/1;
both the runtime preview and board snapshot reported 36×20. Its rendered field
capture is [here](../artifacts/direct-unity-grid-check-20260926/03-galapagos-simulation.png).

**S3-08 telemetry distinction: 2026-09-26.** FSM behavior ticks are recorded
before attack resolution; a successful Fox attack can set the persisted cell to
Eating afterward. The latest retained 130-tick Play Mode report shows 47 Fox
food successes with no Fox Eating decision ticks, while its reproduction funnel
classifies all 4,550 Fox candidates and reconciles. These are separate
decision/outcome measures, not contradictory counts. The Markdown report now
labels FSM decision ticks explicitly, and a focused regression asserts the
post-resolution Eating state. That targeted EditMode test passed 1/1 in the
locally installed Unity 6000.4.6f1 Editor. This does not replace the longer
six-phase reproduction evidence requested by P1-035; no balance values changed.

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

**Roadmap v2.2 remains active.** M0 and M1 are complete; Josh closed M1 on
2026-09-29 after accepting the distributed exit-gate evidence. Sprint 2 closed on 2026-09-17 with Fox telemetry as
its sole carry-over. Sprint 3 closed on 2026-09-29 after manual review. The
committed S3 cards and control card are in Trello Done; S3-05 remains in Backlog
as uncommitted stretch work. The latest merged-baseline Clean result is recorded
at the top of this file and in the closeout handoff. S3-02's contract is
complete, S3-03 is Unity-validated, and S3-04 V1 was accepted within its bounded
scope. Chrono is producing the selected Main Menu art, with further polish as
iterative work. At S3 closeout, the S4 plan still needed refinement; S4 was
subsequently activated on 2026-10-01. Retesting may be needed as later work
changes the accepted baseline. Local profile-choice save/restore is deferred beyond S4.
The GalapagOS Desktop is the canonical
player home; the standalone Lab remains a legacy/developer route.
The S3-04 working plan is now recorded. Josh confirmed that Mutation copy will
translate repeatable, predictable Stat-Line impacts into concise qualitative
player guidance, with simpler directional language when the evidence cannot
support a precise claim; raw statistics remain off the player surface. The
earlier S3 bridge used two rotating Hare Mutations plus fixed repeatable
Reinforcements. S3-04 replaced that offer with three distinct free Hare
Mutations from the five-effect pool; repeat selections at later boundaries
increase the selected level. Skip has no S3-04 reward. The earlier population
addition and its balance remain provisional historical work.

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
uses the same currency as Genome Upgrades. At Hare phase decisions, a separate
repeatable reinforcement purchase adds one Hare to the next phase for 10 Field
Data while space and Data remain; it does not use the Mutation choice. The
phase screen shows the available Data and labels free Mutation offers. S3-04
Mutation choices are free and
should be readable at a glance through an icon, identity, and evidence-backed
qualitative direction (for example, “Hunter Lv2 — better tracking and sharper
teeth”); no Stat-Line breakdown is shown. Restart is removed; End abandons
the run after confirmation, forfeits rewards if used before round 6, and Pause
remains available. Round 1 has no upgrade;
rounds 1–5 each lead to a three-Mutation choice or Skip; round 6 ends in results
with no upgrade. Victory is survival to the end of round 6; rewards use a
performance measure currently in development (not simply final population).
Extra bonus-event rewards are possible but undecided. Extinction ends
immediately as a failed run with no rewards. The Forest Edge production default
is 36×20; playable plants (including Fern) remain on hold. Older engineering
and research records still contain ten-phase/200-tick values; those are historical
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
results, so it was not acceptance evidence. This gap is superseded by the
retained 2026-09-24 full suite and focused My Collection result linked in Loose
Ends P1-031.

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
- Closed Sprint 3 closeout and M1 gate review: [`Sprints/S3-control-record.md`](Sprints/S3-control-record.md)
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
