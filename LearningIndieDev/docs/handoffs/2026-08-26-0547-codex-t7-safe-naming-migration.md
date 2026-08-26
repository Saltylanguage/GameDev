# T7 safe naming migration

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/figma-noesis-pilot
- Baseline commit: c916f124
- Date: 2026-08-26

## Summary

T7 applies the project naming convention to the simulation shell after the
replacement composition path was already working. The change removes the
remaining legacy ViewModel/view names without changing the simulation boundary.

## Changes

- Renamed `SpeciesSimulationViewModel` to `VM_SimulationShell`.
- Renamed `SpeciesSimulationShell.xaml` to `V_Panel_SimulationShell.xaml`.
- Preserved both Unity asset GUIDs and updated the host, scene identifier, and
  focused presentation tests.
- Updated durable architecture, UI contract, project context, and relevant
  feature documentation.
- Left `SpeciesSimulationPreview` unchanged because it is a shared preview
  component, not a ViewModel, view, or helper.

## Decisions and assumptions

- `SpeciesSimulationNoesisHost` remains a composition host; host names are not
  part of the `VM_*`, `V_Panel_*`, or `Helper_*` convention.
- Compatibility-oriented preview/editor/test references are not removed
  speculatively; they remain valid under the existing `SpeciesSimulationPreview`
  type.

## Validation

- Unity refresh/compile command succeeded with no compilation logs.
- Unity scene validation found one host, one `VM_SimulationShell`, one
  `VM_SimulationBoard`, and the renamed XAML asset; the scene was not dirty.
- Serialized host references resolved to the renamed ViewModel and XAML asset.
- Simulation shell, Main Menu, and Lab XAML files parsed as XML.
- Unity Console contained zero errors, exceptions, or asserts after refresh.

## Risks and incomplete work

- The normal Editor Test Runner completion callback remains unavailable through
  the current MCP surface, so this note does not claim a completed Play Mode
  suite.
- Unrelated Figma metadata and solution-file edits were present before this
  work and remain outside the T7 scope.

## Next useful step

Review the rename as a focused commit, then run the normal Unity Play Mode
acceptance window before considering the broader T7 exit gate closed.
