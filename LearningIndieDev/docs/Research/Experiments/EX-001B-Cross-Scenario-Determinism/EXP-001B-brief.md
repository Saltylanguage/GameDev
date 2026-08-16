# EXP-001B - Cross-scenario determinism extension

**Experiment ID:** `EXP-001B`  
**Question:** Does the shared simulation engine reproduce identical machine-readable outcomes across the currently authored cellular-automata scenarios when each scenario is repeated with identical inputs?  
**Stage:** Instrument trust extension  
**Decision owner:** Human design owner  
**Status:** Accepted bounded cross-scenario reproducibility result; all four scenario pairs match

## Hypothesis

If the shared simulation engine is deterministic, repeating each authored
scenario with the same scenario asset, ruleset fingerprint, seed range, player
species, duration, and step interval will produce identical outcomes for that
scenario. This tests engine-level reproducibility across the current scenario
library; it does not claim that ecological findings transfer between scenarios
or to arbitrary cellular automata.

## Scope

The extension covers the four authored scenario assets currently in the project:

- `ForestEdge` (`hare`, authored grid `32 x 32`)
- `OpenRange` (`deer`, authored grid `32 x 20`)
- `Wetland` (`snail`, authored grid `32 x 20`)
- `BaselineParity` (`herbivore`, authored grid `32 x 20`)

Each scenario is run twice over seeds `10100` through `10119` using the current
schema-4 `CellSim` runner. Grid dimensions remain scenario-authored; they are
not overridden for this extension.

## Run protocol

1. Confirm Unity is closed and the working tree is not being edited by another developer.
2. Run each scenario twice with the command shape below, changing only the scenario path and player species.
3. Record the raw JSON/CSV artifact paths, Unity logs, fingerprints, and raw hashes.
4. Compare controlled metadata, all run payloads, population histories, and final summaries after excluding generated timestamps and output paths.
5. Generate one factual report covering all scenario pairs.
6. Generate separate analysis and record a human decision.

```powershell
.\CellSim.cmd Run -ScenarioPath Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset -SeedStart 10100 -SeedCount 20 -PlayerSpeciesId hare
.\CellSim.cmd Run -ScenarioPath Assets/Data/CellularSimulation/Scenarios/OpenRange.asset -SeedStart 10100 -SeedCount 20 -PlayerSpeciesId deer
.\CellSim.cmd Run -ScenarioPath Assets/Data/CellularSimulation/Scenarios/Wetland.asset -SeedStart 10100 -SeedCount 20 -PlayerSpeciesId snail
.\CellSim.cmd Run -ScenarioPath Assets/Data/CellularSimulation/Scenarios/BaselineParity.asset -SeedStart 10100 -SeedCount 20 -PlayerSpeciesId herbivore
```

Each command is repeated once without changing its inputs. `CellSim Report` may
be used to generate readable per-artifact summaries; the package report records
the paired comparison and canonical normalized hashes.

## Success criteria

- Each scenario pair has matching scenario path, player species, seed range,
  grid, duration, step interval, and ruleset fingerprint.
- Every pair contains seeds `10100` through `10119` exactly once.
- Every machine-readable run payload and population history matches within each
  scenario pair after generated metadata is excluded.
- Each pair's final population summary matches exactly.
- No unexplained intentional nondeterminism appears.

## Failure or gap conditions

- A repeated pair differs in any controlled input or outcome payload.
- A ruleset fingerprint changes without an intentional source or asset change.
- A scenario cannot load, complete, or produce a schema-4 report.
- The result depends on manually editing generated artifacts.

## Scope boundaries

This extension does not claim that every cellular automaton is deterministic,
that ecological findings transfer across scenarios, or that the simulation is
correct or balanced. It only tests repeatability of the currently authored
scenario assets through the shared engine.
