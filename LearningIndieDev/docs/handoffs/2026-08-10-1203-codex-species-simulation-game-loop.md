# species simulation game loop

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: codex
- Branch: GridDesignWork
- Baseline commit: 0cbd5396
- Date: 2026-08-10

## Summary

Added the first playable species-simulation slice for the cellular-automata roguelike direction. The existing generic grid remains the data container while species rules, deterministic ticks, run state, rewards, and progression live in separate plain-C# types.

## Changes

- Added `SpeciesArchetype`, `SpeciesCell`, `SpeciesRules`, `SpeciesDefinition`, and `SpeciesProgression`.
- Added deterministic `SpeciesSimulation` attack, block, diet movement, and reproduction behavior using `GridPattern` offsets.
- Added `SpeciesSimulationRunner`, timed `SimulationRunState`, and `SimulationRunResult`.
- Added a first numeric `SpeciesUpgrade` and a centered full-screen `SpeciesSimulationPreview` with Start Simulation, results currency, and a movement-speed purchase.
- Updated `CellularAutomataPrototypeRuntime` and its Play Mode coverage to use the species preview; cave runtime creation remains disabled.

## Decisions and assumptions

- One occupant per grid cell for the first slice.
- The player controls a species population; the prototype defaults to herbivore.
- Runs are fixed-step and deterministic from the run seed. The preview defaults to a 20-second run at 0.1-second steps.
- Upgrades apply between runs. The first upgrade changes movement speed; rule-pattern upgrades remain future work.
- Resolution is deterministic but currently uses a straightforward grid scan order rather than a separate intent buffer.

## Validation

- `dotnet restore LearningIndieDev.slnx`: succeeded.
- `dotnet build LearningIndieDev.slnx --no-restore`: succeeded with 0 errors and 6 existing warnings from Noesis and `DelegateCommand`.
- `git diff --check`: passed.
- Unity Play Mode was not run because the project was already open in the editor.

## Risks and incomplete work

- Currency currently equals surviving player population; reward balancing is not final.
- The UI is an exploratory `OnGUI` preview, not the production results/upgrade interface.
- Species definitions and upgrades are runtime objects rather than authored assets.
- Unity may need a domain reload after the script import correction before the scene can be played.

## Next useful step

Open `CellularAutomataPrototype`, click Start Simulation, verify the three-species interactions visually, and run the focused Edit/Play Mode tests before expanding the upgrade catalog or replacing the prototype UI.
