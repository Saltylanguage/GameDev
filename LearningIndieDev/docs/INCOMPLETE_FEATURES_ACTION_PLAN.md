# Incomplete Features Action Plan

## Purpose

The [consecutive-phase migration](CONTINUOUS_SIMULATION_FLOW_PLAN.md) is now
implemented and canonical: the next phase preserves the ecosystem after purchase
or Skip, with ten 200-tick phases, phase/final Stat-Lines, checkpoints, and a
validated research schedule. Remaining work is the player-facing Mutation,
outcome, reward, persistence, and integrated release-acceptance layer built on
that runtime.

This plan converts the stable-but-incomplete feature inventory into an ordered
production sequence. The goal is to finish one coherent vertical slice rather
than continue expanding the number of partially usable systems.

The current product target remains Forest Edge: Fern support, Hare player, and
Fox opposition. Work is complete only when the player can understand a choice,
observe its consequence, finish a run, receive a reward, and begin a meaningful
next run.

## Status at 2026-09-07

| Area | Current status | What remains |
| --- | --- | --- |
| Board and presentation | Graphics baseline accepted | Continue feature-specific screen/content work and any readability polish found during play. |
| Simulation truth | Bounded baselines and telemetry are recorded | Balance promotion, especially Forest Edge carrying limits and trustworthy player-facing upgrade effects. |
| Continuous ten-phase runtime | Implemented and canonical | Three player choices, terminal outcome presentation, results explanation, and integrated performance/build acceptance. |
| Main Menu and Lab | UI foundation implemented | Real launch/result handoff, full navigation acceptance, and persistence-aware data. |
| Profile and Genome | Not implemented | Versioned save/profile, one persistent unlock, and the open Mutation/Genome player contract. |
| Research/tooling | P3 closed as bounded | No next experiment selected; evidence-hygiene controls and artifact retention remain prerequisites for future confirmatory work. |
| Retained prototypes | Classified | Island Survivor is deprecated, Discord bridge is on hold, and other retained prototypes change only for a named need. |

## Immediate priority: make the board beautiful and readable

The grid is the product's primary screen. It must become pleasant to look at
before we add more species, upgrades, or Lab surfaces. A technically correct
simulation with an unattractive or confusing board will not prove the game.

### V0.1 — Establish the visual target

- Review the supplied colored species references and terrain sheets at actual
  board scale, not only as isolated source images.
- Lock a compact visual language: terrain palette, role colors, species scale,
  silhouette contrast, outline/shadow treatment, grid spacing, selection state,
  danger state, and upgrade feedback.
- Define representative acceptance captures at 1280x720 and 1920x1080.
- Decide the intended board composition: background treatment, playable-board
  framing, HUD density, panel hierarchy, and how much empty space surrounds the
  simulation.

**Exit:** a short visual target note and two annotated reference captures that
state what “beautiful, readable, and pleasant to watch” means at gameplay scale.

### V0.2 — Make smart tiling correct with the authored assets

- Run the editor smart-tiling preview for all 47 normalized blob masks.
- Confirm the mask convention (`N=1`, `E=2`, `S=4`, `W=8`) against the authored
  terrain sheet and correct only the resolver lookup when an edge is wrong.
- Verify grass and temporary desert/bare families, including isolated, straight,
  corner, T, and surrounded tiles.
- Verify atlas import settings, texture filtering, transparency, and pixel
  scale at the target board size.
- Keep neighbor masks and atlas indices presentation-only; do not move them into
  simulation state.

**Exit:** all 47 valid normalized blob masks render correctly in the preview and in the live board,
with no visible seam or orientation error.

### V0.3 — Make species and terrain presentation coherent

- Verify stable name-based mapping for all eight authored animal atlas entries.
- Keep direct Fox/Rabbit scene overrides as optional layers over the complete
  atlas fallback.
- Replace hard-coded or role-only fallback visuals where they create identity
  confusion.
- Use the supplied compact geometric silhouettes at actual cell size; tune
  contrast and sprite scale before adding more art.
- Add a dedicated plant treatment when it improves readability; until then,
  keep plant-resource presentation deliberately consistent with the terrain
  language rather than borrowing an unrelated animal glyph.
- Add readable overlays for selected species, active danger, recent death,
  feeding, reproduction, and upgrade influence without covering the board.

**Exit:** a player can identify terrain, Fern, Hare, Fox, occupied cells, and
the important current pressure without developer explanation.

### V0.4 — Validate the view in Unity

- Open the terrain preview and `CellularAutomataPrototype` in Unity.
- Capture gameplay-scale screenshots at both target resolutions.
- Check board scale, sprite readability, terrain seams, UI overlap, and visual
  hierarchy during running, paused, reward, and results states.
- Record defects and fix the smallest presentation seam responsible.

**Status:** Accepted for the current desktop/Simulation composition on 2026-09-07.
The full 1280×720 graphics PlayMode batch passed 22/22 and the focused
1920×1080 run captured the same route. The older Windows development build
completed a bounded smoke; a current combined-build smoke remains open.

## Phase 1 — Make the simulation truth trustworthy

### 1.1 Forest Edge balance and rules

**Status:** The fixed-seed evidence and telemetry reconciliation are bounded and
recorded. The held-out upgrade arms did not earn balance promotion, so Forest
Edge carrying limits and player-facing balance remain open by design.

- Use the shared Unity preflight and run the focused simulation tests plus the
  known Forest Edge seed.
- Reconcile candidate, energy, mate, group-limit, chance, no-space, and success
  counters against births.
- Validate the Fox eating/action telemetry fix: reports now separate resolver
  attempts, successes, and failures from pre-resolution behavior-state ticks.
- Run fixed-seed base comparisons before changing balance values.
- Establish a meaningful regional Fern carrying limit and compare Hare outcomes
  against the vertical-slice target.

**Exit:** the known seed reproduces, telemetry reconciles, and a fixed multi-seed
baseline is recorded without unapproved balance changes.

### 1.2 Scenario and authored-data boundary

**Status:** Complete for the current slice. Forest Edge is product-owned;
OpenRange, Wetland, and BaselineParity remain explicit Dev Lab/research fixtures.

- Keep Forest Edge as the production scenario.
- Treat OpenRange, Wetland, and BaselineParity as Dev Lab/research fixtures.
- Validate that scenario assets produce immutable run-start snapshots and stable
  fingerprints.
- Do not tune the full species library before the Hare slice is understandable.

**Exit:** one scenario is product-owned; the others are explicitly experimental
and cannot silently expand the slice.

## Phase 2 — Finish the actual run and Mutation loop

### 2.1 Mutation grammar and balance foundation

Define a small explicit catalog, not a general modifier framework. It must cover:

- Trailblazer: movement/perception and mobility tradeoffs.
- Warren: protection, crowding, or controlled reproduction tradeoffs.
- Gardeners: feeding efficiency, food reserve, and seed-dispersal tradeoffs.

For every Mutation, record the affected rule, shared capability, valid range,
stacking/exclusion rule, visible preview, expected consequence, ecological
obligation, counterplay, and direct plus ecosystem telemetry. Follow
[`SG-005`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md); an
internal Adaptation Value estimate is a planning budget, not production
approval.

### 2.2 Ten-phase run contract

**Status:** The continuous-state ten-phase lifecycle, ordered loadout, phase
boundaries, and phase/final reporting are implemented and canonical. The
remaining work below is player-facing content and acceptance.

- Preserve the implemented ten phases and 200-tick phase boundary with automatic pause at reward breaks.
- Offer three meaningful choices after phases one through nine.
- Record the ordered Mutation loadout in the effective ruleset and result.
- Implement victory, narrow survival, defeat, phase summaries, and immediate
  extinction according to `PRODUCT_BRIEF.md`.
- Make the results screen explain population changes, deaths by cause, feeding,
  movement, combat, and Mutation contributions.

**Exit:** three Hare Mutation builds produce visibly different, reproducible
behavior in the same Forest Edge scenario, with their local effects, likely
combinations, and wider Hare / Fox / Fern consequences reviewed.

## Phase 3 — Build the player-facing shell

### 3.1 Main Menu and Lab foundation

**Status:** The representative-data UI foundation and Sprint readiness closure
are complete. Target-resolution graphics acceptance and the real wallet/
simulation handoff remain part of the integrated player slice.

- Preserve the closed Sprint 0 readiness decisions and recorded Sprint 1 ownership.
- Preserve Main Menu → Lab Overview → Hare Genome preview in
  `MainMenu.unity` using representative data only.
- Add visible focus, deterministic Back behavior, target-resolution checks, and
  a Windows development-build smoke path.
- Keep the player Lab separate from the current developer/authoring surface.

**Exit:** the UI-only route is accepted before any real wallet or simulation
handoff is connected.

### 3.2 Expedition and results connection

- Add Expedition Setup for Forest Edge + Hare.
- Pass stable scenario/species IDs and selected options through an immutable
  launch request.
- Connect simulation completion to Results and return to Lab without pretending
  that representative data is persistent.

**Exit:** a player can navigate from Lab to a clearly identified run and back.

## Phase 4 — Add persistence and meta-progression

- Define versioned profile and settings data with migration/corrupt-save tests.
- Implement the scientific-data wallet only after run telemetry is trustworthy.
- Settle earned, spent, banked, and lost data deterministically.
- Add one permanent Hare Genome-node unlock, one configurable active state, and
  one predetermined first-victory unlock.
- Resolve every participating species' frozen active Genome at simulation
  launch, including species that are not player-controlled, without mutating
  authored base data. Mutations remain exclusive to Species Simulations.
- Add the smallest useful Hare mastery objective.
- Keep active-run save/resume, multiple profiles, cloud saves, and broad Genome
  trees out of the first slice.

**Exit:** a fresh profile can complete a run, receive one defined unlock,
restart, turn the Genome node on or off, and see it affect Hare on the next run
only when active—even when Hare is not the selected species.

## Phase 5 — Research and developer tooling

### 5.1 CellSim and report pipeline

**Status:** The bounded continuous-flow/report slice is implemented and validated
through EX-010. Raw-artifact retention and full provenance remain open controls
for future confirmatory research.

- Retain current automated-test and historical build-smoke logs as evidence;
  run a fresh build smoke for release acceptance. Keep schema semantics explicit
  across legacy fresh-window and current continued-world reports. Historical
  schema-6 EX-002 reports remain valid for their bounded window, while two cited
  raw control bundles are absent from this checkout.
- Keep JSON/CSV factual exports separate from presentation and dashboard ideas.
- Validate the committed JSON/CSV editor converter and assign its owner.

### 5.2 Predictive ecology research

**Status:** P3 is closed as bounded. EX-003 is deferred without a result and no
P4–P6 experiment is selected. New research requires a human-approved contract,
capacity decision, and artifact-retention plan.

- Preserve the completed bounded EX-002 schema-6 matrix and its held-out check;
  do not rerun or broaden it without a new protocol.
- Keep causal claims, calibration, and workflow-value claims separate from the
  already accepted reproducibility evidence.

### 5.3 Optional report dashboard spike

The spreadsheet-authored dashboard is a proposal, not a current feature. Do not
build it until the report schema and one real user workflow justify a one-to-two
day feasibility spike.

## Phase 6 — Decide the fate of retained prototypes

### Island Survivor

Deprecated. Keep its surviving scene/tests isolated only as historical reference;
do not extend, reconnect, or include it in current acceptance work.

### Cave generation

Keep the deterministic generator and tests. The orphaned `CavePreview`
presentation path was removed in the focused 2026-09-06 cleanup; add a new
preview only if a concrete experiment requires it.

### Life simulation

Keep the generic grid/Life code and tests as reference infrastructure. Do not
invest in a presentation scene unless a concrete product or research use appears.

### Discord collaboration bridge

Leave deferred until coordination cost justifies authenticated transport and a
restricted proof. Repository handoffs remain authoritative.

### Alpha offspring

Keep as a bounded custom-rule experiment. Revisit qualification, inheritance,
caps, or pack behavior only when a scenario or upgrade requires them.

## Definition of vertical-slice completion

The project can stop calling the core feature set “half finished” when all of
the following are true:

- The board is visually attractive and readable at both target resolutions.
- Smart tiling has passed the 47-mask and live-board checks.
- Forest Edge has a trustworthy fixed-seed baseline and reconciled telemetry.
- Trailblazer, Warren, and Gardeners are understandable and measurably distinct.
- A player can complete the ten-phase run without developer fields.
- Results explain the outcome and award one persistent unlock.
- Main Menu → Lab → Run → Results → next Lab is navigable and tested.
- One fresh profile can restart and use the unlock.
- Remaining prototypes and research tools have explicit retained, deferred, or
  archived status.

## Current blockers and decisions

1. The embedded Noesis editor analytics path requires a vendor/project privacy
   decision before release; it is editor-only and not compiled into the player.
2. A current Windows development-build smoke and ten-phase duration/memory
   measurement remain; target-resolution graphics acceptance is complete.
3. Forest Edge upgrade evidence does not support balance promotion. Any new
   upgrade/control claim needs a fresh bounded contract; P3 research closure does
   not approve player balance.
4. The player-facing Mutation/Genome relationship, three-choice catalog,
   terminal outcomes, reward settlement, and persistent unlock loop remain
   incomplete.
