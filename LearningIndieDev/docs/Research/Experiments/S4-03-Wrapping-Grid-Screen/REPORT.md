# Wrapping-grid observations - 2026-10-04

## At a glance

All 80 matched runs completed and passed input, acquisition, population-accounting and source-provenance checks. No wrapped run retained Hares at tick 600. Bounded control retained them in 7/20 and bounded Movement-first Trailblazer in 2/20. The feature works in focused domain checks; the proposed survival improvement did not appear in this panel. Manual visual acceptance and a decision about authored defaults remain open.

Experiment: S4-03-20261004-wrap-400-20-10. Report: v1. Source: `6a0ab08e155b1a8a2962d19a6c5eca1bc8233118` plus local wrapping and previously completed scheduled-avoidance fixes, dirty `BevBranch`. Source hashes and diff: local experiment-contract.json/source.diff. Runtime: live Unity 6000.4.6f1; schema 32; cellsim-experiment-metrics v1. Exact inputs and lifecycle: [method](README.md). Owners: Bevin/Salty. Decision pending.

## Outcomes

Survival means a positive Hare population at tick 600. Extinction runs remain included. Mean ending tick includes surviving runs censored at the 600-tick horizon.

| Path | Edges | Survived / 20 | Mean final Hares | Median final Hares | Mean ending tick |
|---|---|---:|---:|---:|---:|
| Skip all | Bounded | 7 | 3.00 | 0 | 460.15 |
| Skip all | Wrapping | 0 | 0.00 | 0 | 406.95 |
| Movement then Avoidance | Bounded | 2 | 0.10 | 0 | 396.05 |
| Movement then Avoidance | Wrapping | 0 | 0.00 | 0 | 429.25 |

Paired final-population changes were worse in 7 seeds and equal in 13 for control; worse in 2 and equal in 18 for Trailblazer. No wrapped seed improved final Hare population. Some wrapped runs lasted longer; wrapping did not shorten every run.

## Slashlines and ecology

Values below are arithmetic means of per-run counts or Valid per-run ratios, not ratios of pooled totals. Runs have different durations and population histories. Counts do not isolate causes.

| Path / edges | ECN encounters | PREY deaths | STRV deaths | BIR births | eAVI | Mean final Foxes | Mean final plants |
|---|---:|---:|---:|---:|---:|---:|---:|
| Control / bounded | 120.90 | 68.90 | 55.50 | 107.40 | 0.700 | 14.45 | 302.60 |
| Control / wrapping | 202.15 | 116.70 | 123.05 | 219.75 | 0.608 | 38.70 | 71.25 |
| Trailblazer / bounded | 135.05 | 78.55 | 40.70 | 99.35 | 0.678 | 22.80 | 288.60 |
| Trailblazer / wrapping | 185.50 | 104.40 | 150.15 | 234.55 | 0.637 | 33.45 | 29.00 |

eAVI measures encounter avoidance using exposed/total herbivore steps. Its mean declines in both wrapped paths. Wrapped paths have more births and deaths, larger Hare step totals, and fewer final plants. These are observations; border trapping, predation and resource depletion have not been causally separated.

All 80 pAVI/eAVI/predAVG/bAVG/RFS/APS values are Valid. sAVI has 79 Valid and 1 N/A; cAVI has 9 Valid and 71 N/A. All 40 wrapped cAVI values are N/A. Exported slashlines retain statuses; N/A is never replaced with zero. The package contains 373 actual phase windows.

## Verification and limits

- All 20 initial layouts match across topologies for species, terrain, energy and terrain energy; all 80 declared starting populations are 400/20/10. First phases match between paths within each topology before any choice. Reachable L1 acquisitions remain at ticks 100/200 and final/phase FPO accounting reconciles.
- All process-independent run fields in both bounded arms match the corresponding prior screen. Only process-scoped entity IDs and versioned rules fingerprints are excluded from that historical comparison.
- Additional wrapped Trailblazer seed 10100 replays identically after excluding entityId values, including phase fingerprints and ordered acquisitions.
- Six focused assertion methods ran directly in the live Editor: grid seams/corners/unique neighbors/copies, domain movement/flee/perception/navigation/combat/food/reproduction/counting, transient Inspector fixture and locking, data copies/checkpoint restore, scheduled avoidance, and 384 existing fresh-grid avoidance comparisons. These are direct assertion checks, not a full standard NUnit/Clean-suite result.
- Unity recompilation completed with no compilation errors. Final Console: zero errors, one Unity AI package account-availability warning (captured separately). Five authored scenario/species assets remain byte-identical. No source drift during the batch; `git diff --check` passed. The Editor remains stopped; no scene-save operation was invoked.
- No visual acceptance, performance benchmark, 200-seed confirmation, full strategy-path acceptance, build readiness, or whole-ecosystem balance claim.
