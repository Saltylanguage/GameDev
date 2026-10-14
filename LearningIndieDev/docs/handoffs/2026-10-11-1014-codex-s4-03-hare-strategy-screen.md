# S4-03 Hare strategy screen

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-11-1014-codex-s4-03-hare-strategy-screen
- Owner: Codex
- Branch: codex/s4-gestation-checkpoint-20261010
- Baseline commit: a7667e77c68607fc832bc6412ba03c36eccc887c
- Date: 2026-10-11
- Supersedes: none

## Summary

Completed the predeclared S4-03 13-arm × 20-seed exploratory screen at 400 Plants / 20 Hares / 10 Foxes. Found and fixed two defects that made current Threat Avoidance inactive: scheduled phases kept the launch avoidance chance, and combat required a flee-speed bonus the upgrade no longer supplies. The corrected control and three affected arms were replayed with identical seeds and settings. The compact [factual report](../Sprints/S4-03-screen-20261011/report.md), [metadata](../Sprints/S4-03-screen-20261011/metadata.json), [per-seed data](../Sprints/S4-03-screen-20261011/per-seed.csv), and separate [AI analysis](../Sprints/S4-03-screen-20261011/ai-analysis-v1.md) are tracked.

## Changes

- Commit `a7667e77` updates scheduled experimental options at every phase boundary and removes the obsolete flee-speed condition from pre-contact avoidance. Focused Editor and runtime regression tests cover both faults.
- The original 13 arms ran from frozen source `83e1fbea` and the four replay arms from `a7667e77`. Original control matched corrected control across all 20 archived run summaries. Original control/Avoidance raw files were accidentally overwritten during replay, then regenerated from the original commit; their archived run summaries matched, and original and corrected manifests validate all 17 report hashes. The report metadata records each source commit, report hash, and the regeneration.
- Direct Unity CLI was used because `CellSim.ps1` still references missing `Resolve-UnityExecutionLane`. Raw JSON/CSV/logs remain ignored and local under `LearningIndieDev/artifacts/s4-screen-20261011/` in this managed worktree. The tracked bundle is compact and reviewable without the raw files.
- Updated the S4 control record and working-state index; the production Forest Edge asset and skill values were not changed.

## Decisions and assumptions

- Josh chose 400/20/10 as the main screening fixture and retained 400/25/15 for a separate stress comparison. This screen follows the predeclared six-phase schedule and seeds 11100–11119.
- This is mechanics and choice-reach evidence, not acceptance of the three build identities or skill balance. Warren Burrows remains a 1.0 design target outside S4. No confirmation thresholds were selected after seeing this panel.

## Validation

- Unity 6000.4.6f1: 319/319 EditMode and 42/42 PlayMode tests passed after the fix.
- Each of the 13 original and four corrected reports passed seed, schedule, starting population, baseline ruleset fingerprint, and SHA-256 checks. The corrected control's archived run summary equaled the original control's for all 20 seeds.
- Hare survival at tick 600: control 6/20; corrected Avoidance 4/20; Movement 7/20; Trailblazer pair orders 6/20 and 7/20; Warren orders 5/20 and 6/20; Gardeners orders 5/20 each. Movement activity rose, and successful Gardeners seed drops exactly matched reserve units spent. These are observations, not causal survival claims.

## Risks and incomplete work

- No direct map coverage, local occupancy, or seed-patch revisit metric is in these reports. Aggregate movement and seed drops cannot establish the requested player-facing identities.
- Control accrued only 13 Hare crowding-energy units across 20 seeds, making Warren's crowding comparison weak. Burrows is absent by S4 scope.
- The 20-seed panel has frequent extinction and mixed paired survival results. It is too small and too early for balance acceptance. The scheduled batch path bypasses the player offer UI.
- The 400/25/15 stress comparison, in-game review, Josh/Sim accept-or-revise decision, and frozen 200-seed confirmation remain open. The raw reports are local, not in Git.

## Next useful step

Review the screen's concrete behavior and limits with Josh and Sim. Add or extract spatial coverage, crowding exposure, and patch-revisit evidence for selected seeds, then run the player offer flow in-game. Record what each build feels like and an accept/revise decision before changing values or freezing the independent 200-seed confirmation panel. Review S4-03 capacity if the added diagnostic/review work exceeds its original estimate.
