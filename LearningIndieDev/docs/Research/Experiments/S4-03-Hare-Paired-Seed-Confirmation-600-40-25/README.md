# S4 Hare paired-seed threshold confirmation: 600/40/25

[Experiment registry](../README.md) | **Status: complete locally; human review pending** | Date: 2026-10-06

## At a glance

On 200 fresh matched seeds, the no-choice Hare run cleared the defined all-species threshold in 25/200 runs (12.5%). Seed Dispersal cleared 58/200 (29%); Gardeners with Efficient Digestion before Seed Dispersal cleared 50/200 (25%); the reverse order cleared 57/200 (28.5%). Each intervention beat the matched control on more seeds than it lost. The two Gardeners orders did not show a clear order advantage. These results support Seed Dispersal as a promising challenge upgrade in this setup, but do not approve skill tuning or a production population. See the [factual report](REPORT.md) and separate [analysis](ANALYSIS.md).

## Question and hypothesis

On fresh common seeds, do Seed Dispersal and either Gardeners order raise the chance of clearing the Hare player's Forest Edge node above skip-all control under Bevin's all-species threshold? The frozen hypothesis was that each intervention would improve clear rate; order might not matter.

## Frozen method

- Production Forest Edge scenario; 36x20; wrapping enabled.
- 600 Plants / 40 Hares / 25 Foxes; Hare player.
- Six 100-tick phases; maximum 600 ticks; 0.1-second step.
- Natural attack opportunities, opposed-roll combat, `bev-experimental`; coupled responses off; no extra Genome or Hare reinforcements.
- Four arms, 200 identical seeds each: skip-all control; Seed Dispersal at tick 100; Gardeners Efficient Digestion then Seed Dispersal at ticks 100/200; Gardeners Seed Dispersal then Efficient Digestion at ticks 100/200.
- Fresh seed interval 14000-14199, disjoint from the prior screen's 13000-13099.
- Clear at tick 600 only when Plants > 0, Hares >= 5, and Foxes >= 5. Exactly five animals qualifies. Extinction or any smaller final population is a failed clear.

## Evidence and limits

Unity's existing command-line experiment runner produced full JSON/CSV run histories in the ignored artifact directory. Four one-seed pilots were separate from the 800 main runs. An independent streaming analyzer checked every assigned seed, tick-zero population, wrap/grid configuration, continuous tick history, phase window, scheduled first acquisition of each distinct skill, Hare slashline formulas and validity states, final-population reconciliation, and Seed Dispersal accounting. The raw data and validation are in [`artifacts/s4-paired-confirmation-20261006-600-40-25-wrapped-200/`](../../../../artifacts/s4-paired-confirmation-20261006-600-40-25-wrapped-200/).

This is one starting population, wrapped topology, Hare player, skill level, six-phase node, and 600-tick horizon. It does not test randomized player offers, adaptive choices, other species' upgrades, another starting population, continued nodes, or long-term ecological stability. No gameplay values, authored assets, or production defaults were changed. See the [handoff](../../../handoffs/2026-10-06-1432-codex-s4-03-200-seed-threshold-confirmation.md). No commit, push, Trello update, or human balance decision is included.
