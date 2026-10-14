# S4 Hare paired-seed screen: 600/40/25

[Experiment registry](../README.md) | **Status: complete locally; human review pending** | Date: 2026-10-05

## At a glance

Bevin requested a larger paired screen at 600 Plants / 40 Hares / 25 Foxes and selected 100 fresh common seeds per arm. The frozen screen contains 13 arms and 1,300 scheduled simulations on the 36x20 Forest Edge with wrapping enabled. Runs use seeds 13000-13099. The screen completed 1,300/1,300 runs without job failures; independent checks validated 7,219 actual phase windows. See the [factual report](REPORT.md) and [separate analysis](ANALYSIS.md). No balance or tuning decision is approved.

## Question and hypothesis

At this population and wrapped topology, how do the six individual Hare skills and three two-skill strategies (including both acquisition orders) compare with a matched skip-all control across a larger fresh seed panel?

The previous bounded 400/20/10 result should not be carried over as an expected direction. This screen measures outcomes without a pass threshold: strategy value may depend on population, topology and acquisition order. A paired comparison controls seed, but cannot isolate every mechanism in an evolved world.

## Frozen method

- Scenario: `Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset`; 36x20.
- Start: 600 Plants / 40 Hares / 25 Foxes; total 665 starting populations on 720 cells.
- Edge wrapping enabled, matching the visual-test configuration Bevin preferred. This is not the authored default.
- 600-tick maximum, six 100-tick phases, 0.1-second simulation step.
- Hare player; opposed-roll combat; natural attack opportunities; `bev-experimental` features enabled; coupled responses off; no Genome additions or reinforcements.
- 100 identical seeds per arm, 13000-13099. The seed interval is disjoint from the 10100-10119 strategy screen and the 11000-12019 population screen and recheck.
- Arms: skip-all control; General Movement; Threat Avoidance; Tough Hide; Crowding Tolerance; Efficient Digestion; Seed Dispersal; Trailblazer Movement → Avoidance; Trailblazer Avoidance → Movement; Warren Hide → Crowding; Warren Crowding → Hide; Gardeners Digestion → Dispersal; Gardeners Dispersal → Digestion.
- Singles receive one level-one skill after tick 100. Paired paths receive their first skill after tick 100 and second after tick 200. Remaining choices are skipped. Early Hare extinction is retained and stops later acquisitions naturally.
- The run uses the existing Unity scheduled simulation runner in the connected Editor. Research dispatch and compact analysis scripts live only in the ignored artifact folder; no scenario or skill values are changed.

## Report contract

Retain all 1,300 assigned runs, including early extinctions. Validate inputs, seed and ruleset parity, wrapped topology, tick-zero populations, the identical pre-choice phase for each matched seed, reachable ordered acquisition snapshots and registry fingerprint, final and phase FPO reconciliation, and Seed Dispersal food-spend accounting. Report seed-paired survival wins/ties/losses, population outcomes, actual stop time, phase slashlines, direct counters, valid denominators, and N/A/INVALID status. APS is contextual evidence, not a standalone strategy ranking.

This is one population/topology screen, not production balance acceptance. Two-pick paths are not full five-pick builds; scripted schedules bypass normal randomized offer selection. Survivorship and later phase measurements are censored by Hare extinction. Human review and any follow-up decision remain separate.

## Artifacts

- [Factual report](REPORT.md) and [AI analysis](ANALYSIS.md).
- Frozen request, per-run hashes, source hashes, validation, manifest and scripts are under [`artifacts/s4-paired-screen-20261005-600-40-25-wrapped-100/`](../../../../artifacts/s4-paired-screen-20261005-600-40-25-wrapped-100/).


`validation.json` records 1,300 validated records and output hashes; `run-manifest.json` records every run ID, actual tick and file hash. `research-script-hashes.json` identifies the local dispatcher and analyzer. The [handoff](../../../handoffs/2026-10-05-2348-codex-s4-600-40-25-paired-seed-screen.md) records local validation and next review steps.

