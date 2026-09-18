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

Run project checks from `LearningIndieDev` through `CellSim.ps1`. The wrappers
retain logs, result XML, reports, manifests, and screenshots under ignored
`artifacts/` directories.

`-Execution Auto` is the normal developer default:

- `Live` uses this project's Pipeline connection when its Editor reports
  `ready`. This avoids a second Editor launch and is the fastest feedback loop.
- `Clean` starts a CLI-managed batch Editor when this project is closed. Use it
  for full-suite acceptance and reproducible retained evidence.
- `Auto` selects between those lanes. It stops with the observed project state
  when the Editor is busy, in Safe Mode, or locked but unreachable; it does not
  delete locks or terminate unrelated Unity processes.

Run a quick environment diagnosis without starting a full Editor:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Doctor
```

Run both suites in the selected lane:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test -Mode All
```

Use `-Execution Clean` for a release or handoff gate. For fast local diagnosis,
use `-Mode EditMode` or `-Mode PlayMode` and filter by test name, assembly, or
category:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test `
    -Mode EditMode -FilterType Category -TestFilter Core
```

Current categories are `Core`, `Simulation`, `Graphics`, `UI`, `Authoring`, and
`Tooling`. `All` attempts EditMode and PlayMode and aggregates their results, so
one assertion failure does not hide the other suite. Live PlayMode runs are
asynchronous because entering Play Mode may reload the domain; the wrapper
reconnects and polls the same test run. After a failure, inspect its XML/log and
the controlling code before deciding whether another run would add evidence.

Seeded experiments use the same routing:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Run `
    -Execution Auto -SeedStart 10100 -SeedCount 20 -RunTicks 600 `
    -ScenarioPath Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset `
    -PlayerSpeciesId hare
```

The live lane calls the project-owned `cellsim_run` Pipeline command. The clean
lane uses the established Editor batch method through `unity run`. Both execute
the same experiment implementation and produce the same report bundle.

For graphics acceptance, use the project visual runner with a focused test. Its
default output is 1280x720. `Auto` can reuse a ready graphics-capable Editor or
start a clean graphics-capable run when the project is closed:

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

The package enables Unity CLI commands against this project's running Editor.
Confirm the connection with `unity status --project-path <path>`. When invoking
`unity command` directly, put `--caller plugin --skill <skill-name>` before the
command name and always supply the project path. The installed beta advertises
`--no-pager` but rejects it, so project wrappers do not use that flag.

Codex MCP is configured through the Unity CLI and pinned to this project:

```powershell
unity mcp configure codex --project-path "D:\GameDev\GameDev\LearningIndieDev"
```

This replaces the older `unity_mcp` user-relay configuration with a single
`unity` server that launches `unity mcp --project-path ...`. Restart Codex or
start a new task after changing MCP configuration so the new server is loaded.
See [`UNITY_MCP_RELAY_OPERATIONS.md`](UNITY_MCP_RELAY_OPERATIONS.md).

## Current verification record

On 2026-09-18, the new lanes were exercised end to end:

- `Doctor` completed with healthy CLI and license diagnostics.
- Clean EditMode passed 233/233.
- Live EditMode category `Core` passed 26/26.
- Live experiments and a clean 600 tick experiment both produced complete
  report bundles.
- A live graphics acceptance test passed 1/1 and its 1280x720 screenshot was
  visually reviewed.
- Live PlayMode category `UI` ran through domain reload and polling; 10/11
  passed. Its `ProfileCreationEnablesContinueAndLoadsDesktop` failure did not
  reproduce in the clean full PlayMode suite and is recorded as a warm Editor
  state/test-isolation issue.
- The final full clean command passed EditMode 234/234 and PlayMode with 28
  passes, two expected graphics skips, and zero failures.

The current full-suite result and artifact links belong in
[`WORKING_STATE.md`](WORKING_STATE.md) and the latest handoff. A focused or
partial pass is never a green full-suite claim.
