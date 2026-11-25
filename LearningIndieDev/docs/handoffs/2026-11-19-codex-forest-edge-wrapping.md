# Forest Edge wrapping

[Working state](../WORKING_STATE.md) | Status: authored, Unity verification pending

- Owner: Codex
- Branch: `codex/rabbit-identity-rename`
- Baseline commit: `348e25da`
- Date: 2026-11-19

## Decision and change

Josh requested wrapping now. The authored Forest Edge scenario connects its
left/right and top/bottom edges. `ScenarioDefinitionAsset` stores topology in
each scenario and passes it into runtime data. The preview applies the selected
scenario's topology and resets legacy mode to bounded; an explicit Inspector
toggle still works before a session. The experiment runner inherits authored
topology when `-wrapEdges` is omitted and keeps explicit `true`/`false`
overrides. The S4 comparison fixture requests bounded edges explicitly. The
D5 visual review scenario records the wrapping already configured in its scene.

The five-species sandbox and other authored scenarios remain bounded. No
population, species rules, or historical report was changed. Since topology
changes encounters, this work does not establish viability or balance for the
current Forest Edge route.

## Verification

- `git diff --check` passed.
- Unity CLI `status` found no reachable Pipeline Editor. `CellSim.ps1 -Command
  Doctor` reported healthy CLI/license state but an unreachable project lock.
- The focused EditMode test was requested through `CellSim.ps1`; Auto stopped
  before starting Unity because the same project lock remained unreachable.
  Unity import, compilation, tests, and visual behavior are still unverified.

## Next check

When the project Editor is reachable, run the focused
`ForestEdgeWrappingReachesRunsAndScenarioSwitches` and
`S4EditorFixtureIsTransientAndSupportsPausedSingleTicks` EditMode tests, then
play the Forest Edge route to inspect seam crossings and current survival.
