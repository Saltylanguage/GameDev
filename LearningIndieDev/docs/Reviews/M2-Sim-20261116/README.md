# Sim's independent M2 review - November 16, 2026

- Reviewer: Sim / Bevin, existing project developer.
- Initial source: fda773195777730c55ae106fc540a91c0a604cf9 from M2_Gate/ValidationTesting.
- Retest source: the same baseline with the legacy-preview-health compatibility fix included on codex/m2-independent-review.
- Environment: Windows, Unity Editor 6000.4.6f1. Normal Forest Edge / Hare Journey; displayed seed 10100 in the retained run evidence. Both 1280x720 and 1920x1080 reviewed. Exact profile name and every attempt's setup were not separately recorded.
- Status: independent review recorded; M2 acceptance remains open. These are Sim's answers; Josh's answers have not been read or compared. Preserve independent answers until both reviews are complete.

## Findings to pick up

1. **Startup fixed:** old saved preview settings lack MaximumHealth, deserialize it as zero, and fail initialization. Normalize zero to the legacy one-health value, preserving positive values. Simulation advancement and speed switching now work for Bevin. See the [fix handoff](../../handoffs/2026-11-16-1443-codex-m2-tick-zero-legacy-preview-health-fix.md).
2. **No sound:** Bevin hears no audio during Journey or simulation. Cause and mute/volume behavior are unverified.
3. **Early Journey failure blocks progression:** normal Journey cannot be completed; no successful reward to test permanent purchases. Latest failure is phase 5, tick 427, with starvation and combat losses recorded below.
4. **Events/strategy are difficult to perceive:** the side panel somewhat helps, but board events are hard to identify at the grid/creature density. All three opening responses lack an easy-to-see/feel difference, even though the result screen records some direct effects.
5. **UI follow-ups:** launch identity/goal prominence; zoomed-out detail at 1280x720; keyboard reachability/discoverability; no apparent return from the Field Data test route to normal Journey; reduced motion could not be found.
6. **Replay feedback:** Bevin would not replay right now. No single decisive moment was separately supplied.

The explicit biome/species selection menu was Bevin's discussion suggestion, not an approved scope change. No audio, navigation, ecology, or upgrade-strength fix was made alongside the startup correction.

## Latest recorded answers

Q1-Q22: **9 Yes, 9 Needs change, 4 Blocked**. Q22 is player feedback, not a stand-alone milestone criterion. Q23 is separate factual-report agreement.

| Question | Latest answer | Observation / limitation |
| --- | --- | --- |
| Q0 | Setup recorded | Sim / Bevin, existing project developer; Windows, Unity Editor 6000.4.6f1. Initial source fda7731; resumed on the included legacy-preview-health fix. |
| Q1 | Yes | Fresh-profile Journey entry works and selected-profile identity is clear. Original tick-zero blocker was subsequently fixed and recovery human-confirmed. |
| Q2 | Needs change | Bevin sees the biome/species tagline at the top and goal subtext at the bottom, but wants both clearer, especially the easily missed goal. Suggestion for discussion: a specific choose-biome / choose-species menu; this is not an approved scope change. See q2-launch-clarity.png. |
| Q3 | Yes | Bevin confirms that current position, an available next node, and the action to start it are clear on the journey map without help. |
| Q4 | Yes | Bevin reports all three opening choices are available on every starting Journey branch. His understanding: Escape Instinct helps Hares avoid Fox encounters before being attacked; Tough Hide increases Hare health; Reproductive Drive increases reproduction chance. These are player interpretations, not measured effect verification. |
| Q5 | Needs change | Bevin reports everything is readable at 1280x720, but the zoomed-out grid and inhabitants appear blurry and lack detail. Zooming in retains good fidelity. Readability confirmed; visual detail concern remains. See q5-1280x720-board-detail.png. |
| Q6 | Yes | Bevin reports everything is much more readable and clear at 1920x1080, even without zooming in. This improves on the zoomed-out detail concern at 1280x720. See q6-1920x1080-readability.png. |
| Q7 | Yes | Bevin confirms pointer inspection works properly, including after zooming and panning. Selected-cell information matches the cell clicked. |
| Q8 | Needs change | Bevin can move the grid pointer with the keyboard and clearly see grid focus, but thinks an initial click into the grid is required. No keyboard control of other important controls was apparent; undiscovered hotkeys/inputs may exist. |
| Q9 | Yes | Bevin confirms switching simulation speed works. This is a human retest after the startup fix; original blocked response remains preserved. |
| Q10 | Yes | Both sizes are readable and scale correctly, with much better simulation/text clarity at 1920x1080. Results-screen readability was confirmed in the resumed batch. |
| Q11 | Yes | Bevin is happy to keep the current Hero background, Rabbit art, and green/blue reward icon for now. Provisional acceptance of the current review direction; no replacement requested. |
| Q12 | Needs change | Bevin reports no sounds for any of the prompted UI, Mutation-choice, danger/outcome cues, ambience, or music. This records observed silence; implementation absence and the cause have not been established. |
| Q13 | Blocked | Bevin confirms there is no sound during simulation or Journey. Sound comfort and audible mute/volume effects cannot be judged; control discoverability and function are not independently established. |
| Q14 | Needs change | Bevin can somewhat track events through the side panel, but visually on the board it is hard to tell which events occur, especially given grid size relative to creature numbers. Missing audio remains a separate limitation. See q14-busy-run-event-readability.png. |
| Q15 | Blocked | Bevin cannot complete the normal Journey. Latest failure screenshot reports phase 5, tick 427, Hare extinction: 42 starvation deaths and 39 combat deaths. Earlier approximate collapse timing and Fox-pressure perception are retained as observations, not the final measured cause. |
| Q16 | Needs change | Bevin reports all three opening choices lack a visible or easy-to-feel impact/difference. Per-choice observed benefits and tradeoffs on isolated comparable attempts remain unestablished; this does not prove zero mechanical effect. |
| Q17 | Yes | Bevin can understand the outcome and numerical failure reasons from Journey Observations on Expedition failed. Screenshot shows leading loss starvation and measured counters for all three introductory Mutations. |
| Q18 | Blocked | Bevin cannot complete a successful Journey, so does not earn data to purchase permanent upgrades. Earn-and-buy sequence cannot be tested; owned/equipped distinction remains untested. |
| Q19 | Blocked | No successful reward/purchase prerequisite to test retained unlocks. This Editor session also cannot establish a real built-game process exit/relaunch. |
| Q20 | Needs change | Bevin cannot find reduced motion. This is a setting discoverability concern; the existence of an implemented setting and reduced-motion event parity remain unverified. |
| Q21 | Needs change | Bevin reports the flow is otherwise good, but after switching to the Field Data test route there is no apparent way to switch back to the normal journey map. Full completion/replay remains limited by the separate tick-zero blocker. |
| Q22 | Needs change | Bevin explicitly would not replay right now. Player feedback only, not a stand-alone M2 decision; a single decisive moment was not separately named. |
| Q23 | Agree | Bevin agrees the M2-04 factual report accurately records the approved comparison and limits. This is independent accuracy input; Josh retains the separate report/milestone decision. |

## Failure-screen evidence

[Expedition failed screenshot](q17-expedition-failed-tick427.png) reports:

- Extinction at tick 427, phase 5; expedition ended without rewards.
- Plant 626, Hare 0, Fox 66; 26 Hare births and 81 deaths.
- 187 predator encounters; 39 kills. Losses: 39 combat, 42 starvation, 0 other. Displayed leading loss: starvation.
- Escape Instinct: 4 predator contacts prevented by added avoidance chance.
- Tough Hide: 46 full-health predator hits supported because of added health.
- Reproductive Drive: 0 reproduction-chance rolls passed because of added chance; this does not mean zero births.
- Combined loadout: Tough Hide level 3, Escape Instinct level 1, Reproductive Drive level 1.

These are screen-reported counters, not an independent raw-telemetry audit. This is a combined-loadout run, not isolated comparable strategy arms. Bevin's earlier Fox-pressure perception and approximate collapse around ticks 300-400 are retained as player observations. This screenshot provides an exact terminal observation for its attempt; starvation narrowly exceeds combat as the reported death cause, without establishing the full causal ecology mechanism or a tuning decision.

## Visual evidence

- [Launch identity and goal prominence](q2-launch-clarity.png)
- [1280x720 board detail](q5-1280x720-board-detail.png)
- [1920x1080 readability](q6-1920x1080-readability.png)
- [Busy-run event summary versus board readability](q14-busy-run-event-readability.png)
- [Failure explanation and measured effects](q17-expedition-failed-tick427.png)

Resolutions are Bevin's reported Game view settings, not inferred from attachment dimensions. The busy-run screenshot is paused at tick 460 with surviving Hares; the terminal tick-427 screenshot is separate attempt evidence. Do not combine them into a single run timeline.

## Initial review and retest history

Before the startup fix: 7 Yes, 4 Needs change, 11 Blocked across Q1-Q22. Q9/Q12/Q13/Q22 were explicitly blocked human answers; Q14-Q20 were assistant classifications from unmet prerequisites, not separate human answers. Q23 was Agree.

After the fix, Bevin confirmed ticking and speed work. The remaining batch was answered as recorded above. Q10 result coverage was supplemented. Original local response records remain archived; no blocked response was converted to a pass solely because an automated test passed. Q16's isolated per-response benefit/cost evidence and Q20's reduced-motion event parity remain unverified. Permanent-purchase/relaunch checks are still blocked; Q19 needs a built game.

## Code validation at the fix checkpoint

- Regression before the production fix: one pass / one failure, with the same maximumHealth=0 startup exception.
- Unity recompile: no errors; seven existing SpeciesArchetype obsolete-API warnings.
- PopulationConfigurationTests: 42/42 EditMode passed, including zero-health compatibility and positive-health preservation.
- MainMenuPlayModeTests.ProfileCreationEnablesContinueAndLoadsDesktop: 1/1 PlayMode passed with automatic ticks and positive recorded simulation ticks. The test has controlled Genome setup, so it does not establish a full neutral-Genome route or balance result.
- All three preview-default keys and profile/recovery stores matched their pre-test captures afterward.
- No full-suite run, player build, full authored-route success, or M2 closeout is claimed.

Raw test logs and personal game-preference backups remain local ignored artifacts. The commands for focused reruns from LearningIndieDev are:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test -Mode EditMode -Execution Auto -TestFilter 'SaltyGame.EditorTests.PopulationConfigurationTests'
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test -Mode PlayMode -Execution Auto -TestFilter 'SaltyGame.PlayModeTests.MainMenuPlayModeTests.ProfileCreationEnablesContinueAndLoadsDesktop'
```

## Follow-up

Investigate silent audio and early failure with reproducible source/seed evidence. Address feedback/readability and navigation/accessibility findings. Any ecology or upgrade-value change requires a separate concrete balance decision. Compare by question ID when both independent reviews are ready; no interim Discord message was sent.