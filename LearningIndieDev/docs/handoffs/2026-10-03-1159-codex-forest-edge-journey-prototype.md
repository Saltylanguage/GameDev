# Forest Edge journey prototype

[Working state](../WORKING_STATE.md) | Status: historical prototype snapshot

> This handoff records the first local journey prototype and its original
> inheritance rule. The integrated Desktop flow now uses an opening reward
> node, report-to-map transitions, and Mutations that persist until the run
> ends. See [the current journey contract](../JOURNEY_PROTOTYPE.md) for the
> playable behavior and verification.

- Handoff schema: 1
- Handoff ID: 2026-10-03-1159-codex-forest-edge-journey-prototype
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: b97b9adf
- Date: 2026-10-03
- Supersedes: none

## Summary

Implemented the first Forest Edge/Hare journey in the canonical Desktop flow.
Two six-phase cycles share one ecosystem. At the cycle boundary, one selected
Mutation is inherited and the player chooses an authored condition node. The
map shows the complete intended shape with later nodes disabled as placeholders.
This is local, uncommitted work on `ProjectMain`; nothing was pushed or updated
in Trello.

## Changes

- Added `JourneyMapAsset` and connected `JourneyNodeAsset` ScriptableObjects,
  plus two `SpeciesUpgradeAsset` condition effects. The first condition spreads
  Plant seeds more often; the second speeds Foxes and grants field data.
- The preview runs twelve phase windows in one `SimulationRunState`, retaining
  cells, time, metrics, population history, and seed. It rebuilds Hare rules
  from the frozen starting rules and ordered launch upgrades plus the single
  inherited Mutation. It removes other cycle-1 Mutations from the active
  loadout. Condition values and rewards are snapshotted before the run.
- The Desktop icon and Lab launch show the map before play. Results lead back
  to the map for a new journey. A MAP button opens it during play and pauses
  the simulation until closed if play was running. The results card compares Plant, Hare, and Fox
  populations at the end of cycle 1 and the final observation. Forest Edge/Hare
  restores continuous cadence on selection even if an older saved developer
  setting had disabled it.
- Added focused Editor and Play Mode tests and documented the prototype in
  [`JOURNEY_PROTOTYPE.md`](../JOURNEY_PROTOTYPE.md). Updated the Working State
  index and the continuous-flow plan's prototype note.

## Decisions and assumptions

- The inherited Mutation affects every surviving Hare from the next tick.
  Its repeat picks retain their levels. If all Mutation choices were skipped,
  the player can choose a condition without inheritance.
- The authored conditions are prototype values, not accepted balance. Future
  Upgrade, Event, and Finale rows are visible ScriptableObject nodes with
  authored graph links; they are intentionally unreachable in this two-cycle
  slice.
- Forest Edge/Hare uses the journey by default; other scenarios and the
  explicit test/developer fallback retain the six-phase path. The active
  Genome snapshot remains a launch input separate from Mutations.

## Validation

- Clean Unity EditMode map asset test: 1 passed, 0 failed
  (`artifacts/unity-tests-20261003-113451/`).
- Final clean Unity PlayMode `SpeciesPresentationPlayModeTests` fixture:
  20 passed, 0 failed, 2 graphics tests skipped
  (`artifacts/unity-tests-20261003-120844/`). This includes the Desktop icon,
  Lab launch map, MAP pause/resume, one-Mutation same-world transition, and
  return to map from Results.
- Clean Unity PlayMode Lab close-policy and immutable launch tests: 1 passed
  each (`artifacts/unity-tests-20261003-115059/` and
  `artifacts/unity-tests-20261003-115146/`).
- Graphics-capable Desktop visual fixture: 4 passed, 0 failed
  (`artifacts/visual-evidence-20261003-121456/`). The final focused map
  capture passed after the 1280x720 layout adjustment
  (`artifacts/visual-evidence-20261003-121756/`); screenshots, including the
  complete five-row map, are in `artifacts/journey-map-visual-fit-20261003/`.
- XAML parsed as XML and `git diff --check` passed. The first Editor test had
  an NUnit array `Has.Count` assertion error, which was corrected; a later
  PlayMode attempt had a test-assembly reference error, which was corrected.
  The final runs compiled and passed.

## Risks and incomplete work

- No matched multi-seed balance study validates the 1,200-tick default journey,
  the two condition values, or the 20-data route reward. Extinction can end
  the run before the second cycle; no outcome rate is claimed.
- Phase results retain each window's effective loadout, ruleset fingerprint,
  metrics, and populations. The acquisition timeline does not yet have an
  explicit deactivation record for discarded Mutations. Add that before
  interpreting exported journey evidence as a full causal event timeline.
- The map rows beyond cycle 2 are visual and authored placeholders. There is
  no player save/resume for a partly completed journey.
- The existing `VM_SimulationShell` directly calls the preview for journey
  commands, matching its current phase-decision path. This extends the
  compatibility exception noted in proposed SG-003; review moving both paths
  behind a dedicated helper when the simulation shell is next refactored.
- Existing uncommitted edits in `SCIENTIFIC_DATA_ECONOMY.md`, the S4 control
  record, and the P1-029 handoff were present before this work and were left
  untouched.

## Next useful step

Review the two-cycle flow in the Desktop, then set a target run duration and
run a matched-seed comparison for Seedfall, Fox Tracks, and inheritance. Use
that evidence to decide which future map nodes to activate and what telemetry
the route choices need.

## 2026-10-05 visual polish addendum

The journey map now follows the simulation's field journal presentation: a
green expedition header, numbered route rail, cards with clear current,
available, chosen, and uncharted states, and a field dossier for continuity
and inheritance. The visible future nodes have in-world teaser copy instead
of implementation placeholder text. Mutation and Skip commands refresh the
shell immediately so the boundary UI is responsive. Existing shared image
resources were reused; no new art assets or image paths were introduced.

Clean validation after the pass: Desktop visual fixture 5 passed, 0 failed
(`artifacts/visual-evidence-20261005-114406/`); presentation PlayMode fixture
20 passed, 0 failed, 2 graphics skips
(`artifacts/unity-tests-20261005-114503/`); authored map Editor test 1 passed
(`artifacts/unity-tests-20261005-114600/`). The 1280x720 Desktop launch capture
is at `artifacts/journey-map-polish-20261005-verified/03a-galapagos-journey-map.png`.
A true 1920x1080 launch capture was also inspected at
`artifacts/journey-map-polish-20261005-1920-corrected/03a-galapagos-journey-map.png`.
The decision test verifies Noesis panel visibility and that both authored
routes become selectable after inheritance. Manual `Camera.Render()` capture
in the test runner sometimes lags the Noesis decision overlay by a frame, so
the retained screenshot evidence is the stable Desktop launch view.
