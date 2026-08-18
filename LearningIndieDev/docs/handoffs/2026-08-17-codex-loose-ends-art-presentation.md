# loose-ends-art-presentation

[Working state](../WORKING_STATE.md) | Status: shared; runtime acceptance and
simulation follow-up remain open

- Owner: codex
- Branch: `codex/cellular-sprite-tiling`
- Baseline commit: `4f04f4b2`
- Date: 2026-08-17

## Summary

A fresh LooseEnds review was completed after the cellular-art and presentation
batch. The branch is clean and synchronized with origin. The durable ledger,
sprite-tiling plan, and current Noesis migration context now describe the
implementation that is actually checked in rather than the old Resources-based
and IMGUI-era assumptions.

## Shared changes and context

- Standardized animal exports are under `Assets/Art/Species/Animals/Standardized`
  and are packed through scene-wired SpriteAtlas assets.
- Terrain art is under `Assets/Art/Terrain`; the board and Noesis host receive
  presentation inputs from the scene instead of loading sheets from `Resources`.
- `tools/Get-UnityMcpRelayHealth.ps1` is available for a quick relay check.
- `docs/LOOSE_ENDS.md` contains the current P1/P2 triage and resolved items.

## Validation performed

- `git status --short --branch`: clean on `codex/cellular-sprite-tiling`.
- Branch is synchronized with origin at `4f04f4b2`.
- `Get-UnityMcpRelayHealth.ps1 -Json`: `Status: OK`, zero relay processes.
  This is expected when Unity/Codex relay services are not active.
- Latest retained Play Mode report is the 2026-08-16 ForestEdge run: 32 x 32,
  seed `-877772592`, 200 ticks, final Fox 5, Hare 19, Plant 902. It recorded
  one Fox birth and six Fox food events, but no aggregate Fox Eating row.
- No Unity visual screenshots or gameplay-scale acceptance checks were run
  after `4f04f4b2`.

## Risks and incomplete work

1. `TerrainTilePreviewWindow` still references the deleted
   `Assets/Resources/CellularArt/Terrain_01_SpriteSheet.png` path.
2. Direct Fox/Rabbit sprite overrides can bypass atlas fallback for other
   authored species in `SpeciesSimulationViewModel`.
3. The latest single balance run is below the Hare target and is not enough for
   a multi-seed conclusion; Fox reproduction is only partially established.
4. Fox food/action telemetry still needs the Eating-state accounting fix.
5. EX-002 schema-5 execution remains blocked by native Unity startup failure.
6. Sprint 1 still needs a durable kickoff/control record and acceptance owners.

## Next actions for the next environment

1. Pull this branch and verify the commit before editing.
2. Fix the terrain preview path and species fallback, then run Unity editor and
   gameplay-scale screenshot checks.
3. Repair Unity batch startup and run the EX-002 schema-5 control.
4. Re-run ForestEdge with fixed seeds before changing balance values.
5. Record Sprint 1 kickoff and acceptance evidence in the agreed Trello/docs
   control record.

