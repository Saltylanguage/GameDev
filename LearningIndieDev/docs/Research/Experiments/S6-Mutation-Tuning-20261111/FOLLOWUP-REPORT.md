# S6 avoidance and reproduction follow-up — factual report

**Experiment ID:** `S6-PF-MUTATION-TUNING-20261111`  
**Follow-up date:** 2026-11-11  
**Manual review:** Not started  
**Decision owner:** Josh

This follow-up prepared and measured avoidance and reproduction candidates against the fixed Forest Edge baseline. It used seeded simulation only; it did not include a player or manual playtest.

## Frozen setup

| Input | Value |
| --- | --- |
| Scenario | Forest Edge, 36×20, nonwrapping |
| Player species | Hare |
| Node 1 opening | 55 Hares, 12 Foxes, 500 Plants |
| Node 2 opening | Carry only the Node 1 closing Hare count; reset to 48 Foxes and 500 Plants |
| Hare reset state | Recreated Hares with default individual state |
| Combat | Opposed roll; natural attack opportunities; `bev-experimental` |
| Schedule | 12 phases, 100 ticks each; two nodes of six phases |
| Screen seeds | 10100–10111 |
| Confirmation seeds | 20200–20299 |

Every completed batch passed the portable run checks. The summarizer also checked all phase records for 12 phases, the scheduled loadout, and the Hare/Fox/Plant population at each node opening. The new confirmation control's final state digests match the earlier no-upgrade confirmation for all 100 seeds.

## One-rank candidate screen

Each candidate rank was active from phase 2. The control and both candidates used seeds 10100–10111.

| Condition | Node 1 Hare failures | Cumulative Node 2 Hare failures (95% Wilson) | Mean Hares at Node 1 / Node 2 close | Post-acquisition births | Fox kills | Minimum Plant checkpoint |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| No Mutation | 0/12 | 8/12 (39.1–86.2%) | 16.92 / 6.42 | 1,342 | 1,468 | 396 |
| Quick Escape `+0.5` flee speed | 0/12 | 9/12 (46.8–91.1%) | 19.50 / 0.25 | 1,394 | 1,535 | 439 |
| Larger Maximum Litter `+1` | 0/12 | 10/12 (55.2–95.3%) | 13.42 / 1.50 | 1,279 | 1,424 | 417 |

Quick Escape increased Hare movement steps by 12.7% in this screen. Fox-adjacent observations per 1,000 Hare observations were 70.02 for Quick Escape and 64.02 for control. The one-rank candidate did not reduce cumulative Node 2 failure.

## Two-rank paths and dose screen

Each path acquired rank 1 after phase 1 and rank 2 after phase 2. The second rank remained active through phase 12.

| Condition | Node 1 failures | Cumulative Node 2 failures (95% Wilson) | Failures among Node 1 survivors | Mean Hares at Node 1 / Node 2 close | Post-acquisition births | Fox kills | Fox-adjacent observations per 1,000 Hare observations | Minimum Plant checkpoint |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| No Mutation | 0/12 | 8/12 (39.1–86.2%) | 8/12 | 16.92 / 6.42 | 1,342 | 1,468 | 64.02 | 396 |
| Quick Escape rank 1→2 | 1/12 | 6/12 (25.4–74.6%) | 5/11 | 20.83 / 25.33 | 1,796 | 1,631 | 61.65 | 358 |
| Larger Minimum Litter rank 1→2 | 1/12 | 7/12 (32.0–80.7%) | 6/11 | 17.17 / 6.08 | 1,594 | 1,604 | 69.23 | 268 |

In matched seeds versus the screen control, Quick Escape rank 1→2 rescued 4 control failures and introduced 2 failures in control survivors. Larger Minimum Litter rank 1→2 rescued 3 and introduced 2. Neither screen point estimate reached the 35–45% cumulative Node 2 failure target.

Quick Escape rank 1→2 increased movement steps by 54.2% and reduced Fox-adjacent observations per 1,000 Hare observations by about 3.7%; Fox kills still increased. Larger Minimum Litter rank 1→2 increased births by 18.8%, and its minimum Plant checkpoint was 268.

## Higher rank paths

Each candidate reached rank 3 in phase 4, after ranks 1 and 2 in phases 2 and 3.

| Condition | Node 1 failures | Cumulative Node 2 failures (95% Wilson) | Mean Hares at Node 1 / Node 2 close | Post-acquisition births | Fox kills | Minimum Plant checkpoint | Paired rescues / new failures |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| No Mutation | 0/12 | 8/12 (39.1–86.2%) | 16.92 / 6.42 | 1,342 | 1,468 | 396 | — |
| Quick Escape rank 1→2→3 | 0/12 | 10/12 (55.2–95.3%) | 21.58 / 4.83 | 1,560 | 1,680 | 418 | 1 / 3 |
| Larger Minimum Litter rank 1→2→3 | 1/12 | 10/12 (55.2–95.3%) | 15.83 / 0.17 | 1,524 | 1,602 | 265 | 0 / 2 |

## 100-seed matched confirmation

The two-rank paths and no-upgrade control used the same seeds, 20200–20299.

| Condition | Node 1 failures | Cumulative Node 2 failures (95% Wilson) | Failures among Node 1 survivors | Mean Hares at Node 1 / Node 2 close | Hare births after acquisition | Fox kills | Hare starvation deaths | Minimum Plant checkpoint | Paired rescues / new failures vs control |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| No Mutation | 3/100 | 60/100 (50.2–69.1%) | 57/97 | 16.79 / 6.88 | 11,019 | 12,273 | 3,558 | 381 | — |
| Quick Escape rank 1→2 | 4/100 | 66/100 (56.3–74.5%) | 62/96 | 21.37 / 8.87 | 12,404 | 12,995 | 4,022 | 309 | 13 / 19 |
| Larger Minimum Litter rank 1→2 | 1/100 | 70/100 (60.4–78.1%) | 69/99 | 21.80 / 7.13 | 12,693 | 13,410 | 4,070 | 244 | 16 / 26 |

No seed reached a zero-Plant phase checkpoint in these three arms. Compared with control, Quick Escape rank 1→2 had 6 net more Node 2 failures; Larger Minimum Litter rank 1→2 had 10 net more.

## Faster-gestation diagnostic

A separate 12-seed probe overrode Hare gestation to 36 ticks from run start, compared with the authored 48 ticks. This was a stat diagnostic, not a Mutation and not a phase-2 effect.

| Condition | Node 1 failures | Cumulative Node 2 failures (95% Wilson) | Mean Hares at Node 1 / Node 2 close | Post-acquisition births | Fox kills | Hare starvation deaths | Minimum Plant checkpoint |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| No Mutation | 0/12 | 8/12 (39.1–86.2%) | 16.92 / 6.42 | 1,342 | 1,468 | 457 | 396 |
| Gestation 36 ticks from start | 0/12 | 6/12 (25.4–74.6%) | 17.67 / 0.50 | 1,540 | 1,569 | 828 | 193 |

The faster-gestation arm had 4 paired control failures rescued and 2 new failures. The stat override increased births and also increased Hare starvation deaths; its minimum Plant checkpoint remained above zero.

## Stronger avoidance doses and reproduction upper-bound check

Because the two-rank Quick Escape and litter paths failed 100-seed confirmation, two more simulation-only checks were run before considering a manual review. Both use the fixed opening and the same paired seeds as their controls.

### Threat Exposure dose screen

Threat Exposure prevents some Fox–Hare interactions before combat. It was screened at cumulative ranks 2–5; each additional rank was acquired one phase after the previous rank. These are 12-seed diagnostic results, not confirmations.

| Condition | Node 1 failures | Cumulative Node 2 failures | Mean Hares at Node 1 / Node 2 close | Paired rescues / new failures vs control |
| --- | ---: | ---: | ---: | ---: |
| No Mutation | 0/12 | 8/12 | 16.92 / 6.42 | — |
| Threat Exposure rank 2 | 0/12 | 8/12 | 15.25 / 9.00 | 3 / 3 |
| Threat Exposure rank 3 | 1/12 | 6/12 | 19.67 / 0.50 | 4 / 2 |
| Threat Exposure rank 4 | 1/12 | 8/12 | 16.58 / 11.25 | 3 / 3 |
| Threat Exposure rank 5 | 1/12 | 9/12 | 18.00 / 2.25 | 2 / 3 |

The rank-3 result is directionally better in this small panel, but it still had 50% cumulative Node 2 failure, above the 35–45% target. The response was not dose-monotonic: ranks 4 and 5 did not preserve the rank-3 result. These values do not qualify for a 100-seed confirmation yet. The compact report has no standalone count of pre-contact interactions skipped, so Fox attempt totals are only a rough mechanism check and vary with how many Hares remain exposed.

The rank-3 path was then given a matched 100-seed confirmation because it was the only dose with a positive paired signal in the 12-seed screen. It acquired one rank per phase starting in phase 2 and reached rank 3 in phase 4.

| Condition | Node 1 failures | Cumulative Node 2 failures (95% Wilson) | Mean Hares at Node 1 / Node 2 close | Fox attack attempts | Fox kills | Paired rescues / new failures |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| No Mutation | 3/100 | 60/100 (50.2–69.1%) | 16.79 / 6.88 | 21,725 | 12,273 | — |
| Threat Exposure rank 1→2→3 | 3/100 | 62/100 (52.2–70.9%) | 19.39 / 5.30 | 21,261 | 12,226 | 22 / 24 |

The rank-3 path had 2.1% fewer Fox attempts and 47 fewer kills, but cumulative failure was two seeds higher than control. Its paired result was two more regressions than rescues. The measured attack reduction did not translate into better survival in this panel. This path is not ready for manual review, and it uses three Mutation choices rather than one.

### Faster-gestation upper-bound diagnostic

The 36-tick gestation value was also tested across the 100 confirmation seeds, applied from run start. This intentionally gives the possible Mutation more exposure than a phase-2 acquisition, so it is an upper-bound diagnostic, not a simulated Mutation.

| Condition | Node 1 failures | Cumulative Node 2 failures (95% Wilson) | Mean Hares at Node 1 / Node 2 close | Births | Fox kills | Starvation deaths | Minimum Plant checkpoint | Paired rescues / new failures |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| No Mutation | 3/100 | 60/100 (50.2–69.1%) | 16.79 / 6.88 | 11,019 | 12,273 | 3,558 | 381 | — |
| Gestation 36 ticks from run start | 7/100 | 70/100 (60.4–78.1%) | 16.95 / 5.39 | 12,038 | 13,385 | 5,263 | 123 | 14 / 24 |

The override increased births by 9.2%, but cumulative failure rose by 10 percentage points and the paired seeds had ten net more failures. Starvation deaths rose 48%, and the minimum Plant checkpoint dropped from 381 to 123, though no seed reached zero Plants at a checkpoint. Faster gestation from the opening therefore is not a good candidate to implement as the reproduction Mutation in this scenario.

## Current readiness

No avoidance or faster-reproduction value is ready for manual review. Higher Threat Exposure doses did not reliably reduce failure, and the stronger-than-a-Mutation gestation diagnostic worsened survival. The next reproduction candidate needs a different mechanism that increases surviving replacements without creating the same food and Fox pressure. The next avoidance candidate needs a directly measurable reduction in successful Fox encounters. Tough Hide remains the only tested Mutation with a confirmed point estimate in the target window.

## Run provenance and limits

- The experiment reuses the previously Unity-exported Forest Edge snapshot data. Its snapshot copy is bound to portable worker source hash `cbe74a675ed62726d4a1a758ee22fdd2b3ae58d22068df6e1cfbfce92c1d17b7`; the source commit is `5eead86a` with a dirty worktree.
- The higher-dose Threat Exposure screens completed 72/72 runs. The Threat Exposure rank-3 confirmation and gestation upper-bound diagnostic each completed 200/200 runs; both control arms matched the earlier control final-state digests on all 100 seeds.
- The portable source hash changed only for the Quick Escape catalog wiring. The 100-seed control was identical to the earlier control on every seed, checked by final state digest.
- The Unity Editor lock prevented a fresh export and Unity test run. The portable worker build and self-test passed, and all experiment plans passed dry run before execution.
- Screen panels contain only 12 seeds and have wide confidence intervals. The 100-seed panel is the confirmation evidence for the two-rank candidates.
- The gestation diagnostic applies from run start and does not show what an actual phase-2 Mutation would do.
- The tested Fox schedule remains a provisional Forest Edge calibration, not a balance result for other biomes or species.

No manual review, commit, or push was started.
