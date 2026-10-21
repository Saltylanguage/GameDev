# Field Data: current parity and matched spending comparison

- Date: October 19, 2026.
- Decision owner: Bevin. Executed by Codex after approval to complete the two recommended evidence refreshes.
- Status: both automated refreshes complete. Population tuning and human accept/revise remain open.
- Question: do the current Unity and headless journey domains agree, and what happens when the three approved spending policies run on the current authored setup?
- Working hypothesis: the two runtimes should reproduce the same seeded decisions, transactions, board states and results. The spending comparison is descriptive; it has no required survival percentage or population floor.
- Source: local `BevBranch`, HEAD `acc371ac84aa01476e562e9b29e3e441263b8f2e` plus uncommitted implementation. HEAD alone does not identify this build.

## Frozen setup

Forest Edge uses a 36×20 nonwrapping map, 400 Plants, 55 Hares and **20 Foxes**.
The selected species is Hare, the active Genome is empty, and the simulation
interval is authored as 0.2 seconds (`float` export: 0.20000000298023224).
The five-node route contains three six-round simulations and two shops, for
18 rounds of 100 ticks each. Extinction ends a journey immediately.

The export includes the current starter definition and the explicit six-skill
Hare reward pool. Income modifiers are shop-only. Ordinary skill rewards can
repeat to their existing caps. Population controls are allowed at ordinary
rounds 1–5 within each simulation, never at node-end settlement or in shops.
Each cleared simulation awards 10 FD. No ecology values, prices or gameplay
code changed during this refresh.

| Frozen identity | Value |
| --- | --- |
| Shared source hash | `476b1e8d8d5e06e084247976a21c7b2ed8c4ffa34f05431e85324356d991d21a` |
| Scenario fingerprint | `61833cae37ef2b63a5bb5b05f0f24c8483f47d690c5bad8e2c0e5813ea2ab70a` |
| Journey definition fingerprint | `7744847f493ab0ab284bf1de6fab80f46712feffd9c217cd1db1e2d4358f7c7a` |

The manifest additionally pins the snapshot, complete runner/dependencies,
action schedules and all 200 seeds with SHA-256 hashes. A working-tree listing
records the uncommitted baseline. The frozen outputs remain local under
`artifacts/`; they are not Git-tracked publication artifacts.

## Unity and headless verification

The .NET Release build passed with zero warnings/errors, and its self-test
passed. A fresh clean Unity export produced an ordinary ecology reference for
seed 10100 through tick 1800. The standalone replay matched it using the existing
verifier: exact state, counters, choices and simulation timing, with derived
rates within its existing 1e-6 tolerance.

The dedicated Unity EditMode parity test passed **1/1, zero skips**, exercising
these **eight** frozen action fixtures:

| Fixture | Seed | Coverage and actual terminal result |
| --- | ---: | --- |
| No-action success | 10100 | All 18 rounds, both shops, success discard; closes at 1800. |
| Natural failure | 10108 | Extinction and journey-wallet forfeiture at tick 236. |
| Population actions | 10100 | Paid additions/removal, insufficient-funds rejection, placement receipts, node-end purchase rejection with no charge; fails at 1219. |
| Signed species effects | 10100 | Positive shop effect at 600 and negative effect at 1200; fails at 1421. |
| Shop income and random reorder | 10119 | Naturally offered +1/Hare and ×1.5, random charges 10 then 11 FD, invalid chosen permutation rejected; fails at 1160. |
| Paid chosen reorder | 10125 | Naturally offered +1/Hare and ×1.5, reversed execution order for 100 FD; fails at 1095. |
| Second-shop ×2 | 10132 | +1/Hare and ×2 purchases plus random reorder; completes and closes at 1800. |
| All six owned before round 7 | 10100 | Owns all six ordinary skills by 600, accepts another free mutation at 700 and later rounds; fails naturally at 1506. |

The test compares every reached decision boundary, before/after board digests,
balances, ordered income, reward IDs and shop offers/costs/locks; scheduled
action facts and charges; payout and node-clear receipts; acquisitions and
reorder histories; and terminal state, balance, failure loss and success discard.
These are direct domain comparisons, not a new rendered UI or player-build test.
Action-coverage seeds were selected after bounded scouting; they are not the
representative study sample. No forced offers or ecology overrides were used.

A repeat of the random-income fixture was identical in every reported field
except measured machine runtime. All eight fixture hashes were checked.
Fixture preparation also retained one invalid duplicate-request schedule and
its crashed report; it is explicitly excluded from the accepted manifest.

## Matched comparison

Every seed **10100–10299** appears exactly once in each policy, giving **600
completed, validated journeys**. All policies skip every free round/node
reward. The intervention policies attempt one 10-FD purchase at each of the
15 ordinary-round decision windows. The saving policy makes no population
purchases, saves through shop one, and attempts the 50-FD ×2 at shop two;
purchase succeeds only when the offer exists and is affordable.

The new **v3 schedules** remove the former tick-600/1200 population requests to
match current rules. Earlier v2 evidence is preserved and does not represent
this configuration. The labels “weaker” and “stronger” name spending policies,
not demonstrated ecological strength.

| Policy | Completed | Survived to 1800 | Failed | Successful paid actions | Rejected paid actions | Scheduled paid actions not reached |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Remove 1 Hare each ordinary round | 200 | **58 (29%)** | 142 | 853 | 894 | 1,253 |
| Add 1 Hare each ordinary round | 200 | **1 (0.5%)** | 199 | 1,404 | 249 | 1,347 |
| Save for second-shop ×2 | 200 | **52 (26%)** | 148 | 25 | 54 | 121 |

Execution failure and in-game failure are different: **zero replays crashed or
were incomplete**. Extinction is a completed, valid journey result.

All 600 reports passed unique seed/policy coverage, source/scenario/definition
identity, exactly-once payout and clear-award counts, legal population schedule,
action-charge, terminal-balance and complete wallet-reconciliation checks.
First payouts were identical across all three paths for each seed, before any
intervention could affect the world. Frozen input hashes and historical anchor
file hashes remained unchanged; the output-hash manifest pins all 600 reports.

## What the evidence says

- **The opener permits spending in most seeds.** All paths average 15.935 Hares
  at the first payout (median 16), down from 55 at launch. **180/200** seeds can
  afford a 10-FD action then; 20 cannot. This does not establish a desirable pace.
- **Survival often means one remaining Hare.** All 58 Remove-Hare survivors
  finish with one Hare; 51 of 52 saving-path survivors do too. The sole Add-Hare
  survivor is seed 10194, finishing with nine Hares. Seed 10100 is the saving
  path's exception, finishing with 194. A survivor count alone hides this issue.
- **Repeated additions perform poorly in this fixed policy.** Add Hare survives
  just once despite 1,404 successful purchases. This does not prove a price bug
  or identify an ecological mechanism. The policy buys every available round,
  skips all mutations, and does not model adaptive emergency intervention.
- **Removal is not a universally better path.** Against saving, 20 matched seeds
  survive on both paths, 38 only with removal, 32 only with saving, and 110 on
  neither. The six-run aggregate advantage hides many changed trajectories.
- **The final-individual guard matters.** Of 894 rejected removal requests,
  778 attempted to remove the final Hare; 116 lacked funds. They charged no FD.
- **×2 is genuinely conditional.** The saving path reaches shop one in 145/200
  seeds and shop two in 78/200. ×2 appears in 28 of those second shops; 25 can
  afford and buy it, while three cannot. Median arrival balances are 80 FD at
  shop one and 62 FD at shop two, conditional on reaching each shop. Seventeen
  of the 25 buyers survive; that selected subset is not causal evidence that
  ×2 improves ecology.

Three rejection messages require context: Add-Hare seeds 10115/10124 die at
tick 1300, and saving seed 10219 dies at tick 1200. The replay harness evaluates
scheduled actions at that exact terminal tick, so those requests are rejected
after failure, with no charge. They do not show active ordinary-round controls
or a live shop failing. The exact records are retained separately.

These counts describe the current panel. Earlier 35-Fox/cohort results differ
in multiple rules and inputs and are not a controlled before/after estimate
of the 20-Fox change. No new balance threshold, tuning bundle or content
addition is approved by this report.

## Timing and evidence locations

The 600-process comparison used eight workers and took **181.668164 seconds**
of measured wall time, including launch and per-report reads. Aggregate measured
replay time was **1,331.7622599 seconds** across concurrent processes. These
exclude export, Unity parity, setup and later analysis. No new human decision
times were collected; Bevin's earlier informal play feedback remains separate.
Environment: Unity 6000.4.6f1, Windows, Intel i7-13700K; the frozen manifest records
the .NET SDK and full runner dependency hashes.

- Current parity bundle: `artifacts/field-data-t14/verified-20261019/`.
- Unity parity result: `artifacts/unity-tests-20261019-155907/EditMode-results.xml`.
- Current study bundle: `artifacts/field-data-t15/verified-20261019/`.
- Its `frozen-manifest.json`, `execution-record.json`, `validation-record.json`,
  `paired-summary.json`, `paired-outcomes.json`, `cohort-summary.json`,
  `output-hashes.json`, and `terminal-boundary-rejections.json` hold provenance
  and summaries. `cohort-200x3/` retains all 600 reports and the index.
- `run-study.ps1` refuses to overwrite a cohort; a deliberate repeat needs a
  new output directory and a new freeze. `validate-and-summarize.py` contains
  the assertions used for this accepted run.

To repeat direct parity, set `CELLSIM_FIELD_DATA_PARITY_MANIFEST` to the current
parity manifest's absolute path, then use `CellSim.ps1 -Command Test -Execution
Clean -Mode EditMode -TestFilter FrozenJourneyActionsMatchUnityAtEveryDecisionAndTerminalState`.
The manifest paths identify local files; move the complete bundle and update
path references deliberately before attempting cross-machine reproduction.

## Handoff

Automated evidence for FD-T14/T15 is refreshed for current rules. FD-T13's
independent whole-flow acceptance and FD-T16's recorded team self-play and
accept/revise remain open. Use the [self-play review](../../../FIELD_DATA_SELF_PLAY_REVIEW.md)
to make the human decision; no numeric gate is imposed here. A bounded next
investigation could examine why repeated additions lead to later extinction,
but it is not run or approved as tuning by this refresh. No Trello changes,
commit or push were made during this work.
