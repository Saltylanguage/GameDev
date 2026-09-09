# Handoff — Cross-chat simulation and GalapagOS closeout

**Date:** 2026-09-06  
**Owner:** Codex  
**Branch:** `codex/simulation-window-production`  
**Status:** Local closeout batch; ready for commit and push after validation

## Purpose

This note packages the work accumulated across the recent simulation,
GalapagOS desktop, art-direction, and project-hygiene conversations since the
last commit. The source changes are intentionally grouped into separate commits
so the next work block can start from a readable baseline.

## Included work

### GalapagOS desktop and UI library

- Recorded the approved light pastel eco-desktop direction and canonical
  concept art.
- Added the reusable button, metric-row, window, window-drag, and desktop-icon
  control surfaces, including icon support and desktop drag/snap behavior.
- Expanded the GalapagOS desktop test scene with the vertical icon layout,
  start bar, lab panel, app shell, shared resources, window variants, and
  extracted icon assets.
- Added the first desktop application feature map and durable art/design notes.

### Simulation composition and launch flow

- Added a dedicated Simulation camera and Noesis view to
  `GalapagOSDesktopTest.unity` instead of attempting to swap the mounted
  desktop view's XAML at runtime.
- Connected the real `V_Panel_SimulationShell` and `SpeciesSimulationBoard`
  to their shell and board ViewModels, including sprite and snapshot wiring.
- Fixed the launch path so Simulation cannot fall through to the generic
  desktop-app placeholder.
- Restored the desktop ViewModel DataContext assignment during host startup;
  this was required for the startup visibility binding and desktop icon
  commands to function.
- Added safe rollback when the simulation XAML or board control cannot load.

### Simulation/runtime cleanup and continuation work

- Moved the active simulation surface to the Noesis/XAML path and removed the
  deprecated runtime IMGUI HUD/debug components and orphan Life preview.
- Updated the continuous expedition flow, phase-aware telemetry, checkpoint
  and schedule integration, and the related Play Mode coverage.
- Reconciled the ten-phase player contract, Stat-Line semantics, and EX-010
  sequential-continuation draft with the recorded Josh/Sim decision.

### Documentation and project hygiene

- Updated the roadmap, project context, working state, engineering standards,
  simulation evidence notes, Main Menu/Lab delivery plan, and Loose Ends ledger.
- Added focused handoffs for the desktop controls, panel/board surface,
  start-menu behavior, Simulation composition, and the cross-chat closeout.

## Verification

- `Test-StudioPolicy.ps1`: passed; alert-only policy is valid.
- XML parsing: all seven GalapagOS/HUD XAML files parsed successfully.
- `git diff --check`: passed; only normal Git LF/CRLF conversion warnings were
  reported.
- Fresh Unity runtime route check: the desktop command initialized, the
  desktop camera disabled, the Simulation camera enabled, and the board
  ViewModel supplied a non-null snapshot.
- Full `Invoke-UnityTests.ps1 -Mode All`: blocked by the repository preflight
  because the Unity editor was already running (PID `16440`). Existing retained
  evidence remains Unity Edit Mode `210/210` and Play Mode `17/18` with one
  intentional graphics-only skip. A new full-batch Game-view screenshot has
  not been captured.

## Open follow-ups for the next block

- Run graphics-capable desktop and Simulation acceptance with Unity closed,
  including the target resolutions and the actual Game view.
- Finish the remaining GalapagOS app screens, starting with the Settings
  vertical slice, then continue through the planned app sequence.
- Finalize and approve the EX-010 schedule before running its experiment;
  generic schedule smoke coverage is not EX-010 evidence.
- Keep the existing research, worker-tooling, balance, and analytics/privacy
  Loose Ends open until their recorded gates are satisfied.

## Handoff expectation

The pushed branch should be treated as a feature baseline, not as a claim that
graphics acceptance or EX-010 research execution is complete. The next task can
begin from the GalapagOS Settings slice after the normal Unity visual gate is
run.
