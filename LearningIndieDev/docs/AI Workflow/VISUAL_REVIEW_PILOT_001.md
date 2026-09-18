# Visual review pilot 001 — GalapagOS desktop home

**Status:** Test and contract prepared; fresh captures and Josh's review are pending  
**Owner:** Josh  
**Decision ID:** `DEC-VR-001`

## Goal

Make one visual check easy for another contributor to repeat. This pilot checks
whether the GalapagOS desktop home is readable and well composed at two common
16:9 resolutions. It does not approve the whole interface, Simulation screen,
art direction, or production readiness.

## Exact review state

| Setting | Required value |
|---|---|
| Scene | `Assets/Scenes/GalapagOSDesktopTest.unity` |
| State | GalapagOS desktop home immediately after scene load; no app windows open |
| Camera | `GalapagOS Desktop Test Camera` |
| Input sequence | No player input; load the scene and capture the initial desktop state |
| Primary resolution | 1280 × 720 |
| Secondary resolution | 1920 × 1080 |

The focused test
`GalapagOSDesktopHomeCapturesVisualEvidenceWithoutOpeningWindows` checks that
the desktop camera is enabled and that no app window is open before it captures
the image. It does not issue desktop commands or open, close, hide, or resize an
app window. Each run starts in a fresh Unity test session and loads the scene
before capture; it does not reuse an active expedition.

## How to capture

Run these from `LearningIndieDev` with the Unity Editor closed. The project
requires Unity tests and research tools to run with the approved elevated host
permissions described in `AGENTS.md`.

```powershell
.\tools\Invoke-UnityVisualEvidence.ps1 -TestFilter "SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.GalapagOSDesktopHomeCapturesVisualEvidenceWithoutOpeningWindows" -ScreenWidth 1280 -ScreenHeight 720
.\tools\Invoke-UnityVisualEvidence.ps1 -TestFilter "SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.GalapagOSDesktopHomeCapturesVisualEvidenceWithoutOpeningWindows" -ScreenWidth 1920 -ScreenHeight 1080
```

Each command creates a new `artifacts/visual-evidence-*` directory containing
`01-galapagos-desktop-home.png`, `PlayMode-results.xml`, and `PlayMode.log`.
The two captures are separate runs so the requested resolution is fixed before
the scene loads.

For the review record, add the Unity version, branch and source commit, whether
the working tree was dirty, both artifact paths, the PlayMode result, and each
PNG's SHA-256. Do not compare raw image hashes as a pass/fail rule; rendering
differences can create noise that says nothing about readability.

## Human review checklist

Review both PNGs at their native resolution. Mark each item **Pass**, **Revise**,
or **Not assessable**, and add a short note for any revision:

- The full composition fits the screen without unintended cropping or bars.
- Text and icon labels are readable at the target resolution.
- Important controls and information have a clear visual hierarchy.
- No elements overlap, clip, or disappear at either resolution.
- The initial focus or selection state is clear, if one is visibly present.
- The background, icons, panels, and controls feel like parts of one interface.

These are human visual judgments. The automated test only verifies the capture
state and writes the images; it does not grade appearance. Do not use a
pixel-perfect screenshot comparison as the acceptance gate.

## Prior evidence boundary

The retained captures in
`artifacts/visual-evidence-20260907-023518/` show an earlier setup state.
They demonstrate that the camera-capture path produced PNGs, but they are not
current visual acceptance evidence: the current working-state contract is a
direct-start Forest Edge/Hare simulation. This pilot therefore requires fresh
captures from the current reviewed source state.

## Human decision

**Reviewer:** Josh  
**Review date:** Pending  
**Decision:** Pending — Accept, Revise, or Reject  
**Key observation:** Pending inspection of both fresh captures.  
**Allowed reuse:** If accepted, only the GalapagOS desktop-home composition at
1280 × 720 and 1920 × 1080 for the reviewed source revision. This is not
approval of other desktop apps, Simulation transitions, generated art, or the
complete player experience.  
**Evidence:** Pending fresh artifact paths and review notes.

## Completion gate

Close this pilot only when both captures and PlayMode results exist, the source
revision is recorded, the checklist has a named human disposition, and another
contributor can follow the same test name and resolution commands from the same
source revision.
