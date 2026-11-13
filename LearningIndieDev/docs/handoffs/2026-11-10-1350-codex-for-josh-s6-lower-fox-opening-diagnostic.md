# S6 lower Fox opening diagnostic

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-10-1350-codex-for-josh-s6-lower-fox-opening-diagnostic
- Owner: Codex for Josh
- Branch: codex/s6-predator-first
- Baseline commit: 9c298699
- Date: 2026-11-10
- Supersedes: none

## Summary

The approved follow-up screened 8, 10, and 12 fixed Foxes per node after the corrected 16/18/20 sweep missed the first-node safety target. Each lower-count arm had 2/12 first-node Hare failures; cumulative node-two failures were 4/12, 2/12, and 3/12. The small panel did not trigger confirmation, so no rate or balance value is approved. The result suggests considering different authored Fox counts by node if the goal is a gentle opening followed by sharp pressure.

## Changes

- Added the experiment contract, factual report, AI analysis, and per-seed summary tool under `docs/Research/Experiments/S6-Fox-Soft-Opening-20261110/`.
- Raw diagnostic bundle is ignored under `artifacts/s6-fox-soft-opening-20261110/`; prior run evidence remains untouched.
- The 10-Fox arm reached zero Plants by node-two close in at least one seed, with 125 Hare starvation deaths across that node's 12 runs.
- No production code or authored balance asset changed.

## Decisions and assumptions

- Josh approved a lower fixed-Fox-count diagnostic. The arms used 8/10/12 Foxes at both nodes, 400 Plants, 55 initial Hares, no upgrades or player interventions, and seeds 10100–10111.
- Only the played Hare population count carries between nodes; Hares restart as fresh individuals and nonplayed Fox/Plant counts reset to each arm's fixed count.
- The preregistered trigger required at most 1/12 first-node failures and 4–10/12 cumulative node-two failures to confirm an arm over 100 disjoint seeds. No arm qualified.
- The observed count differences remain exploratory because the placement shuffle can also move Hare and Plant starting cells.

## Validation

- `CellSim.Batch` diagnostic completed 36/36 runs; batch identity `66820d9c1eca4d8afae3c5b0e28da889a23f102267ab3884b139837cfde94a57`.
- `summarize.py` verified exact seeds, 12 phases/1200 ticks, all node openings and reset deltas, then generated `artifacts/s6-fox-soft-opening-20261110/paired-summary.json`.
- The worker's embedded source hash matched the frozen Unity snapshot's source hash. The snapshot was Unity-exported and parity-checked in the preceding corrected sweep; no Unity Editor was open during this diagnostic.
- No Unity test suite, fresh Unity export, visual replay, or human playtest ran for this experiment.

## Risks and incomplete work

- Twelve seeds are insufficient to establish either target rate; no 100-seed confirmation ran.
- This screen used one fixed Fox count at both nodes. Testing gentler node one plus stronger node two requires per-node fixed starting-count support or an isolated fixture.
- The snapshot's `sourceCommit` says `59ad0f4`, its export-time base; the embedded source hash matches current simulation code at `9c298699`.
- A lower-count setup can shift placements and can bring Plant exhaustion back as a secondary pressure.

## Next useful step

Review the diagnostic and choose whether to pursue node-specific authored Fox counts, test a still-lower global range, or shift to the three Mutation responses. If choosing node-specific counts, define a small diagnostic matrix and decide how the starting layout will be held or recorded before expanding the runner inputs.
