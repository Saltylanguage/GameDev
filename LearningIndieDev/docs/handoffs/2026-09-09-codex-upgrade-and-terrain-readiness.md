# Handoff — Upgrade and terrain readiness

**Date:** 2026-09-09

**Owner:** Codex

**Branch:** `codex/simulation-window-production`
**Status:** Local audit complete; terrain import gate added; no push performed

## Outcome

The Mutation foundation is working and should not be rewritten. Its next useful
work is the shared capability map, Forest Edge reference panel, and balance
evidence for the seven current Hare assets.

Genome is not implemented yet. The Gene Lab is a concept surface with local
toggle text; the profile stores only profile identity, and the launch request
contains only the selected species' temporary Mutation loadout. The first
Genome slice must therefore begin at profile ownership and the launch boundary,
not in the UI and not inside the existing per-run Mutation list.

The smart-tiling resolver is deterministic and its 47-mask file convention is
usable for import. A new automated gate checks both 47-tile families and their
atlas packable. Josh chose Grass and Desert as peer visual biomes with a
separate universal base. The board now draws the neutral base first and then
the computed Grass or Desert mask selected by the cell's explicit terrain ID.
Chrono's family tiles can be staged immediately, but their `000` and diagonal
meaning must still be confirmed; production art for the universal base is also
still needed.

## What was verified

### Mutation

Facts:

- `SpeciesUpgradeSnapshot` is an immutable, versioned, signed-additive stat
  contract backed by the explicit `SpeciesAttributeRegistry`.
- Production assets resolve through a Scriptable Object adapter; runtime state
  contains copied values rather than live assets.
- Ordered IDs, resolved values, registry identity, and loadout fingerprints are
  carried into launch, continuation, results, and research reports.
- The current production catalog contains seven Hare assets. Their authoring
  contract is accepted; their values and paths still need balance evidence.
- Starting-energy and starting-reserve changes correctly refuse mid-run
  application. Other current Mutations apply at a frozen phase boundary.
- The product target remains nine Mutation decisions across a ten-phase Species
  Simulation. Mutation never belongs in a Biome Simulation.

Assessment:

The execution path is sound for V1. Adding another framework, operation type,
or parallel catalog now would add risk without answering the open balance
questions. Keep one-time effects and abilities out of this contract until one
of them has an explicit timing, telemetry, and replay design.

### Genome

Facts:

- `Helper_ProfileSession` persists profile ID and display name only.
- `ProfileSessionSnapshot` has no wallet, unlock, or active-Genome state.
- `SimulationLaunchRequest` has no species-keyed Genome snapshot.
- `SpeciesSimulationPreview` applies the ordered Mutation snapshots to the
  selected player species.
- The Gene Lab's Guarded Burrow toggle and `6 / 8` display are local window
  state. They do not persist, enter a launch, or change simulation rules.
- There is no implemented Biome Simulation mode or separate Biome result path
  yet, so the documented mode rules remain a future consumer contract.

Assessment:

The visible mock is useful design reference, but it is not a safe starting
point for runtime ownership. Reusing the selected player's Mutation loadout for
Genome would violate accepted concern `UPG-C07`: active Genome must be frozen by
stable species identity and apply even when that species is not player
controlled. Reporting work must also preserve `UPG-C08` by keeping Species and
Biome outcomes separate.

### Smart tiling

Facts:

- The resolver uses 47 normalized eight-neighbor masks with names such as
  `Grass_029` and `Desert_029`.
- The bit order is `N=1, NE=2, E=4, SE=8, S=16, SW=32, W=64, NW=128`.
- A diagonal-only neighbor is deliberately normalized by adding its two
  adjacent cardinal directions. This is not the only common 47-mask convention,
  so Chrono's mapping must match it.
- The current blob folder contains all 47 Grass and all 47 Desert PNGs.
- The terrain atlas packs `Assets/Art/Terrain/Blob/128`, so exact-name
  replacements in its two family folders are included automatically.
- Source sprites use 128×128 pixels, 128 PPU, point filtering, no mipmaps, and
  one sprite per file. The atlas had overridden them with bilinear filtering;
  it now uses point filtering too.
- The live Noesis board now paints a neutral universal base beneath every cell.
  Grass and Desert each draw their own computed mask; Bare draws neither.
- Mask `000` means “draw no transition overlay” in the current generated set.
  An isolated family cell therefore shows the neutral base unless Chrono's
  `000` tiles use the filled-island contract.
- The simulation terrain registry still requires Bare and Grass. A production
  Desert scenario must explicitly author its own movement and resource rules;
  the presentation change does not invent those rules.

Assessment:

The import naming, settings, and peer-family renderer are ready. The full
dual-biome presentation is not accepted until the art contract answers what
mask `000` means, the separate base art arrives, and targeted captures pass.
Josh accepted the mask and ownership safeguards as Mild concerns `TILE-C01`
and `TILE-C02`; their progress is recorded in
[`../Planning Concerns/terrain-smart-tiling.md`](../Planning%20Concerns/terrain-smart-tiling.md).

## Day plan — order of execution and importance

The day's main outcome is an approved foundation for evaluating Mutations and
designing Genome without rewriting working code. Terrain remains a conditional
integration lane: prepare it first, then execute it when Chrono's files are
available.

| Order | Importance | Work block | Time box | Finish line |
| --- | --- | --- | --- | --- |
| 1 | P0 | Restore context and check the terrain contract | 15 minutes | The existing dirty files are protected, and the four delivery questions in the terrain runbook have answers or are waiting on Chrono. |
| 2 | P0 | Write the shared capability map | 45 minutes | Every current Mutation maps to familiar player language, exact registered stats, one local measure, and one ecosystem measure. |
| 3 | P0 | Lock the Forest Edge reference panel | 45 minutes | Seeds, duration, phase timing, ruleset identity, species roles, and separate Species/Biome measures are named for approval. |
| 4 | P1 | Pre-register one Mutation comparison | 60 minutes | Control plus one Trailblazer, Warren, and Gardeners candidate can be run without choosing thresholds after seeing results. |
| 5 | P1 | Execute and review the first matrix | 60–90 minutes | The complete artifact bundle exists, provenance passes, and conclusions are bounded to the declared panel. |
| 6 | P1, conditional | Import and review Chrono's terrain | 60–90 minutes | If the files are available, the automated contract, 47-mask previews, and targeted runtime checks pass. Otherwise skip this block without losing momentum. |
| 7 | P2 | Write the first Genome slice decision record | 45 minutes | Profile ownership, unlocked versus active state, launch freezing, capacity policy, and first-node acceptance tests are explicit. |
| 8 | P0 | Close the day cleanly | 30 minutes | Relevant tests pass, evidence and local changes are recorded, and tomorrow has one unambiguous first action. |

### Started today

- The shared capability map and Forest Edge reference panel now live in
  [`UPGRADE_CAPABILITY_REFERENCE_PANEL.md`](../UPGRADE_CAPABILITY_REFERENCE_PANEL.md).
- The first local pilot is preregistered there as ten arms: one control plus
  early, middle, and late activation for Long Stride, Guarded Burrow, and
  Careful Sowing.
- The pilot is diagnostic only. It cannot promote values, replace missing
  direct telemetry, or combine Species and Biome results into one score.
- The ten-arm pilot completed on seeds 12001–12005. All ten bundles passed the
  existing artifact validator and the intended production snapshots and phase
  boundaries were present.
- The peer-family terrain seam compiled with 213/213 EditMode tests passing in
  `artifacts/unity-tests-20260909-113856`.

### Must finish before lunch

Complete blocks 1–4 in order. Keep the capability map and reference panel in one
small document so they cannot drift apart. Use the existing stat registry and
telemetry; do not build a parallel balance model.

For the first Mutation comparison:

- select one representative from Trailblazer, Warren, and Gardeners;
- compare each with control at early, middle, and late acquisition timing;
- resolve production assets through `SpeciesUpgradePredictionInputAdapter`;
- record the expected local effect, expected ecosystem consequence, suspected
  interaction, and decision rule before execution.

Do not mint a broad experiment program, optimizer, or dashboard. One honest
matrix is the day's evidence target.

### Afternoon execution rule

Finish the running Mutation command and its evidence check before switching
contexts. If Chrono's delivery is ready, perform the terrain runbook next. If it
is not ready, move directly to the Genome decision record.

Terrain import activates accepted Mild concerns `TILE-C01` and `TILE-C02`.
Confirm mask meaning and Desert ownership before replacing production images;
then continue with the smallest mitigation in the terrain concern record.

### Genome finish line for this day

The Genome deliverable is a decision-ready first slice, not a UI implementation.
It should specify:

- one versioned signed-additive Hare node with stable ID, capacity cost,
  explicit modifiers, and fingerprint;
- profile-owned permanent unlocks separated from active node IDs per species;
- a species-keyed active Genome snapshot frozen at launch;
- Genome application to every matching participating species before the
  selected species' temporary Mutations;
- separate Genome and Mutation provenance;
- tests for reload, deactivate-without-loss, capacity, frozen launch state,
  background-species application, species isolation, and Mutation regression.

The active capacity is now 8 points per species. Reallocation is free between
runs and frozen during a run. Node cost semantics, named loadouts, and wallet
settlement still need Josh's approval. If those decisions are approved early
and all higher-priority blocks are complete, implementation may begin with the
plain profile snapshot and its tests; UI wiring remains later.

### End-of-day acceptance

- The capability map and reference panel are understandable without reading
  runtime code.
- The Mutation comparison is either executed or ready to run with no missing
  decision.
- Terrain is either integrated through the full runbook or clearly waiting on
  named delivery information.
- Genome has an approved first-slice boundary without triggering `UPG-C07` or
  collapsing Species and Biome evidence under `UPG-C08`.
- No general upgrade framework, RuleTile dependency, or speculative UI work was
  added.

## Chrono terrain import runbook

Use this lane as soon as the files arrive. It is independent of the upgrade
balance work.

### Before replacing assets — 10 minutes

Confirm with the delivery:

1. Is it 47 individual 128×128 PNGs per family, or a sheet that needs a mapping?
2. Does `000` mean an isolated filled tile or no overlay?
3. Was the diagonal-only state authored for the project's “promote adjacent
   cardinals” rule?
4. Is a production universal base tile included? If so, what is its filename
   and intended atlas placement?

### Import — 20–30 minutes

- Stage and inspect the delivery outside `Assets/` first.
- Map it to the exact three-digit resolver names.
- Replace image contents in
  `Assets/Art/Terrain/Blob/128/{Grass,Desert}` while preserving the existing
  `.meta` files and GUIDs.
- Do not run `Generate-BlobTerrainTiles.ps1` over Chrono's authored set; that
  tool creates the old procedural placeholder masks.
- Let Unity reimport and run the EditMode suite. The terrain contract test will
  reject missing, extra, misnamed, wrongly sized, or wrongly imported tiles and
  a disconnected atlas root.

### Visual acceptance — 30–45 minutes

- Preview all 47 masks for both families at native scale.
- Inspect at least: isolated, one cardinal, diagonal-only, inside corner,
  outside corner, corridor, full surround, and board edge.
- Run the real Noesis board at 1920×1080 and 1280×720 at normal simulation
  speed.
- Capture one dense Grass region, one sparse/isolated Grass region, and mixed
  boundaries. Chrono and the integration owner both review the result.

### Renderer verification — 15 minutes

- Confirm Bare shows only the universal base.
- Confirm Grass and Desert each draw their own computed mask over that base.
- Confirm unknown or non-family terrain is not silently presented as Desert.
- If a production Desert scenario is added, author its simulation rules
  explicitly rather than inferring them from “anything that is not Grass.”

Terrain is accepted only when the automated contract is green and the targeted
runtime captures show the intended `000`, diagonal, edge, and mixed-boundary
behavior. A clean atlas import by itself is not visual acceptance.

## Local changes from this audit

- Added `TerrainTileAssetContractTests` and its `.meta` file.
- Changed the terrain atlas from bilinear to point filtering.
- Corrected the legacy placeholder generator's missing Grass source filename.
- Updated the phase-decision PlayMode test to expect the authored
  `trailblazer-long-stride` reward now that the scene explicitly selects Hare.
- Recorded the accepted terrain mask-contract and biome-ownership concerns and
  linked them from the canonical smart-tiling plan.
- Did not edit the user's existing dirty files and did not push anything.

Validation: `212/212` EditMode tests passed. PlayMode passed all 21 runnable
tests with one intentional skip. The first EditMode attempt was `209/210`
because Burst briefly failed to load a generated cache DLL; the unchanged rerun
passed `210/210` before the new two terrain tests were added. The first PlayMode
attempt exposed the stale legacy-reward assertion; the authored-reward update
then passed.
