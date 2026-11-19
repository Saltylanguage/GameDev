# M2 independent review packet for Josh and Sim

Use this packet with the [same numbered questionnaire](M2_MANUAL_REVIEW_QUESTIONNAIRE.md). Josh and Sim each complete it **independently** before comparing answers. The purpose is to find where their observations and expectations agree or differ; it does not pre-approve an M2 item.

## Use the same starting point

1. Check out `codex/s6-predator-first` at the questionnaire publication commit shared with both reviewers. The gameplay baseline is `71c0c7b7`; the questionnaire commit adds documentation only. Record the **exact** Git HEAD or build ID in Q0. If gameplay changes before both reviews finish, use the same pinned revision for both or restart the affected checks.
2. Use separate fresh profiles. Start from the normal menu and authored **Forest Edge / Rabbit** route. Use seed `10100` where the player flow exposes a seed choice; otherwise record the seed displayed by the run. Record any difference in scenario, profile state, input device, build type, or operating system. Do not share save data.
3. Check 1280×720 and 1920×1080, keyboard and pointer input, 1× and 4× speed, and reduced motion where the question asks. A real process exit/relaunch requires a built game; if testing only in the Editor, mark that part **Blocked**. Keep fixtures or debug shortcuts out of the legitimate route result.
4. Answer Q0–Q23 in order using a separate copy of the [response template](M2_MANUAL_REVIEW_RESPONSE_TEMPLATE.md), or have an assistant ask one question at a time and record the answers. Do not read or discuss the other reviewer's answers until both copies are complete. A one-sentence observation is enough; attach a seed or screenshot for a failure when convenient. **Blocked** means the state was unreachable, not that it passed.

Both reviewers are project insiders. Their independent answers are useful for alignment, but they do not replace the first-time external-player evidence required by M2-10. Josh remains the M2 and M2-04 factual-report decision owner; Sim's Q23 answer is an independent accuracy review.

## Reproduce the same automated checks

Run these from `LearningIndieDev` with the host Unity permissions required by `AGENTS.md`. Keep the generated `artifacts/` directories and note their paths in your response copy. These checks support the manual answers; screenshots and passing assertions do not make a human readability decision.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Doctor
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test -Mode All -Execution Clean
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Invoke-UnityVisualEvidence.ps1 -Execution Clean -TestFilter 'SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.GalapagOSDesktopAndSimulationCaptureGameViewEvidence' -ScreenWidth 1280 -ScreenHeight 720
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Invoke-UnityVisualEvidence.ps1 -Execution Clean -TestFilter 'SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.GalapagOSDesktopAndSimulationCaptureGameViewEvidence' -ScreenWidth 1920 -ScreenHeight 1080
```

The most recent pre-questionnaire baseline was **EditMode 778 passed, 0 failed, 1 skipped** and **PlayMode 101 passed, 1 failed, 7 skipped**. The PlayMode failure is the authored Rabbit route, which loses all Rabbits before node one ends. Treat a different result as something to investigate, and do not tune ecology merely to make this gate green. The focused art visual test passed 1/1 at each target size on `71c0c7b7`.

## Compare only after both are finished

Put Josh's and Sim's answers side by side by Q number. Mark **Aligned**, **Different observation**, **Different judgment**, or **Different setup/Blocked**. For each difference, keep both original observations, then decide whether to reproduce a defect, change the design expectation, or gather more evidence. Summarize each M2 item as **Accept**, **Revise**, or **Inconclusive** only after reviewing all of its questions and the automated evidence. Record Josh's separate report and milestone decisions explicitly; never infer them from a tally of Yes answers.

Shareable links: [current M2 handoff](handoffs/2026-11-16-1055-codex-m2-05-art-and-m2-review-handoff.md), [M2-04 factual report](Research/Experiments/M2-04-Three-Response-20261115/REPORT.md), and [Chrono art candidates](Art/ChronoReview-20261116/README.md). The raw M2-04 experiment bundles and visual screenshots are local ignored artifacts; another machine needs a transfer or rerun to inspect them.
