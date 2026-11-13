# S6 Fox start sweep

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-10-0939-codex-s6-fox-start-sweep
- Owner: codex
- Branch: codex/s6-predator-first
- Baseline commit: b94929aa
- Date: 2026-11-10
- Supersedes: none

## Summary

The [approved Fox-count sweep](../Research/Experiments/S6-Fox-Start-Sweep-20261110/REPORT.md) completed 336/336 no-action simulations of the first two Forest Edge nodes. Starting with 16, 18, or 20 Foxes is technically valid, but none meets the provisional safe-opening or roughly 60% cumulative failure screens on 100 disjoint matched seeds per count. The second node is dominated by Hare starvation and Plant depletion rather than sustained Fox pressure. The [AI analysis](../Research/Experiments/S6-Fox-Start-Sweep-20261110/AI_ANALYSIS.md) proposes the next controlled design question; Josh's balance decision is pending.

## Changes

- Added the experiment contract, factual report, separate analysis, and compact [per-seed summary](../Research/Experiments/S6-Fox-Start-Sweep-20261110/paired-summary.json).
- Preserved raw plans, manifests, summaries, and 336 run records under ignored `LearningIndieDev/artifacts/s6-fox-start-sweep-20261110/` with hashes in the factual report.
- Updated `WORKING_STATE.md` to point to the new evidence. No simulation rule, Mutation, authored scenario, or player flow changed.

## Decisions and assumptions

- Josh approved the 16/18/20 Fox sweep and node-two check in the November 10 design chat. The first-node at-most-5% and cumulative 50–70% bands are experiment screens, not accepted game balance values.
- On the confirmation seeds, Hare extinction by tick 600 was 19/100, 15/100, and 20/100 for 16, 18, and 20 Foxes; by tick 1200 it was 25/100, 26/100, and 31/100. The 12-seed diagnostic had a much higher 20-Fox loss count (6/12 and 7/12), so do not choose a setting from that panel.
- Josh has not approved a new starting population, predator escalation rule, or Plant tuning. Keep analysis suggestions separate from the locked predator-first objective.

## Validation

- `CellSim.Batch batch <plan> --dry-run` passed for diagnostic and confirmation plans; actual batch executions completed 36/36 and 300/300 validated runs with project-required elevated permissions.
- Verified exact starting populations, 12 complete 100-tick phases, and no Mutation loadout for all 336 runs. Fox kills equaled Hare combat deaths in all 4,032 phase windows.
- Verified the 20-Fox diagnostic arm against the prior route proxy through tick 1200 on all 12 matched seeds: Hare extinction tick, every boundary population, phase activity, and species stat lines. Prior proxy had matched the authored Unity route at shared boundaries.
- Raw SHA-256 values, worker hash, snapshot identity, plan hashes, and seed ranges are recorded in the [factual report](../Research/Experiments/S6-Fox-Start-Sweep-20261110/REPORT.md). No Unity suite was rerun because this task changed only experiment inputs and documents.

## Risks and incomplete work

- The 100 confirmation seeds are consecutive rather than randomly sampled, and this is a portable no-action ecology proxy, not a new-player playtest or an authored 16/18-Fox Unity route.
- Plants first reached zero before Hare extinction in 6/6, 10/11, and 11/11 node-two losses, but the final death cause was not directly traced for every loss.
- Raw artifacts are local and ignored by Git; the versioned compact summary carries per-seed outcomes and checkpoints.

## Next useful step

Josh should review the [analysis](../Research/Experiments/S6-Fox-Start-Sweep-20261110/AI_ANALYSIS.md) and choose the next bounded balance mechanism. The suggested experiment separates first-node safety, a legible second-node Fox escalation, and Plant sustainability so the cause of any improvement stays clear. Mutation availability and efficacy need later fresh-profile testing.
