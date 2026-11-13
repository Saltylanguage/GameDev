# S6 gentler Fox opening — diagnostic report

**Experiment:** `S6-FOX-SOFT-OPENING-20261110` ([contract](CONTRACT.md))
**Report:** `RPT-S6-FOX-SOFT-OPENING-DIAG-20261110`
**Feature:** Introductory Hare journey, Fox pressure
**Task:** Screen lower fixed Fox counts under the corrected fresh-node rule
**Focus:** Hare failure by node one and cumulative by node two
**Status:** Diagnostic complete; no arm met the preregistered confirmation screen; balance decision pending
**Generated:** 2026-11-10

## Evidence sources

| Run ID | Artifact path | Schema | Artifact hash |
|---|---|---:|---|
| `RUN-S6-FOX-SOFT-DIAG-20261110` | `artifacts/s6-fox-soft-opening-20261110/diagnostic/runs.jsonl` | 1 | `b4e75006c124a7b32a8c38b9a55c227a120d8e96d0eb899c238e6e23dbe7934d` |
| `PLAN-S6-FOX-SOFT-DIAG-20261110` | `artifacts/s6-fox-soft-opening-20261110/diagnostic-plan.json` | 1 | `9dfb049fcbd4b56eeabff8ba1aea0bbdf9ae7497172047047bd17f79a6af95cc` |

The machine-readable [paired summary](paired-summary.json) preserves each seed and arm. [summarize.py](summarize.py) checked 36/36 completed runs, exact seed coverage, 12 phases/1200 ticks, node-one starts, node-two Hare carryover, fixed Fox/Plant node-two starts, and reset-ledger deltas.

## Hypothesis and success criteria

**Question:** Does a fixed count below 16 Foxes make node one safer while retaining roughly 50–70% cumulative no-action Hare failure by node two?
**Hypothesis:** Lower Fox counts reduce node-one losses, but may make node two too safe because the same Fox count is restored there.
**Success screen:** At confirmation scale, at most 5/100 node-one failures and 50–70/100 cumulative failures by node two.
**Diagnostic follow-up screen:** An arm needed at most 1/12 node-one failures and 4–10/12 cumulative node-two failures to trigger a disjoint 100-seed confirmation. No arm qualified, so no confirmation was run.

## Controlled inputs

| Field | Value |
|---|---|
| Scenario/configuration | Authored Forest Edge; 400 Plants, 55 Hares, 8/10/12 Foxes per node |
| Ruleset fingerprint | `e89a6ed32cdf87a9314dd86349ce8cca8607bb266908e9553f8dd88ad5d0d616` |
| Seed range | 10100–10111, shared across three arms |
| Grid/configuration | 36×20, nonwrapping, opposed-roll combat, `bev-experimental`, neutral Genome |
| Build/commit | Branch `codex/s6-predator-first`, commit `9c2986995ecd190495cda8a037456f6c68332b05`; portable worker SHA-256 `ad2755bea7f7b9b3a96f9c3d7ba9abb93abb79962a59d2b7bc6177f6c073bfef` |
| Lifecycle mode/version | 12 phases of 100 ticks; six phases per fresh simulation node; no Mutations, purchases, population actions, or research |
| Snapshot | `artifacts/s6-fox-node-reset-20261110/forest-edge-node-reset-v3.json`, SHA-256 `2d3ba580456aba3ddd215689487e132a0685ac89baa14999d8dce52fb0eab7ed`; shared source hash `a1f5b1e1490e9a77ef5842b8d2eb343f4ba69595b97d0fa656cb9fba4e81f2f2` |
| Absolute tick window | 0–1200; observe after Hare extinction for diagnostic comparisons |
| Upgrade schedule | `none` in all 12 phases |

The snapshot's `sourceCommit` field is the pre-implementation base `59ad0f4`, because Unity exported it before the source commit was created. Its shared source hash matches the current worker and the simulation source at `9c298699`; the original Unity/portable parity check for this snapshot passed before the prior sweep.

## Observed results

Failure means zero Hares at the node closing tick. Confidence intervals are 95% Wilson intervals; with 12 seeds they are wide.

| Foxes per node | Failed by node 1 | Failed cumulatively by node 2 | Failed during node 2 among survivors | Mean Hares at tick 600 | Mean Plants at tick 600 / 1200 |
|---:|---:|---:|---:|---:|---:|
| 8 | 2/12 (4.7–44.8%) | 4/12 (13.8–60.9%) | 2/10 | 5.42 | 320.33 / 608.58 |
| 10 | 2/12 (4.7–44.8%) | 2/12 (4.7–44.8%) | 0/10 | 9.08 | 497.50 / 527.92 |
| 12 | 2/12 (4.7–44.8%) | 3/12 (8.9–53.2%) | 1/10 | 13.67 | 615.92 / 580.00 |

Cause-specific Hare deaths and Fox kills were recorded in the raw reports. Across the 12 node-one runs in each arm, Fox kills / Hare starvation deaths were 1,331 / 1,396 for 8 Foxes, 1,365 / 788 for 10, and 1,080 / 413 for 12. Across node two they were 114 / 20, 173 / 125, and 280 / 31. Minimum Plant counts at ticks 600 / 1200 were 139 / 184 for 8 Foxes, 236 / 0 for 10, and 392 / 84 for 12; at least one 10-Fox run reached zero Plants by node-two close.

No arm met the diagnostic follow-up screen. The 8-Fox arm reached its node-two lower screening bound (4/12), but had 2/12 first-node failures. No arm met the at-most-1/12 first-node requirement. The other two arms also fell below the node-two follow-up bound.

## Comparison method

The three arms used identical seed IDs and all other declared inputs. `CellSim.Batch` completed 36/36 runs with identity `66820d9c1eca4d8afae3c5b0e28da889a23f102267ab3884b139837cfde94a57`. The frozen v3 snapshot hash, current embedded-source hash, and worker assembly hash are retained in `diagnostic/batch.json` and the provenance fields above. Node openings and reset deltas were checked against each run's phase records.

The previous 16/18/20-Fox diagnostic used the same 12 seeds. Those outcomes can be compared as matched observations, but changing Fox count also shifts later species' starting cells through the shared placement shuffle. The comparison therefore measures each complete starting setup, not Fox count isolated from layout.

## Replay/visual verification

No Unity replay or visual inspection was part of this portable diagnostic. The reused snapshot had passed Unity parity in the preceding corrected sweep. No authored scenario or balance asset changed.

## Known limitations

- Twelve seeds are only a screening panel; these rates cannot establish either design target.
- The same Fox count resets at both nodes in this runner. This panel does not test lower node-one Foxes with a stronger node-two reset.
- Changing Fox count can shift Hare and Plant starting positions under the current grid generator.
- No Mutation response or human playtest was exercised.

## Human decision

**Decision ID:** `DEC-S6-FOX-SOFT-OPENING-20261110`
**Decision:** Pending
**Owner:** Josh
**Key observation:** All three lower fixed-count arms still had 2/12 first-node failures, while the 8-Fox arm had the highest observed cumulative node-two failure at 4/12.
**Scope:** Diagnostic evidence only; no Fox count is approved and no balance value changed.
**Follow-up:** Decide whether to screen lower Fox counts, or design a per-node static Fox-count comparison that starts gentler and restores more pressure at node two.
