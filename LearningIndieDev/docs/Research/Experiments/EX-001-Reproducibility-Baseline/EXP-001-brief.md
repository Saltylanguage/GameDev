# EXP-001 - Experiment brief

**Experiment ID:** `EXP-001`  
**Question:** Does the ForestEdge scenario reproduce exactly from the same seed,
scenario asset, ruleset fingerprint, and run configuration?  
**Stage:** Instrument trust  
**Decision owner:** Human design owner  
**Status:** Provisional evidence captured; fresh current-code execution pending

## Hypothesis

If the simulation is deterministic, repeating the same run matrix with the same
scenario asset, ruleset fingerprint, seed range, grid, player species, duration,
and step interval will produce identical machine-readable run outcomes,
including population histories and final summaries.

## Controlled configuration

| Input | Value |
|---|---|
| Scenario asset | `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset` |
| Player species | `hare` |
| Seed start | `10100` |
| Seed count | `20` (`10100` through `10119`) |
| Grid | `32 x 20` |
| Duration | `20.0` seconds |
| Step interval | `0.10000000149011612` seconds |
| Expected ruleset fingerprint for captured pair | `90efffe28fbfb7d65573e03e8f206c7d34c32c41422995970174d884a078f93f` |

The captured pair predates the current runner schema. A fresh execution must
record the current schema and fingerprint rather than assuming the values above
remain current.

## Run protocol

1. Confirm the Unity project is closed and the working tree is not being edited
   by another developer.
2. Run the ForestEdge matrix once with `CellSim Run`.
3. Convert the JSON to the standard factual Markdown report with `CellSim Report`.
4. Repeat the exact command without changing the scenario, code, seed range, or
   configuration.
5. Compare metadata, ruleset fingerprints, every run payload, population
   history, and final summaries. Do not compare only the raw file hash because
   timestamps and output paths are expected to differ.
6. Select one representative seed near the median player outcome and one
   boundary seed with the strongest pressure signal.
7. Replay both seeds with `CellSim Visuals` and record whether the replay
   configuration and observed result match the source report.
8. Complete the factual report and separate AI analysis.
9. Record the human decision before opening EX-002.

Suggested command shape for the fresh rerun:

```powershell
.\CellSim.cmd Run `
  -ScenarioPath Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset `
  -SeedStart 10100 `
  -SeedCount 20 `
  -PlayerSpeciesId hare
```

Repeat the command, then use the resulting report paths with `CellSim Report`
and `CellSim Compare`. Replay commands must use the selected report and seed;
do not substitute a new seed during the replay check.

## Success criteria

### Required for a current-code pass

- Both runs contain the same scenario path, run configuration, seed range, and
  current ruleset fingerprint.
- All seeds are present exactly once in each report.
- Every machine-readable run payload matches after excluding generated
  metadata such as timestamps and output paths.
- Final population summaries match exactly.
- A representative and a boundary seed can be replayed from recorded metadata.
- Any intentional nondeterminism is named and bounded.

### Failure or gap conditions

- A ruleset fingerprint changes without an intentional code/data change.
- Any run payload or population-history value differs for identical inputs.
- A report cannot reconstruct the scenario, seed, grid, player species, or ruleset
  needed for replay.
- Replay configuration cannot be loaded or does not correspond to the source
  report.
- The experiment passes only because raw output files were manually edited or
  compared after removing unexplained differences.

## Scope boundaries

This experiment does not tune balance, add mechanics, introduce an AI runtime,
or decide whether the player-facing game should expose diagnostics. It tests the
simulation as an instrument. Any balance observation belongs in EX-002 or a
separate design experiment.
