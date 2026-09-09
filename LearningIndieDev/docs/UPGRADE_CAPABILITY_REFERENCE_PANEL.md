# Upgrade Capability and Reference Panel

Status: **local pilot executed; reference panel is not approved balance or promotion evidence**

Owner: **Josh**

Implementation support: **Codex**

Applies to: **the current Hare Mutation catalog and later Genome planning**

This document gives the upgrade work one shared measurement map and one small
reference panel. It does not approve any Mutation value, Genome capacity, or
new runtime system. It turns the next balance run into a repeatable diagnostic
instead of an open-ended simulation sweep.

Active safeguards:

- [`Planning Concerns/upgrade-system.md`](Planning%20Concerns/upgrade-system.md)
- [`SG-005 — Upgrade and Ecology Balance`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md)

## Outcome we need

For every upgrade candidate, we must be able to explain this chain:

```text
authored stat -> creature capability -> world interaction -> measured outcome
```

A run is useful only when it helps locate a change in that chain. A final Hare
population or combined score by itself is not enough.

## Shared capability map

| Capability | Current authored inputs | Direct evidence already available | Proxy or missing evidence |
| --- | --- | --- | --- |
| Mobility and escape | `movement.speed` | Movement steps normalized by Hare state ticks; predator encounters; preyed count; state transitions | Movement attempts, travel distance, target arrivals, and escape events are not recorded directly |
| Detection and search | `awareness.vision-range` | Food attempts, successes, and failures; movement and state activity | Newly visible targets and detections beyond the natural range are not recorded directly |
| Energy endurance | `energy.metabolism`, `energy.starting`, `resource.starting-food-reserve` | Food consumed; food actions; starvation deaths | Energy distribution through time and starting-reserve use are not summarized directly |
| Defense | `combat.block` | Combat opportunities, attempts, hits, blocks, damage applications, and lethal/non-lethal results | Prevented damage can be derived from blocked results but is not a separate total |
| Reproduction access and capacity | `reproduction.neighbor-count`, `reproduction.group-size` | Candidates, missing-mate blocks, group-limit blocks, successful attempts, and births | Local mate density must be inferred from the world and block counts |
| Crowding resilience | `crowding.energy-penalty`, `crowding.tolerance` | Crowding deaths, group-limit blocks, births, and population | Local density and energy lost to crowding are not reported directly |
| Resource recovery | `resource.seed-drop-chance` | Fern births, Fern population, Hare food activity, and food consumed | Seed-drop attempts and successes are not separate from other Fern births |

The proxy gaps are measurement limits, not permission to infer the missing
event. If a decision depends on one of them, add the smallest direct metric or
run a focused mechanic test before judging balance.

## Current Mutation coverage

| Mutation | Main capability | Main Species evidence | Separate Biome context |
| --- | --- | --- | --- |
| `trailblazer-long-stride` | Mobility and escape | Movement steps, predator encounters, preyed count, missing-mate blocks | Hare pressure on Fern and the Hare/Fox relationship |
| `trailblazer-far-sight` | Detection and search; energy endurance | Food actions and starvation are available, but direct long-range detection is missing | Resource pressure and persistence of intended feeding relationships |
| `warren-guarded-burrow` | Defense; mobility | Blocks, hits, damage, lethal results, movement steps | Fox feeding pressure and Hare persistence |
| `warren-room-to-breed` | Reproduction capacity; crowding resilience | Group-limit blocks, crowding deaths, births, and starvation | Fern depletion, population pressure, and recovery |
| `gardeners-seed-pouches` | Energy endurance; resource recovery | Early starvation and food activity are proxies; direct reserve use is missing | Early Fern recovery and Hare pressure |
| `gardeners-careful-sowing` | Resource recovery; mobility | Fern births are a proxy for successful drops; movement steps | Fern recovery, Hare food pressure, and population persistence |
| `familial-bond-large-litters` | Crowding resilience | Crowding deaths, births, group-limit blocks, and starvation | Fern depletion and Hare population pressure |

## Forest Edge reference panel

Use this panel for the first production-Mutation timing diagnostic. Keep the
panel fixed between arms so that the Mutation and activation phase are the only
planned differences.

| Field | Locked diagnostic value |
| --- | --- |
| Scenario | `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset` |
| Player species | `hare` |
| Grid | Scenario default, currently 32 x 32 |
| Run length | 2,000 ticks |
| Phase structure | 10 phases of 200 ticks |
| Combat | Opposed roll |
| Attack opportunities | Natural |
| Experimental feature set | `bev-experimental`, matching the accepted EX-010 timing baseline |
| Pilot seeds | 12001–12005 |
| Reserved confirmation seeds | 12101–12105; do not inspect until a follow-up question and method are registered |
| Upgrade catalog | Production Mutation assets |

This is a reference panel for comparison, not a claim that Forest Edge covers
every capability. Detection and seed-drop contribution still need direct
telemetry before their full contribution can be accepted.

## First diagnostic matrix

Purpose: determine whether three representative live Mutations produce a
visible, phase-attributable change, and whether early, middle, and late
activation behave consistently in one continuing expedition.

This is a local pilot. It may reject a broken mechanic or unclear measurement,
but it cannot promote values to accepted balance.

| Arm | Mutation becomes active | Phase schedule |
| --- | --- | --- |
| Control | Never | Ten `none` entries |
| Long Stride — early | Phase 2 | `none`, then Long Stride through phase 10 |
| Long Stride — middle | Phase 6 | Five `none`, then Long Stride through phase 10 |
| Long Stride — late | Phase 10 | Nine `none`, then Long Stride |
| Guarded Burrow — early | Phase 2 | `none`, then Guarded Burrow through phase 10 |
| Guarded Burrow — middle | Phase 6 | Five `none`, then Guarded Burrow through phase 10 |
| Guarded Burrow — late | Phase 10 | Nine `none`, then Guarded Burrow |
| Careful Sowing — early | Phase 2 | `none`, then Careful Sowing through phase 10 |
| Careful Sowing — middle | Phase 6 | Five `none`, then Careful Sowing through phase 10 |
| Careful Sowing — late | Phase 10 | Nine `none`, then Careful Sowing |

The first phase is deliberately unchanged. Every later entry is cumulative so
the recorded ordered loadout, effective tick, contract version, registry
fingerprint, and loadout fingerprint remain auditable.

### Questions registered before the pilot

1. Does Long Stride change movement activity after activation without creating
   an unexplained pre-activation difference?
2. Does Guarded Burrow change blocked combat results after activation, and is
   the mobility cost visible?
3. Does Careful Sowing change the Fern-birth proxy after activation, and is the
   mobility cost visible?
4. Does shortening exposure from early to middle to late activation move the
   observed difference in the expected direction?

### Reading the result

Keep two views separate:

- **Species view:** Hare mechanic and survival evidence, including movement,
  combat, reproduction, food activity, starvation, and preyed count.
- **Biome view:** Fern, Hare, and Fox persistence, resource pressure, recovery,
  and intended food relationships.

Do not turn these into one approval score. Report the observed distributions
and phase-local changes. A result is inconclusive when a required direct event
is missing, sample variation is larger than the observed difference, or the
effect appears before its recorded activation tick.

## Local pilot result — 2026-09-09

All ten arms ran on seeds 12001–12005. Every artifact bundle passed
`Test-CellSimArtifactBundle.ps1` as `VALID`. The scenario, seed set, ruleset,
registry, phase windows, combat mode, and opportunity mode match across arms.
Each non-control arm records one production snapshot at tick 200, 1000, or
1800, with the intended signed modifiers, contract version, registry
fingerprint, and snapshot fingerprint.

Aggregate population, activity, behavior, and stat-line evidence is identical
to control before each activation boundary. Process-local entity IDs differ in
some tracked-creature snapshots, so those IDs were not treated as comparison
evidence.

The values below are mean paired arm-minus-control differences across five
seeds, measured only from activation through phase 10. Movement and block
rates are percentage-point changes. Fern births remain a proxy for seed drops.

### Trailblazer: Long Stride

| Timing | Hare movement rate | Final Hare | Final Fern |
| --- | ---: | ---: | ---: |
| Early | +1.52 pp | -33.4 | +281.0 |
| Middle | +11.92 pp | -28.8 | +232.8 |
| Late | +32.65 pp | -16.4 | +7.2 |

The normalized movement measure changes in the authored direction at every
timing. Raw movement totals fall because the added mate requirement changes
the Hare population; this is why unnormalized movement is not contribution
evidence. The population and Fern changes belong in the separate Biome view.

### Warren: Guarded Burrow

| Timing | Hare movement rate | Fox blocked-hit rate | Final Hare | Final Fern |
| --- | ---: | ---: | ---: | ---: |
| Early | -33.18 pp | +9.39 pp | +9.6 | +55.0 |
| Middle | -33.70 pp | +3.18 pp | +14.0 | -17.0 |
| Late | -35.61 pp | -13.32 pp | +18.8 | +103.2 |

The movement cost is visible at every timing. Block evidence moves in the
expected direction for early and middle activation, but the single late phase
does not. The late defensive result is inconclusive rather than a rejection;
it has little exposure and a small, diverged combat sample.

### Gardeners: Careful Sowing

| Timing | Hare movement rate | Fern births proxy | Final Hare | Final Fern |
| --- | ---: | ---: | ---: | ---: |
| Early | -80.90 pp | +66,074.4 | +230.4 | -91.8 |
| Middle | -79.89 pp | +38,090.6 | +292.2 | -225.2 |
| Late | -75.29 pp | +4,523.8 | +289.0 | -174.8 |

The movement cost is visible and the Fern-birth proxy grows strongly with
longer exposure. More Fern births do not produce more Ferns at the end: the
larger Hare population consumes more food and final Fern population falls.
This is a useful Species/Biome tradeoff, but the size of the cascade is a
balance warning, not an accepted value. Direct seed-drop attempts and
successes are required before attributing the birth increase to the Mutation.

### Pilot decision

- Keep the existing Mutation runtime and snapshot contract.
- Use normalized capability rates instead of raw activity totals when
  population changes strongly.
- Treat Long Stride's movement mechanic and all three movement tradeoffs as
  visible in this diagnostic scope.
- Keep Guarded Burrow's late defensive contribution open.
- Do not promote Careful Sowing's `+0.10` value. Add direct seed-drop telemetry
  before a confirmation run or balance decision.
- Keep seeds 12101–12105 unopened until a follow-up question and method are
  registered.

### Artifact bundles

| Arm | Artifact folder |
| --- | --- |
| Control | `artifacts/cellular-experiment-20260909-110516` |
| Long Stride — early / middle / late | `artifacts/cellular-experiment-20260909-110642`; `...-110758`; `...-110920` |
| Guarded Burrow — early / middle / late | `artifacts/cellular-experiment-20260909-111044`; `...-111206`; `...-111329` |
| Careful Sowing — early / middle / late | `artifacts/cellular-experiment-20260909-111455`; `...-111801`; `...-112017` |

## Pilot completion gate

The pilot is complete when:

- all ten artifact bundles pass the existing bundle validator;
- all arms record the same scenario, ruleset, seed set, phase length, combat
  mode, and opportunity mode;
- each non-control arm records the intended production asset, resolved signed
  modifiers, order, effective tick, contract version, and fingerprints;
- results are summarized as Species evidence and Biome context, with proxy
  limitations named; and
- no value is labelled accepted without a separate human decision and fresh
  confirmation plan.

## Decisions this document does not make

- final Mutation values or costs;
- the full nine-choice tree or hybrid rules;
- Genome node cost semantics;
- Genome runtime or UI implementation;
- promotion of a diagnostic result into balance evidence.
