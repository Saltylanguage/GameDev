# rabbit-genome-build-screening

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-21-0405-codex-rabbit-genome-build-screening
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6ffb7b10
- Date: 2026-10-21
- Supersedes: none

## Summary

Completed Step 2's approved four-arm, 48-run Genome screening panel. The POC
builds produce different survival/resource/predator outcomes, but the untreated
setup collapses and no production balance margins are established.

## Changes

- Added the approved scope, factual results and separate AI analysis under
  `docs/Research/2026-10-21-Rabbit-Genome-build-screening*.md`.
- Retained source/input copies and hashes (389 files), execution/analysis
  scripts, run index, per-arm summaries, full resolved Genome inputs, paired
  per-seed metrics and derived mortality under
  `artifacts/rabbit-genome-screening-20261021-01/`. Raw reports remain in their
  four `cellular-experiment-20261021-*` folders, indexed in that bundle.
- Updated WORKING_STATE. No simulator, asset, profile, production-value,
  dependency, branch, commit/push or Trello changes were made in this work block.

## Decisions and assumptions

- Josh explicitly approved the exact 48-run panel before execution: 12 matched
  seeds 10120–10131, 600 ticks, Forest Edge 36×20, 400 Plants/20 Hares/10 Foxes,
  Hare selected, no Mutations or phase choices, opposed-roll natural combat.
- Arms: Natural (0 capacity), Fertile (1), Fertile + Efficient Digestion (3),
  Long Stride + Long Vision (4). Unequal costs prevent an equal-budget verdict.
- These are fresh Species-Simulation windows, not the Desktop journey or Biome
  mode. Prior 300-tick pilot and 20-tick delivery smoke are not controls.

## Validation

- Elevated host Doctor passed; licensing healthy, Editor closed. All four
  execution arms used the clean Unity lane (6000.4.6f1), schema 38, and completed
  48/48 runs. Source and asset hashes remained unchanged across every arm.
- Analysis assertions verified fixed input fields, exact seeds/ticks/durations,
  tick-zero populations, legal costs/IDs, catalog and registry identity,
  per-run Genome fingerprints and manifest/source consistency.
- Initial analysis deliberately stopped on a blank baseline catalog fingerprint.
  Source inspection confirmed empty allocations return Empty before catalog
  resolution. Selected arms agree; unchanged file hashes preserve the baseline
  catalog provenance. The exception is explicitly documented rather than hidden.
- Discovered existing plant-wilt attribution defect: ResolveWilt removes a plant
  layer, while RecordDeath chooses an occupying animal's identity. Reassigning
  only Wilt+isCreature events to the sole plant species reconciles initial +
  births − derived deaths = final population for all 144 species/run accounts.
  There were 744 mislabeled events. Raw reports were retained unchanged.
- Existing artifact summarizer ran for all arms; comparison.json separately
  preserves schema-38 Genome metadata the generic summarizer omits.
- No new Unity tests or player/visual playtest were needed or run for these
  documentation and analysis changes; this is experiment evidence, not a new
  full-suite acceptance result.

## Risks and incomplete work

- Raw mortality counters and per-arm generic summaries retain the original
  mislabeled plant deaths. Use the documented derivedDeaths fields in
  comparison.json for this panel. The runtime bug remains unfixed.
- Natural and Fertile share numeric rulesetFingerprint because the ability is
  represented by the separate Genome fingerprint; compare both identifiers.
- Natural Hare extinction 11/12 and Fox extinction 12/12 means this setup is
  not yet an accepted healthy baseline. Fertile's mean Hare count 43.42 hides
  six extinctions and a range up to 241. Gardener has lower mean 8.17 but three
  extinctions and joint endpoint presence in six seeds. Trailblazer retains
  at most one Hare per seed. Endpoint presence is not long-term stability.
- Single exploratory scenario/12 seeds; no held-out confirmation, causal
  attribution of scent kills, production pricing, AV, or fun verdict. Josh's
  content/balance decision remains pending.

## Next useful step

Fix the stacked-plant wilt attribution with one focused regression. Then use
the retained early mortality windows and seeds 10120/10121/10125 to diagnose
early predator–prey pressure with Josh/Sim. Propose one bounded follow-up if
new simulations are needed; no additional panel or production tuning has been
approved by these results.
