# Genome fixture and document reconciliation

[Working state](../WORKING_STATE.md) | Status: needs Unity verification; EX-011 decision recorded

- Owner: Codex
- Branch: ProjectMain
- Baseline commit: `cab5838d`
- Date: 2026-09-17

## Summary

Reconciled the Genome documentation with the implemented identity/profile/
snapshot/catalog foundation without claiming that Genome effects or progression
are complete. Removed the visualization-only Hare and Fox Genome maps from the
canonical player scene and retained the simulation setup that came from the
Bev/Sim branch.

The current Desktop acceptance contract is direct-start Forest Edge/Hare.
The recorded PlayMode failures have not been verified individually, so their
causes must be established from retained results or a fresh run before changing
fixtures or runtime behavior. The Desktop route still needs a real profile and
`SimulationLaunchRequest` owner.

## Changes

- Updated the GDD, TDD, roadmap, and working state with the actual Genome
  implementation boundary.
- Removed the five-node Hare and seven-node Fox dummy assets, their scene
  provider, and the debug map selector.
- Kept the generic Genome authoring types, immutable catalog, local profile
  state, launch/run/checkpoint/result snapshots, and Gene Lab bindings.
- Updated focused Editor and PlayMode tests to use in-memory catalog data and
  expect an empty production catalog in the player scene.
- Reduced EX-011 to one bounded Accept/Reject/Revise question in its human
  decision record.
- Recorded the direct-start Forest Edge/Hare test-contract decision in the GDD,
  TDD, roadmap, and working state.

## Preserved simulation behavior

- `GalapagOSDesktopTest` still references the same seven authored Mutation
  assets as the pre-Genome Bev/Sim baseline.
- `bev-experimental` behavior remains enabled in the preview.
- Genome snapshots remain metadata/provenance only and do not change simulation
  rules.

## Validation

- `git diff --check`: passed.
- Gene Lab XAML XML parse: passed.
- Executable asset/test search found no remaining dummy Genome IDs, map GUIDs,
  debug selector, or scene provider references.
- The player scene's seven-item Mutation catalog matches the pre-fixture
  Bev/Sim catalog.
- Unity PlayMode rerun: not executed. The tooling stopped because Unity was
  already open at PID 3448.
- Generated-project `dotnet build`: not evidence. It stopped before compilation
  because Unity's temporary NuGet assets file was absent.

## Remaining decisions and checks

1. EX-011 was accepted narrowly by Josh on 2026-09-17; see the decision record.
2. Close Unity and run focused Genome/UI coverage plus the full PlayMode suite.
3. Triage individual PlayMode failures against the recorded direct-start
   Forest Edge/Hare contract and update only assertions that are confirmed stale.
4. Do not author production Genome nodes until one effect, cost, player action,
   persistence boundary, and evidence plan are approved.

## Concurrent work note

A separate terrain-art work block was present in the shared working tree. Its
files and documentation were preserved and were not included in this task's
fixture cleanup.
