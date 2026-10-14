# s4 wrapping grid experiment

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-04-0022-codex-s4-wrapping-grid-experiment
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 6a0ab08e
- Date: 2026-10-04
- Supersedes: none

## Summary

Optional four-edge wrapping is implemented locally with a `Wrap Edges` Inspector toggle. Bounded remains the default. The 80-run matched screen found no wrapped Hare survivors at tick 600 in either path, versus 7/20 control and 2/20 Movement-first Trailblazer bounded. This enables a visual experiment; it does not support changing the default now.

Read the [method](../Research/Experiments/S4-03-Wrapping-Grid-Screen/README.md), [factual report](../Research/Experiments/S4-03-Wrapping-Grid-Screen/REPORT.md), and [separate analysis](../Research/Experiments/S4-03-Wrapping-Grid-Screen/ANALYSIS.md). Earlier paired-screen changes remain in this same dirty checkout; this handoff adds wrapping rather than replacing those results.

## Changes

- `Grid<T>` resolves coordinates and shortest distances for opt-in wrapping. Pattern scans exclude self aliases and duplicate cells, including vision ranges larger than grid dimensions. Copies retain topology and reuse the per-grid pattern cache.
- Runtime data carries `WrapEdges` through all copies/upgrade snapshots; the initial grid, navigation, perception, flee choices, combat, food, reproduction, crowding and seed planting use the same topology. Occupied and impassable destinations retain existing movement guards.
- `SpeciesSimulationPreview` has a serialized default-false toggle. The Play-mode custom Inspector exposes it before a session starts and after Reset to Start, with runtime enforcement of that lock. The S4 fixture preserves the selected topology and still sets 400/20/10 without changing scenario assets.
- CLI `-wrapEdges true/false`; JSON/CSV schema 32 records the flag. Data fingerprint v11 includes it. Older bounded fingerprint strings change, but historical bounded behavior is verified unchanged.

## Decisions and assumptions

- Bevin explicitly authorized implementing/testing the proposed wrapping behavior with an Inspector control. No authorization to promote it to authored defaults or tune skills was inferred.
- Both Hares and Foxes wrap on both axes; their perception and interactions cross seams too. This changes predator and mating access alongside escape routes. No separate Hare-only escape mechanic.
- Same 20 development seeds, populations, skill values, phase schedule and no extra Genome/reinforcements. Movement-first Trailblazer gains L1 General Movement at 100 and L1 Threat Avoidance at 200 if still alive; later decisions skipped. Salty's journey/DNA/secondary ecology planning remains separate.

## Validation

- Unity 6000.4.6f1 live recompilation: completed, zero compilation errors. Six focused assertion methods executed directly via the existing installed Unity CLI, including shared-grid seam/copy tests, domain interaction/flee/navigation tests, Inspector-lock/fixture test, runtime-data/checkpoint test and prior avoidance regressions (384 fresh-grid comparisons). No standard NUnit/Clean-suite pass claimed.
- All 20 starting layouts match across topologies. 80 batch runs / 373 actual phase windows validate starting counts, ordered reachable acquisitions, accounting, within-topology first phases and source hashes. Five authored scenario/species assets unchanged.
- Both bounded arms match all historical process-independent run fields, excluding entityId and the versioned rulesetFingerprint. Wrapped Trailblazer seed 10100 replays all fields except entityId identically, including phase fingerprints.
- Raw ignored evidence: `artifacts/s4-wrap-screen-20261004-400-20-10/`, especially `experiment-contract.json`, `validation.json`, `focused-checks-all-layouts.json`, reports/requests, CSV slashlines and `source.diff`.
- `git diff --check` passed. Editor stopped; no scene save invoked. Final console status reported no errors and one Unity AI account-availability warning from the installed package; exact warning recorded in `console-native-warnings.json`.

## Risks and incomplete work

- User visual acceptance remains open. This is not independent fresh-seed confirmation, whole-ecosystem balance, performance benchmarking or build readiness.
- The border-trapping explanation is a hypothesis. Wrapping increases births and deaths in this panel, with lower mean eAVI and fewer final plants; separate causal attribution is unmeasured.
- Source and documents remain uncommitted/unpushed on BevBranch baseline `6a0ab08e155b1a8a2962d19a6c5eca1bc8233118`, together with the prior paired-screen fixes. No Trello write this turn. Inspect the combined diff before publication; raw artifacts are ignored/local.
- Adding topology to fingerprints invalidates old fingerprint equality even for unchanged bounded behavior. Use the verified semantic comparison and report schema rather than treating an old fingerprint as current evidence.

## Next useful step

In the CellularAutomataPrototype scene, select the object containing Species Simulation Preview. In Play mode use Reset to Start, Apply S4 Fixture, enable Wrap Edges under Manual Simulation Testing, then Start. Pause/single-step to inspect crossings. Repeat with wrapping off and the same seed. During Edit mode the checkbox is under the component's Grid section; save the scene yourself only if you want that scene's serialized default changed. Decide on the desired boundary experience after visual review before any further balance tuning.
