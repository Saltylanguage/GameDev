# Reactive Species / Ecology Plan — Co-evolutionary Arms Race

> Status: Proposed design spike; not an implementation commitment  
> Date: 2026-08-23  
> Related: [`UPGRADE_SYSTEM_DIRECTION.md`](UPGRADE_SYSTEM_DIRECTION.md), [`Species Design/HARE_FOX_ITERATIVE_TREATMENT.md`](Species%20Design/HARE_FOX_ITERATIVE_TREATMENT.md), [`Species Design/HARE_FOX_IMPLEMENTATION_PLAN.md`](Species%20Design/HARE_FOX_IMPLEMENTATION_PLAN.md), [`Research/Experiments/EX-002-Herbivore-Collapse-Attribution/EX-002-MATRIX-PROTOCOL.md`](Research/Experiments/EX-002-Herbivore-Collapse-Attribution/EX-002-MATRIX-PROTOCOL.md)

## Intended player experience

An upgrade should change the ecology, not just increase a number. When one
species gains an advantage, the other species should receive a readable way to
respond. The player should be able to identify the current pressure, choose a
counter, watch the tradeoff play out, and recover from a setback.

The target is a pressured cycle rather than a permanent snowball:

```text
species growth -> opportunity for its counter-species -> pressure peak
    -> counterplay or dispersal -> weakened opportunity -> recovery
```

This is an extension of the existing Forest Edge Hare/Fox treatment. It does
not replace the treatment or authorize a full adaptive-AI system.

## What is known, inferred, and still speculative

### Verified project evidence

- Hare/Fox is already defined as a coupled interaction: grass supports Hare
  growth, Hare concentration creates Fox opportunity, and interrupted prey
  access should weaken Foxes.
- Run upgrades are ordered, deterministic, temporary loadouts. Permanent Lab
  research is a separate progression layer.
- The BEV combat labs show that attack, block, damage, and cooldown can have the
  intended local combat effect while producing non-monotonic or seed-sensitive
  population results.
- Reports already contain the useful evidence surface: population history,
  births, deaths by cause, feeding, movement, combat, energy, and upgrade
  fingerprints.

### Design inference

“Higher stat equals higher ecological fitness” is not a safe balancing rule.
Repeated contact, food access, reproduction timing, terrain, and population
density can reverse the expected population outcome. Each upgrade therefore
needs a proximal effect, a tradeoff, and a named counter before its ecological
value is discussed.

### Open or speculative decisions

- Whether the player upgrades one focal species or chooses among broader
  species-type research offers.
- Whether a counter is selected directly by the player or surfaced as a
  deterministic offer at the next reward boundary.
- The exact pressure metric and thresholds. These must come from baseline
  distributions, not invented target populations.

## Design contract

Keep the first version explicit and small. Do not build a general evolution
framework until concrete upgrade pairs prove which fields are necessary.

Every reactive upgrade pair should document:

| Field | Required decision |
| --- | --- |
| Upgrade ID and owner | Which species/role receives it and where it sits in the ordered loadout. |
| Proximal effect | The immediate rule change, such as pursuit reliability, escape, block, resource recovery, or reproduction timing. |
| Ecological pressure | Which opportunity or resource relationship it changes. |
| Counter | The specific response available to the other species. |
| Cost/weakness | What the upgrade makes worse, delays, exposes, or makes more expensive. |
| Window | When the effect begins, how long it lasts, and when another adaptation may be chosen. |
| Telemetry | The metric that can confirm the proximal effect and the metric that can test ecological direction. |
| Exclusions | Incompatible branches or stacking limits. |

Initial counter families:

| Upgrading side | Pressure branch | Counterplay branch | Intended tension |
| --- | --- | --- | --- |
| Herbivore | Trailblazer: escape and migration | Tracker-resistant route choice or a temporary safe food frontier | Mobility gains access and escape, but can reduce local reproduction or food conversion. |
| Herbivore | Warren: pocket survival and reproduction | Ambush avoidance, dispersal, or pressure on the pocket's food supply | A safe pocket is productive but becomes a valuable, predictable target. |
| Herbivore | Gardeners: grass persistence/recovery | Fox target reliability around the cultivated patch | Better food creates a contestable location rather than a free advantage. |
| Carnivore | Tracker: pursuit of visible/recent Hare movement | Trailblazer movement, route breaks, or dispersal | Pursuit is strong while a target exists and inefficient when prey is dispersed. |
| Carnivore | Ambusher: edge-transition attack pressure | Open-ground feeding, route timing, or avoiding edge chokepoints | The strongest terrain for the Fox is not always the best terrain for the Hare. |
| Carnivore | Opportunistic breeder: hunt-streak reproduction | Break the streak by denying repeated contact | Short-term success can create a delayed population threat without guaranteeing growth. |

Plants remain a third ecological participant. Plant upgrades should primarily
change resource timing, patch persistence, or recovery geography. They can make
a location valuable to both sides, but should not silently act as a global
catch-up multiplier.

## Recommended rubber-band model

Use ecological opportunity and bounded counterplay, not hidden per-tick stat
inflation.

1. At an upgrade/reward boundary, summarize a recent deterministic window using
   role-appropriate signals: population area-under-curve, births and deaths by
   cause, attack opportunities and hit rate, prey access, energy margin,
   resource access, and time spent in danger.
2. Compare each side with its own scenario baseline envelope. Do not compare raw
   Hare and Fox population counts as if they were interchangeable units.
3. Classify the interaction as `calm`, `contested`, or `pressured`. Add a
   deadband and hysteresis so one noisy tick cannot flip the state.
4. When a side is `pressured`, expose one bounded counterplay opportunity at the
   next legal upgrade boundary: a deterministic counter offer, a temporary
   adaptation slot, or a targeted resource/terrain opportunity. Prefer the
   counter offer first because it is visible, player-controlled, and easy to
   attribute in a report.
5. Close the opportunity when the pressure returns to the contested band. Apply
   a cooldown, a maximum one adjustment per window, and no stacking of free
   counters.

The leading species is not directly punished. Its upgrade still carries its
documented cost or weakness, and its success can create more prey exposure,
resource demand, repeated-contact risk, or a predictable location for the
counter-species. Rubber banding should prevent an unrecoverable runaway, not
guarantee equal populations or revive an extinct species.

Avoid these first-pass mechanisms:

- a hidden global multiplier applied every tick;
- direct population spawning or healing for the trailing side;
- silently changing authored rules mid-run;
- unlimited free upgrades or a catch-up bonus that cannot be explained in the
  result summary;
- a scalar “fitness” value that hides whether food, contact, terrain, or
  reproduction caused the change.

All adaptation decisions must happen after a completed tick and use the
recorded seed, event order, scenario fingerprint, and ordered loadout so a run
can be replayed exactly.

## Evidence sequence

### RE-0 — Baseline pressure envelope

Run the current Forest Edge control across the existing fixed and held-out seed
protocol. Record role-specific envelopes for population history, contact
pressure, food/resource access, energy, births, and deaths. This establishes
what “pressured” means without choosing a desired population in advance.

### RE-1 — Isolated upgrade semantics

Use direct combat/resource fixtures to prove one proximal effect at a time:
Tracker pursuit, Ambusher terrain pressure, Trailblazer escape, Warren local
survival, and Gardeners resource recovery. A local effect must be correct before
an ecological result is interpreted.

### RE-2 — Paired counter trials

Run same-seed pairs such as Tracker ↔ Trailblazer, Ambusher ↔ Warren, and
Gardeners ↔ Fox pressure. Keep the scenario, roster, duration, and starting
state fixed. Confirm that the intended counter changes the relevant opportunity
without becoming a universal bonus.

### RE-3 — Pressure and recovery

Start from controlled lead/lag states and test the bounded counterplay window.
Measure time to pressure, time to recovery, number of counter offers, and
whether the leading side retains a meaningful advantage. Test both directions;
the system must not only rescue Herbivores.

### RE-4 — Promotion sweep

Repeat the chosen pairs on held-out seeds and more than one Forest Edge layout
before tuning values. Promote only if replay equality, counter attribution,
proximal semantics, and recovery evidence all pass. A better average alone is
not sufficient when the causal story is unclear or the effect reverses on
held-out seeds.

## Roadmap delivery

1. **Design spike (S3 species/scenario co-design):** approve the pressure state,
   counter-pair table, player-facing language, and telemetry fields. No generic
   modifier or evolution framework.
2. **Instrumentation lane (S3 accelerator):** expose the boundary snapshot and
   counter attribution through the existing deterministic report path.
3. **First paired slice (after S2 catalog):** implement one Herbivore pair and
   one Carnivore pair with explicit tradeoffs. Keep rubber banding opt-in to the
   experiment fixture.
4. **Recovery validation (S6 integration):** run matched and held-out sweeps,
   then decide whether the bounded counterplay window belongs in the playable
   vertical slice.
5. **Content alpha:** broaden species, plant interactions, and mastery only
   after the first pairs are readable, reproducible, and not runaway.

## Acceptance gates

- Same seed, scenario, ruleset fingerprint, and ordered loadout reproduce the
  same pressure state and counter offer.
- Every paired upgrade has a verified proximal effect, a visible tradeoff, and
  a named counter.
- The baseline remains unchanged when reactive ecology is disabled.
- At least one pressure wave is recoverable, while the leading side can still
  win through a good build.
- Counter offers are bounded, attributable, and do not create direct population
  injection or hidden mid-tick rule changes.
- Directional ecological claims survive matched and held-out seed checks.
- Players can explain what caused the current pressure and what the proposed
  counter is expected to change.

## Explicit non-goals

- inherited traits, mutation trees, scent diffusion, pack coordination, or
  neural/adaptive agents;
- a universal modifier/plugin/evolution framework;
- permanent stat inflation that invalidates early scenarios;
- exact balance values before baseline envelopes exist;
- guaranteeing equal predator and prey populations.
