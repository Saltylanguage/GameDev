# Incomplete Features Action Plan

## Purpose

This plan converts the stable-but-incomplete feature inventory into an ordered
production sequence. The goal is to finish one coherent vertical slice rather
than continue expanding the number of partially usable systems.

The current product target remains Forest Edge: Fern support, Hare player, and
Fox opposition. Work is complete only when the player can understand a choice,
observe its consequence, finish a run, receive a reward, and begin a meaningful
next run.

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

- Run the editor smart-tiling preview for all 16 cardinal masks.
- Confirm the mask convention (`N=1`, `E=2`, `S=4`, `W=8`) against the authored
  terrain sheet and correct only the resolver lookup when an edge is wrong.
- Verify grass and temporary desert/bare families, including isolated, straight,
  corner, T, and surrounded tiles.
- Verify atlas import settings, texture filtering, transparency, and pixel
  scale at the target board size.
- Keep neighbor masks and atlas indices presentation-only; do not move them into
  simulation state.

**Exit:** all 16 masks render correctly in the preview and in the live board,
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

**Status:** The previous licensing/startup blocker is resolved. The shared
Unity preflight now verifies the local entitlement, cleans only a stale lock,
and bounds the licensing probe. Current-head graphics PlayMode is 6/6,
EditMode is 139/139, and the Windows development build launched for a bounded
15-second smoke in `artifacts/audit-windows-build-current-20260820-101211/`.

## Phase 1 — Make the simulation truth trustworthy

### 1.1 Forest Edge balance and rules

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

- Keep Forest Edge as the production scenario.
- Treat OpenRange, Wetland, and BaselineParity as Dev Lab/research fixtures.
- Validate that scenario assets produce immutable run-start snapshots and stable
  fingerprints.
- Do not tune the full species library before the Hare slice is understandable.

**Exit:** one scenario is product-owned; the others are explicitly experimental
and cannot silently expand the slice.

## Phase 2 — Finish the actual run and upgrade loop

### 2.1 Upgrade grammar

Define a small explicit catalog, not a general modifier framework. It must cover:

- Trailblazer: movement/perception and mobility tradeoffs.
- Warren: protection, crowding, or controlled reproduction tradeoffs.
- Gardeners: feeding efficiency, food reserve, and seed-dispersal tradeoffs.

For every upgrade, record the affected rule, valid range, stacking/exclusion
rule, visible preview, expected consequence, counterplay, and telemetry.

### 2.2 Five-phase run contract

- Implement five 200-tick phases with automatic pause at reward breaks.
- Offer three meaningful choices after phases one through four.
- Record the ordered upgrade loadout in the effective ruleset and result.
- Implement victory, narrow survival, defeat, phase summaries, and immediate
  extinction according to `PRODUCT_BRIEF.md`.
- Make the results screen explain population changes, deaths by cause, feeding,
  movement, combat, and upgrade contributions.

**Exit:** three Hare builds produce visibly different, reproducible behavior in
the same Forest Edge scenario.

## Phase 3 — Build the player-facing shell

### 3.1 Main Menu and Lab foundation

- Formally close Sprint 0 readiness decisions and assign Sprint 1 owners.
- Implement Main Menu → Lab Overview → Herbivore Research preview in
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
- Add one permanent research node and one predetermined first-victory unlock.
- Add the smallest useful Hare mastery objective.
- Keep active-run save/resume, multiple profiles, cloud saves, and broad research
  trees out of the first slice.

**Exit:** a fresh profile can complete a run, receive one defined unlock, restart,
and see/use that unlock on the next run.

## Phase 5 — Research and developer tooling

### 5.1 CellSim and report pipeline

- Current-head focused tests and Windows smoke are complete; retain their
  preflight/test/build logs as evidence. Accept current schema-7 report output
  only after reproduction and food-action telemetry reconcile with the
  simulation. Historical schema-6 EX-002 reports remain valid evidence for
  their bounded experiment window.
- Keep JSON/CSV factual exports separate from presentation and dashboard ideas.
- Validate the committed JSON/CSV editor converter and assign its owner.

### 5.2 Predictive ecology research

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

Keep the scene and tests as a siloed reference. Do not extend it during vertical-
slice work. Reopen only through an explicit product decision.

### Cave generation

Keep the deterministic generator and tests. Decide later whether `CavePreview`
gets a dedicated experiment scene; otherwise remove only that orphaned
presentation path with a focused migration.

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
- Smart tiling has passed the 16-mask and live-board checks.
- Forest Edge has a trustworthy fixed-seed baseline and reconciled telemetry.
- Trailblazer, Warren, and Gardeners are understandable and measurably distinct.
- A player can complete the five-phase run without developer fields.
- Results explain the outcome and award one persistent unlock.
- Main Menu → Lab → Run → Results → next Lab is navigable and tested.
- One fresh profile can restart and use the unlock.
- Remaining prototypes and research tools have explicit retained, deferred, or
  archived status.

## Current blockers

1. The embedded Noesis editor analytics path requires a vendor/project privacy
   decision before release; it is editor-only and not compiled into the player.
2. A current schema-7 Forest Edge report and held-out baseline remain before
   balance changes are promoted.
