# FD2 permanent progression acceptance

**Checkpoint:** 2026-10-27 | Owner/reviewer: Bevin | Automated acceptance and
live human clarity review complete. Bevin explicitly accepted T12 clarity at
the end of the success/Gene Lab/reset/replacement/failure walkthrough. This
accepts functional screen clarity, with balance, release and push approval
remaining separate. Implementation remains local and unpushed on
`codex/fd2-t01-integration`.

[Ticket](https://trello.com/c/wxVgh3Q6/148-fd2-t12-verify-migration-and-end-to-end-permanent-progression)
| [Handoff](handoffs/2026-10-27-1613-codex-fd2-t12-permanent-progression-end-to-end-acceptance.md)

## Live review update - October 27 evening

Bevin accepted the live saved-success receipt and Gene Lab wallet/owned-active
labels. These were scoped acceptances; Bevin later accepted the full clarity check. During manual
reset selection, buying the fifth node immediately activated it and ended the
empty-loadout replacement window. Previously owned inactive nodes then needed
another paid reset. The live state was88 Hare Data,0 RD,all five nodes owned,
Population Reserves alone active; capture `live-selection-blocker.png`.

The correction blocks new-node purchases while a paid reset remains empty,
explaining that the replacement must be applied first because buying activates
a node. Existing purchase/activation and20 RD replacement rules remain intact.
A regression case verifies no save/charge/selection loss from a blocked purchase,
then free owned selection and a later ordinary purchase. A bound scene case
checks the disabled control. Full EditMode701 passed/zero failed/one existing skip; focused graphics8/8
and final bound-control capture1/1 pass. Code `330d94d` remains local.
Renewed manual selection/application succeeded. Bevin said "okay yes that
worked". The saved active nodes are Tough Hide,Long Vision,Long Stride and
Population Reserves; Efficient Digestion stays owned/inactive. A real End/Confirm
failure then forfeited20 FD and retained172 Hare/0 RD,allfive owned,four active
andfive receipts. Live reload confirmed the selected build before launch.
Final acceptance: "Yes — accept T12 clarity" on2026-10-27,21:55 EDT.
Temporary demo source/helper/import residue was removed and the owned demo
Editor closed gracefully; demo save archived under ignored artifacts.

## Concrete human review

T12 explicitly requires: "human review confirms result and Gene Lab clarity."
The completed live review supplemented these1280x720 automated captures:

1. [First earned success](../artifacts/fd2-t12-20261027/visuals-final/t12-01-first-earned-success.png):390 FD converts to39 Hare Data; fixed10 RD bonus; both wallets start at0. Is the award and saved status clear?
2. [Failure preserves progress](../artifacts/fd2-t12-20261027/visuals-final/t12-04-failure-keeps-earned-profile.png):confirmed End forfeits20 FD while retaining93 Hare Data,0 RD, license and four owned nodes. Is forfeiture versus permanent retention clear?
3. [Saved replacement loadout](../artifacts/fd2-t12-20261027/visuals-final/t12-03-earned-reset-and-saved-loadout.png):Research1/3, four nodes owned but only Long Vision active,2/8 points,93 Hare Data/0 RD. Are ownership/active state, the20 RD reset, disabled reasons and next-journey application clear?

- [x] Bevin confirms the results and Gene Lab are clear enough for T12.
- Human decision/date/evidence: Bevin,2026-10-27 evening,explicit "Yes — accept T12 clarity" after live review. Final [failure capture](../artifacts/fd2-t12-20261027/live-final-failure.png); [reset correction](../artifacts/fd2-t12-20261027/reset-fix-visuals/t12-reset-protects-free-selection.png).

When this document is read from the original checkout, its ignored evidence
lives in the isolated project at
`C:/Users/bevin/.codex/worktrees/fd2-t01-integration/GameDev/LearningIndieDev/artifacts/fd2-t12-20261027/`.

## Automated acceptance matrix

| Scenario | Verified result |
|---|---|
| New profile with zero wallets/no Genome | Three full18-round journeys earn all funds; Research, license, four node purchases, paid reset and free replacement use bound Lab controls |
| Exactly-once settlement | Altered duplicate callback returns original receipt; actual storage reload keeps one deposit per journey |
| Failure | Player End confirmation after round1 forfeits20 FD; permanent wallets, ownership, active nodes and three completed receipts remain |
| Legacy v2 |73 generic currency becomes RD; existing canonical Hare root/license, Fox unlock, species/features, completion IDs and recovery JSON survive success/purchases/reset/reload |
| Local species-wallet v3 |42 RD/19 Hare/7 Fox preserved; supported historical schema excludes incoming completion fields; complete permanent loop passes |
| Incoming Rabbit v3 |5 generic+70 RD become75 RD;9 Rabbit Data preserved as Hare; existing completion/Genome progress survives complete loop |
| Launch | Reloaded Long Vision-only saved selection is frozen into next launch; owned prerequisite can remain inactive |
| Repeated navigation | Reopening Simulation after results waits at the Field Data map for an explicit start; standalone/POC behavior retained |
| Wider checks | Full EditMode700 pass/0 fail/1 existing skip;29 focused scene cases pass within33-case broader run;4 exact known baseline failures remain |
| Rendered checks | Final graphics4/4; ten current captures inspected, including migration receipts/loadouts and consistent failure accounting |

## Exact tested settings and accounting

Controlled transaction fixture: ForestEdge36x20, seed10100,200 plants,20 Hares,
0 Foxes; max/min population0,1800 ticks,0.1 step interval. Runtime Hare movement
and reproduction disabled, metabolism0, starting/max energy100. Other authored
rules retained. No asset/tuning edit. Skip all free rewards, traverse both shops
through map controls and make no shop purchases. No direct permanent-currency
funding or pre-purchased Genome in the new-profile case.

Each success earns20 FD x18 rounds +10 x3 node clears =390 FD. First conversion
is39 Hare Data +10 RD. Research level1 costs10 RD; license10 Hare Data; four
nodes20 Hare Data. Two later successes each earn42 Hare Data +10 RD. One20 RD
reset, without refund, then free Long Vision-only application leaves93 Hare
Data/0 RD. A later confirmed End loses only20 new FD.

## Remaining ecology/content and validation questions

- This survival fixture proves transactions/navigation, not normal-world winnability, pacing or balance. Matched production seeds and natural behavior remain separate research work.
- The older authored success seed10100 fails at tick882; its natural-failure expectation reaches Shop. Both are retained baseline failures, not fixed by controlled survival.
- POC catalog-count7-vs-9 and population-control114-vs-90 assertions also remain baseline failures. Exact messages match T11 evidence.
- Exchange10 FD, success10 RD, Research10/20/30, license10 Hare, nodes5 Hare and reset20 RD remain approved first-test settings, not measured balance.
- These tests explicitly select the existing Field Data route control; the older POC route remains available. No default-route product decision is made here.
- No player build, full PlayMode green result, performance measurement, fun or balance approval is claimed.

## Provenance

- Baseline: `51579267d2f22d65de7c7252c7a84f96ed2375c0`.
- Code/tests: `ada175980a31d5d8f095a107de15bf15b2146e5e`.
- Unity6000.4.6f1, isolated direct batch CLI. Logs/XML/source hashes and exact baseline comparisons are retained in `artifacts/fd2-t12-20261027/`.
- Original profile/recovery preferences and four protected original source hashes remain unchanged; original Editor reports ready.


## Trello closeout

T12 is marked complete in live Done. [Explicit acceptance and verification comment](https://trello.com/c/wxVgh3Q6/148-fd2-t12-verify-migration-and-end-to-end-permanent-progression#comment-6ae1567563aaa80267e1df6a) posted and visibly read back;original description preserved. Proof:`artifacts/fd2-board-20261027/t12-done.png`.
