# Live Plant population display

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-08-2242-codex-live-plant-population-display
- Owner: codex
- Branch: BevBranch
- Baseline commit: 87a456b0
- Date: 2026-10-08
- Supersedes: none

## Summary

Bevin could not report Plant totals because the live HUD exposed only animal
populations. Added `PLANTS: <current total>` beside the Population heading in
the Field Ledger. It uses the latest authoritative population snapshot and
remains visible with the phase-decision overlay open. Local and uncommitted.

## Changes

- `VM_SimulationShell.PlantPopulationText` projects the Plant-role population
  through the existing dashboard refresh and property-change notification path.
- `V_Window_CellSimulation.xaml` binds that text in the existing Population
  heading row. No extra row, simulation rules, defaults or scene changes.
- The D5 visual-review guide now points to the live Plant-total label.

## Decisions and assumptions

- The label beneath the collectible jars is separate fixed display text; it is
  not the ecological Plant total. The new label belongs in the Population area.
- Bevin stopped Play Mode and explicitly authorized applying the staged fix.
  Earlier partial-run observations were preserved under ignored
  `artifacts/plant-population-display/current-run-observation.json`.
- Keep this display fix distinct from the prior inspection fixture. No tuning
  or new balance decision is introduced.

## Validation

- Unity recompile completed: failed=false, errors empty.
- Seed 60000 D5 live preview: initial snapshot and VM label 325 Plants;
  first decision at tick 100, actual and bound Noesis label 316 Plants;
  endpoint at tick 476, actual/VM/bound Noesis label 86 Plants and control visible.
- [Endpoint assertions and record](../../artifacts/plant-population-display/endpoint-verification.json).
- [Ready screenshot](../../artifacts/plant-population-display/ready.png) and
  [decision screenshot](../../artifacts/plant-population-display/decision.png)
  visually inspected at 1067x600; Plant label is readable and stays visible
  to the right of the Mutation/purchase dialog.
- [Running screenshot](../../artifacts/plant-population-display/running.png)
  shows a live count of 354 at tick 84. This is a running capture, not endpoint
  evidence; the endpoint assertion record above supplies terminal evidence.
- The unfocused Editor initially supplied a stale render tree during camera
  captures. An explicit Noesis ExternalUpdate refreshed the capture; its
  previous EnableExternalUpdate setting was restored immediately afterward.
- `git diff --check` passed. Final Editor readback: Play Mode stopped,
  `ForestEdge_D5_VisualReview` scene open.

## Risks and incomplete work

- No full test suite or standalone build run; validation is scoped to this
  population projection, binding and rendered label.
- Human ecology/readability review and tick-700 viability remain open.
- Raw evidence is ignored local output. Source/docs remain uncommitted/unpushed.

## Next useful step

Bevin can resume the visual review and read Plant totals from the Population
heading. Use Reset D5 Review to Seed before each controlled comparison run.
