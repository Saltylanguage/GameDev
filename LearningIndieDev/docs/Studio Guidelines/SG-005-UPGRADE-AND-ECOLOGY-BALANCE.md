# SG-005 — Upgrade and Ecology Balance

**Guideline ID:** SG-005  
**Status:** Active  
**Version:** 1.1  
**Adopted:** 2026-09-06  
**Owner:** Josh  
**Audience:** Designers, developers, researchers, and AI agents working on
species, Mutations, Genomes, scenarios, simulation rules, or balance tools.

## Purpose

The game may eventually contain many species and hundreds of upgrades. We will
keep that manageable by balancing a small set of shared rules and ecological
relationships instead of treating every species and upgrade as an unrelated
special case.

This guideline defines the balance language, evidence, and review gates for:

- a species' natural starting rules;
- its permanently unlocked and selectively active **Genome**;
- the nine temporary **Mutations** chosen during an expedition;
- stat changes, one-time effects, and ability unlocks;
- species matchups, environmental conditions, and whole ecosystems.

The long-term goal is not to make one species dominant. It is to let the player
develop many species toward a diverse, resilient ecosystem. Improving one
species may create a new imbalance that gives the player a reason to study and
improve another.

## 1. Keep Mutations and Genomes separate

| System | Meaning | Lifetime | Who receives it |
| --- | --- | --- | --- |
| **Mutation** | An acute adaptation to the current environment | Current expedition only | The selected species for that expedition |
| **Genome** | A permanently unlocked species option that the player can turn on or off between simulations | The unlock persists; the active configuration is frozen for one simulation | Every population of that species, including when it is not player-controlled |

A species' effective rules in a Species Simulation are:

```text
Natural species rules + active Genome + ordered expedition Mutations
```

Its effective rules in a Biome Simulation are:

```text
Natural species rules + active Genome
```

A completed ten-phase expedition contains nine Mutation decisions. Mutation
order and acquisition phase remain part of the run record. Starting or ending
an expedition does not remove a Genome unlock. Mutations never transfer into a
Biome Simulation.

The permanent collection of unlocked nodes and the active Genome are different
things. Turning a node off does not refund or remove the unlock. The exact
active-node budget and reallocation cost remain product decisions. Whatever
model is chosen, the active Genome must be fixed when a simulation launches.

If non-player species gain temporary adaptations in a future mode, that is a
separate feature decision. It must not be inferred from the Genome rule above.

## 2. Give each simulation mode its own goal

The two simulation modes do not need one shared victory condition.

| Mode | Main player question | Upgrade rules | Appropriate success measures |
| --- | --- | --- | --- |
| **Species Simulation** | How can I help this species succeed under the current pressures? | Every present species uses its active Genome; the selected species also gains temporary Mutations | The authored species objective, survival, reproduction, resource use, and readable interaction with threats and opportunities |
| **Biome Simulation** | Which combination of species Genomes creates the ecology I want? | Every participating species uses its active Genome; Mutations are absent | Scenario-specific diversity, stability, balance, recovery, and persistence of intended relationships |

Focal-species success is valid evidence for a Species Simulation. It becomes a
problem only when it is treated as the sole measure of Genome or Biome health.
A Genome option may help its species in one mode and destabilize a biome in the
other. That can be an intentional and interesting tradeoff.

Primary progress is the permanent set of Genome options the player has
unlocked, not whether every active configuration succeeds. A poor configuration
should produce useful feedback and invite reallocation; it should not erase the
player's unlocks.

## 3. Define a balanced ecosystem correctly

Balance does not mean equal population sizes, flat population graphs, or equal
odds in every matchup. Plants, herbivores, and predators occupy different roles
and may be healthy at very different population levels.

A balanced ecosystem is one that, within the declared scenario and time
window:

- keeps its intended species and resources present;
- supports recognizable growth and pressure cycles;
- avoids unavoidable runaway growth or collapse;
- can recover from expected disturbances;
- offers more than one useful player response;
- makes the causes of improvement or failure understandable.

Each scenario must define its own healthy operating ranges and pressures. Do
not invent a universal target population and apply it to every species.

Representative active loadouts built from highly developed Genome libraries
should be tested together. The available combination space should contain
stable, diverse ecosystems without requiring every unlocked node to be active
or making every possible configuration healthy.

## 4. Balance through shared layers

Use this shared path when designing and evaluating content:

```text
Trait or upgrade
    -> shared stat or rule
        -> capability
            -> ecological interaction
                -> outcome
```

Example:

```text
Longer Legs
    -> movement speed rises and energy use rises
        -> escape and migration improve
            -> predator contact and food access change
                -> survival, reproduction, and resource pressure change
```

### Shared stats and rules

These are the authored quantities used by the simulation, such as movement
speed, attack, defense, vision, energy use, litter size, food value, or seed
drop chance. Use stable IDs from the project registry when a supported value
already exists.

### Capabilities

Capabilities explain what several lower-level values allow a species to do.
Examples include pursuit, escape, concealment, detection, feeding efficiency,
resource recovery, reproduction, shelter, or crowding tolerance.

Keep the capability list small enough to understand. A starting target of
roughly 12–20 capabilities is reasonable, but evidence decides the final
number. Add a capability only when real content cannot be described accurately
through the existing set.

### Ecological interactions and outcomes

An interaction describes what happens between a species, another species, and
the environment. Outcomes include survival, extinction risk, energy balance,
population growth, resource recovery, predator–prey pressure, and recovery
after disruption.

An upgrade should normally change shared stats or a named ability and let the
simulation produce the ecological outcome. Avoid upgrades that directly force
an outcome such as “win this matchup” or “set population health to good.”

## 5. Allow distinctive abilities without creating hidden exceptions

Species need unique behaviours such as burrowing, dam building, ambush, or seed
dispersal. A shared balance language does not require every ability to become a
number-only stat change.

Every ability must still declare:

- the capability it strengthens or changes;
- when and where it can act;
- its energy, time, space, resource, or opportunity cost;
- the species, environment, or behaviour that can counter it;
- the local measurement that proves the ability worked;
- the ecological measurements used to judge its wider effect.

The ability remains an explicit simulation rule. Do not hide bespoke behaviour
inside reflection, field names, a generic expression language, or a catch-all
upgrade script.

## 6. Use Adaptation Value as an estimate, not a verdict

**Adaptation Value (AV)** is an internal design budget used to compare unlike
effects before expensive simulation and playtesting. It is not shown to the
player and is not a universal fitness score.

The project may calibrate one AV as a small expected improvement in a declared
outcome under a named reference panel. Any exchange rate must state:

- the affected stat or capability;
- the starting value at which it was measured;
- the species role and scenario panel;
- the measurement window;
- the average result and its variation;
- the evidence version or experiment that produced it.

For example, the project may learn that one unit of movement speed is worth an
estimated amount of AV for Hare escape in Forest Edge. That estimate does not
automatically apply to a plant, a confined map, a different starting speed, or
a different expedition phase.

AV has three jobs:

1. Catch an obviously overfilled or underfilled upgrade before testing.
2. Give tiers and paths a common planning budget.
3. Show where measured results disagree with the original estimate.

AV must not:

- replace births, deaths, energy, movement, feeding, combat, population, or
  recovery measurements;
- combine every ecological outcome into one unexplained score;
- turn a provisional estimate into a balance claim;
- imply that two upgrades with equal AV are equally useful in every context.

Keep mode-specific estimates separate. Mutation AV estimates temporary value in
a Species Simulation and must account for when the Mutation is acquired. A
Genome's capacity cost estimates the size and flexibility of the species-level
change. Its Biome impact remains a separate set of observed effects, such as
resource pressure, persistence, diversity, and recovery. A Genome node does not
become cheap merely because it is harmful to biome stability.

The first AV exchange rates and tier budgets remain **TBD** until a reference
scenario panel and baseline measurements are approved. Values such as 2 AV for
a minor upgrade or 10 AV for a major upgrade are examples, not current project
balance.

## 7. Give upgrades and build paths a budget

Every Mutation and Genome node should declare a provisional tier and AV range.
Compare the estimate with the node's measured effect after testing.

Do not balance a species by adding the value of every node in its tree. Players
may not be able to acquire every Mutation, and early choices act for longer
than late choices. Instead, compare:

- choices offered at the same expedition phase;
- complete reachable Mutation paths across all nine decisions;
- active Genome strength at comparable unlock and allocation stages;
- the combined active Genome and Mutation states a player can actually produce
  in Species Simulations;
- representative cross-species Genome configurations that fit the active
  budget in Biome Simulations.

A species may have more or fewer nodes than another. What matters is the power,
flexibility, cost, and ecological role of reachable builds—not equal node
counts.

An upgrade can intentionally sit outside its normal budget when its purpose and
cost are explicit. Record the exception and test the resulting path; do not
quietly change the budget to make the upgrade appear compliant.

## 8. Separate baseline value from contextual value

Each upgrade has two useful value descriptions:

- **Baseline value:** its approximate effect across the approved reference
  panel.
- **Contextual value:** how strongly its value changes with simulation mode,
  biome, opponent, starting population, current phase, active Genome, and
  earlier Mutations where they are allowed.

Situational strength is desirable. Cold resistance can be excellent in a
tundra and nearly irrelevant in a tropical biome. That is ecology, not a flaw.

Required balance tags should remain descriptive and stable. They may include:

- species and ecological role;
- Mutation or Genome;
- stat change, one-time effect, or ability;
- capability family;
- relevant environments and pressures;
- costs and maintenance needs;
- counters and weaknesses;
- prerequisites, exclusions, and stacking rule;
- application time and duration;
- provisional tier and AV range;
- measurements needed for review.

Tags help group and analyze content. They do not execute gameplay logic.

## 9. Make power create ecological obligations

Across a species and its upgrade paths, stronger capabilities should usually
create a need, exposure, or opportunity for another part of the ecosystem.

Examples include:

- better senses using more energy;
- faster movement increasing food demand;
- stronger armor reducing mobility;
- larger litters increasing local crowding and resource pressure;
- stronger predators depending on reliable prey access;
- faster plant growth increasing competition for space or water.

Not every upgrade needs a printed penalty. The cost can emerge through energy,
time, space, food, exposure, or a lost alternative. It must still be possible
to explain and measure.

Use diminishing returns where evidence shows that a stat scales without a
healthy limit. A curve such as `stat / (stat + K)` is one possible tool, not a
default formula for every system. The chosen curve and constant must come from
the behaviour being controlled and must be covered by boundary tests.

## 10. Balance matchups and ecosystems, not isolated numbers

An individual stat can be strong without being unhealthy. Review what happens
when a species meets another species under a declared environment, active
Genome configuration, and—only in a Species Simulation—Mutation state.

Maintain a representative test panel rather than testing every theoretical
combination. The panel should grow with shipped content and include:

- neutral reference scenarios;
- scenarios that favour and resist each major capability;
- intended predator–prey and plant–consumer relationships;
- low, middle, and highly developed Genome libraries with legal active
  configurations;
- early, middle, and late Mutation acquisition points;
- at least one stress or recovery case.

A matchup matrix is a diagnostic view, not a demand for 50/50 outcomes. Flag a
species or build when it dominates most relevant opponents and environments,
has no readable counter, or makes other choices irrelevant.

Keep separate Species and Biome scorecards. A result can be good for the focal
species, bad for biome stability, and still be correct evidence in both views.

## 11. Measure marginal value, dominance, and synergy

### Marginal value

Measure the value of another stat point at several starting values. If ten more
defense has a major effect at a low value and almost no effect at a high value,
the diminishing return may be healthy. If every added point becomes more
powerful, investigate runaway scaling.

### Choice dominance

Track how often an upgrade is selected when it is eligible in structured bot,
search, or human-play data. A very high or very low pick rate is a prompt to
investigate, not an automatic failure. The important warning is **choice
collapse**: one option is correct across nearly every relevant context.

### Synergy

Test combinations as well as individual upgrades. Estimate unexpected synergy
as:

```text
combined impact - impact of A - impact of B
```

Large positive or negative differences identify interactions that the
individual estimates did not predict. Preserve acquisition order when testing
Mutations because accumulated world state can make A then B differ from B then
A.

Do not attempt every possible combination. Start with shared capabilities,
known multipliers, branch neighbours, popular choices, and the strongest
measured nodes. Expand when evidence shows a new risk.

## 12. Use the existing deterministic simulation as the evidence base

The current `CellSim` tools already support fixed-seed runs, population history,
activity and mortality records, upgrade fingerprints, matched comparisons, and
phase-aware schedules. Extend that path in small steps instead of building a
second simulator or a large dashboard first.

Balance work follows the studio evidence protocol:

```text
Experiment -> Run -> Report -> Analysis -> Human Decision
```

For each upgrade or path:

1. **Validate the definition.** Confirm IDs, targets, values, timing,
   prerequisites, exclusions, costs, and recorded fingerprints.
2. **Prove the local effect.** Use the smallest fixture that can show the stat,
   event, or ability behaves as described.
3. **Run a matched comparison.** Use the same seeds, scenario, starting state,
   and window for baseline and changed arms.
4. **Test context.** Use the approved panel to find where the option is strong,
   weak, or irrelevant.
5. **Test reachable combinations.** Cover likely paths, order-sensitive pairs,
   and suspected synergies.
6. **Apply the correct scorecard.** For a Mutation, use its Species Simulation
   goal and declared interactions. For a Genome, record both focal-species and
   Biome effects without assuming they should point in the same direction.
7. **Review the ecosystem where relevant.** Examine resources, other species,
   pressure cycles, extinction, and recovery for Genome and Biome decisions.
8. **Make a human decision.** Approve, revise, restrict, or remove the content
   and record the evidence boundary.

Start with a small diagnostic seed set. Increase the panel when variance,
rarity, or the importance of the decision requires it. A very large run count
does not repair a poorly defined question or mismatched inputs.

## 13. Grow balance tooling in evidence-backed stages

Add tools only after the related manual analysis is understood.

Recommended order:

1. Add the simulation mode, permanently unlocked Genome nodes, active
   per-species Genome configurations, and allowed Mutation loadout to run
   provenance.
2. Add a versioned capability and AV reference table outside runtime rules.
3. Produce a scenario/species matchup table from existing reports.
4. Add parameter sweeps that measure value at several starting levels.
5. Add reachable-path and targeted pair/synergy comparisons.
6. Add choice-dominance analysis when bot/search or sufficient player-choice
   data exists.
7. Build a dashboard or scheduled broad sweep only when the report contracts
   and repeated questions are stable.

Every automated warning must show the input panel, expected range, observed
result, variation, and source report. It should identify a review candidate,
not silently rebalance content.

## 14. Required review gate for production content

An upgrade or material balance change is ready for production review when:

- its player-facing effect, timing, duration, cost, and weakness are clear;
- it uses a stable, supported rule or an explicitly implemented ability;
- its tier and provisional AV estimate are recorded;
- its local effect is proven;
- matched runs use the same seeds and comparable inputs;
- its strong and weak contexts are known;
- likely combinations and acquisition order have appropriate coverage;
- its effect has been judged with the scorecard for every mode where it can
  appear;
- Genome effects on other species and resources have been reviewed;
- no universal choice or matchup dominance is left unexplained;
- the report and analysis preserve exact versions and fingerprints;
- Josh records the final approve, revise, restrict, or remove decision.

Passing this gate does not freeze a value forever. Later species, environments,
Genome stages, or player evidence can trigger a new review.

## 15. Architecture and authoring rules

- Keep simulation and balance calculations in plain deterministic domain code.
- Keep authoring assets as definitions; resolve them into immutable run data.
- Keep UI and dashboards read-only with respect to simulation results and
  balance authority.
- Use one stable registry for supported stats and one explicit runtime mapping
  for each executable effect.
- Store permanently unlocked Genome nodes separately from each species' active
  configuration.
- Freeze active Genome configurations for every participating species by stable
  species ID before either simulation mode starts.
- Record simulation mode, base species data, unlocked and active Genome state,
  ordered Mutations when permitted, acquisition phase, scenario, seed,
  versions, and fingerprints in every balance run.
- Do not use asset names, paths, reflection, or display text as gameplay
  identity.
- Do not rewrite historical reports when values or formulas change.
- Do not create a universal modifier, scripting, behaviour-plugin, or event-bus
  framework in anticipation of future content.

## 16. What this guideline does not require

It does not require:

- equal species, equal populations, or equal pick rates;
- every upgrade to work in every environment;
- exact real-world biology;
- one universal fitness score;
- identical goals for Species and Biome Simulations;
- every legal Genome configuration to improve biome stability;
- exhaustive testing of every possible combination;
- automatic AI balancing or automatic production value changes;
- a dashboard before the first balance questions can be answered manually;
- broad content production before the Hare / Fox / plant slice is understood.

The goal is controlled variety: species can be very different while their
costs, opportunities, counters, and ecological consequences remain explainable
and testable.

## Revision history

| Version | Date | Change |
| --- | --- | --- |
| 1.1 | 2026-09-06 | Separated Species and Biome Simulation goals; made Genome unlocks permanent but activation configurable; excluded Mutations from Biome Simulations. |
| 1.0 | 2026-09-06 | Established Mutation / Genome balance language, Adaptation Value boundaries, ecosystem and matchup review, and staged automation guidance. |
