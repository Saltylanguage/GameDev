# Fertile Droppings Gene Lab playtest offer

[Working state](../WORKING_STATE.md) | Status: implemented locally; journey payout integrated, verification pending

- Handoff schema: 1
- Handoff ID: 2026-10-17-1719-codex-fertile-droppings-gene-lab-offer
- Owner: codex
- Branch: ProjectMain
- Baseline commit: 6ffb7b10
- Date: 2026-10-17
- Supersedes: [scent first slice](2026-10-17-1545-codex-fertile-droppings-scent-first-slice.md) for Gene Lab offer status

## Summary

The first permanent Gardener node is available in the production Hare catalog and Gene Lab. A profile can buy Rabbit Genome access, activate Fertile Droppings, save/reload it, and reset it with the approved provisional prices. Desktop copies the latest profile Genome into a ready journey at open; an active journey retains its frozen snapshot.

## Changes

- `Assets/Data/Genome/HareGenome.asset` and `HareGenome/FertileDroppings.asset` author the Gardener root (`fertile-droppings`) at 1 Rabbit Data and 1 of 8 capacity points. `GalapagOSDesktopTest.unity` binds a `GenomeCatalogProvider` to the production map. The older TestData maps are untouched.
- The catalog snapshot carries branch, price, and capacity metadata. The profile purchase helper rejects a requested price/capacity that differs from the authored node. Tree access and reset use 1 Rabbit Data and 1 Research Data playtest constants.
- Gene Lab shows the loaded profile's Rabbit Data, Research Data, completed runs, node status, and active capacity. Its tree, node, and reset commands call the existing profile transactions. Disabled states explain the three-run gate or profile requirement. The Noesis panel fits its action message at 1280×720 and 1920×1080. The authored catalog hash and placeholder balances are no longer player copy.
- The Desktop host refreshes a ready simulation from `Current.CreateGenomeSnapshot()`
  immediately before opening it. A started run is not rewritten. The Gene Lab
  view model refreshes when the profile snapshot changes.
- Profile launches now subscribe to the simulation completion event. Eligible
  completed journeys record one profile completion and grant the approved 1
  Rabbit Data plus 1 Research Data in a single saved profile update. Each
  prepared run receives a stable completion ID so duplicate handlers cannot
  issue a second payout.

## Decisions and assumptions

- Josh approved the Gardener root, 1 capacity point, 1 Rabbit Data node purchase, 1 Rabbit Data tree access, and 1 Research Data reset fee as provisional playtest terms on October 17. The node effect remains the provisional +0.10 seed-drop chance and agreed scent values from the earlier handoff.
- Josh approved a provisional journey payout of 1 Rabbit Data and 1 Research Data per successful profile journey on October 17. Failed and early-ended journeys receive no payout or completed-run credit.
- The current catalog contains one player-selectable node. When additional nodes are authored, add explicit node selection in Gene Lab before treating a multi-node tree as playable.

## Validation

- Production asset import test: 1/1 passed at `artifacts/unity-tests-20261017-170508/EditMode-results.xml`.
- Gene Lab PlayMode purchase, reload, reset, and frozen-next-journey test: 1/1 passed at `artifacts/unity-tests-20261017-170534/PlayMode-results.xml`.
- Full clean Unity gate: EditMode **349/349** passed, PlayMode **47 passed, 4 skipped, 0 failed** at `artifacts/unity-tests-20261017-171353/`. An earlier full run found an existing Desktop test assumed capacity 0 even when another test's saved profile was active; its assertion now compares with the loaded profile.
- Existing Desktop multi-window test after final catalog-copy change: 1/1 passed at `artifacts/unity-tests-20261017-171938/PlayMode-results.xml`.
- Rendered locked and active Gene Lab captures were visually inspected at 1280×720 and 1920×1080. Final UI capture paths: `artifacts/gene-lab-visual-final-1280/` and `artifacts/gene-lab-visual-final-1920/`. The visual runner's request-file values were not seen by the clean Editor, so width, height, and output were passed as environment variables for these captures.
- The successful-journey payout connection was added after the test and visual runs above; it has not yet been run through Unity or manually exercised.
- Compile-only check was attempted with
  `dotnet build Assembly-CSharp.csproj --no-restore`; the Unity-generated
  `Temp/obj/Assembly-CSharp/project.assets.json` was missing. A regular
  `dotnet build Assembly-CSharp.csproj` then exited 1 during restore without a
  diagnostic. No Unity compile or tests were run after the payout change.

## Risks and incomplete work

- The successful payout path is wired for profile launches, but runtime verification is still open. Exercise one successful journey and confirm its Rabbit Data, Research Data, and completion count each increase by one; then exercise a failed or early-ended journey and confirm none increase. Run three successful journeys end-to-end to verify the real Gene Lab unlock path without test grants.
- Gene Lab is bound to one production Hare node. Further Gardener, Warren, and Trailblazer nodes still need exact behavior, authored offers, and a clickable node-selection surface.
- These are UI and transaction tests plus rendered captures. No human play acceptance or larger balance panel was performed in this pass.

## Next useful step

Verify the successful and failed journey settlement paths in the Desktop profile flow, then play through the first three successful journeys to verify the Gene Lab purchase path without test grants. Compare a larger matched-seed panel before changing the provisional +0.10 seed-drop bonus.
