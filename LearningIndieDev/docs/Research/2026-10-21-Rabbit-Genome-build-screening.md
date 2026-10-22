# Rabbit Genome build screening — Step 2

**Experiment:** `EXP-RABBIT-GENOME-20261021-01`  
**Human decision owner:** Josh  
**Status:** Completed; awaiting human content/balance decision  
**Scope version:** 1, October 21, 2026 (America/Toronto)

## Question and hypothesis

Under one declared Forest Edge setup, do the current legal Gardener and
Trailblazer POC builds produce measurable, different benefits and ecological
costs that give us useful starting points for content tuning?

Predictions, recorded before execution:

- Fertile Droppings increases Hare seed-drop activity and plant renewal. Its
  longer scent exposure activates Fox dropping acquisition, sniffing and named
  tracking; the net Hare and Fox outcomes remain uncertain.
- Adding Efficient Digestion to Fertile Droppings improves Hare reproduction
  or persistence through better feeding returns, while potentially increasing
  crowding and demand for plants. This is a marginal comparison of a legal
  dependent node, not a standalone digestion or factorial synergy estimate.
- Long Stride plus Long Vision changes movement and threat/food access. Its
  effect may differ from the renewal-oriented Gardener build; an advantage is
  a hypothesis, not a promised outcome.

This screens the implemented POC nodes. It does not test the unimplemented
Gardener tree, establish production prices, or measure player enjoyment.

## Proposed fixed inputs

| Input | Value |
| --- | --- |
| Scenario | `Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset` |
| Catalog, all arms | `Assets/Data/TestData/Genome/RabbitSelectionPOC/RabbitSelectionPOC.asset` |
| Mode | Fresh Species-Simulation window; Hare selected |
| Map | 36×20, bounded edges |
| Starting counts | `plant=400,hare=20,fox=10`, explicit overrides |
| Seeds | 10120–10131 inclusive, same 12 in every arm |
| Window | 600 ticks, 0.1 simulation seconds per tick; 60 simulated seconds |
| Mutations, schedule, policy | None; no acquisition decisions or purchases |
| Combat | `opposed-roll`, natural attack opportunities |
| Experimental features | None; coupled species responses off |
| Other species Genomes | Empty |
| Runner/report | Current CellSim, expected report schema 38; Unity 6000.4.6f1 |
| Source at preparation | `codex/audio-direction-and-sourcing`, HEAD `6ffb7b10`, dirty shared checkout |

The starting counts deliberately reproduce the earlier pilot's declared
pressure setup, rather than the scenario asset's authored 400/55/35 counts.
These are new matched runs. The older schema-37 pilot and 20-tick delivery smoke
are context only, not comparison controls. The new seeds are disjoint from
that pilot's 10100–10119 block; this is exploratory evidence, not a held-out
confirmation of a fitted model.

## Arms

| Arm | Ordered active node IDs | Capacity | Changes from control |
| --- | --- | ---: | --- |
| Natural baseline | Empty | 0/8 | Natural rules, including ordinary scent |
| Fertile | `fertile-droppings` | 1/8 | Existing seed-renewal and extended dropping scent ability |
| Gardener POC | `fertile-droppings,poc.hare.efficient-digestion` | 3/8 | Fertile plus `digestion.energy-bonus` +0.25 |
| Trailblazer POC | `poc.hare.long-stride,poc.hare.long-vision` | 4/8 | `movement.speed` +0.25; `awareness.vision-range` +2 |

Total: **48 simulation runs**. No extra scenarios, seeds, sweeps or automatic
retuning are included. Gardener and Trailblazer have unequal provisional
capacity costs; their comparison describes the builds at those values and
cannot establish equal-budget branch superiority. Burrows Preview is excluded.

## Expected report and analysis

Retain each wrapper's raw JSON/CSV, request, Unity/CLI logs, manifest,
metric dictionary, source hashes, source revision/dirty state, resolved node
values and fingerprints. Use artifact-summarization for compact per-arm and
paired evidence; retain the raw files unchanged.

**Focal Hare scorecard:** population at ticks 100/300/600; minimum/maximum and
mean population over the window; extinction occurrence/tick; births; food
consumed; movement; deaths by available recorded cause; reproduction funnel;
crowding animal-ticks and crowding energy loss.

**Ecological context scorecard:** plant availability over the window, plant
births and seed-drop activity; Fox persistence/extinction, births, deaths,
food consumed, combat attempts and kills; dropping detections/sniffs,
named-scent samples and pursuit decisions. These are other-species outcomes
inside a Species Simulation, not evidence from a separate Biome mode.

Show arm summaries and same-seed differences against the natural baseline,
plus Gardener minus Fertile for the incremental digestion effect. Keep all
12 seed differences, medians/ranges and direction counts visible. Any interval
estimate is descriptive screening uncertainty, not a production acceptance
bound. Report population checkpoints and extinction separately so a mean does
not conceal collapse. Scent decisions do not prove movement, catches, or kills
caused by scent; this report cannot attribute individual kills to a scent cue.

## Success, failure and stop criteria

Technical success requires all 48 runs to complete their declared window,
report legal builds with expected costs/values, and match fixed inputs and
tick-zero populations per seed. Report/schema/metric contracts and unchanged
source/catalog hashes must agree across arms; effective fingerprints may
differ with the selected Genome. A mismatch or incomplete run
blocks paired interpretation; preserve it as a failed/partial artifact.

Scientific success is a reproducible description of the direction, magnitude
and variation of the current builds' effects, sufficient to choose the next
small content or diagnostic step. A null, mixed, or harmful result is valid
evidence. There is no pre-approved universal population target. Joint species
persistence, resource pressure and peak/trough patterns are observations to
review with Josh, not automatic definitions of a balanced ecosystem.

If the declared window fails to expose a mechanism, or variance/extinction
makes a build ranking inconclusive, report that limit and propose the smallest
follow-up. Do not silently extend the run or change starting counts. Do not
promote nodes, alter production rules/economy, or infer fun from populations.

## Commands prepared for approval

Run from `LearningIndieDev` with the project's elevated host default. `Auto`
selects a ready Editor or the clean lane; stop on busy/Safe Mode/locked states.
No parallel Unity launches are needed.

```powershell
$screeningInputs = @{
    Command = 'Run'
    Execution = 'Auto'
    ScenarioPath = 'Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset'
    RabbitGenomeCatalogPath = 'Assets/Data/TestData/Genome/RabbitSelectionPOC/RabbitSelectionPOC.asset'
    PlayerSpeciesId = 'hare'
    SeedStart = 10120
    SeedCount = 12
    GridWidth = 36
    GridHeight = 20
    StartingPopulations = 'plant=400,hare=20,fox=10'
    RunTicks = 600
    StepIntervalSeconds = 0.1
    CombatMode = 'opposed-roll'
    AttackOpportunityMode = 'natural'
}

.\CellSim.ps1 @screeningInputs
.\CellSim.ps1 @screeningInputs -RabbitGenomeNodeIds 'fertile-droppings'
.\CellSim.ps1 @screeningInputs -RabbitGenomeNodeIds 'fertile-droppings,poc.hare.efficient-digestion'
.\CellSim.ps1 @screeningInputs -RabbitGenomeNodeIds 'poc.hare.long-stride,poc.hare.long-vision'
```

## Preparation and authorization

- Josh requested continuation to Step 2. The concrete panel above was presented
  for scope approval under SG-001 and the cellsim-experiment skill. Approval of
  Step 1 and its technical smoke is not recorded as approval of this new panel.
- Read-only elevated `CellSim Doctor` passed, with licensing healthy and this
  project closed. Evidence: `artifacts/unity-doctor-20261021-035649/` (the folder
  timestamp is host-generated; this document uses the supplied client date).
- Current catalog assets confirm the listed node IDs, prerequisite order,
  effects and costs. Existing schema-38 output contains the requested activity,
  scent, population, reproduction and mortality fields.
- **Human scope approval:** Josh selected “Run the 48-run screening experiment”
  in response to the exact panel request, before execution.
- **Execution:** all 48 runs completed; evidence bundle
  `artifacts/rabbit-genome-screening-20261021-01/`.
- Post-run reporting caveats, disclosed without changing the experiment:
  the empty control intentionally omits its resolved catalog fingerprint;
  unchanged source/asset hashes and agreement among selected-arm catalogs
  establish the shared input. Numeric ruleset fingerprints alone do not cover
  the separate Fertile Droppings ability; Genome fingerprints are retained.
  Plant-wilt events under animals are mislabeled as animal deaths. A separate
  derived accounting reattributes those events according to the source rule
  and reconciles all 144 species/run accounts. Raw reports remain unchanged;
  the production telemetry defect is still open.
- [Observed results](2026-10-21-Rabbit-Genome-build-screening-results.md) and
  [separate AI analysis](2026-10-21-Rabbit-Genome-build-screening-analysis-v1.md).
- **Human balance/content decision after results:** pending, owner Josh.
