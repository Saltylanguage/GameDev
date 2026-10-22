# Field Data opening: Hare collapse before the first purchase

- **Date:** October 18, 2026
- **Question:** Why do Hares disappear before the player can respond, and which small change is worth a playtest?
- **Status:** Completed diagnostic screen. Assets were unchanged during research; Bevin subsequently approved the 20-Fox candidate as the next authored playtest default on October 18. Frozen baseline results below retain the original 35-Fox setup. See the [change record](../../../handoffs/2026-10-18-fd-20-fox-playtest-default.md).
- **User observation:** Hares collapse too quickly. Bevin accepted the improved map presentation; this is a separate population/pacing concern.
- **Revision:** local `BevBranch`, base `acc371a`, with the October 18 route/reward corrections.

## Main finding

The authored opener removes about 93% of the Hare population before the first
paid decision. All 20 baseline seeds reached the first payout, but none earned
the 10 FD required for a population intervention. Free species rewards are
available then, so this is a paid-response bottleneck rather than an absence
of every possible choice.

Reducing the starting Fox population to 20 improves the opening enough to test:
18 of 20 seeds could afford an intervention after round one. It does not solve
the entire loop. Later population crashes remain common, and most successful
journeys still finish with one Hare.

## Frozen setup

- Authored Forest Edge: 36x20; 400 Plants, 55 Hares, 35 Foxes.
- 0.2-second interval; wrapping **off**, matching the authored asset and the
  validated Desktop route. The earlier desired wrapping design is not the
  current authored scenario setting.
- Hare starting energy 12; other species rules unchanged unless listed below.
- Seeds 10100-10119, identical across four conditions.
- Eighteen 100-tick rounds, through tick 1800 or selected-species extinction.
- No upgrades, purchases, active Genome or income modifiers. The ecology
  screen uses 18 continuous no-op phases; actual Field Data replays confirm
  the baseline and 20-Fox candidate with all free choices skipped.
- 80 validated screen runs. The reported batch invocation took 22.17 seconds;
  this is automated execution time, not playtime or human decision time.

## Results

Mean Hare populations include zero after observed extinction. These are small
matched diagnostic cohorts, not universal success probabilities or a numeric
acceptance gate.

| Condition | Mean Hares at first payout | Can afford 10 FD after round 1 | Hares present at tick 600 | Hares present at tick 1800 |
| --- | ---: | ---: | ---: | ---: |
| Current: 35 starting Foxes | 3.80 | 0/20 | 5/20 | 4/20 |
| 20 starting Foxes | 16.55 | 18/20 | 13/20 | 5/20 |
| 10 starting Foxes | 32.45 | 20/20 | 12/20 | 1/20 |
| Current Foxes; Hare starting energy 24 | 3.30 | 1/20 | 11/20 | 5/20 |

Average first-round Hare events:

| Condition | Predator deaths | Starvation deaths | Births |
| --- | ---: | ---: | ---: |
| Current | 46.30 | 4.90 | 0.00 |
| 20 Foxes | 30.90 | 7.55 | 0.00 |
| 10 Foxes | 16.25 | 6.30 | 0.00 |
| Hare energy 24 | 30.05 | 23.20 | 1.55 |

Predation dominates the baseline opening. Raising starting energy changes the
feeding/reproduction trajectory and shifts losses toward starvation; it does
not improve first-payout population or affordability in this screen. No claim
is made that this identifies a code defect in feeding or reproduction.

Ten Foxes produces a larger early population but a later crash: median Hare
counts across the six round boundaries are 33, 80, 73.5, 57.5, 13.5 and 1.5.
The 20-Fox medians are 17, 8, 3, 2, 1 and 1. Means conceal substantial
variation: at tick 1800, the five 20-Fox survivors are 194 Hares in seed 10100
and one Hare each in four other seeds. All four baseline survivors have one
Hare. Survival-only route completion is therefore insufficient evidence of a
healthy or engaging population loop.

The retained median chart is
`artifacts/field-data-balance-20261018-opening/opening-populations.png`.
Other-species populations after early termination are censored, not inferred
extinct. Cohort reports retain full mortality/reproduction statistics and
reconciled Hare population accounting.

## Recommended bounded playtest

Use **20 starting Foxes** as a temporary test candidate, with Plant 400, Hare
55 and all other settings unchanged. Compare the current 35-Fox opener and
the candidate on seeds **10100 and 10102**, through the first six-round node.
Four such simulations take about eight minutes at nominal speed before
decision pauses. Match the spending/upgrade approach when comparing population
settings, or retain the changed choices when interpreting differences.

Seed 10100 gives a readable contrast: the baseline has 5 Hares at round one;
the candidate has 25 and follows 25, 27, 16, 19, 18, 21 across the first node.
Seed 10102 exposes the remaining difficulty: the candidate follows 16, 3, 1,
1, 1, 1 without intervention. Do not use only the favourable seed to judge it.

Judge whether the opening leaves time and funds to act, whether those actions
matter, and whether the later crash is understandable. Do not adopt 20 Foxes
as a final balance value from this screen. The next simulation question is the
later food/reproduction cycle, with the opening predator pressure held fixed.
Human play observations and a tuning decision have not been supplied yet.

## Reproduction and evidence boundaries

All files below are under `artifacts/field-data-balance-20261018-opening/`:

- `authored-forest-edge.json`, `authored-screen-plan.json`, `frozen-runner/`:
  fresh live-Editor export and pinned current runner. Source hash:
  `8e2047fe13efc2f2e209cc456ecac5db4df9efd296ae2fb28fba0a1afbc0a0a8`.
- `authored-unity-reference.jsonl` and `authored-pilot/`: two current Unity
  reference cases match the standalone runner exactly for state, counters,
  choices and timing, with the existing derived-rate tolerance.
- `authored-screen/`: all 80 runs, 16 validated chunks, exact seed coverage,
  status and output hash; `analysis.json`, `per-seed.csv`, `analyze.py` retain
  the checked extraction and population reconciliation.
- `resolved-journey-confirmation/verification.json`: 40 actual Field Data
  replays (20 baseline, 20 candidate) match the screen at every reached
  decision boundary and terminal tick; first payouts match Hare counts and
  no free income offers appear. Sparse omitted zero counts are normalized
  during comparison, not treated as different ecology.
- `fox-20-resolved.json`: the candidate's starting population is frozen into
  resolved data, with its own fingerprint. Existing `journey-replay` does not
  apply scientific argument overrides; an argument-only candidate would replay
  the original scenario. The first `journey-confirmation/` attempt records that
  tooling limitation and is explicitly excluded as candidate evidence.
- `unity-journey-check.json` and `unity-journey-check-verified.json`: seed 10100
  for baseline and candidate also match inside Unity's actual session code at
  every reached boundary, final state and balance. The CLI transport timed out
  at five seconds; the bounded evaluation completed and wrote these results,
  which were read back and compared. The Editor returned ready, on MainMenu,
  outside Play Mode, with the scene unmodified.
- An initial two-seed wrapping-enabled pilot is labeled **not the authored
  baseline** and excluded from the comparison. Historical October 17 artifacts
  were not modified or reused as current-source results.
- `evidence-manifest.json` hashes the main inputs, outputs and runner.

This work runs domain/session simulations, not human play or a new rendered
acceptance pass. No production tuning, Trello status change, commit or push
was performed. Team self-play and the explicit accept/revise decision remain
open in the [review record](../../../FIELD_DATA_SELF_PLAY_REVIEW.md).
