# Genome tree tile visual pass

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/simulation-window-production
- Baseline commit: 993a8440
- Date: 2026-09-12

## Summary

The Genome Lab now presents authored Genome nodes as square upgrade tiles in a
deterministic tree layout. Parent-child branch segments are generated from the
immutable prerequisite graph, so the visual structure follows the supplied
Genome map rather than a hardcoded screen arrangement.

## Changes

- Added position data to `GenomeLabNodeViewModel` and a tree-segment projection
  to `GenomeLabViewModel`.
- Nodes are arranged by prerequisite depth and authored order; connector
  segments are rendered behind the nodes on a Canvas.
- Replaced the wide list-entry cards with 156×112 square cards containing the
  shared Genome icon, display label, and current state badge.
- Forwarded tree collections and dynamic dimensions through
  `GalapagOSDesktopWindow` for Noesis binding.
- Updated the Gene Lab PlayMode expectations to verify tree data is present.

## Decisions and assumptions

- The UI consumes the immutable `SpeciesGenomeMapSnapshot`; XAML does not
  evaluate prerequisites or own tree layout decisions.
- Multiple prerequisites are represented as separate parent-to-child branch
  segments. The current layout is intentionally simple and deterministic; line
  crossing avoidance, pan/zoom, selection, and richer icons are later polish.
- Cards show only icon, label, and state. Exact effect details remain the
  responsibility of the deferred details-panel/effect-contract pass.

## Validation

- Focused UI source compile: passed, 0 errors, 1 existing serialized-field
  warning.
- Focused PlayMode test source compile: passed, 0 errors, 2 existing Unity API
  deprecation warnings.
- Gene Lab XAML XML parse: passed.
- `git diff --check`: passed.
- Unity Test Runner and visual screenshot inspection remain pending because the
  Unity editor is open and not targetable in the current session.

## Risks and incomplete work

- The layout has not yet been visually confirmed in Noesis with the five-node
  fixture, so scaling and connector appearance still need an editor pass.
- The current selected-node behavior still defaults to the first node; tile
  selection and a complete details panel are deferred.

## Next useful step

Open `GalapagOSDesktopTest` after Unity reimports the changed XAML and scripts,
inspect the five-node Hare tree, and tune spacing/colors only if the actual
rendered result needs it.
