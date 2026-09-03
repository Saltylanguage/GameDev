# EX-009 — Same-held-out-seed upgrade-order comparison

**Experiment ID:** `EXP-009`  
**Status:** Blocked before valid execution; Unity preflight failed  
**Parent:** `EXP-007` / exploratory follow-up `EXP-008`  
**Decision owner:** Human design owner  
**Scenario:** `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset`  
**Species:** `hare`

## Question

When the same two upgrades are applied in opposite orders, do the outcomes
change on the same held-out seeds?

## Hypothesis

If `faster-movement` and `crowding-tolerance` are independent rule changes,
the forward and reversed sequences should produce identical or equivalent
same-seed outcomes. Any paired difference is evidence that order, application
state, or another implementation detail matters.

## Locked comparison

| Arm | Ordered loadout | Seeds |
|---|---|---:|
| A | `faster-movement,crowding-tolerance` | 106–110 |
| B | `crowding-tolerance,faster-movement` | 106–110 |

Both arms must use ForestEdge, Hare, 32x32, 20.0 seconds, 0.1-second steps,
`opposed-roll` combat, `natural` attack opportunities, `bev-experimental`, the
same source revision, and the same report/statline validators. The existing B
arm is recorded at `artifacts/cellular-experiment-20260903-160827`.

## Success criteria

- Both arms produce complete, validated `report.json`, `report.csv`,
  `statline.csv`, `manifest.json`, and `unity.log` bundles.
- Every seed has an A/B pair with matching provenance except for ordered
  loadout and derived ruleset fingerprints.
- Compare final population, births, starvation, crowding, predation, movement,
  resource, and available encounter telemetry per seed.
- Report exact paired deltas and whether any difference is consistent across
  the five held-out seeds.
- Do not claim order independence unless the same-seed comparison supports it.

## Process corrections carried forward

1. **Same held-out panel:** Both orders use seeds 106–110. The prior EX-007 and
   EX-008 held-out panels differed, so they cannot answer this question.
2. **Metric definitions:** `PREY` is Hare deaths caused by carnivores, not food
   gathering. The comparison must use the corrected definition.
3. **No silent extrapolation:** This is a discrete two-order comparison only;
   it does not establish continuous scaling, cross-scenario transfer, or
   balance/fun conclusions.
4. **Evidence gate:** An incomplete Unity run is a failure record, not research
   evidence. No preflight bypass is permitted.
5. **Separate interpretation:** A/B differences describe model behavior; they
   do not decide whether either order is desirable for the game.

## Current blocker

The forward arm was attempted twice on 2026-09-03 and stopped at the intentional
Unity preflight gate before simulation. Both attempts failed because Unity could
not connect to the Package Manager IPC service:

- `artifacts/unity-preflight-20260903-173849/license-probe.log`
- `artifacts/unity-preflight-20260903-174035/license-probe.log`

No forward-order report was generated, so no A/B result is currently valid.
The Unity/Hub licensing and Package Manager service state must be repaired or
restarted before rerunning this package.

## Evidence available for continuation

- Existing reversed-order arm B: `artifacts/cellular-experiment-20260903-160827`
- Existing same-panel no-upgrade control: `artifacts/cellular-experiment-20260903-160706`
- Prior forward-order arm on different held-out seeds:
  `artifacts/cellular-experiment-20260903-153149`

The prior forward-order artifact is context only and must not be substituted
for the missing same-seed arm.
