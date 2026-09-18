# Unity plugin workflows

Use this guide with [`AGENTS.md`](../AGENTS.md),
[`PROJECT_CONTEXT.md`](PROJECT_CONTEXT.md),
[`WORKING_STATE.md`](WORKING_STATE.md), and
[`UNITY_ENGINEERING_STANDARDS.md`](UNITY_ENGINEERING_STANDARDS.md). Those
project documents decide intent and constraints; Unity plugin skills supply
specialist Unity procedures. Ponytail selects the smallest option that meets
those constraints. Correctness-First Engineering applies when evidence is
uncertain, a check fails, or a change has meaningful regression risk.

## Skill routing

| Request area | Unity skill to load |
| --- | --- |
| Unity CLI, builds, tests, logs, or live Editor control | `unity-cli` |
| Read-only search for Editor assets or scene objects | `generate-editor-search-query` |
| Add, remove, upgrade, or inspect UPM packages | `unity-package-management` |
| Unity UI Toolkit, uGUI, or IMGUI implementation | `ui-uitk`, `ui-ugui`, or `ui-imgui`; use `ui` for general Unity UI questions |
| Pixel-art rendering, sprite slicing/metadata, atlases, or tile authoring | `2d-pixel-perfect`, `sprite-editor`, `manage-sprite-atlas`, `sprite-segment-3x3grid`, `tilemap-palette-create`, `tilemap-ruletile-createempty`, or `tilemap-ruletile-createfromsegment`, matching the request |
| Render pipeline, post-processing, Shader Graph, TMP, or Web build | `migrate-birp-to-urp`, `urp-postprocessing`, `validate-urp-render-graph-renderer-feature`, `shader-graph-create-custom-node`, `optimize-text-mesh-pro`, or `optimize-web` |
| Physics, navigation, audio, or localization | `physics-3d-collision`, `initialize-ai-navigation`, `audio-setup-mixers`, `optimize-audio`, or `localization` |
| Multiplayer, voice, IAP, ads, or Unity backend services | `setup-multiplayer-services`, `setup-vivox-voice-chat`, `implement-in-app-purchases`, `levelplay-unity-integration`, or `build-live-game` |
| A genuinely new Unity project | `new-unity-project` |

Load only the skill that matches the work. Plugin availability is guidance; it
does not authorize a product feature, dependency, account, or external service.

## Project constraints

- Noesis/XAML remains the player UI. Use the Unity UI skills only when a task
  specifically targets UITK, uGUI, or IMGUI; do not use them to migrate the
  player UI.
- The live cellular board is a batched Noesis renderer using
  `TerrainTileResolver` and the authored `Terrain_01.spriteatlasv2`. It is not a
  Unity Tilemap. Do not apply RuleTile workflows or add Tilemap packages to the
  board. See [`CELLULAR_SPRITE_TILING_PLAN.md`](CELLULAR_SPRITE_TILING_PLAN.md).
- Preserve `.meta` files, GUIDs, importer identity, and serialized references.
  Use `sprite-editor` for sprite metadata work; never hand-edit `.meta` files.
  Keep the authored V2 atlas and stable sprite names unless a concrete request
  changes that contract. Follow [`TILE_AUTHORING_GUIDE.md`](TILE_AUTHORING_GUIDE.md)
  for new terrain art.
- Audio skills are conditional. The current project has no authored AudioMixer
  or audio clip assets; the Main Menu chime is generated in code. Do not create
  a mixer, groups, or audio assets just to exercise a skill. Use mixer routing
  only when project-owned AudioSources and existing groups are in scope; use
  audio optimization only after measuring an actual device/build issue.
- Treat rendering migrations, multiplayer, payments, advertising, localization,
  and backend services as task-specific work, not automatic follow-ups. Confirm
  the project requirement and current standards before adding a package or
  service.

## Verification workflow

Run project checks from `LearningIndieDev` with Unity closed. The project
wrappers check preconditions, retain logs and result XML under ignored
`artifacts/`, and should be preferred to an ad hoc CLI invocation:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test -Mode All
```

For focused diagnosis, use `-Mode EditMode` or `-Mode PlayMode`. Inspect the
result XML and matching log. If `All` stops on a failing EditMode suite, run
PlayMode separately only to gather that independent evidence; the full suite is
still failing until both pass together. After a failure, inspect the controlling
test/code and report the failure; do not blindly repeat the same run.

For graphics acceptance, use the project visual runner with a focused test. Its
default output is 1280x720:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Visuals -TestFilter 'SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.GalapagOSDesktopAndSimulationCaptureGameViewEvidence'
```

To verify another resolution, call the underlying runner directly. The current
acceptance targets are 1280x720 and 1920x1080:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Invoke-UnityVisualEvidence.ps1 -TestFilter 'SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.GalapagOSDesktopAndSimulationCaptureGameViewEvidence' -ScreenWidth 1920 -ScreenHeight 1080
```

Review the generated screenshots as well as the test result. A `dotnet build`
can supplement Unity checks, but it does not establish scene import, asset
loading, PlayMode behavior, or graphics acceptance. Follow
[`UNITY_SIMULATION_TOOLING.md`](UNITY_SIMULATION_TOOLING.md) for seeded
simulation runs and report handling.

## Unity CLI boundary

The installed Unity CLI was verified as `1.0.0-beta.6` on 2026-09-18, and this
project now includes `com.unity.pipeline` version `0.7.0-exp.1` from the Unity
registry. It is recorded in `Packages/manifest.json` and
`Packages/packages-lock.json`. For another project that has explicitly adopted
the integration, the dedicated installer is:

```powershell
unity --no-banner --non-interactive pipeline install --project-path "<project-path>"
```

The package enables Unity CLI live-Editor commands when the target Editor is
running. Confirm the connection with `unity status`; when multiple Editors may
be open, pass `--project-path` to target the intended project. Include
`--caller plugin --skill <skill-name>` on every `unity command` invocation.
The 2026-09-18 smoke check opened this project with `unity open`, confirmed
`state: ready` with `unity status`, and listed 151 live Editor commands. The
check verified the Pipeline connection without changing a scene or asset. The
Editor was then asked to exit through Pipeline `eval`; the CLI reported
`COMMAND_FAILED` as Unity shut down before it could return a valid response.
The process exited, and a fresh status check reported no connected Editors.
Keep project tests on the guarded wrapper with the Editor closed.

## Current verification record

The 2026-09-18 workflow check exercised the project wrappers before and after
installing the Pipeline package, then verified a live Editor connection. The
package version, test results, artifact paths, and non-blocking EditMode
failure are recorded in the package handoff; live connection evidence is in
the follow-up smoke-test handoff linked from
[`WORKING_STATE.md`](WORKING_STATE.md). Do not promote a partial pass to a
green full-suite claim.
