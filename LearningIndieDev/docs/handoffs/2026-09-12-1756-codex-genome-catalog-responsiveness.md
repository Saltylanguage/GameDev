# Genome catalog responsiveness

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/simulation-window-production
- Baseline commit: 993a8440
- Date: 2026-09-12

## Summary

The data-driven Genome skeleton now has an explicit species binding and a
refresh path from authoring assets to the open Gene Lab. The UI still consumes
only immutable catalog snapshots; editing a Genome map or node causes a new
snapshot to be captured and the Gene Lab view-model to rebind when its
fingerprint changes.

## Changes

- `GenomeUpgradeAsset` and `SpeciesGenomeMapAsset` emit authoring-change
  notifications from `OnValidate`.
- `GenomeCatalogProvider` subscribes to assigned maps and their upgrade assets,
  recaptures on changes, and emits `SnapshotChanged` only when the catalog
  fingerprint changes. Invalid catalogs clear the captured snapshot.
- `GenomeLabViewModel` now implements `INotifyPropertyChanged` and can rebind
  its node collection and species metadata to a new catalog snapshot.
- `VM_GalapagOS_Desktop` forwards provider changes to open Gene Lab windows;
  `GalapagOSDesktopWindow` forwards view-model changes to the existing XAML
  bindings.
- Focused coverage now checks upgrade edits, map edits, fingerprint-based
  notification deduplication, and Gene Lab node rebinding.

## Decisions and assumptions

- `SpeciesId` remains the authoritative binding. A node whose target species
  does not match its containing map is rejected by the immutable snapshot
  contract.
- The Gene Lab's configured species remains explicit (`hare` in the desktop
  test scene); a catalog update does not silently switch the UI to another
  species.
- This slice carries identity, display metadata, and prerequisite topology
  only. Effects, costs, economy, profile actions, and simulation rule
  application remain deferred.

## Validation

- Runtime Genome source compile: passed, 0 errors, 7 existing obsolete-species
  warnings.
- Focused UI source compile: passed, 0 errors, 1 expected serialized-field
  warning.
- Focused Editor Genome test source compile: passed, 0 errors, 0 warnings.
- Focused PlayMode test source compile: passed, 0 errors, 2 existing Unity API
  deprecation warnings.
- XML parse of the Gene Lab XAML and `git diff --check`: passed.
- Unity Test Runner and visual verification were not run because the Unity
  editor was already open (PID 4760); generated Unity assemblies may remain
  stale until reimport.

## Risks and incomplete work

- The current tree surface is an ordered authored node list with prerequisite
  labels, not a spatial branch/connector layout.
- `OnValidate` is the editor authoring notification seam; runtime simulation
  state does not observe mutable assets.
- The open Gene Lab refresh path is implemented but still needs Unity import,
  Test Runner, and visual acceptance with the editor closed.

## Next useful step

Close Unity, allow the project to reimport the new assets, run the focused
Genome EditMode/PlayMode tests, and visually confirm that changing the Hare map
or node updates the open Gene Lab. Then review the first real effect catalog and
node-cost semantics before connecting gameplay behavior.
