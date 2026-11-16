# M2 tick-zero legacy preview health fix

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-16-1443-codex-m2-tick-zero-legacy-preview-health-fix
- Owner: Codex
- Branch: codex/m2-independent-review
- Baseline commit: fda773195777730c55ae106fc540a91c0a604cf9
- Date: 2026-11-16
- Supersedes: none
- Pickup branch: origin/codex/m2-independent-review.

## Summary

Bevin's independent Editor review reached the Journey but the simulation stayed at tick 0 despite Running status. Existing local preview defaults predate MaximumHealth: the JSON has no such field, and all three saved species drafts deserialize it as zero. SpeciesRules rejects zero during preview initialization. In the observed broken session the preview was disabled, so its Update never advanced the active run. A compatibility fallback now maps zero-valued preview draft health to the legacy value of 1; positive values remain unchanged.

## Changes

- [SpeciesSimulationPreview.cs](../../Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs): normalize only zero MaximumHealth when converting saved/editable drafts to species rules. This reuses the small compatibility correction already present in separate map-wrapping commit 1ef3f17 without integrating topology work.
- [PopulationConfigurationTests.cs](../../Assets/Tests/Editor/PopulationConfigurationTests.cs): regression coverage for legacy zero values and preservation of positive health values.
- No profile reset, PlayerPrefs clearing, scene/asset edit, ecology tuning, runtime force-enable, or unrelated UI change.

## Decisions and assumptions

- Old drafts without this field use the historical one-health default. Positive authored/saved health stays intact; negative values still reach the existing strict validation.
- Original independent responses stay tied to fda7731. A repaired-source human continuation must identify this local patch/new revision; automated checks do not replace blocked human answers.
- Bevin authorized publishing the fix, completed review, and selected screenshot evidence on November 16. No merge or M2 acceptance was authorized by the push.

## Validation

- Before the production fix, the two-case focused regression was 1 passed / 1 failed; zero health failed with the same ArgumentOutOfRangeException as startup: [red result](../../artifacts/unity-tests-20261116-144132/EditMode-results.json).
- Unity 6000.4.6f1 recompile succeeded without errors. Seven existing SpeciesArchetype obsolete-API warnings were reported.
- After the fix, the full affected PopulationConfigurationTests class passed 42/42, including both health regressions: [EditMode result](../../artifacts/unity-tests-20261116-144205/EditMode-results.json).
- Focused MainMenuPlayModeTests.ProfileCreationEnablesContinueAndLoadsDesktop passed 1/1: [PlayMode result](../../artifacts/unity-tests-20261116-144247/PlayMode-results.json). It creates a test profile, follows menu/Desktop launch, chooses a Journey opening/first simulation, waits for automatic ticking, and asserts positive recorded simulation ticks. Its controlled Genome setup is not a human balance or journey-completion result.
- Compared captured preferences before/after: v3/v4/v5 preview settings, profile store, and recovery store all unchanged.
- git diff --check passed. Editor returned to clean MainMenu scene outside Play Mode, ready for a fresh session.

## Independent questionnaire checkpoint

Bevin completed the resumed independent review after human-confirming the simulation now advances. Latest Q1-Q22 answers: **9 Yes, 9 Needs change, 4 Blocked**; Q23 is **Agree** on factual-report accuracy. The [shared review and screenshots](../Reviews/M2-Sim-20261116/README.md) include every latest answer, original/retest provenance, exact failure-screen counters, and unresolved findings.

Normal Journey completion remains blocked. Latest failure screenshot reports phase 5, tick 427, 42 starvation deaths and 39 combat deaths, with starvation labeled the leading loss. Direct Mutation counters are present, but the effects are difficult to perceive during play. No isolated per-strategy comparison or causal balance diagnosis follows.

Unresolved: silent audio; busy-board event visibility; strategy feel; launch messaging; 1280x720 detail; keyboard discoverability; apparent missing return from Field Data test route; reduced-motion setting discoverability; no successful reward/purchase/relaunch evidence; negative replay interest. Current selected art remains acceptable for now. Q19 needs a built game. Q23's agreement does not approve balance or M2 closeout. Josh's answers have not been read or compared.

Raw local test reports and preference backups remain ignored. Only curated review Markdown and five project-game screenshots are included for sharing; no PlayerPrefs or profile-store backup is published.
## Risks and incomplete work

No full-suite run, player build, full authored-route success, or new ecology/balance conclusion is claimed. Bevin has resumed human checks and confirms early Hare extinction still blocks route completion. Q19 real process-relaunch evidence needs a built game. Other questionnaire findings were not fixed in this patch.

## Next useful step

Pick up codex/m2-independent-review and read the shared review. Compare independent responses by question ID when both reviewers are ready, then investigate the remaining audio, route, feedback, and UI findings. Preserve the reported evidence and make any ecology change through a separate balance decision. No M2 item or milestone is closed by this fix.
## Board synchronization

After Bevin confirmed the available posting identity, the publication note was posted to the [M2 milestone card](https://trello.com/c/PoxE1gpz/19-m2-vertical-slice#comment-6afb7776cb5f74d3422e9bad) and read back successfully. It links code/review commit `7995db108f27c47a4dc4e10c7c733dbc61ca26f1`, the full answers and screenshots, and this fix handoff. The note records 42/42 affected EditMode and 1/1 focused PlayMode evidence, human-confirmed tick/speed recovery, current 9 Yes / 9 Needs change / 4 Blocked answers and Q23 factual-report agreement. M2 remains open; no card completion, checklist acceptance, balance approval or Discord message was performed.