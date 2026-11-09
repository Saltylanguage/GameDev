# November GLJP handoff for Salty

November 9, 2026. The GLJP implementation is on `BevBranch` in commit `de26d3b69df15019480dd1e0c5607053d274da5f`. Use this branch for the current Gene Lab and Field Data work. The Sprint 6 planning baseline is `ProjectMain` at `5bd1a9a`.

## What is ready to review

- Gene Lab research unlocks temporary journey Mutations independently of which nodes are equipped. Heightened Senses opens separate Long Vision and Keen Senses choices. Seed Dispersal covers the seed-planting unlock.
- Population Reserves has five ranks, costs one equipment point, and raises the purchased Hare allowance per simulation node. The Field Data test journey supports all three shop payout tiers. The desktop fixture verified purchases, replacement timing, results, and restart.
- Burrowing supports family shelter, capacity upgrades, expiry and checkpoint restoration. Equipped Born Burrower grants digging from launch. The board uses a brown circle to mark burrows.
- November is the canonical simulation baseline. Ordinary Fox pursuit of Hare movement scent defaults off. The older comparisons are historical.

## Verification and open work

- Full Unity EditMode had 763 passes and one replay-manifest skip. The full PlayMode rerun had 100 passes, one failure and two visual-runner skips. The sole failure is authored Forest Edge seed 10100 at tick 1210, after shop two. The controlled four-simulation route finishes at tick 2400. The November CellSim.Batch Release build and self-test also passed. [Suite results and capture references](../GLJP_FINISH_PLAN.md#november-9-closeout-evidence)
- GLJP-02 and GLJP-03 are Done. GLJP-01 and GLJP-06 are In Progress. GLJP-04 and GLJP-05 are in Current Work. Remaining items are catalog prices and prerequisites, Faster Gestation and Increased Minimum Litter, the Sprint terrain contract, player review, and the authored route survival expectation. [Decisions](../GLJP_MORNING_REVIEW.md#catalog-choices-for-closeout) · [Trello board](https://trello.com/b/Z14AeSnU)

For the handoff, start with the [finish plan](../GLJP_FINISH_PLAN.md) and the [current November route evidence](2026-11-07-gljp06-route-preflight.md#november-9-controlled-route-completion). Unity screenshots and raw test results are retained under the local `artifacts/` folder. The linked handoffs record what each test exercised.
