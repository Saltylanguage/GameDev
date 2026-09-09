# EX-010 alternate-order contract

**Experiment:** `EXP-010` — Sequential upgrade continuation
**Purpose:** Matched alternate sequence for the first EX-010 run
**Status:** Approved by Josh for execution on 2026-09-06; Sim review follows the run

## What changes

Only the order of upgrade acquisition changes. Everything else stays the same:

- ForestEdge scenario and Hare player species.
- Same combat and simulation options.
- Ten phases of 200 ticks each.
- Same Skip positions: before phases 2, 4, and 8.
- Same development seeds 1–20 and held-out seeds 106–110.
- Same six production upgrades, each used once.

The first EX-010 sequence acquired the upgrades in this order:

`trailblazer-long-stride`, `trailblazer-far-sight`,
`warren-guarded-burrow`, `warren-room-to-breed`,
`gardeners-careful-sowing`, `familial-bond-large-litters`

This alternate sequence reverses that order:

`familial-bond-large-litters`, `gardeners-careful-sowing`,
`warren-room-to-breed`, `warren-guarded-burrow`,
`trailblazer-far-sight`, `trailblazer-long-stride`

## Locked phase schedule

| Phase | Tick window | Decision before this phase |
| --- | --- | --- |
| 1 | `(0, 200]` | No added upgrade |
| 2 | `(200, 400]` | Skip |
| 3 | `(400, 600]` | `familial-bond-large-litters` |
| 4 | `(600, 800]` | Skip |
| 5 | `(800, 1000]` | `gardeners-careful-sowing` |
| 6 | `(1000, 1200]` | `warren-room-to-breed` |
| 7 | `(1200, 1400]` | `warren-guarded-burrow` |
| 8 | `(1400, 1600]` | Skip |
| 9 | `(1600, 1800]` | `trailblazer-far-sight` |
| 10 | `(1800, 2000]` | `trailblazer-long-stride` |

At each boundary, the simulation stops at the completed tick. The selected
upgrade becomes active on the next tick. A Skip leaves the ordered upgrade list
unchanged.

## Review rule

Compare this sequence with the original run by matching the same seed. Review
the complete phase Stat-Lines, the chronological per-stat deltas, and the
independent final Stat-Line. Do not create a new combined score or claim a
general order effect from this one matched pair of panels.
