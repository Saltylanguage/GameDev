# FiveSpeciesSandbox

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-19-1343-codex-fivespeciessandbox
- Owner: Codex
- Branch: codex/rabbit-identity-rename
- Baseline commit: d82e4fec
- Date: 2026-11-19
- Supersedes: none

## Summary

The confirmed five-species roster can now be selected and launched through the
GalapagOS Desktop Expedition Planner. The change adds production Scriptable
Object definitions for Deer, Owl, and Wolf, a five-species test ecology, and
test-only Mutation and Genome injections for every species. The default
Forest Edge/Rabbit route remains the starting choice and its existing scenario
data was preserved.

## Changes

- Added production Deer and Wolf definitions from the existing test baselines
  and a provisional Owl definition under
  `Assets/Data/ProductionData/CellularSimulation/Species/`.
- Added `FiveSpeciesSandbox` under `Assets/Data/TestData/`. It includes all five
  roster species plus Plant and Fern resources, with deterministic starting
  populations and a 600-tick run.
- Added five test Mutations (+0.25 movement speed) and five test Genome nodes
  (+1 vision range). They are loaded by the Desktop scene and the Genome
  catalog; the planner injects them only into the selected species for the
  launched run.
- Replaced the static planner card with three screens: scenario selection,
  scenario-filtered species selection, and optional test injections.
- Added Owl/Deer/Wolf selection art keys. Owl currently uses the existing Eagle
  sprite as placeholder art; the simulation board maps Owl to that sprite too.
- Kept the profile-backed Forest Edge/Rabbit default and journey route intact.

## Decisions and assumptions

- All added values are provisional and intended for balancing. The Genome and
  Mutation assets remain in TestData and do not create profile unlocks or
  purchases.
- Forest Edge offers Rabbit and Fox. The Five Species Sandbox offers Rabbit,
  Fox, Owl, Deer, and Wolf. Other existing test scenarios contain species
  outside the approved roster, so the planner does not offer them.
- The scenario starts Plant 110, Fern 90, Rabbit 14, Fox 3, Owl 2, Deer 10,
  and Wolf 2 on a 40x24 grid. Rabbit starts below its current 15-animal cap.

## Validation

- `git diff --check` completed without whitespace errors; it reported existing
  LF-to-CRLF notices on other changed files in the working tree.
- PowerShell XML parsing succeeded for the Desktop XAML and image resource
  dictionary.
- Static GUID/reference inspection found no missing references in the new
  scenario, species, Genome maps, and test upgrade assets.
- No test suites were added or run.
- Unity import, C# compilation, and visual review are unverified. `unity open .`
  returned without starting an Editor process; `unity status` found no Pipeline
  Editor, and `unity doctor` reported the CLI is unsigned and cannot read the
  Unity Hub account database at `C:\Users\joshc\AppData\Roaming\UnityHub\accounts.db`.

## Risks and incomplete work

- The local Unity Editor could not be launched, so serialized asset import,
  script compilation, and player-facing planner behavior need a Unity-side
  review after the CLI/Hub account state is available.
- The numeric baselines are scaffolding only. Owl also needs species-specific
  art to replace the Eagle placeholder.
- The working tree includes earlier Rabbit identity and balance edits. They
  were preserved and this handoff does not claim they are committed or shared.

## Next useful step

Open the project in a working Unity Editor, confirm import/compilation, then
exercise Forest Edge/Rabbit and each Five Species Sandbox player selection,
with both test injections independently enabled, before tuning the baseline
values or replacing Owl's placeholder art.
