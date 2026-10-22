# gene-lab-node-selection-poc

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-20-2032-codex-gene-lab-node-selection-poc
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6ffb7b10
- Date: 2026-10-20
- Supersedes: none

## Summary

Completed item 3 of the Rabbit Genome prerequisite sequence as Josh's approved
proof of concept. Gene Lab now supports real selection/allocation across a
small explicit test catalog, with save/load and frozen next-journey effects.

## Changes

- `GenomeLabViewModel` now owns selected-node and branch-filter window state.
  The initial view chooses the first authored branch (Gardener). Selection
  survives wallet/profile refreshes and resets to a valid node when catalogs
  or branch filters change.
- Node buttons bind their own selection command and notify the selected visual
  state. The XAML shows selected/active/available/locked/preview states and
  renders occupied capacity points. Specific blockers identify prerequisites
  by display name, missing Rabbit Data, full capacity, license/gate status,
  already-active nodes and unsupported previews.
- Existing profile helpers remain authoritative for purchases, receipts,
  eight-point capacity, refund/reset and persistence. Non-Rabbit allocation
  actions are disabled. No new persistence schema or modifier operation.
- New Unity-created assets under
  `Assets/Data/TestData/Genome/RabbitSelectionPOC/` provide five working stat
  nodes and a non-purchasable Burrows preview. The test map also references
  the existing approved Fertile Droppings asset. The Desktop test scene now
  explicitly uses this POC map; the production Hare map is unchanged.
- Five stat offers: Long Stride, Long Vision, Tough Hide, Larger Litters and
  Efficient Digestion. Each costs 1 Rabbit Data and 2 capacity points for the
  POC. Values and topology are documented in the
  [catalog record](../Research/2026-10-20-Gene-Lab-selection-POC.md).
- Added a PlayMode test that finds actual Noesis node buttons and the branch
  ComboBox, executes their bound selection commands, checks selected styling,
  purchases, prerequisite/Data/capacity blockers, blocked-purchase wallet
  safety, reset/refund, session reload, full scene reconstruction, active
  next-journey stats and frozen in-progress allocation behavior.
- Added an authoring gate proving every purchasable POC node has an executable
  effect, while the unfinished ability preview cannot execute or charge for an
  offer. Updated the older one-node Desktop test expectations.

## Decisions and assumptions

- Josh explicitly approved using unfinished Genome content to test the feature.
  These assets are POC offers, not production balance approvals or the full
  proposed 25-node tree. The production catalog stays separate.
- Existing tree license/reset fee, earned-currency flow and three-success
  Gene Lab gate remain in effect. Tests temporarily seed their own profiles;
  the feature grants no developer currency to player saves.
- Unsupported abilities remain inspectable but non-purchasable. Burrows does
  not pretend to implement safe mating or predator hiding.
- Existing Noesis resources/images are reused through semantic keys. No new
  UI framework, global state, runtime discovery or image pipeline.
- UI selection/filter are window state, while active allocations are saved
  profile state. The reconstructed scene may select its first node while
  retaining the complete saved build.

## Validation

- Elevated host environment diagnosis passed. Unity was closed when work
  resumed after usage recovery, so verification used the project's clean CLI
  lane with Unity 6000.4.6f1 and normal licensing identity access. The optional
  Unity MCP tool was no longer available. CLI version remains 1.0.0-beta.6;
  `self-update --check` is unsupported by that installed version. No upgrade
  or Editor-process termination was performed.
- Final clean graphics Lab PlayMode regression: **12/12 passed**, artifact
  `artifacts/visual-evidence-20261020-203657/PlayMode-results.xml`.
- Clean 720p focused selection/capture test: **1/1 passed**, artifact
  `artifacts/visual-evidence-20261020-203559/PlayMode-results.xml`. It includes
  the final label sizes; the later default-branch change is covered by the
  final 1080p full regression.
- Clean focused Genome authoring/contract checks: **24/24 passed**, artifact
  `artifacts/unity-tests-20261020-203758/EditMode-results.xml`.
- Inspected captured locked, prerequisite, unavailable-preview, full-capacity
  and active-build states at 1280×720 and 1920×1080. Images are retained under
  `artifacts/gene-selection-poc/1280x720/` and `1920x1080/`. Fixed clipped
  two-line node badges, increased blocker/prerequisite readability, and sized
  purchase labels to fit. The final full-run log has no matching Noesis
  resource or binding error diagnostics. This is agent visual inspection,
  not human usability/balance acceptance.
- The clean Editor clears Temp on startup, which removed the wrapper's first
  visual request file. That first focused run passed but captured no images.
  Subsequent runs passed `CELLSIM_VISUAL_*` through the child-process
  environment (the supported configuration priority); captures therefore live
  in the explicit folder above rather than the wrapper's timestamped folder.
- Corrected initial test timing after Noesis rebuilt item containers, and an
  older assertion expecting the former single-node catalog. The failed and
  passing clean reports remain retained. An automatic approval review failed
  on the earlier recompile because account usage was exhausted; verification
  resumed after Josh requested continuation, through reviewed host commands.
- Scoped `git diff --check` passed. Preserve unrelated pre-existing art/meta,
  simulation, project settings and audio work. No commit, push, branch switch
  or Trello update was performed.

## Risks and incomplete work

- The all-branches overview is dense at 720p; branch filtering is the readable
  working view. This POC does not redesign the full 25-node graph layout.
- POC IDs persist in profiles. Reset allocations or implement a migration
  before removing this catalog or replacing its IDs. Do not swap the scene
  back to the production-only map while those allocations remain active.
- Production offers/prices/unlock conditions, new ability mechanics, other
  species' allocation economy, and arbitrary CellSim build selection remain
  separate work. No ecological balance conclusion follows from these tests.

## Next useful step

Human playtest the selection flow in Desktop → Gene Lab. Next technical work
can make arbitrary legal Genome builds reproducible in balance tooling while
the production content/economy and missing abilities are designed separately.
