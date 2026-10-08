# S4-03 Trailblazer early-choice timing diagnostic

[Working state](../WORKING_STATE.md) | Status: complete locally; Bevin/Salty balance review pending

- Handoff schema: 1
- Handoff ID: 2026-10-07-0024-codex-s4-03-trailblazer-early-choice-timing
- Owner: Codex; decision owners: Bevin and Salty
- Branch: BevBranch
- Baseline commit: 6a0ab08e155b1a8a2962d19a6c5eca1bc8233118
- Date: 2026-10-07 local
- Sharing: local documentation and ignored research artifacts only; no commit, push, Trello update, or message sent

## Summary

Bevin approved a timing-only comparison on 200 common seeds at wrapped Forest Edge 600/40/25. Moving Trailblazer's first choice from tick 100 to tick 0 yielded 28/200 strict all-species clears (14%), versus 30/200 (15%) on the current schedule. The paired difference was -1 percentage point (95% CI -7.35 to +5.35; exact McNemar p=.878). This does not support adopting the timing shift as a balance fix; the rough 40–50% goal remains unmet.

## Evidence

- [Protocol](../Research/Experiments/S4-03-Trailblazer-Early-Choice-Timing-600-40-25/PROTOCOL.md), [factual report](../Research/Experiments/S4-03-Trailblazer-Early-Choice-Timing-600-40-25/REPORT.md), and [separate interpretation](../Research/Experiments/S4-03-Trailblazer-Early-Choice-Timing-600-40-25/ANALYSIS.md) contain setup, outcomes, and limits.
- The early arm used choices at ticks 0/100/200/300/400 and automatic continuation at tick 500; the reference used choices at 100/200/300/400/500. The pilot passed, and all common corresponding offer/selection records matched.
- The strict clear outcome is tick 600 with Plants > 0 and both Hares and Foxes >= 5. The early arm had a median first-phase Hare count of 26 versus 21, but total Hare deaths were both 45 median; Fox first-phase end population was 29 versus 31.
- The independent analyzer verified all 200 candidate and baseline runs, matched seeds, source/scenario hashes, histories, phase boundaries, valid slashlines, offer choice legality, FPO, seed-drop accounting, and acquisition ticks. Summary: `artifacts/s4-03-trailblazer-early-choice-timing-20261006-600-40-25-wrapped-200/validated-summary.json`.
- Raw JSON, CSV, logs, the frozen contract, pilot, and analyzer remain under that ignored artifact folder. The baseline arm remains under `artifacts/s4-03-strategy-choice-policy-screen-20261006-600-40-25-wrapped-200/`.

## Interpretation and next step

Do not change player-facing choice timing based on this panel. The point estimate is flat, and uncertainty includes modest improvement or worsening. The phase-one shift is mixed and small; among completed-to-horizon runs, the early arm's higher conditional population medians should not be mistaken for an overall success improvement because fewer runs reached tick 600.

The next balance investigation should examine phase-specific Fox food/energy access, starvation, and reproduction across all runs, including early-ending runs. Use that diagnosis to define one Fox-side counterfactual and compare it on common seeds with unchanged Trailblazer and no-choice controls. No gameplay values or scenario defaults changed here.

The worktree had unrelated/pre-existing dirty changes. Nothing was staged, committed, pushed, posted to Trello, or sent to Salty.
