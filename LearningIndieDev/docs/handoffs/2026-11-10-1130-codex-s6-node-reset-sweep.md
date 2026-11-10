# S6 fresh-node population carryover and corrected Fox sweep

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-10-1130-codex-s6-node-reset-sweep
- Owner: Codex for Josh
- Branch: codex/s6-predator-first
- Baseline commit: 59ad0f44498772148f992456e0db83e9dc3be09a
- Date: 2026-11-10
- Supersedes: 2026-11-10-0955-codex-s6-node-boundary-correction.md

**Decision authority:** Josh chose population-count-only carryover for the played species; all other populations reset to authored counts at every simulation node.

## What changed

- `SpeciesSimulationRunner.TryStartNextSimulationNode` builds a fresh grid from fixed starting counts, substituting the played species' closing count. All species need explicit counts, including zero; the Preview settings path now retains zero values.
- `SimulationRunState.InstallNextSimulationGrid` preserves the closed phase snapshot, sets the fresh opening snapshot, clears Hare individual lineage and Burrow state, and records sorted node-reset population deltas with the derived node seed. Tick, progression, wallet, Mutations, and Genome state continue.
- Field Data and legacy journey transitions call the shared reset at simulation node boundaries. The portable scientific runner accepts `-phasesPerNode 6` for an explicit 12-phase schedule, using the same production reset code. Its self-test fixture now declares the default herbivore's zero count; journey replay errors expose settlement details.
- The focused tests cover Hare and Fox players, a fourth nonplayed Deer species, default age and energy, route UI transitions, and stable full-route completion.

## Verification and evidence

- Unity EditMode `29/29`: `artifacts/unity-tests-20261110-112023/EditMode-results.xml`.
- Unity PlayMode `1/1` each for legacy journey fresh-grid transition (`artifacts/unity-tests-20261110-104850`), authored Field Data first shop (`artifacts/unity-tests-20261110-105215`), and stable four-node route (`artifacts/unity-tests-20261110-105259`). The authored fixture now checks the first shop and node opening because its former seed 10100 naturally dies at tick 236; the stable fixture checks successful completion.
- Portable Release build and self-test passed. Unity v3 export and portable worker passed exact two-seed parity. The production Field Data replay on seed 10101 crossed two shops under the corrected rule.
- The [new experiment contract](../Research/Experiments/S6-Fox-Node-Reset-20261110/CONTRACT.md), [factual report](../Research/Experiments/S6-Fox-Node-Reset-20261110/REPORT.md), [per-seed summary](../Research/Experiments/S6-Fox-Node-Reset-20261110/paired-summary.json), and [AI analysis](../Research/Experiments/S6-Fox-Node-Reset-20261110/AI_ANALYSIS.md) supersede the earlier retained-world node-two interpretation. The original artifacts remain immutable.

On the disjoint 100-seed panel, 16/18/20 Foxes produced **19/15/20 first-node failures** and **55/51/61 cumulative node-two failures**. All 300 node-two openings were verified at fixed Fox and Plant counts and carried Hare count. Node-two pressure is present; none of these counts meets the opening safety screen. Predation is the leading measured cause, and Plants remained present at both node closes. No authored balance value changed.

## Next decision and limits

Josh and Sim should choose the next bounded balance experiment and player playtest. The analysis recommends a safer first-node pressure screen, then fixed per-node counts if one shared Fox count cannot meet both node targets. Avoidance, survival, and faster reproduction still need legitimate fresh-profile choice and route evidence. The scientific count arms also shift initial Hare/Plant placement under the current grid generator, so paired differences are not a pure Fox-count effect.

The reset refuses a node when carried played population plus fixed others cannot fit the authored grid or population cap; it does not silently drop creatures. The current 100-seed panel fit (maximum first-node closing Hare count 154), but high-reproduction runs may need an authored capacity decision. Full Unity suites and a player playtest are outside this focused verification.
