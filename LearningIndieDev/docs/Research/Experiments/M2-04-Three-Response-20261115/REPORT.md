# M2-04 three-response matched diagnostic — factual report

- **Experiment:** `M2-04-THREE-20261115` ([approved contract](CONTRACT.md)).
- **Report ID:** `M2-04-THREE-20261115-R1`.
- **Execution:** November 15, 2026; Unity `6000.4.6f1`, Clean lane.
- **Status:** Four arms and 48 assigned seed runs completed. All four herbivore stat validations returned `VALIDATED_WITH_LIMITATIONS`. No M2 acceptance or production balance value follows from this report.

## Inputs and provenance

The four arms used the current Forest Edge asset, a closed 36×20 grid, 400 Plants, 55 Hares and 20 Foxes, neutral Genome, opposed-roll combat, natural attack opportunities, `bev-experimental`, and seeds 10100–10111. Each report contains 24 phases of 100 ticks, grouped as four six-phase nodes. The control chose no Mutation. Each response arm acquired one rank at tick 100 and retained it without later actions. The CellSim schedule is a mechanics proxy; it does not reproduce a player's offer, choice, shop, or input flow.

The source was `codex/s6-predator-first` at `f4c1ee34e5174224f2dfe1161083b53839f4129f` plus the frozen [dirty source patch](../../../../artifacts/m2-04-three-response-20261115/source.patch), SHA-256 `fd5e4476f11763910896f5677a46daa593345a2279959332f8f973a9246ec970`. The [source identity](../../../../artifacts/m2-04-three-response-20261115/source-identity.json) also records the scenario asset SHA-256 `0ece03f19616942f052b16f07fe171d9bd73663cc1deea26604c75438024f69e`. The relevant tracked diff still matched that patch after execution, accounting for CRLF export. All four reports have the same ruleset fingerprint `02f7388f465af4c8cd809e432771c40b58688e135d18597c7e4f86e80488e3f6` and the same fixed inputs and neutral Genome fingerprint. Acquisition timelines record the expected tick and upgrade ID in every response seed. At each node boundary, Foxes and Plants reset to 20 and 400 while the Hare count carried forward.

| Arm | Raw report and command | SHA-256 of report | Acquisition |
| --- | --- | --- | --- |
| No Mutation | [control artifacts](../../../../artifacts/cellular-experiment-20261115-103544/) | `d692a35511f2…` | None |
| Escape Instinct | [escape artifacts](../../../../artifacts/cellular-experiment-20261115-104116/) | `10785eef9acd…` | `threat-exposure`, tick 100 |
| Tough Hide | [Tough Hide artifacts](../../../../artifacts/cellular-experiment-20261115-104540/) | `32c3fccc2b9e…` | `tough-hide`, tick 100 |
| Reproductive Drive | [reproduction artifacts](../../../../artifacts/cellular-experiment-20261115-105023/) | `64816d64e3f1…` | `reproductive-drive`, tick 100 |

The full report hashes, fixed-input fields, per-seed counts, and first Hare-extinction ticks are in [METRICS.json](METRICS.json). Each artifact directory retains the manifest, exact command request, raw `report.json`, compressed report, stat validation, and Unity log. The raw local artifacts are ignored by Git.

## Recorded outcomes

All 48 runs reached zero Hares before node two. CellSim continued the nonplayer simulation to tick 2400 after Hare extinction; the first zero-Hare tick below is the terminal *player-route* observation, not the run's final tick. The twelve assigned seeds are a diagnostic panel, not a population-wide success estimate.

| Arm | Hares alive at node-one close | Mean first zero-Hare tick | Mean Hare births | Mean Hare starvation deaths | Mean Hare combat deaths | Mean Fox attack attempts | Mean Fox nonlethal hits |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| No Mutation | 0/12 | 444.7 | 33.7 | 42.7 | 46.0 | 79.3 | 0.0 |
| Escape Instinct | 0/12 | 444.6 | 29.0 | 40.4 | 43.6 | 76.2 | 0.0 |
| Tough Hide | 0/12 | 425.8 | 25.8 | 37.9 | 42.9 | 123.8 | 28.2 |
| Reproductive Drive | 0/12 | 441.3 | 33.9 | 43.4 | 45.5 | 76.9 | 0.0 |

The Tough Hide report records 338 Fox nonlethal hits across 12 seeds, versus zero in each other arm. Its acquired loadout grants +2 maximum Hare health. The Reproductive Drive report records 407 Hare births, versus 404 in control; the acquired loadout grants +0.1 reproduction chance. Escape Instinct records 348 births and 523 Fox kills, versus 404 and 552 in control. These are observed totals; they are not direct Mutation-effect counts.

First zero-Hare tick by paired seed:

| Seed | Control | Escape | Tough Hide | Reproduction |
| ---: | ---: | ---: | ---: | ---: |
| 10100 | 383 | 408 | 448 | 355 |
| 10101 | 504 | 514 | 477 | 504 |
| 10102 | 445 | 451 | 405 | 418 |
| 10103 | 281 | 392 | 460 | 281 |
| 10104 | 411 | 382 | 314 | 411 |
| 10105 | 447 | 447 | 424 | 447 |
| 10106 | 538 | 472 | 433 | 504 |
| 10107 | 473 | 501 | 443 | 474 |
| 10108 | 493 | 366 | 312 | 493 |
| 10109 | 344 | 392 | 417 | 412 |
| 10110 | 530 | 537 | 566 | 509 |
| 10111 | 487 | 473 | 411 | 487 |

## Validation limits and deviations

- Report SHA-256 matched each manifest. All 48 seed reports have 24 phases, 2400 simulated ticks, the assigned seed, the opening populations, and expected acquisition. The herbivore stat validator passed all 48 available seed ledgers: 84 raw cross-checks passed and 36 were unavailable in each arm. The unavailable checks concern independent event lists for HPS, EHS, and ECN; the serialized stat lines and population histories remain in the raw reports.
- The approved contract expected the *run* to stop on Hare extinction. This runner instead continues to tick 2400 with zero Hares and resets Foxes and Plants at each later node. This report derives first Hare extinction from each tick-level population history and does not count empty later phases as a playable journey.
- The CellSim report schema does not export `MutationEffects.AvoidedContacts`, `DecisiveFullHealthHits`, or `ChangedReproductionRolls`. Their controlled mechanic tests passed in the separate [November 15 validation batch](../../../handoffs/2026-11-15-0940-codex-m2-validation-batch.md), but this matched panel cannot assign a direct effect count to each arm.
- The headless schedule bypasses actual offer availability and player understanding. The 12 seeds and one-rank, no-later-action policy cannot establish that a different legitimate player strategy would fail or succeed. The separate authored UI route checks for seeds 10100 and 10101 also failed before node one ended.

Interpretation and proposed next work are kept in [AI_ANALYSIS.md](AI_ANALYSIS.md). Josh's current [human decision](HUMAN_DECISION.md) is `Needs Review`.
