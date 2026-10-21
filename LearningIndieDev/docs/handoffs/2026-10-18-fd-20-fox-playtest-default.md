# Forest Edge: 20-Fox playtest default

Date: October 18, 2026. Local `BevBranch`, based on `acc371a`.

Bevin explicitly approved switching to 20 starting Foxes after the
[opening-population diagnostic](../Research/Experiments/FD-Opening-Hare-Collapse-20261018/REPORT.md).
The authored `Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset`
now starts **20 Foxes, 55 Hares and 400 Plants**. Only the Fox count changed,
through Unity's SerializedObject API; asset identity and other settings remain.
Use a fresh journey to pick up the new default.

## Verification

- Unity resolved the new populations and fingerprint
  `61833cae37ef2b63a5bb5b05f0f24c8483f47d690c5bad8e2c0e5813ea2ab70a`,
  exactly matching the frozen 20-Fox candidate already tested. Saved population
  overrides were disabled when inspected.
- `PopulationConfigurationTests`: **17/17 passed**, live Editor, results under
  `artifacts/unity-tests-20261018-111425/`. Two authored-count expectations now
  reflect 20. An initial run used stale compiled expectations; recompilation
  completed without errors before the passing run.
- `FieldDataNaturalFailureForfeitsJourneyWalletAndRestartStartsAtZero`:
  **1/1 passed**, live PlayMode, results under
  `artifacts/unity-tests-20261018-111558/`. The natural-failure fixtures now use
  seed **10108**, which still fails during round 3 with 20 Foxes. The prior
  seed 10125 lasts beyond their 600-tick limit. An intermediate 10106 fixture
  failed its round-3 assertion because extinction occurred in round 4; this
  was a fixture mismatch, not a settlement logic change.

The graphics fixture uses the same replacement seed; its rendered check was
not rerun for this value change. Earlier broad suites and 35-Fox panels remain
historical evidence. No new full-suite or player-build claim is made.

## Next

Play fresh journeys with the approved opener. Matched seeds 10100 and 10102
remain useful contrasting cases from the diagnostic. Better early affordability
does not resolve the later crashes or establish overall balance acceptance.
FD-T13 independent visual review and FD-T16 team self-play remain open.
Frozen research artifacts are preserved. No Trello changes, commit or push.
