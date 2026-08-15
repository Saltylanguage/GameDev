# Scenario roster and player-species selection

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: BevLaptopBranch
- Baseline commit: f1a585c
- Date: 2026-08-15

## Delivered

- The active scenario's authored species list is now the runtime roster authority. The preview exposes the roster in authored order and a playable subset containing non-plant species only.
- Player Settings now includes a `Player species` selector. Changing it uses the existing validated preview selection path, resets the scenario to its prepared starting run, and updates the active player species.
- The runtime HUD shows the selected scenario, selected player species, and the current roster with live per-species population counts.
- A gold outline marks every creature cell belonging to the selected player species on the simulation board.

## Scope and design notes

- No scenario assets, prefab references, scene serialization, or Project Settings were changed.
- Existing `ScenarioDefinitionAsset` and `SpeciesDefinitionAsset` data remains the source of truth; no duplicated species catalog or generalized UI framework was introduced.
- Plants are visible in the roster but deliberately excluded from player selection because the simulation's existing player-selection rule accepts only playable, non-plant species.

## Validation

- `dotnet build SaltyGame.Tests.csproj -v:q` completed with 0 errors.
- `dotnet build Assembly-CSharp.csproj -v:q` completed with 0 errors.
- Unity 6000.4.6f1 completed batch script compilation/import with exit code 0 after the change.
- Added focused runtime tests covering roster filtering, plant rejection, and selecting Carnivore. The standalone `dotnet test` host compiles the Unity test project but does not execute Unity NUnit tests; an EditMode Unity test run was started and should be checked in `Logs/codex-species-selection-tests.log` / `Logs/codex-species-selection-test-results.xml` before merge.

## Remaining review

- Open `CellularAutomataPrototype.unity`, select each scenario, and visually confirm its available player choices, roster summary, and gold board outline in Player Settings and during runtime.
