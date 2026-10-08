# S4-03 200-seed threshold confirmation

[Working state](../WORKING_STATE.md) | Status: complete locally; Bevin/Salty balance review pending

- Handoff schema: 1
- Handoff ID: 2026-10-06-1432-codex-s4-03-200-seed-threshold-confirmation
- Owner: Codex; decision owners: Bevin and Salty
- Branch: BevBranch
- Baseline commit: 6a0ab08e155b1a8a2962d19a6c5eca1bc8233118
- Date: 2026-10-06 local
- Supersedes: none; follows the [100-seed S4-03 screen](2026-10-05-2348-codex-s4-600-40-25-paired-seed-screen.md)
- Sharing: local documentation and ignored research artifacts only; no commit, push, or board update

## Summary

Bevin asked to continue balancing around a challenge where the player intervenes to keep an ecology above the all-species finish threshold. A fresh matched-seed confirmation completed four arms (200 shared seeds 14000-14199 each) on wrapped Forest Edge, 600 Plants / 40 Hares / 25 Foxes. Under the user-defined tick-600 rule (Plants > 0, Hares >= 5 and Foxes >= 5), the clear counts were 25/200 control, 58/200 Seed Dispersal, 50/200 Gardeners Digestion-first, and 57/200 Gardeners Dispersal-first. Each intervention cleared more matched seeds than control alone; the order comparison did not identify a clear winner.

## Changes

- Added a separate [experiment package](../Research/Experiments/S4-03-Hare-Paired-Seed-Confirmation-600-40-25/README.md), with a factual [report](../Research/Experiments/S4-03-Hare-Paired-Seed-Confirmation-600-40-25/REPORT.md) and distinct [AI analysis](../Research/Experiments/S4-03-Hare-Paired-Seed-Confirmation-600-40-25/ANALYSIS.md).
- Added ignored local artifacts at `artifacts/s4-paired-confirmation-20261006-600-40-25-wrapped-200/`, including the frozen contract, four full JSON/CSV reports, pilot data, logs, streaming validation/summary script and validated outputs.
- Updated `docs/Research/Experiments/README.md` and `docs/WORKING_STATE.md` to point to this result.
- No gameplay source, species rules, skill value, scenario asset, or population default was modified.

## Decisions and assumptions

- The exact-five Hares/Foxes cutoff and the requirement to finish at tick 600 were frozen from Bevin's stated success condition before the confirmation runs.
- The 100-seed screen's 13/100 control and 31-33/100 Seed Dispersal/Gardeners counts motivated this independent confirmation. It is a fresh evidence check, not balance approval.
- The main result is limited to 600/40/25, 36x20, wrapping enabled, six 100-tick phases, one Hare player, two offered-skill schedules and one node horizon. It does not establish journey-level success, adaptive human strategy, or a stable secondary ecology.
- No skill-order winner or clear-rate target has been approved.

## Validation

- Unity command-line runner completed 200 seeds in each arm with successful exit status; four separate one-seed pilots are not included in the 800 main runs.
- Independent analyzer checked all assigned seeds, input/topology, tick-zero population, every tick in each history, phase-window coverage, reached unique-skill acquisition schedule, Hare slashline formulas/status/denominators, final FPO and Seed Dispersal reconciliation. It generated `validated-summary.json` and one `*-validated.csv` per arm.
- All 800 final run and observed phase slashlines passed; source-file and Forest Edge scenario hashes remained equal to the frozen contract. The branch's pre-existing dirty source state was preserved.
- `git diff --check` passed. No NUnit/PlayMode suite, build or visual test ran because this was a research-only batch with no gameplay/editor-source changes.

## Risks and incomplete work

- The branch remains locally dirty with pre-existing code and documentation changes plus these new research docs; nothing was committed or pushed.
- Unity emitted licensing-client access-token/entitlement messages while batch runs still completed and exited successfully.
- The strategy arms clear only 25-29% under this rule. For the non-clearing intervention runs that reached tick 600, the Fox cutoff was the most common missing condition. That observation does not by itself identify a causal fix.
- The exact paired tests were not adjusted for the three planned intervention-vs-control comparisons. The main interpretation is the matched effect size and fresh replication direction.

## Next useful step

Bevin and Salty should decide what first-node clear rate and ecological population margins would feel fair. If the current rate is too punishing, test the real player offer/choice flow and likely Fox-persistence counterplay before changing Hare skill values. Keep the 400/20/10 authored/reference setting and production skills untouched until that review.
