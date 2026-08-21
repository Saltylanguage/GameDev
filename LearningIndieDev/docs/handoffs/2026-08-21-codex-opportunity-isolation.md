# Controlled Fox attack-opportunity isolation result

## Verdict

**NOT ISOLATED**

## Mechanism

**M5 — unexpected / mixed control result.** The deterministic schedule was
paired exactly, but the valid Fox-to-Hare contact set diverged after the arms
began producing different combat outcomes. The resulting eligible/attempt
exposure difference is large enough to explain the mortality result, so the
Block+2 arm cannot be interpreted as an accuracy-only comparison.

## Runtime definition

Natural flow is:

```text
visible diet target
→ adjacent target marks Fox Attacking
→ ResolveAttacks scans the authored attack pattern
→ source target is rechecked in next grid
→ one target resolves for that attacker in the tick
```

There is no separate attack cooldown. Hunger, behavior state, movement,
target availability, and combat deaths all affect later opportunities.

## Isolation implementation

The diagnostic-only `fixed-rate-diagnostic` mode polls one global slot every
three simulation ticks (`seed % 3 == 0`). On a scheduled slot it enumerates all
currently valid creature attacker/contact candidates and selects one using a
seed-indexed deterministic index. The schedule does not consume simulation RNG
and is independent of `UpgradeId`. Only the natural `Attacking`/`ShouldForage`
gate is bypassed; movement, aging, metabolism, resource regrowth, starvation,
reproduction, target validity, opposed-roll randomness, and death handling stay
natural.

Each run records scheduled slots, eligible candidates, executed attempts, and
unfulfilled slots by reason. Normal mode remains the default and unchanged.

## Accepted controlled runs

All four arms used the same Forest Edge scenario, opposed-roll combat, the same
20 calibration seeds (`10100–10119`) and 20 held-out seeds (`10125–10144`), for
80 runs total. The earlier 30-tick runs were discarded as non-evidence because
they sampled too few contacts; see the design record for that correction.

### Calibration `10100–10119`

| Metric | None | Block +2 | Delta |
| --- | ---: | ---: | ---: |
| Scheduled slots | 1,334 | 1,334 | 0 |
| Eligible opportunities | 99 | 115 | +16 |
| Executed attempts | 99 | 115 | +16 |
| Unfulfilled: no target | 1,235 | 1,219 | -16 |
| Fox hit rate | 68.687% | 60.870% | -7.817 pp |
| Successful hits | 68 | 70 | +2 |
| Fox-caused Hare deaths | 68 | 70 | +2 |
| Lethality per hit | 100% | 100% | 0 pp |
| Hare total deaths | 622 | 616 | -6 |
| Hare starvation deaths | 486 | 486 | 0 |
| Mean Hare population | 17.843 | 17.201 | -0.642 |
| Hare AUC | 3,586.45 | 3,457.45 | -129.00 |
| Final Hare population | 23.65 | 22.15 | -1.50 |

### Held-out `10125–10144`

| Metric | None | Block +2 | Delta |
| --- | ---: | ---: | ---: |
| Scheduled slots | 1,333 | 1,333 | 0 |
| Eligible opportunities | 120 | 135 | +15 |
| Executed attempts | 120 | 135 | +15 |
| Unfulfilled: no target | 1,213 | 1,198 | -15 |
| Fox hit rate | 70.000% | 63.704% | -6.296 pp |
| Successful hits | 84 | 86 | +2 |
| Fox-caused Hare deaths | 84 | 86 | +2 |
| Lethality per hit | 100% | 100% | 0 pp |
| Hare total deaths | 678 | 682 | +4 |
| Hare starvation deaths | 525 | 529 | +4 |
| Mean Hare population | 18.800 | 18.670 | -0.130 |
| Hare AUC | 3,778.75 | 3,752.60 | -26.15 |
| Final Hare population | 22.65 | 22.40 | -0.25 |

Attempt mismatches occur in four calibration pairs (`10109, 10114, 10117,
10118`) and seven held-out pairs (`10126, 10130, 10133, 10138, 10139, 10140,
10142`). The Block+2 arm has more eligible attempts in both groups; therefore
the observed +2 Fox deaths cannot be assigned to opposed-roll accuracy alone.

## Fox-side safety

Fox starting population remained 4 in every arm. Calibration mean final Fox was
`2.20 → 2.25`, with extinction `5% → 5%`; held-out mean final Fox was
`2.55 → 2.55`, with extinction `0% → 0%`. Fox starvation was `42 → 41`
calibration and `34 → 35` held-out. These are context telemetry, not balance
targets.

## Accounting and tests

- Food reconciliation failures: `0`.
- Reproduction reconciliation failures: `0`.
- Combat reconciliation failures: `0`.
- Opportunity accounting failures: `0`.
- Focused/updated EditMode: **145/145 passed**, artifact
  `artifacts/unity-tests-20260821-013713/EditMode-results.xml`.
- Full suite: EditMode **145/145 passed**; PlayMode **4/6 passed**, with the
  same two pre-existing Noesis `TextureSource` native-pointer failures as the
  prior baseline. Artifact directory:
  `artifacts/unity-tests-20260821-014601/`.

## Evidence

Accepted calibration:

- `artifacts/cellular-experiment-20260821-014211/report.json`
- `artifacts/cellular-experiment-20260821-014211/analysis.md`
- `artifacts/cellular-experiment-20260821-014242/report.json`
- `artifacts/cellular-experiment-20260821-014242/analysis.md`
- `artifacts/cellular-experiment-20260821-014242/comparison.md`

Accepted held-out:

- `artifacts/cellular-experiment-20260821-014318/report.json`
- `artifacts/cellular-experiment-20260821-014318/analysis.md`
- `artifacts/cellular-experiment-20260821-014351/report.json`
- `artifacts/cellular-experiment-20260821-014351/analysis.md`
- `artifacts/cellular-experiment-20260821-014351/comparison.md`

Non-evidence control checks are retained at `20260821-013330` through
`20260821-013509` (first-contact bias) and `20260821-013752` through
`20260821-013929` (30-tick underpowered sampling).

## Recommended next experiment

Implement **paired lockstep opportunity replay/intersection**: generate one
shared Fox→Hare opportunity schedule from the two arms' common eligible-contact
intersection at each tick, then apply that same valid opportunity to both arms
and record skipped intersection slots. This is the single highest-information
next step because the current fixed-rate mode proves schedule equality is easy,
but arm-local target eligibility is the remaining confounder. Do not change
Block+2, damage, resources, reproduction, capacity, or starting populations.

## Repository state

- Branch: `BevBranch`.
- Implementation revision: `83989e0` (`Increase diagnostic contact sampling cadence`).
- The handoff is committed on top of the implementation; working tree is clean.
- Origin was aligned at `ab5fc89` before this goal; the branch is now four commits
  ahead after the implementation/design/handoff commits. No push was performed.

## Trello status

Card 59 remains **In Progress** and Card 29 remains **Backlog & Ideas**. The
controlled result is not a production balance validation and does not justify
moving either card. Trello comments are still pending action-time confirmation
before posting.
