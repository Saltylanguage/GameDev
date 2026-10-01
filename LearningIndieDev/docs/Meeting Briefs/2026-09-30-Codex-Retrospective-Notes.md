# Codex retrospective notes — for discussion

**Status:** My preparation for a team conversation, not a team verdict or an approved process change.  
**Evidence cutoff:** Current checkout on 2026-09-30, including Sim's S4 review at `a1bd51ae`.  
**Likely scope:** Sprint 3/M1 and the handoff into the proposed Sprint 4. Sim and Chrono should correct or challenge these notes from their own experience.

## My overall read

The team has become good at making technical claims inspectable. S3 and M1 closed with a named human decision, clean test evidence, explicit caveats, and a clear boundary around later work. The less mature part is turning that evidence into a consistently observable player experience while keeping project state simple enough for every contributor to trust quickly. The risk is spending growing effort on reports and context repair without learning proportionally more about the game or how efficiently we build it.

## What has gone well

1. **A bounded milestone actually closed.** The [S3 closeout](../Sprints/S3-control-record.md) records a safe six-round player route, 263/263 clean EditMode tests, 34 passing PlayMode tests, two expected graphics skips, and no known blocking P0. Josh then accepted [M1](../handoffs/2026-09-29-codex-m1-closeout.md) separately. The closeout names what the evidence supports and what it does not. That is a strong alternative to an ambiguous “done.”
2. **Simulation work is reproducible and interpretable within stated limits.** Matched seeds, report fingerprints, validation, metric definitions, and separate analysis/human decisions make mistakes visible. The [AI workflow learnings draft](../AI_GAME_DEVELOPMENT_WORKFLOW_LEARNINGS.md) documents cases where a metric interpretation was corrected rather than polished away. Sim's [S4 review](../handoffs/2026-09-30-2018-codex-s4-sim-review-and-stat-sheet-strategy-plan.md) also insists on raw counts, denominators, acquisition reach, seed variation, and in-game review instead of a single score.
3. **The team has protected scope and authority.** The [S4 control record](../Sprints/S4-control-record.md) is still Proposed after Sim confirmed his 20 hours. Josh's ownership, P1-032 capacity, and the 8-hour reserve remain explicit. Sim's plan treats the three builds as combinations of shared skills, leaving player meaning and the final accept/revise decision with Josh.
4. **Visual work has begun to use evidence.** The [Forest Edge visual pass](../handoffs/2026-09-27-forest-edge-visual-pass.md) captured both target resolutions, reviewed the images, and caught a reaction panel missing from a test state. It honestly records that the presentation test does not prove the natural event route. That limitation is useful information.

## What I would improve

1. **Reconcile the actual starting contract before planning or running experiments.** Sim's latest [S4 handoff](../handoffs/2026-09-30-2018-codex-s4-sim-review-and-stat-sheet-strategy-plan.md) calls the *current production* Forest Edge population 400 Plants / 25 Hares / 15 Foxes. The checked-in [ForestEdge.asset](../../Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset) and [Working State](../WORKING_STATE.md) say 400 / 55 / 35. This is a verified documentation-to-asset discrepancy, not evidence that the asset should be changed. Before the proposed 13-arm screen, Josh and Sim should choose the intended starting state and record the exact asset revision, step interval, seed IDs, response mode, and configuration fingerprint in one frozen run contract. The S4 handoff already distinguishes its proposed 0.1-second step from the asset's 0.2-second step.
2. **Give player-facing validation a comparable place beside technical validation.** The M1 decision accepted distributed evidence and explicitly lacks one paired complete player run and developer report. The visual pass's acceptance test invoked presentation methods directly; a natural birth/hunt through both hosts remains open. The [visual-review pilot](../AI%20Workflow/VISUAL_REVIEW_PILOT_001.md) still awaits fresh captures and Josh's disposition. A small repeated check should follow a real player route, capture the important visual/audio states, and ask whether the player can explain what changed. This should complement, rather than reopen, M1.
3. **Size evidence work as part of feature work.** The new S4 proposal calls for a 13-arm × 20-seed screen (260 runs), then a separate 200-seed confirmation for selected strategies, while the original S4-03 estimate is 4 hours of Sim's feature time. I cannot infer the runtime or preparation cost from that count. The combination of integration, attribution work, analysis, and player review is enough to warrant an early effort checkpoint and an explicit capacity trade if the estimate does not hold.
4. **Keep the memory system easier to audit.** On this checkout I counted 151 handoff notes plus their README and 673 lines in `WORKING_STATE.md`, whose own introduction says it should not become a master changelog. A read-only run of `tools/Test-Handoffs.ps1` on 2026-09-30 reports 10 errors in older notes, including missing local artifact links and unsupported status text. New notes can pass while the corpus still fails. A short current-by-workstream index, a distinction between handoffs and minor checkpoints, and a named owner for validator debt would help more than another long summary.
5. **Measure whether AI assistance pays for its review cost.** The [workflow value summary](../AI%20Workflow/HUMAN_SUMMARY.md) still has 3 of the 10 minimum sampled tasks and no human decision. The ledger asks for correction cycles, first useful evidence, review time, and usefulness. Until there is a representative sample, claims that this workflow is faster or cheaper remain hypotheses.
6. **Make artifact retention and infrastructure failures routine to handle.** The [retention audit](../handoffs/2026-09-09-artifact-retention-audit.md) found substantial raw evidence before a conservative cleanup; five semantic duplicate bundles remain candidates. The visual-pass handoff records a clean PlayMode attempt that never started tests because of licensing/Package Manager trouble. Run manifests should state why evidence is retained, and failed infrastructure attempts should get a clear retry/escalation rule so they are never mistaken for product failures.

## What I would stop or sharply limit

- **Stop using a recent handoff as a live configuration source without checking the authored asset and commit.** The current Forest Edge mismatch shows the cost of skipping that check.
- **Stop calling a focused component or capture test an end-to-end player check.** Keep those tests; label their coverage accurately and add one real route check when the decision depends on the full path.
- **Stop adding a new report, metric, automation, or handoff format without naming the decision it will improve and who will maintain it.** Existing tools and notes already cover a great deal; retrieval and review are now part of their cost.
- **Stop letting provisional balance changes float without a decision point.** The [Loose Ends ledger](../LOOSE_ENDS.md) correctly marks Forest Edge values provisional. Each future tuning slice should have a frozen comparison, a named reviewer, and an accept/revise/defer result.

These are proposals for discussion, not claims that a particular contributor has been doing something wrong. Some practices above are already written down; the question is whether they work reliably in real handoffs.

## How I could be more useful

1. **Act as a source-of-truth reconciler before expensive work.** At task start, I can compare the relevant asset/code, current plan, handoff, board state, and Git revision, then return a one-page contract with discrepancies and exactly what needs a human decision. The Forest Edge starting-state conflict is the immediate example. This is a focused check, not a new general platform.
2. **Turn intent into a linked verification chain.** For each S4 basic skill, I can trace authored value → offer visibility → resolver effect → deterministic test → report field → visible player cue. I can identify missing links before a large seed panel runs, and produce an evidence matrix that Josh and Sim can review. That would make matched results more actionable and help Chrono see which outcomes need visual communication.
3. **Synthesize simulation and player evidence together.** I can prepare compact matched-seed comparisons that retain the raw references and uncertainty, then pair them with two or three specific in-game observations and a short comprehension prompt. The resulting question is “Can a player see and explain the intended tradeoff?” rather than only “Did a metric move?”
4. **Reduce my own documentation load.** I should write a concise decision record after a material change, link existing evidence, and mark what is proposed versus accepted. I should avoid repeating whole histories in `WORKING_STATE.md` or producing a new document for every minor checkpoint. In this conversation I drafted a S3/M1 meeting brief before the scope was confirmed; although I labeled it a proposal, I should have kept that assumption more visibly provisional and asked for the scheduling details earlier.
5. **Report uncertainty sooner and more usefully.** I should say what was directly checked, what is an inference, and which check would settle it. After Unity failures I should distinguish product regressions from Editor/licensing failures, use the smallest relevant retry, and stop when the available evidence cannot support a stronger claim.

## Questions I would put to Josh, Sim, and Chrono

1. Which one handoff or review step saved you time, and which one made you repeat work?
2. At what point did simulation evidence, visual intent, and the integrated player route diverge or reconnect?
3. Which documents or reports do you actually use to start work? Which ones are hard to find or trust?
4. What is the smallest change to our next sprint that would make the game clearer to a new player?
5. Which of my proposed checks would you want me to perform routinely, and which would add process without value?

## My first three proposed actions

1. Before any S4 screening run, resolve and freeze the Forest Edge starting-state discrepancy and the rest of the experiment inputs. **Suggested owners:** Josh and Sim.
2. Pair one S4 strategy comparison with a real player-route observation and a brief comprehension check. **Suggested owners:** Josh, Sim, and Chrono for the relevant visual cues.
3. Pilot a compact Codex start/end-of-task evidence summary on a few real tasks and record correction/review effort in the existing AI workflow ledger. **Suggested owner:** Codex, with Josh's usefulness rating if he chooses to provide one.
