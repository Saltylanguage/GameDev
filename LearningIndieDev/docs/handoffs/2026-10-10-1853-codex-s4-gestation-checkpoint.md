# S4 gestation checkpoint

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-10-1853-codex-s4-gestation-checkpoint
- Owner: Codex
- Branch: codex/s4-gestation-checkpoint-20261010
- Baseline commit: fea5592674b70587c05f9534880697fc086476c1
- Date: 2026-10-10
- Supersedes: none

## Summary

Animal gestation now delays birth by a provisional 48 ticks. The same checkpoint records Josh's S4 Hare build identities and tradeoffs so the next comparison has a clear design target. This branch isolates those changes from other local Journey, art, and UI work.

## Changes

- Added configurable gestation to Hare and Fox production species definitions, simulation rules, state, data fingerprinting, and preview display.
- Conceived litters remain pending until the gestation interval finishes. The whole litter waits for enough birth cells and retries while the parent remains alive.
- Preserved a pregnant animal's pending litter through normal and mate-seeking movement. Added a regression for a parent moving before birth, alongside conception, timing, blocked-birth, and state tests.
- Updated project context, the Hare/Fox treatment, and the S4 control record with Trailblazer, Warren, Gardeners, crowding, energy, and burrow decisions.

## Decisions and assumptions

- Hare and Fox gestation use 48 ticks as a tunable starting value, not a balance-accepted final rate.
- Trailblazer aims for fast broad map coverage with lower crowding. Warren aims for a settled local territory. Gardeners aims for several nearby renewing food patches and local travel among them.
- A successful Gardeners seed drop keeps its current one-unit stored food reserve cost for the first comparison.
- Burrows belong to the later Rabbit Genome tree and 1.0 scope, outside S4 implementation. The proposed capacity is litter size plus two; access belongs to the founding pair and offspring.
- Stronger density-based Rabbit crowding and per-move species energy charges are recorded as future experiments. This checkpoint implements neither rule.

## Validation

- Full isolated-worktree Unity EditMode suite: 317 passed, 0 failed, 0 skipped. Report: D:/GameDev/GameDev/artifacts/s4-checkpoint-editmode-20261010.xml.
- Full isolated-worktree Unity PlayMode suite: 42 passed, 0 failed, 0 skipped. Report: D:/GameDev/GameDev/artifacts/s4-checkpoint-playmode-20261010.xml.
- Focused moving-parent EditMode regression passed in the original checkout before transfer. The full isolated suites above include that regression and are the checkpoint verification.
- Git diff check passed before handoff. Unity-generated sprite metadata rewrites were excluded from this branch.

## Risks and incomplete work

- Gestation changes population timing. Earlier S4 runs cannot serve as direct balance controls; repeat matched-seed comparisons.
- S4 build outcomes, Gardeners reserve pressure, and strategy readability have no matched-seed acceptance evidence yet.
- The S4 code seam is shared with ongoing species simulation and presentation work; review against newer changes before merging.
- Burrow shelter, density-based crowding changes, and movement energy charges remain unimplemented.

## Next useful step

Review the branch with the S4-01 and S4-02 owners, then run matched-seed control and candidate simulations against the same gestation baseline. Inspect whether Trailblazer travels and disperses, Gardeners renews and revisits local patches, and Warren remains local with its current provisional skill set.
