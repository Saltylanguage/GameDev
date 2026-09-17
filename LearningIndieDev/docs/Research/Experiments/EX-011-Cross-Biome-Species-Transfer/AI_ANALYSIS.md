# EX-011 AI analysis

**Status:** Scored; recommendation awaiting human decision
**Prediction:** [`HELD_OUT_PREDICTION.json`](HELD_OUT_PREDICTION.json)
**Factual result:** [`REPORT.md`](REPORT.md)

## Prediction verdict

The preregistered direction was correct. Development mean FPO increased by
70.8 Deer and held-out mean FPO increased by 52.4. All 25 paired differences
were positive, exceeding both panel thresholds.

This is one successful bounded prediction. It is not enough to claim model
calibration or reliable transfer in general.

## What appears to transfer

Within this contract, the direction of the combined launch-time intervention
transferred from Forest Edge/Hare to Open Range/Deer: adding movement speed
`+0.5` followed by crowding tolerance `+1` increased final focal-herbivore
population relative to a same-seed control.

The evidence supports that sentence only with the scenario, species, values,
order, run window, combat settings, and seed panels attached.

## What did not transfer

- The Hare effect size was not transferred or predicted.
- The result does not identify whether movement, crowding tolerance, or their
  interaction produced the change.
- It does not establish transfer to another biome, species, upgrade value,
  order, or continued-world schedule.
- It does not transfer a claim that “higher APS means more final population.”
  APS decreased in both panels while FPO increased.
- It does not establish that the ecological outcome is desirable to players.

## Mechanism interpretation

Both panels combined higher final populations with far fewer crowding and
starvation deaths, but also fewer births. This is consistent with reduced
population churn. It is not a causal decomposition because the intervention
contains two changes and the experiment has no single-variable arms.

Held-out predator contact also differed: PREY and ECN increased while crowding
and starvation decreased. That makes a simple “the upgrade merely avoided
predators” explanation unsupported.

## Workflow findings

The transfer attempt exposed two valuable assumptions in the tooling:

1. Stat-Line export and bundle requirements were hard-coded to species ID
   `hare`, even though the runtime correctly generated the same contract for
   Deer. The wrapper now detects actual Stat-Line records instead.
2. The artifact summarizer initially treated metric-dictionary identity as an
   unknown field. It now retains the dictionary ID, version, file, and hash.

These are useful transfer failures: the experiment generalized the evidence
pipeline as well as testing the ecological finding.

## Recommendation to the human owner

Accept the directional result as **bounded transfer evidence** for this exact
contract. Keep the assistant in an abstaining posture for other species,
scenarios, values, and metrics until they receive their own evidence.

Do not promote the two-value bundle as a player upgrade or balance rule from
this experiment alone. The next scientific follow-up, if wanted, is a
pre-registered factorial arm set that separates movement, crowding tolerance,
and their interaction. That follow-up is not required to close this smoke test.
