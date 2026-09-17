# Genome map swap debug fixture

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/simulation-window-production
- Baseline commit: 993a8440
- Date: 2026-09-12

## Summary

The GalapagOS desktop test scene now exposes two species-bound Genome maps in
the same `GenomeCatalogProvider`. The Gene Lab debug selector swaps between the
five-node Hare tree and a visually distinct seven-node Fox tree.

## Changes

- Added the seven-node `FoxGenome` ScriptableObject map and its seven dummy
  `GenomeUpgradeAsset` nodes.
- Added a `SelectGenomeSpeciesCommand` to the Gene Lab window.
- Made the selected species mutable in `GenomeLabViewModel`; switching species
  rebinds the current immutable catalog snapshot and rebuilds tiles, branches,
  dimensions, header text, and details data.
- Added a PlayMode source test that switches Fox -> Hare and verifies the
  reflected node IDs, counts, species labels, branch data, and tree height.

## Decisions and assumptions

- The second fixture is a real `fox` species map, not a second map with a
  duplicate `hare` identity. The catalog remains correctly keyed by one map
  per species.
- The selector is explicitly debug-only and exists to prove the data-driven
  binding while profile actions and Genome effects remain deferred.

## Validation

- Focused UI source compile: passed, 0 warnings, 0 errors.
- Focused PlayMode test source compile: passed, 0 errors.
- Fox fixture consistency check: passed; seven nodes, valid target IDs,
  prerequisites, and asset metadata references.
- Gene Lab XAML XML parse: passed.
- `git diff --check`: passed; Git reports only existing LF-to-CRLF notices.
- Unity visual/Test Runner execution remains pending because the open Unity
  editor is not targetable in the current session.

## Deferred

- Production species selection, tile selection, species-specific iconography,
  and Genome effects remain outside this skeleton pass.
