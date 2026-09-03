# EX-007 — How well did the prediction do?

**Status:** Scored against training and held-out reports; human decision pending
**Prediction ID:** `PRED-EXP-007-0001`

## The prediction in ordinary language

Before seeing any upgrade results, the AI predicted that:

1. Faster movement would leave somewhat more Hares alive.
2. The combined movement-plus-crowding upgrade would also help, probably by a
   little more.
3. Births would rise and starvation would fall.
4. Crowding deaths would fall when crowding tolerance was enabled.

The final-population and crowding predictions held up. The birth prediction was
weaker on new seeds, and the starvation prediction was clearly wrong.

### Measurement correction

The original prediction input used `PREY` as if it meant food or resource
gathering. In the simulation it means **Hares killed by carnivores**. That was
a definition error in the prediction setup, so the original PREY forecast is
excluded from causal scoring rather than presented as a valid food-access
prediction. The corrected interpretation is recorded in [REPORT.md](REPORT.md).

## What was right and what was wrong

| Prediction | Training seeds | New held-out seeds | Plain-English verdict |
|---|---|---|---|
| Faster movement increases final Hares | +5.8 | +4.6 | Right in both panels |
| Combined upgrade increases final Hares by 2–12 | +5.3 | +15.4 | Direction right; held-out size was larger than predicted |
| Faster movement increases births | +9.4 | +3.0 | Right, but weaker on new seeds |
| Faster movement reduces starvation | +3.3 starvation deaths | +2.4 starvation deaths | Wrong in both panels |
| Faster movement changes Hare deaths from fox attacks | +0.9 | -2.8 | Direction changed across panels; not a stable count effect |
| Combined upgrade reduces crowding deaths | -1.5 | -1.6 | Right in both panels |
| Combined upgrade reduces starvation | +1.9 | -0.4 | Wrong on training; roughly right on held-out |

Positive starvation numbers are worse because they mean more starvation deaths.
All changes are averages against the matching baseline seed; raw evidence is in
[REPORT.md](REPORT.md).

## What the misses teach us

- Faster movement changed activity and population, but it did not guarantee
  better survival. The extra movement may have increased exposure to other
  costs, or simply allowed more Hares to reach the point where they could die
  of starvation.
- Hare deaths from fox attacks were seed-sensitive: the count rose in the
  training panel and fell in the held-out panel. Meanwhile, pAVI (survival per
  recorded predator encounter) rose on average in both panels. This points to
  changed predator exposure and encounter timing, not a settled food-access
  explanation.
- The crowding-tolerance part behaved consistently for crowding deaths, but the
  combined population gain was much larger on held-out seeds. With no
  crowding-only arm, we cannot separate a true interaction from ordinary seed
  variability.

## Confidence check

The AI assigned 56% confidence to S1 and 62% to J1. Counting only whether the
direction was correct across the four predicted outcomes and two panels:

- **S1:** 5 of 8 checks were correct (62.5%).
- **J1:** 7 of 8 checks were correct (87.5%).

This is a useful first calibration note, not proof that the confidence numbers
are calibrated. We need many more experiments for that.

## What the AI missed

The prediction did not call out several meaningful telemetry changes:

- Faster movement changed fox encounter exposure and Hare predation deaths.
- The combined setting increased resource-finding and activity scores on
  held-out seeds.

These are observations to investigate, not recommendations to change balance.

## Review cost and limits

- No experimental reruns were needed after the environment was repaired.
- Human review time has not yet been measured.
- The result covers one scenario, one species, one short time window, and the
  exact catalog values tested here.
- The factual results do not decide whether an upgrade is fun, fair, or ready
  for production.

## Follow-up analysis: crowding tolerance first

The follow-up added the missing crowding-only arm and tested the reversed joint
sequence (`crowding-tolerance,faster-movement`). On the new held-out seeds
106–110, crowding tolerance alone raised final Hare population by **2.6** and
births by **8.2**, while reducing fox kills by **1.8**. The reversed joint arm
raised final population by **1.2**, births by **12.0**, and reduced fox kills by
**3.4**. Both upgrade arms had zero crowding deaths on average.

This is useful evidence, but it does not establish an order effect. The
original forward-order joint arm was held out on seeds 101–105, while the
reversed arm was held out on 106–110. A direct order claim requires both
sequences on the same held-out seeds. The training-panel aggregates matched
exactly, which is consistent with the current implementations being
order-independent for these two upgrades; a same-seed order check remains the
clean follow-up if order itself matters to the design question.

The added crowding-only arm also changes how the original joint result should
be read: the large held-out population gain for the earlier forward-order arm
cannot be credited to “interaction” without a same-seed crowding-only
comparison. The new panel shows that crowding tolerance can account for some
of the birth and survival shift by itself, while the combined arm's extra
movement mainly shows up in births and predator-contact outcomes rather than a
large final-population increase.
