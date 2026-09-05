# Hare + Fox Implementation Plan

> Based on: `HARE_FOX_ITERATIVE_TREATMENT.md` v0.2  
> Status: implementation planning  
> Scenario: Forest Edge  
> Scope: the first coupled Hare/Fox interaction and five minimal upgrade hooks

## Outcome

The [consecutive-phase migration](../CONTINUOUS_SIMULATION_FLOW_PLAN.md) is a
dependency for validating these outcomes across upgrade breaks. Preserve the
same resources and creatures, and measure recovery/pressure across absolute
ticks. The treatment's ecological growth/pressure/recovery phases are not new
worlds or interchangeable with UI decision windows. Fresh-window mechanic
evidence remains useful; late-phase balance and initialization-only upgrades
need the [explicit retests](../CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md).

Deliver a playable, measurable Forest Edge slice in which:

- Grass supports Hare growth without being the primary source of tension.
- Hares can pursue two viable base strategies: settlement or migration.
- Foxes create spatially legible pressure through Hare concentration.
- Foxes lose momentum when Hare access is interrupted.
- At least one Hare recovery after a Fox pressure wave is possible.
- Trailblazer, Warren, and Gardeners produce distinct Hare histories.
- Tracker and Ambusher create meaningful Fox counterplay rather than flat kill-rate bonuses.
- Every important outcome is visible in the run summary and reproducible by seed.

## Guardrails

Do not add scent, pack coordination, shelters, inherited traits, new predators,
or a generalized behavior/modifier framework during this pass. Implement the
smallest explicit rules and upgrade effects that can prove the treatment.

## Work packages

### HF-1 — Freeze the experiment contract

Write the authoritative Forest Edge fixture and capture its fingerprint:

- grid dimensions, duration, tick interval, and seed policy;
- Grass, Hare, and Fox starting probabilities;
- terrain/resource distribution and occupancy-layer assumptions;
- species IDs, diet targets, movement, perception, energy, reproduction, and
  population-limit values;
- victory, narrow-survival, and extinction thresholds.

Acceptance:

- The same seed produces the same starting state and ruleset fingerprint.
- All values required for an A/B run are data, not hidden constants.
- A fixture can be loaded by both Play Mode and batch simulation tooling.

### HF-2 — Complete the minimal coupled simulation rules

Verify or implement only the rules required by the treatment:

**Hare**

- eats Grass/approved plant resources;
- prioritizes Fox avoidance when a Fox is visible;
- otherwise seeks food, then mating/settlement behavior;
- can reproduce only above the reserve threshold and with a legal destination;
- prefers movement that increases distance from a visible Fox;
- uses terrain preference/cost without requiring a shelter mechanic.

**Fox**

- prioritizes visible Hare over food or wandering;
- resolves a discrete attack/feeding interaction;
- gains enough energy from a hunt to extend activity, but not guaranteed growth;
- has better prey perception but no ordinary Grass diet;
- reproduces only after sustained prey access;
- loses energy and can decline when Hares disperse or disappear.

**Shared behavior**

- Preserve creature occupancy exclusivity and the separate small-item layer.
- Keep movement, perception, feeding, attack, reproduction, and death causes
  deterministic for a fixed seed.
- Make the causal order inspectable in telemetry rather than inferred from final
  population alone.

Acceptance:

- Grass is not the dominant Hare death cause in the baseline fixture.
- Foxes cannot sustain indefinite growth without Hare access.
- Hares can escape a visible Fox under at least one valid terrain/layout state.
- No new rule is added solely to make a single seed look better.

### HF-3 — Add the minimal run-upgrade contract

Implement an explicit, deterministic upgrade definition with:

- stable ID and display name;
- affected species and rule field;
- immediate effect;
- tradeoff or weakness;
- prerequisites/exclusions;
- preview text;
- telemetry attribution;
- deterministic application order in the effective ruleset fingerprint.

Implement these five effects as the first catalog:

| Upgrade | Required effect | Required tradeoff |
|---|---|---|
| Trailblazer | Better Hare escape/migration through costly or unfamiliar terrain | Weaker local stability, feeding, or reproduction |
| Warren | Better Hare survival/reproduction in a selected pocket | Reduced value after relocation |
| Gardeners | Better Grass persistence/recovery around Hare activity | Slower immediate mobility or growth |
| Tracker | Better Fox pursuit of visible/recent Hare movement | Poor energy efficiency without a target |
| Ambusher | Better Fox attack pressure near edge transitions | Weaker performance in open terrain |

Acceptance:

- Each upgrade changes at least one visible rule outcome and one measured value.
- A player can see the effect, tradeoff, and next branch before selecting it.
- Applying the same ordered upgrades to the same seed reproduces the same run.
- Upgrades do not mutate the base scenario asset or persist between runs.

### HF-4 — Add telemetry and result presentation

Extend the existing run report so the treatment can be judged without watching
every tick. Record per run and per species:

- population history and peak/trough;
- births, feeding, movement, attacks, kills, and deaths by cause;
- first Fox hunt tick and hunt rate;
- Fox energy gained from hunts and time without prey access;
- Hare escape events, dispersal/settlement indicators, and terrain usage;
- recovery start, recovery duration, and number of positive-growth cycles;
- ordered upgrade IDs and each upgrade's attributed contribution;
- scenario, seed, grid, ruleset fingerprint, and outcome.

Present a compact phase summary showing:

- population change and causes;
- food consumed and movement;
- Fox pressure/encounters;
- selected upgrade and expected consequence;
- whether the run is recovering, stable, or collapsing.

Acceptance:

- A designer can explain a hunt, escape, population trough, and recovery from
  one saved report and a few board snapshots.
- Play Mode and batch reports use the same field names for shared metrics.
- Reports remain readable by both humans and automated analysis.

### HF-5 — Build the fixture and test matrix

Create focused tests and deterministic experiment presets:

1. **Baseline pressure:** plentiful Grass, no upgrades, several Hare/Fox ratios.
2. **Settlement vs migration:** Warren and Trailblazer on the same layout/seeds.
3. **Fox target reliability:** dense and dispersed Hare layouts.
4. **Recovery:** controlled Hare reduction after a Fox peak.
5. **Gardeners:** Grass recovery with and without Fox pressure.
6. **Tracker/Ambusher:** target-rich versus target-poor and edge versus open terrain.

Automated checks should cover:

- diet and occupancy-layer legality;
- visible threat perception and Hare flee choice;
- Fox attack/feeding and energy transfer;
- reproduction thresholds and legal destinations;
- terrain modifiers and upgrade tradeoffs;
- fixed-seed fingerprints and report serialization;
- extinction, narrow survival, and victory classification.

Acceptance:

- Every hypothesis in the treatment has a named metric and reproducible fixture.
- One-variable A/B comparisons can be run without editing code.
- Existing runtime/editor tests remain green.

### HF-6 — Balance and player-readability pass

Run the fixtures in this order:

1. Tune the no-upgrade baseline until the three-phase pressure cycle appears:
   Hare establishment, Fox opportunity, and Hare counterplay/recovery.
2. Tune Trailblazer and Warren until both can succeed with different movement and
   population histories.
3. Tune Gardeners until the created Grass value is attractive but contestable.
4. Tune Tracker and Ambusher against the Hare branches; avoid direct global kill
   multipliers.
5. Test the same upgrades across multiple fixed seeds and inspect outliers.
6. Run a short player-comprehension review: what caused the last hunt, what did
   the upgrade change, and what would the player try next?

Do not promote a value because it produces a better average while making the
causal story unreadable or removing recovery.

## Delivery sequence

```text
HF-1 fixture contract
    -> HF-2 minimal rules
    -> HF-4 telemetry
    -> HF-5 fixtures/tests
    -> HF-3 upgrade hooks/catalog
    -> HF-6 balance/readability
    -> promotion review
```

HF-3 can begin after the rule fields are stable, but its final tuning depends on
HF-4 and HF-5. Do not start roster expansion during HF-6.

## Definition of done

Promote this treatment when all of the following are true:

- Settlement and migration are both viable Hare strategies.
- Fox pressure is predictable, spatially legible, and not deterministic.
- Foxes decline naturally when prey access is lost.
- At least one recovery cycle is observable after a Fox peak.
- The three Hare upgrades produce visibly different histories and tradeoffs.
- Tracker and Ambusher create different Fox situations rather than the same
  bonus with different names.
- Players can explain a successful hunt, escape, and recovery from the UI/report.
- Fixed-seed runs, reports, and upgrade loadouts are reproducible.
- No deferred system was smuggled in to conceal a failed interaction.

If a gate fails, revise the interaction or value set and repeat the focused
experiment. Do not add complexity as a substitute for a readable result.
