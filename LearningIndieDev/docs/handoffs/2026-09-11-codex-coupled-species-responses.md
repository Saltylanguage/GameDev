# Coupled Hare/Fox responses

[Working state](../WORKING_STATE.md) | Status: implemented and focused-validated; balance result requires human review before expansion

- Owner: Codex, requested by Bevin
- Branch: BevBranch
- Baseline: `16ce99e`
- Date: 2026-09-11

## Delivered contract

- Added the default-off `Coupled Hare/Fox responses (experimental)` setting.
  It is editable only before an Expedition, persists with defaults, and is
  carried by `SpeciesExperimentalOptions`.
- At an enabled continuous boundary, the player pays for their selected legacy
  Hare/Fox upgrade; the deterministic counterpart response is free, applied to
  the other species at the same boundary, and recorded immediately after the
  player snapshot. Skip applies neither.
- The supported map is bounded to the five Hare and four Fox experimental
  upgrades. `Keen Senses`, plants, authored production assets, Genome, and
  Biome mode are excluded.
- Batch experiments accept `-CoupledSpeciesResponses true` only with
  `-ExperimentalFeatures bev-experimental`; reports and run provenance record
  the flag and counterpart snapshots. Each acquired snapshot now carries its
  immutable `source` and `triggeringUpgradeId`.

## Mapping

| Player selection | Free counterpart response |
| --- | --- |
| Hare Tough Hide | Fox Piercing Bite |
| Hare Threat Exposure | Fox Relentless Pursuit |
| Hare Efficient Digestion | Fox Hunt Urgency |
| Hare Reproductive Drive or Crowding Tolerance | Fox Brood Drive |
| Fox Piercing Bite | Hare Tough Hide |
| Fox Relentless Pursuit | Hare Threat Exposure |
| Fox Hunt Urgency | Hare Efficient Digestion |
| Fox Brood Drive | Hare Reproductive Drive |

## Validation

- `git diff --check` passed after the implementation edits.
- Unity EditMode: **239/239 passed** in
  `artifacts/unity-tests-20260911-210439/EditMode-results.xml`.
- Focused same-boundary PlayMode (including reward-card preview and immutable
  attribution): **1/1 passed** in
  `artifacts/unity-coupled-response-20260911-211606/PlayMode-results.xml`.
- The broad PlayMode suite remains unsuitable as a feature verdict: its earlier
  run had eleven unrelated failures from a pre-existing GalapagOS XAML error
  and invalid inherited preview starting populations. Those failures are not
  claimed as fixed here.

## CSR-5 matched 100-seed result

All arms used Forest Edge, Hare, `bev-experimental`, opposed-roll natural
opportunities, a 32x32 grid, seeds 1-100, and two 100-tick phases. The base
ruleset fingerprint was
`87e929478fde92686a7f9891a8bedf85561404c067bac590c19e4eff2ea2a3c2` for all
three reports. Independent Hare stat-line validation returned
`VALIDATED_WITH_LIMITATIONS` for every arm.

| Arm | Post-boundary Fox hit conversion | HAT / KIL | Hare PREY / pAVI | Mean final Hare | Extinctions |
| --- | ---: | ---: | ---: | ---: | ---: |
| No upgrade | 58.57% | 589 / 345 | 345 / 41.43% | 69.71 | 0 |
| Tough Hide only | 48.24% | 655 / 316 | 316 / 51.76% | 70.41 | 0 |
| Tough Hide + Piercing Bite | 52.52% | 615 / 323 | 323 / 47.48% | 69.61 | 0 |

The direct-mechanic gate passes: `48.24% < 52.52% <= 58.57%`. The coupled report
records `player-choice:tough-hide` then
`coupled-response:piercing-bite` at tick 100 in all 100 runs. This is bounded
mechanic evidence, not a recommendation to add another response pair without
review.

- No-upgrade: `artifacts/cellular-experiment-20260911-210625/report.json`
- Tough Hide-only: `artifacts/cellular-experiment-20260911-210913/report.json`
- Coupled: `artifacts/cellular-experiment-20260911-211203/report.json`
