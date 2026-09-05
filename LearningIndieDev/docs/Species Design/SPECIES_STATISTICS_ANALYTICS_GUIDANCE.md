# Species statistics and sabermetric-style analytics

> Status: proposed guidance for statistics capture and presentation
>
> Audience: Sim and contributors working on species statistics
>
> Scope: telemetry vocabulary, derived metrics, and future comparison models;
> this does not approve a single composite fitness formula

## The useful idea

**Continuation context:** a phase stat line describes one window in an evolving
expedition. Carry-over creatures are not new starting spawns; rates need that
window's raw numerators, denominators and populations. Pool raw counts before
computing expedition rates. Upgrade attribution needs acquisition/effective
ticks. See the [shared impact review](../CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md)
for proposed contracts and Sim/Josh retests; continuation is not implemented yet.

Baseball analytics is a helpful model because it compares players who create
value in very different ways and under different conditions. Species statistics
have the same problem. A predator, herbivore, scavenger, and plant should not be
judged by one shared raw count or an RPG-style sum of attributes.

The core ecological distinction is:

- **Fitness** describes reproductive success and contribution to lineage
  persistence.
- **Power** describes traits or immediate mechanical strength.
- **Ecological impact** describes what a species does to the wider ecosystem.

These are related, but they are not interchangeable. An invasive species may be
highly fit while destabilizing its ecosystem. A slow or physically weak species
may be extremely successful because it converts resources into surviving
descendants efficiently.

## Metric ladder

Statistics should be built in layers. Each layer answers a different question
and should remain inspectable instead of being hidden inside one score.

```mermaid
flowchart LR
    A[Raw events] --> B[Rates]
    B --> C[Expected performance]
    C --> D[Adjusted + metrics]
    D --> E[Optional composites]
```

| Layer | Question | Example |
| --- | --- | --- |
| Raw event/count | What happened? | 19 successful hunts |
| Rate | How often or efficiently? | 19 captures / 44 attempts |
| Expected | What should conditions have produced? | 34% expected capture rate |
| Adjusted `+` | How did it compare with a relevant baseline? | `Hunt+ 127` |
| Composite | What broader outcome did it contribute to? | provisional `FIT+` |

The raw observations and denominators are the durable part. Rates can be
recalculated later. Expected, adjusted, and composite metrics depend on modeling
choices and must record which baseline and version produced them.

## Start with observable counting statistics

Useful raw statistics include:

| Family | Candidate observations |
| --- | --- |
| Survival | birth tick, death tick, age at death, cause of death, dangerous encounters, encounters survived |
| Feeding | food acquired, food consumed, feeding attempts, successful feeds, energy gained, food lost to competition |
| Predation | prey encounters, hunts attempted, captures, kills, damage, pursuit time or steps |
| Avoidance | predator encounters, escapes, failed escapes, damage avoided |
| Reproduction | reproductive opportunities, attempts, births, offspring count, offspring reaching maturity |
| Movement | steps, useful movement toward a goal, terrain crossed, movement energy cost |
| Competition | contests, wins, losses, displacement, contested resources acquired or lost |
| Space | occupied cell-time, territory held, resources gained from held space |
| Plant life cycle | seeds produced, seeds dispersed, germinations, mature plants, resource production, wilt/death |

The current schema already records population history, births, food consumed,
movement, combat, deaths and proximate causes, reproduction-funnel outcomes,
food-action outcomes, behavior states, and opposed-roll combat details. Extend
that instrument only when a named statistic needs a missing numerator,
denominator, exposure, or lifecycle link.

Raw counts are deliberately not enough to declare a winner. Twelve kills may be
excellent for a wolf, irrelevant for a plant, and evidence of a defect for a
rabbit.

## Derive rates from counts and opportunities

Capture both the result and the opportunity that made it possible. A successful
event without its denominator produces a volume stat, not an efficiency stat.

| Metric | Candidate definition |
| --- | --- |
| Hunt success | captures / hunt attempts |
| Escape rate | predator encounters survived / predator encounters |
| Foraging efficiency | energy gained / energy spent while foraging |
| Reproductive rate | offspring / valid reproductive opportunities |
| Juvenile survival | offspring reaching maturity / offspring born |
| Resource conversion | surviving offspring / food or energy consumed |
| Competition win rate | successful contests / contests |
| Territory efficiency | resources gained / occupied cell-time |
| Migration efficiency | useful distance / movement energy spent |
| Germination rate | germinations / viable seeds dispersed |

Every rate needs an explicit zero-denominator result such as `not applicable` or
`no opportunities`. It must not silently become zero, one, or a favorable score.

### Opportunity semantics matter

An opportunity must be defined at the resolver that actually decides the
outcome. Intent is not always an attempt. The current distinction between
Mating-state ticks and reproduction-funnel candidates is the right pattern.
Use the same discipline for hunts, escapes, contests, and feeding.

## Preserve enough context for expected metrics

Expected metrics compare observed outcomes with what the simulation's own rules
predicted under the experienced conditions. For a hunt, useful inputs might be
speed, awareness, distance, terrain, stamina, visibility, surprise, injury, and
hunger. If the resolver already calculates a probability, record the probability
and its named inputs alongside the outcome rather than trying to reconstruct it
later.

Example:

```text
Actual capture rate:   75%
Expected capture rate: 61%
Capture+:              123
```

Expected metrics can reveal that two organisms reached similar fitness through
different strategies. They should not become a second hidden combat system or a
machine-learned prediction layer unless the existing deterministic calculation
is demonstrably insufficient.

## Habitat and role adjustment

Raw performance is shaped by opportunity and environment. Comparable baselines
should therefore name at least:

- scenario and biome;
- terrain and resource density;
- predator and competitor density;
- population density;
- run window and season, if seasons exist;
- ruleset fingerprint and telemetry schema;
- seed set and any selected upgrade loadout;
- species role or strategy cohort.

An adjusted `+` statistic uses `100` as the average for a declared comparison
group:

```text
100 = comparison-group average
120 = 20% above that average
 85 = 15% below that average
```

The comparison group is part of the metric. `Survival+ 127` is not meaningful
unless the report identifies what environment, role, version, and seed cohort
defined `100`.

Do not compare elephants and rabbits by raw offspring count. A future `FIT+`
could compare how well each species performs relative to the expectations of
its ecological strategy, but it cannot be trustworthy until those expectations
and maturity rules are explicit.

## Keep fitness and ecological impact separate

The following concepts are valuable, but they answer different questions:

| Concept | Question | Readiness |
| --- | --- | --- |
| `FIT+` | How successful is this lineage relative to an appropriate baseline? | Future composite after survival and reproduction measures are trusted |
| `ECO` | What stabilizing or destabilizing effect does this species have on the ecosystem? | Future objective-specific analysis, not a synonym for fitness |
| Fitness above replacement | What additional lineage success occurs versus a defined replacement organism or population? | Counterfactual experiment, not a normal telemetry field |

`ECO` must state the ecosystem objective it measures. Diversity, persistence,
resource stability, and player-species survival are different objectives and
may disagree. A positive value must never be presented as universal ecological
goodness.

Fitness above replacement—provisionally called `FARP` or `EAV`—requires matched
runs in which a species is removed or replaced by an explicitly defined niche
baseline. It should be computed from controlled counterfactual experiments, not
estimated from one ordinary run.

## Role-specific stat sheets

Different roles should surface different statistics. A shared `FIT+` may
eventually connect the sheets, but it should not force every role into the same
raw columns.

| Role | Useful advanced metrics |
| --- | --- |
| Predator | Hunt%, Capture+, energy per kill, pursuit cost, territory efficiency, offspring survival |
| Herbivore | Forage+, Escape+, grazing efficiency, juvenile survival, resource conversion |
| Scavenger | discovery rate, contest avoidance, carrion efficiency, energy return, reproduction |
| Plant | resource efficiency, seed dispersal, germination%, mature offspring, competition, lineage growth |

The player-facing summary can remain simple—population, trend, survival,
feeding, and reproduction—while an advanced view exposes the underlying counts,
rates, expected values, and baselines.

## Capture guidance for Sim

### Preserve now

1. Keep raw outcomes and their denominators separately.
2. Keep run, seed, scenario, ruleset fingerprint, telemetry schema, tick,
   species ID, and upgrade/loadout context with every aggregate or event stream.
3. Preserve entity identity only where an individual lifecycle or causal link is
   required; do not turn all telemetry into an unbounded entity diary.
4. For probability-driven outcomes, retain the expected probability, outcome,
   and named contributing values when the resolver already knows them.
5. Make parent/offspring and maturity links available before attempting juvenile
   survival, lineage persistence, or individual fitness statistics.
6. Track energy gained and energy spent by activity before claiming food,
   movement, or hunting efficiency.
7. Keep statistics diagnostic-only. Capturing them must not change RNG order,
   opportunity validity, simulation decisions, or results.

### Derive outside the simulation

- rates and efficiencies;
- expected-versus-actual summaries;
- habitat/role-adjusted `+` values;
- composites and counterfactual comparisons.

These belong in report analysis unless the runtime UI has a concrete need for a
precomputed value. Reusing the existing report pipeline keeps formulas
versioned and prevents presentation concerns from entering domain behavior.

### Do not implement yet

- a universal fitness score;
- arbitrary weights that add unrelated traits together;
- `FIT+`, `ECO`, or replacement value without a declared baseline;
- territory, disease, migration, or seasons before those mechanics exist;
- a generic telemetry event bus or analytics framework;
- comparisons that treat correlation as causal contribution.

## Suggested implementation order

1. **Inventory:** map requested species statistics to existing schema-9 fields.
2. **Close denominators:** add only the missing attempts, opportunities,
   exposures, energy flows, or lifecycle links needed by the first stat sheet.
3. **Publish rates:** calculate transparent role-specific rates in report code
   and show their raw numerator and denominator.
4. **Add expected metrics:** use probabilities already calculated by resolvers.
5. **Calibrate `+` metrics:** run matched seed cohorts and version the baseline.
6. **Research composites:** define lineage fitness, ecological objectives, and
   replacement populations before proposing `FIT+`, `ECO`, or FARP/EAV.

The smallest useful first slice is one predator sheet, one herbivore sheet, and
one plant sheet built from existing telemetry plus only the missing denominators.
That is enough to test whether the language is readable without committing the
project to a speculative universal score.

## Validation checklist

- Counts reconcile with population, birth, and death ledgers where applicable.
- Attempts equal successes plus classified failures.
- Rates disclose numerator, denominator, and zero-opportunity state.
- Replaying the same ruleset and seed produces identical telemetry.
- Capturing statistics does not alter simulation results or RNG consumption.
- Expected values come from a named deterministic model version.
- Every `+` score names its comparison cohort and baseline version.
- Fitness and ecological impact remain separate in code, reports, and UI.
- Advanced statistics explain outcomes; they do not claim a causal story the
  captured evidence cannot support.

## Decisions still needed

Josh and Sim should decide these before composite work begins:

- Is the primary unit an individual, lineage, species population, run, or
  matched seed cohort?
- What counts as maturity for each species role?
- Which survival and reproduction window defines lineage success?
- What is the comparison cohort behind `100` for each `+` metric?
- Which ecosystem objective, if any, would `ECO` represent?
- What organism or population qualifies as a replacement baseline?
- Which individual-level events justify their storage cost?

Until those answers exist, prioritize accurate counts, denominators, and
context. They create lasting value without pretending the project already knows
what the single best species or build is.
