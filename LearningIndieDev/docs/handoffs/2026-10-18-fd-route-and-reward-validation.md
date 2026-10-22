# Field Data route and reward correction - Unity validation

- **Date:** 2026-10-18
- **Branch/base:** `BevBranch`, `acc371a`, plus the existing uncommitted implementation.
- **Result:** Automated regression and rendered checks pass for the reported correction. Independent human visual acceptance and FD-T16 team self-play remain open.
- **Prior checkpoint:** [October 17 source correction](2026-10-17-fd-route-map-and-shop-income-correction.md), preserved as historical evidence.

## Scope and additional fixes found during validation

The first Field Data action now previews the five-node route, and a separate
Start begins ticks. The preview contains five visible connected cards with
Ready, Current, Upcoming, Cleared or Failed status. The larger woodland map
remains the Desktop entry. Corrected the stale 13-encounter heading and the
clipped Start button on the selected route.

Map controls are present inside the round/shop decision panel and results
panel, where the ordinary HUD control was covered. Opening the map pauses a
running simulation; Back resumes only a previously running simulation. Round
and shop decisions retain their session, tick, balance and pending choice.
Refreshing state before opening also fixes results transition cleanup closing
a map opened in the same frame.

Hare free rewards use the six approved Hare skills and exclude generic Attack.
All three income offers are Shop-only; round and node reward generation never
fills choices with income modifiers. Updated the starter-definition, reward
and portable self-test fixtures to the corrected rule.

Exercising the newly restored Hare pool exposed Reproductive Drive throwing at
an authored reproduction chance of 1.0. Both the shared attribute application
and legacy upgrade application now clamp the resulting reproduction chance to
the valid 0..1 range. Regression coverage checks the upper and lower bounds.
No authored species values or ecology prices were changed.

## Checks and retained evidence

| Check | Result | Evidence |
| --- | --- | --- |
| Initial focused EditMode baseline | 53 passed, 4 failed, 1 skipped; the four failures expected superseded reward sources/pool | `artifacts/unity-tests-20261018-100409/` |
| Updated focused domain checks | 59 passed, 0 failed, 1 dedicated parity fixture skipped | `artifacts/unity-tests-20261018-101329/` |
| Full clean EditMode suite after logic fixes | 399 passed, 0 failed, 1 dedicated parity fixture skipped | `artifacts/unity-tests-20261018-101526/EditMode-results.xml` |
| Full clean PlayMode suite after logic fixes | 49 passed, 0 failed, 6 separate graphics checks skipped | `artifacts/unity-tests-20261018-101526/PlayMode-results.xml` |
| Final 1280x720 Field Data graphics/runtime checks, including final panel Map buttons | 6 passed, 0 failed, 0 skipped | `artifacts/visual-evidence-20261018-101746/PlayMode-results.xml` |
| Final 1920x1080 authored route graphics/runtime check | 1 passed, 0 failed, 0 skipped | `artifacts/visual-evidence-20261018-101848/PlayMode-results.xml` |
| Current portable runner Release build and self-test | Build: zero errors/warnings; self-test passed, including the 18-round journey replay | `dotnet build tools/CellSim.Batch/CellSim.Batch.csproj -c Release`; `dotnet tools/CellSim.Batch/bin/Release/net8.0/CellSim.Batch.dll self-test` |

The initial graphics run at `artifacts/visual-evidence-20261018-101025/`
retains the two reproduced runtime failures and blank map before the additional
fixes. The intermediate six-check pass at `101417` predates the final visible
panel Map buttons. The full suites precede only the final Map-button markup
and rendered-button test assertions; the two final graphics runs cover those.

Reproduction commands from the project directory:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test -Execution Clean -Mode All
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Invoke-UnityVisualEvidence.ps1 -Execution Clean -TestFilter FieldData -ScreenWidth 1280 -ScreenHeight 720
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Invoke-UnityVisualEvidence.ps1 -Execution Clean -TestFilter SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.FieldDataAuthoredRouteCapturesRoundShopReorderAndSuccessEvidence -ScreenWidth 1920 -ScreenHeight 1080
```

## Runtime and visual coverage

- Preview stays Ready at tick zero with no session until explicit Start.
- Closing the selected preview returns to setup; reopening retains selection.
- Running map entry pauses ticks and returns to the same run; paused entry
  remains paused on return.
- Map round trips retain state throughout all 18 rounds, both shops and
  successful results. Rendered Map buttons have visible, enabled, arranged
  controls with working command bindings at rounds 1, 6, 12 and results.
- The actual authored Forest Edge route uses seed 10100 and the authored
  0.2-second interval. It completes 18 rounds and two shops. Separate failure,
  abandonment and reset fixtures also pass.
- Definition tests reject round/node eligibility for each income offer; the
  full-route reward fixture grants no income modifiers outside shops.
- Reviewed the selected preview, original map entry, round reward choices,
  shop screens, shop map highlights and results captures at 1280x720, plus
  preview/round/shop-map captures at 1920x1080.
- The capped-offer/reroll/paid-reorder visual test remains a separately labeled
  preloaded-ownership fixture. No guaranteed income offer or synthetic ownership
  was introduced into the authored route to obtain a screenshot.

## Limits and handoff

These are Editor tests and captured-render inspections, not a player build or
independent human acceptance. No fresh cross-runtime parity export or 200-seed
economy panel was run; the earlier artifacts remain historical. Existing
obsolete-API/import/shutdown warnings remain in logs; the final requested tests
have no failures. The separate larger-minimum-litter exception recorded in the
vault is outside this correction.

Implementation remains local, uncommitted and unpushed. No Trello status or
owner/reviewer assignment was changed. See the updated
[self-play instructions](../FIELD_DATA_SELF_PLAY_REVIEW.md).
