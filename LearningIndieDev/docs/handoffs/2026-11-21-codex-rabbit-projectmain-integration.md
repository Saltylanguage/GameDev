# Rabbit and ProjectMain integration — November 21, 2026

`origin/ProjectMain` at `663b5c2a` was merged into `codex/rabbit-identity-rename`. The merge keeps Rabbit as the canonical species name and ID, the five-species sandbox, the 15-creature cap and Forest Edge setup, while taking ProjectMain's accepted GLJP prices, ranked research, equipment gates, and fixed and irregular map shapes. Existing `.meta` GUIDs were preserved. The saved `foundingHaresRank` field and legacy `hare` IDs remain readable for profile migration.

Map wrapping follows each scenario's authored `wrapEdges` value. Forest Edge and the D5 visual review asset explicitly wrap. Other authored scenarios can remain bounded. The Desktop preview can switch wrapping before a session starts, and portable experiment overrides retain that choice. A bounded shape test checks that missing edges do not connect.

## Verification

- `dotnet build tools/CellSim.Batch/CellSim.Batch.csproj -c Release --no-restore` — passed, zero warnings/errors.
- `dotnet build tools/CellSim.Workbench/CellSim.Workbench.csproj -c Release` — passed; NuGet vulnerability lookup was unavailable.
- Runtime and Runtime Tests generated projects compiled after temporarily including the new `MapTopology.cs` and `MapTopologyTests.cs` files; the generated project files were restored afterward. This checks source compatibility but does not execute Unity tests.
- `CellSim.Batch self-test` — passed, including shape/seam portable JSON and deterministic journey replay.
- `CellSim.ps1 Doctor` — project lock exists, Pipeline Editor unreachable; Unity EditMode/PlayMode and visual checks remain pending on this merged source.

Next: run focused `MapTopologyTests`, `PopulationConfigurationTests`, `FieldDataPermanentProgressionAssetTests`, and profile migration tests in Unity once the Editor can open, then review the Forest Edge and irregular layouts visually. Do not infer acceptance or balance from the portable checks.
