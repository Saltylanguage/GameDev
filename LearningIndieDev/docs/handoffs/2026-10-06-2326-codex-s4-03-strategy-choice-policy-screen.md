# S4-03 strategy-choice policy screen

[Working state](../WORKING_STATE.md) | Status: complete locally; Bevin/Salty balance review pending

- Handoff schema: 1
- Handoff ID: 2026-10-06-2326-codex-s4-03-strategy-choice-policy-screen
- Owner: Codex; decision owners: Bevin and Salty
- Branch: BevBranch
- Baseline commit: 6a0ab08e155b1a8a2962d19a6c5eca1bc8233118
- Date: 2026-10-06 local
- Sharing: local documentation and ignored research artifacts only; no commit, push, Trello update, or message sent

## Summary

Bevin approved an offer-aware four-arm comparison at wrapped Forest Edge 600/40/25, with a rough 40–50% early-node clear target. The 800-run panel used 200 common seeds (14200–14399) for skip-all, Trailblazer, Warren, and Gardeners. Gardeners cleared 51/200 (25.5%), significantly more often than skip-all on matched seeds, but remained below target. Trailblazer (30/200) and Warren (34/200) were near skip-all (32/200). No gameplay values or authored defaults changed.

## Evidence and changes

- [Protocol](../Research/Experiments/S4-03-Strategy-Choice-Policy-Screen-600-40-25/PROTOCOL.md), [factual report](../Research/Experiments/S4-03-Strategy-Choice-Policy-Screen-600-40-25/REPORT.md), and [separate interpretation](../Research/Experiments/S4-03-Strategy-Choice-Policy-Screen-600-40-25/ANALYSIS.md) document the question, exact setup, results, and limits.
- Ignored local raw artifacts, frozen contract, Unity logs, validation analyzer, per-run CSVs, and summary are under `artifacts/s4-03-strategy-choice-policy-screen-20261006-600-40-25-wrapped-200/`.
- The independent analyzer verified the 800 histories, actual offer and choice behavior, acquisition levels, phase populations, slashline validity and formulas, FPO, seed-drop accounting, and frozen source/scenario hashes.
- All Unity CLI arms exited successfully. Startup licensing notices appeared, without compiler errors or simulation exceptions. No Unity test suite, build, or visual review was run.
- The wrapper helper `Resolve-UnityExecutionLane` is absent in this checkout, so the same editor runner was invoked directly through the Unity CLI.

## Interpretation and next step

The clearest next diagnostic is Fox persistence: median Fox starvation deaths were 47–48 per run and median Fox reproduction candidates blocked by energy were about 6,700–7,700, while median births were 40–45. Gardeners' median Fox population was still below the five-Fox finish cutoff at tick 600. These are descriptive signals, not a causal diagnosis. Inspect phase-specific food access, starvation, and reproduction limits before proposing any tuning. The result does not test a journey map or stable ecology.

The working tree remains dirty with prior source changes and local experiment packages. Preserve those changes; nothing was staged, committed, pushed, or posted to Trello.
