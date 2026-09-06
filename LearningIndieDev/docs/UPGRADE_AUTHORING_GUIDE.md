# Species Per-Run Upgrade Authoring Guide

This is the repeatable workflow for creating a species per-run upgrade. The
authoring asset is a Unity `ScriptableObject`; the simulation never consumes a
live asset reference. An asset is resolved into an immutable
`SpeciesUpgradeSnapshot` for launch or a recorded acquisition boundary.

**Continuation migration pending:** a per-run upgrade lasts for the entire
expedition, across all simulation phases. Continued play needs the resolved
snapshot plus its acquisition/effective tick; the current launch-only metadata
does not supply that timeline. Starting-only fields are launch-only under the
locked CF-0 contract and are not live creature grants. In particular, Seed
Pouches is not a phase-break offer unless a separately designed live-state or
newborn mechanic replaces that restriction. See the
[migration plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md) and
[Stat-Line/research impact](CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md).

## Current catalog

The initial production catalog contains seven Hare upgrades:

- `trailblazer-long-stride` — Long Stride
- `trailblazer-far-sight` — Far Sight
- `warren-guarded-burrow` — Guarded Burrow
- `warren-room-to-breed` — Room to Breed
- `gardeners-seed-pouches` — Seed Pouches
- `gardeners-careful-sowing` — Careful Sowing
- `familial-bond-large-litters` — Large Litters

These values are starting hypotheses, not accepted balance.

The exact contract and acceptance state for every production row is tracked in
the [Species Per-Run Upgrade Acceptance Matrix](UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md).

## First example

The first worked example is:

`Assets/Data/CellularSimulation/Upgrades/Production/Trailblazer_LongStride.asset`

It is a provisional starting hypothesis, not accepted balance:

- Stable ID: `trailblazer-long-stride`
- Display name: Trailblazer: Long Stride
- Target species: `hare`
- Cost: `5` scientific data
- Modifiers: `movement.speed +0.5`; `reproduction.neighbor-count +1`

The second modifier is the tradeoff: the Hare needs one additional nearby
partner to reproduce.

## Create another asset

1. In the Unity Project window, open
   `Assets/Data/CellularSimulation/Upgrades/Production/`.
2. Create **Salty Game → Upgrades → Species Per-Run Upgrade**.
3. Give the file a readable name. The filename is presentation only; the
   `Upgrade ID` is the stable identity used by runtime, reports, and research.
4. Enter a unique, stable `Upgrade ID`, display name, and player-readable
   description.
5. Enter exactly one target species ID, such as `hare`, `fox`, or `fern`.
6. Enter a non-negative cost.
7. Add one or more modifier rows. Select the attribute from the registry-backed
   dropdown and enter a signed additive value. Positive values increase an
   attribute; negative values decrease it.
8. Add prerequisite or exclusion IDs only when the relationship is part of the
   accepted design. Do not list an upgrade as both required and excluded.
9. Fix every inspector validation warning before using the asset. Unknown
   attributes, duplicate attributes, empty fields, invalid species IDs, and
   fractional values for integer attributes are rejected.
10. Add the asset to the `Authored Run Upgrades` list on the simulation preview
    component when it should appear in the player-facing reward choices. The
    current prototype scene is `CellularAutomataPrototype`.
11. Keep research/dummy assets outside `Production/`, for example under a
    separate `Research/` folder, so they cannot be mistaken for player content.

## V1 contract

- One target species per upgrade.
- Per-run scope only.
- One stable attribute ID and one finite signed numeric value per modifier.
- Additive modifiers only. Multiplication, set values, ranges, clamping, and
  conditional expressions are not supported in V1.
- An attribute may appear only once in an upgrade.
- Upgrade order is meaningful. Preserve the authored purchase order when
  constructing a loadout; do not sort it.

## What happens at runtime

The authoring adapter calls `TryCreateSnapshot`. The validated snapshot captures
the upgrade values, contract version, registry fingerprint, and deterministic
fingerprint. `SimulationLaunchRequest` carries the ordered snapshots to the
preview and runner. Launch preflight validates the complete loadout before any
state is mutated. The run result and report preserve the same ordered metadata.

Do not pass the `ScriptableObject` itself into simulation state or change an
asset during a run.

## Use the catalog in research

Research experiments can use the same authored values with the PowerShell
wrapper's `-UpgradeAssetSequence` option. Pass stable IDs in the order they
should be applied:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Run-CellularExperiment.ps1 `
    -PlayerSpeciesId hare `
    -UpgradeAssetSequence trailblazer-long-stride,warren-guarded-burrow
```

The `SpeciesUpgradePredictionInputAdapter` resolves those IDs from
`Production/`, applies the resulting snapshots to the experiment, and records
the exact prediction input and fingerprints in `report.json`. This keeps a
research arm aligned with what the player receives. Do not hand-copy modifier
values into a research fixture when this option is appropriate. The legacy
`-UpgradeId` and `-UpgradeSequence` options remain for historical experiments
that intentionally use the old catalog.

Research-only fixtures use the same adapter with an explicit catalog path:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Run-CellularExperiment.ps1 `
    -ScenarioPath Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset `
    -PlayerSpeciesId hare `
    -UpgradeAssetSequence faster-movement,crowding-tolerance `
    -UpgradeAssetCatalogPath Assets/Data/CellularSimulation/Upgrades/Research/EX-007
```

The catalog path must stay inside `Assets/` and is recorded in the report and
prediction input. Use this only when the experiment contract names a separate
research catalog; do not replace historical fixture values with newer
production assets.

## Verification checklist

- [ ] Stable ID is unique and unchanged after publication.
- [ ] Target species is exactly one valid species ID.
- [ ] Every modifier appears in `SpeciesAttributeRegistry`.
- [ ] Values use signed additive V1 semantics and respect integer attributes.
- [ ] Tradeoffs, prerequisites, and exclusions are stated in the description.
- [ ] The asset is in `Production/` only if it is intended as player-facing
      catalog content.
- [ ] The asset resolves successfully to a snapshot before it is added to a
      simulation or experiment.
- [ ] Any balance claim is supported by a deterministic baseline comparison;
      authoring an asset alone does not validate its gameplay effect.

The Editor fixture tests automatically resolve every asset under `Production/`
and reject duplicate stable IDs. When intentionally changing one of the named
first-catalog fixtures, update its expected contract in
`Assets/Tests/Editor/SpeciesUpgradeAssetCatalogTests.cs` in the same change.
Keep the matrix and its fixture expectations in sync when adding or changing a
production asset.

## Catalog validator

Open **Salty Game → Upgrades → Catalog Validator** to scan the production
folder. The read-only window reports valid and invalid assets, duplicate stable
IDs, target species, costs, modifiers, fingerprints, and source paths. Use
**Ping** to select a problem asset, fix it in the normal Inspector, and refresh
the scan. The tool does not rewrite assets, apply upgrades, or discover assets
for runtime gameplay.

## Related implementation

- `Assets/Scripts/Game/Species/SpeciesUpgradeAsset.cs`
- `Assets/Scripts/Game/Species/SpeciesUpgradeContract.cs`
- `Assets/Scripts/Game/Species/SpeciesAttributeRegistry.cs`
- `Assets/Editor/SpeciesUpgradeAssetEditor.cs`
- `docs/UPGRADE_SYSTEM_DIRECTION.md`
