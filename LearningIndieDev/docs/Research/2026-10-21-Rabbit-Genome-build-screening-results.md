# Rabbit Genome screening — observed results

**Experiment:** `EXP-RABBIT-GENOME-20261021-01`  
**Report:** `RPT-RABBIT-GENOME-20261021-01`  
**Owner:** Josh  
**Generated:** October 21, 2026 (client date, America/Toronto)  
**Status:** Complete; human decision pending

[Approved scope and commands](2026-10-21-Rabbit-Genome-build-screening.md) |
[AI analysis v1](2026-10-21-Rabbit-Genome-build-screening-analysis-v1.md)

**Follow-up:** The [mortality reporting fix and early-loss diagnosis](2026-10-21-Hare-early-loss-diagnosis.md)
are now verified. The raw reports below predate that fix and retain the
original defect; their separately derived correction remains necessary.

## Execution and provenance

All **48/48 runs completed 600 ticks**. The four arms used the same seeds
10120–10131, Forest Edge 36×20 with bounded edges, 400 Plants / 20 Hares /
10 Foxes, 0.1-second steps, Hare selected, opposed-roll combat and natural
attack opportunities. No Mutations, purchase policy, phase choices or
experimental features were enabled. Each run is a fresh window, not a full
Desktop journey or Biome-mode run.

Josh approved this exact panel before execution. Runs used CellSim's Clean
lane, Unity 6000.4.6f1, report schema 38 and metric dictionary v1, on
`codex/audio-direction-and-sourcing` at commit `6ffb7b10` with a dirty tree.
The bundle retains copies and SHA-256 hashes of **389 source/input files**;
their hashes were checked before and after every arm. Catalog assets were
unchanged throughout. No production balance values or player profiles changed.

| Arm | Capacity | Raw report folder under `artifacts/` | Raw report SHA-256 |
| --- | ---: | --- | --- |
| Natural | 0/8 | `cellular-experiment-20261021-035859/` | `539c1414047ff469df5a589fe1e3aed8ed68f1696ba9463a95475bd115e7c0e7` |
| Fertile Droppings | 1/8 | `cellular-experiment-20261021-035949/` | `d59c19876e1917fdf04e2faa685bad341c9ba06d25eb108daf1e1f5f9d965a61` |
| Gardener POC | 3/8 | `cellular-experiment-20261021-040041/` | `ad43e7f6c67cced0f3b0a7539a5673e33eed687aee9453c3327eabf1efc7f592` |
| Trailblazer POC | 4/8 | `cellular-experiment-20261021-040136/` | `338404bc1484a08309362c1a49e5175abdf7cb9cd1b4e904a3057679b62aea02` |

The [bundle index](../../artifacts/rabbit-genome-screening-20261021-01/run-index.json)
contains exact report/manifest paths and hashes. The source snapshot, original
approved plan, execution script and analysis script are in the same bundle.
Host-generated artifact timestamps differ from the supplied client date.

## Population observations

Population values below are means across the 12 seeds. Extinctions mean zero
population within this window. Survival at tick 600 does not establish
long-term persistence or a viable breeding population.

| Arm | Hare t100 | Hare t300 | Hare t600 | Median Hare t600 | Plant t600 | Fox t600 | Hare extinctions | Fox extinctions | All three present t600 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Natural | 6.50 | 0.67 | 0.08 | 0 | 471.00 | 0.00 | 11/12 | 12/12 | 0/12 |
| Fertile Droppings | 6.08 | 8.92 | 43.42 | 0.5 | 481.33 | 2.25 | 6/12 | 8/12 | 3/12 |
| Gardener POC | 7.08 | 6.42 | 8.17 | 3 | 502.08 | 3.00 | 3/12 | 6/12 | 6/12 |
| Trailblazer POC | 5.58 | 0.42 | 0.17 | 0 | 485.92 | 0.00 | 10/12 | 12/12 | 0/12 |

The Gardener POC is Fertile Droppings + Efficient Digestion (+0.25 additive
digestion energy bonus). Trailblazer is Long Stride (+0.25 movement speed) +
Long Vision (+2 tiles). Costs are provisional and unequal.

| Same-seed Hare final-population difference | Mean | Median | Minimum to maximum | Higher / lower / equal |
| --- | ---: | ---: | --- | --- |
| Fertile minus Natural | +43.33 | +0.50 | −1 to +241 | 6 / 1 / 5 |
| Gardener minus Natural | +8.08 | +2.50 | 0 to +41 | 9 / 0 / 3 |
| Trailblazer minus Natural | +0.08 | 0 | −1 to +1 | 2 / 1 / 9 |
| Gardener minus Fertile | −35.25 | −0.50 | −236 to +14 | 4 / 6 / 2 |

Fertile's final Hare counts range from 0 to 241; Gardener's range is 0 to 41.
For seed 10125, Fertile ends with 241 Hares and Gardener with 5, while both
end with 13 Foxes. At least two Hares remain in 0/12 Natural, 5/12 Fertile,
7/12 Gardener and 0/12 Trailblazer runs. This threshold is descriptive and
does not prove mating eligibility or future persistence.

## Activity observations

Each cell is the mean total per run. Food consumed and food created use the
runner's resource units; they are not plant counts. Pursuit decisions count
decisions across movement passes, not movement steps or attributed kills.

| Metric | Natural | Fertile | Gardener | Trailblazer |
| --- | ---: | ---: | ---: | ---: |
| Hare births | 10.83 | 67.58 | 33.25 | 6.92 |
| Hare food consumed | 702.08 | 3172.25 | 1596.08 | 532.42 |
| Hare seed-drop successes | 0 | 304.58 | 152.42 | 0 |
| Hare seed-drop food created | 0 | 3045.83 | 1524.17 | 0 |
| Hare crowding energy lost | 0.33 | 21.00 | 0.08 | 0.08 |
| Fox food consumed / combat kills | 27.83 | 35.42 | 41.33 | 22.58 |
| Fox dropping detections | 0 | 106.50 | 126.17 | 0 |
| Fox dropping sniffs | 0 | 38.42 | 44.58 | 0 |
| Fox named-scent samples | 0 | 118.67 | 158.50 | 0 |
| Fox scent-pursuit decisions | 1583.92 | 2003.25 | 2666.67 | 1766.25 |

Both dropping-enabled arms record sniffs and named-scent samples on every
seed. The reports do not attribute individual kills or survival to those cues.
Totals are not adjusted for different population exposure or time alive.

## Mortality accounting defect and derived correction

`SpeciesSimulation.ResolveWilt` removes the plant layer. When an animal occupies
the same tile, `SpeciesSimulationMetrics.RecordDeath` currently chooses the
animal's species/entity identity. The raw report therefore adds a plant wilt
to that animal's `deaths`, `wiltDeaths` and death-event list, although the
animal remains. This defect is not fixed by this experiment.

Misattributed events: Natural **119**, Fertile **261**, Gardener **246**,
Trailblazer **118**; **744 total**. The raw artifacts and existing generic
summarizer output retain those original labels and must not be read as correct
animal mortality totals.

The separate `comparison.json` retains raw counters and adds `derivedDeaths`.
It reassigns only `Wilt` events with `isCreature=true` to this scenario's sole
plant species, matching the source rule. After this reassignment,
**initial population + births − derived deaths = final population in all
144 species/run accounts**. No raw report was rewritten and no simulator
change or additional simulation run was made.

| Derived deaths per run | Natural | Fertile | Gardener | Trailblazer |
| --- | ---: | ---: | ---: | ---: |
| Hare combat | 27.83 | 35.42 | 41.33 | 22.58 |
| Hare starvation | 2.92 | 8.75 | 3.75 | 4.17 |
| Fox starvation | 15.50 | 13.33 | 13.42 | 14.08 |

All actual Fox deaths in this panel are starvation events. In Natural, 334
of 369 actual Hare deaths are combat deaths. By tick 100, Natural has a mean
12.25 Hare combat deaths and 1.25 starvation deaths, with mean population 6.50.
These are observations; the next diagnostic choice belongs to Josh.

## Comparability and evidence limits

- Fixed runner fields, exact seed sets, declared duration, tick-zero populations,
  node allocations/costs, selected catalogs, per-run Genome fingerprints,
  manifests and unchanged source hashes were checked by assertions.
- Empty allocation returns `GenomeSimulationSnapshot.Empty` before catalog
  resolution, so Natural's `catalogFingerprint` is blank. The catalog path is
  explicit and catalog files were frozen and hash-checked; all selected arms
  report the same resolved catalog fingerprint. This known omission is not
  treated as a resolved fingerprint match for Natural.
- Natural and Fertile share their numeric `rulesetFingerprint`; the separate
  Genome fingerprint captures the ability. Both identifiers are retained.
- The generic summarizer does not retain schema-38 Genome values. The separate
  comparison JSON preserves the complete `genomeInput` records for all arms.
- This is one exploratory panel with 12 seeds. It supplies no validated healthy
  operating ranges, significance claim, AV calibration, equal-budget verdict,
  held-out confirmation, player-choice data or measure of fun.
- No visual replay or human playtest was performed in this work block.

## Artifacts and human decision

- [Machine-readable comparison, inputs and per-seed deltas](../../artifacts/rabbit-genome-screening-20261021-01/comparison.json)
- [Per-seed metrics CSV](../../artifacts/rabbit-genome-screening-20261021-01/per-seed.csv)
- [Compact factual summary](../../artifacts/rabbit-genome-screening-20261021-01/observed-results.md)
- Each arm subfolder in the bundle contains the existing summarizer's JSON and
  Markdown, with original mortality labels; use the correction above.

**Decision ID:** `DEC-EXP-RABBIT-GENOME-20261021-01`  
**Owner:** Josh  
**Decision:** Pending. Experiment execution was approved; content promotion,
balance changes and follow-up experiments have not been approved by these results.
