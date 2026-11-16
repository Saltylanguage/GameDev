# S6 Fox carcass-value diagnostic — experiment contract draft

**Experiment ID:** `S6-FOX-CARCASS-VALUE-20261111`  
**Decision owner:** Josh  
**Status:** Scope approved November 12; initial panel and the conditional scale extension completed November 13. No production balance decision is recorded.
**Feature:** Hare carcasses and Fox energy balance

## Question

In one 600-tick node (the agreed 48-hour comparison window), how many Hares
does a starting Fox population need to avoid starvation, and does that
relationship stay similar as both populations scale up?

## Hypothesis

The current energy ledger predicts one 96-energy Hare carcass can replace the
48 energy that two Foxes each lose in 600 ticks. That gives a provisional
starting ratio of **two Foxes per Hare** for maintenance alone. Actual outcomes
may differ because Foxes must encounter, kill, and share prey; both species can
reproduce; and Foxes stop hunting above their forage threshold. This is a
calorie-based prediction, not an ecological balance claim.

## Scope and fixed inputs

| Input | Proposed value |
| --- | --- |
| Scenario and map | Authored Forest Edge, 36×20, nonwrapping |
| Simulation | One fresh Species Simulation node, exactly 600 ticks |
| Player species | Hare (`hare`); no player actions or population interventions |
| Starting Plants | 400 in every arm |
| Genome / Mutations | Neutral Genome; no Mutations or upgrades |
| Fox and Hare behavior | Current production rules, including natural movement, predation, feeding, and reproduction |
| Fox energy candidate | Start 48; maximum 96; metabolism 2 every 25 ticks; effective forage threshold 72 |
| Hare food value | 96 energy per lethal carcass |
| Initial seed panel | Matched seeds 62000–62019 across all five arms; 100 runs |

Only starting Fox and Hare counts vary. Plants, map, scenario terrain,
simulation rules, time window, and seed are held fixed within each matched seed.
Fox reproduction stays enabled and will be reported separately so its energy
and population effects are visible rather than hidden in the maintenance
estimate. The approved uncertainty condition was triggered: the three 2:1
scale arms were extended to 100 matched seeds each with seeds 62021–62100
(240 additional runs). The two fixed-six-Fox controls remain at 20 seeds each.
Seed 62020 was reserved for a separate smoke run and excluded from both panels.

## Proposed population matrix

| Foxes | Hares | Foxes per Hare | Purpose |
| ---: | ---: | ---: | --- |
| 2 | 1 | 2.0 | Small-scale calorie-ratio arm |
| 4 | 2 | 2.0 | Middle-scale calorie-ratio arm |
| 6 | 3 | 2.0 | Larger-scale calorie-ratio arm |
| 6 | 1 | 6.0 | Strong prey scarcity at fixed Fox count |
| 6 | 6 | 1.0 | More prey at fixed Fox count |

This is a first diagnostic, not an exhaustive search for an exact ratio. If
seed variation or confidence intervals leave the ratio unclear, expand only
the relevant arms to 100 matched seeds before making a balance decision.

The project's Predictive AI research is not used here: the new carcass-sharing
rule has no matching held-out calibration evidence. A small direct simulation
panel is the appropriate first evidence source.

## Success, failure, and interpretation criteria

- **Calorie hypothesis supported locally:** the source rules and the direct
  600-tick mechanics check confirm that two Foxes can share one Hare's 96
  energy and each finish a no-reproduction upkeep fixture alive at their
  starting energy.
- **Ecological ratio supported directionally:** the three 2:1 scale arms show
  broadly similar Fox starvation outcomes, while the fixed-six-Fox arms show
  lower starvation as Hare count rises. Report uncertainty; overlapping or
  wide intervals mean inconclusive, not a pass.
- **Ratio hypothesis challenged:** Fox starvation differs materially among
  the 2:1 scale arms, or a higher Hare count does not improve Fox food access
  or survival at six Foxes. Diagnose encounter rate, kills, feeding, and births
  before changing the ratio.
- **Decision limit:** this diagnostic does not approve a universal ratio or
  production balance value. It identifies the next smallest comparison.

## Required measures and report bundle

Report per seed and by arm:

- Fox and Hare openings/closings; Fox births, starvation deaths, and other
  deaths; Hare births, combat deaths, and other deaths.
- Fox combat opportunities, attacks, lethal kills, and total Fox energy
  consumed from carcasses. Derive carcass energy created as `96 × Fox kills`
  and remaining energy at tick 600 as created minus consumed, since this first
  pass has no carcass decay or other Fox food source.
- Hare births and Fox births separately, to distinguish prey supply from
  predator reproduction.
- Per-seed outcomes for any Fox starvation death, any Foxes remaining at tick
  600, and Hare extinction, each with a 95% Wilson interval. These describe
  seed outcomes; they do not imply that every starting Fox individually
  survived, because Foxes can reproduce within the node.
- Medians/ranges for Fox births, Fox starvation deaths, Hare births, Fox kills,
  and carcass energy consumed per starting Fox. Use cause-specific Hare deaths
  when available; disclose any telemetry gap instead of inferring causes from
  the final population count.
- Scenario, actual starting counts, seed, source and snapshot hashes,
  ruleset fingerprint, runner/assembly identity, and run status.

Keep facts in `REPORT.md`, interpretation in `AI_ANALYSIS.md`, and retain
raw per-seed runs and manifests under ignored `artifacts/`. Do not rewrite
older reports or combine their Fox energy values with this changed model.

## Execution and results

**Josh approval:** The displayed five-arm, 20-seed diagnostic scope was
approved on November 12. The conditional expansion rule above was part of the
approved contract and was applied after the initial scale results left the
ratio uncertain.

- Unity resolved the local `Genome.meta` conflict to the branch's canonical
  GUID `c576ac05bbb49f7488aa54f8ac574eaa`; there is no outstanding tracked
  GUID diff.
- Focused `SpeciesDomainTests` passed **160/160**. The full Clean run recorded
  EditMode **764/773 passed, 8 failed, 1 skipped** and PlayMode **92/103 passed,
  4 failed, 7 skipped**. The failure cases and raw results are linked from the
  [factual report](REPORT.md#provenance-and-validation); the full suite is not
  a pass.
- The first fresh batch attempt was rejected because no-preset Hare `PREY`
  telemetry did not reconcile with Fox kills. No run from that attempt was
  accepted. Commit `8a92425866b4b5ef3cd9c77968f53554e17f63b8` corrected the
  telemetry condition; a fresh snapshot and four Unity reference runs then
  reconciled the population ledgers.
- The original five-arm panel completed **100/100** runs. The three 2:1 scale
  arms were then expanded by **240/240** runs. The combined evidence contains
  340 accepted runs, with 100 seeds per 2:1 scale arm and 20 per fixed-six-Fox
  control. See the [factual report](REPORT.md), [AI analysis](AI_ANALYSIS.md),
  [expanded per-seed summary](../../../../artifacts/S6-Fox-Carcass-Value-20261113-153600/expanded-validated-summary.json),
  and retained [initial plan](../../../../artifacts/S6-Fox-Carcass-Value-20261113-153600/plan.json) /
  [scale-extension plan](../../../../artifacts/S6-Fox-Carcass-Value-20261113-153600/scale-extension-plan.json).
- Snapshot source commit: `8a92425866b4b5ef3cd9c77968f53554e17f63b8`; source hash:
  `b2cf7f809a3108b33d6c6878ec9c5006ed3173bccb96ae2eaf57c99cb901fd7f`;
  scenario fingerprint:
  `a8c005fdb3696275f469699cbf739da337e656b35f912abcaee48f88d18b253b`.
- The .NET 8-targeted portable runner used installed .NET 10.0.4 with
  `DOTNET_ROLL_FORWARD=Major`. Predictive AI was not used; there is no matched
  held-out calibration for this carcass rule.

The 2:1 scale outcomes differ materially even after expansion, so the result
does not establish a universal ecological ratio. No production value was
changed. Any next value decision should be made after review of the report and
analysis, with the uncertainty and scope limits retained.
