# Dummy Genome visualization fixture

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/simulation-window-production
- Baseline commit: 993a8440
- Date: 2026-09-12

## Summary

The desktop Genome catalog now uses a five-node Hare fixture so the Gene Lab
can visibly exercise authored ordering and prerequisite topology. All nodes
remain metadata-only placeholders; this is visualization smoke data, not
approved gameplay content.

## Changes

- Added four dummy `GenomeUpgradeAsset` fixtures under the existing Hare map:
  `hare.swift-digging`, `hare.keen-hearing`, `hare.safe-foraging`, and
  `hare.warren-network`.
- Kept `hare.guarded-burrow` as the root and connected the new nodes through
  two branches and dependent nodes.
- Updated the Hare map and focused Editor/PlayMode expectations to resolve all
  five nodes.
- Increased the Gene Lab tree viewport height so the authored fixture can be
  displayed together in the current node-list visualization.

## Decisions and assumptions

- The fixture is intentionally bound to `SpeciesId` `hare` through each node's
  `targetSpeciesId` and the map's `speciesId`.
- Descriptions identify movement, perception, resource, and resilience themes
  only; no effect values or costs are implied.
- The current “tree” visualization is still an ordered list with prerequisite
  labels. Spatial branch connectors remain deferred.

## Validation

- Dummy asset identity/reference check passed for all five expected node IDs.
- `git diff --check` passed.
- Unity Test Runner and visual inspection were not available because the Unity
  editor is open and not exposed as a targetable app in the current desktop
  session. The asset and source expectations are ready for the next Unity
  import/play-mode pass.

## Risks and incomplete work

- The actual Unity scene has not yet been visually inspected with the five-node
  map, so clipping and Noesis sizing still need confirmation.
- These placeholders must not be promoted to real Genome balance content until
  effect and cost contracts are approved.

## Next useful step

Close or reload Unity so it imports the four new assets, open the Gene Lab in
`GalapagOSDesktopTest`, and confirm that the five Hare nodes and prerequisite
labels are visible. Then replace the placeholders with approved content or
remove the fixture when the UI smoke test is complete.
