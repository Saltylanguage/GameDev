# S4-03 Hare paired-seed screen - 600/40/25

[Working state](../WORKING_STATE.md) | Status: complete locally, Bevin/Salty review pending

- Handoff schema: 1
- Handoff ID: 2026-10-05-2348-codex-s4-600-40-25-paired-seed-screen
- Owner: Codex; decision owners: Bevin and Salty
- Branch: BevBranch
- Baseline commit: 6a0ab08e155b1a8a2962d19a6c5eca1bc8233118
- Date: 2026-10-05 local; Unity completion 2026-10-06 03:39 UTC
- Sharing: local source context and documentation only; no commit, push or board update

## Summary

Bevin requested a larger paired-seed run at 600 Plants / 40 Hares / 25 Foxes and selected 100 common fresh seeds per arm. The corrected 13-arm strategy matrix ran with wrapping enabled on seeds 13000-13099. All 1,300 jobs completed; 7,219 phase windows were retained and independently validated. The skip-all control survived with positive Hares at tick 600 in 56/100 runs. Seed Dispersal and both Gardeners acquisition orders retained all three species in 43/100 runs each, with 60-61 Hare survivors. Both Trailblazer orders had fewer Hare survivors (42/100 and 46/100). No strategy or tuning was approved.

## Scope and changes

- Added a separate [experiment package](../Research/Experiments/S4-03-Hare-Paired-Seed-Screen-600-40-25/README.md), including a factual [report](../Research/Experiments/S4-03-Hare-Paired-Seed-Screen-600-40-25/REPORT.md) and separate [AI analysis](../Research/Experiments/S4-03-Hare-Paired-Seed-Screen-600-40-25/ANALYSIS.md).
- Updated the experiment registry and `WORKING_STATE.md` with the completed local result.
- Raw per-run histories, requests, checks, scripts and SHA256 manifests are in the ignored local folder `artifacts/s4-paired-screen-20261005-600-40-25-wrapped-100/`.
- No production source, authored scenario, production population default or skill value was modified by this experiment. Pre-existing dirty source work remains on the branch; exact modified C# file hashes and Forest Edge scenario hash are recorded in the experiment contract.

## Frozen experiment

Forest Edge, 36x20; 600/40/25 starting populations; wrap edges on; .1-second steps; six 100-tick phases; 600-tick maximum; Hare player; natural opposed-roll combat; `bev-experimental`; coupled responses off; no extra Genome or reinforcements. The 13 arms were skip-all control, six singles, and both acquisition orders of Trailblazer, Warren and Gardeners. Singles choose level one at tick 100; pairs choose at ticks 100 and 200; remaining phase choices skip. Every arm uses the same 100 seeds. Hare extinction and missing later phases are retained, not recoded as survived.

## Validation

- Connected Unity Editor 6000.4.6f1, Windows 10; research dispatcher compiled in memory with no diagnostics and used four local workers.
- 1,300/1,300 arm-seed records present, no job errors. Pilot run is separate from the main 1,300.
- Tick-zero population, grid bounds, stop ticks, wrapping, ruleset fingerprints, identical pre-choice phases (excluding process-scoped entity IDs), scheduled acquisition snapshots and registry fingerprint validated.
- All 1,300 whole-run and 7,219 phase slashlines passed independent formula, denominator, status, FPO and seed-dispersal food accounting checks. Source and scenario hashes were unchanged after execution.
- `git diff --check` passed. No standard NUnit/Clean suite or build was run because the experiment changed no runtime/editor project source.

## Result boundary and next step

This is exploratory evidence for one population/topology. Survivorship and terminal FPO are censored by extinction; all-three presence does not mean the ecology is healthy or resilient. The individual survival confidence intervals overlap substantially, 13 arms create a multiple-comparison concern, and the final Hare median remains 0-3.5 for all arms. Both Gardeners orders show the same high all-three count but do not establish an order winner.

Bevin and Salty: review the method, factual report and separate analysis. A reasonable next candidate, if useful, is a narrower fresh-seed confirmation of control, Seed Dispersal and both Gardeners orders with the same 600/40/25 wrapped setup. Keep that as a human decision; no further run, source change, push or Trello update is implied here.
