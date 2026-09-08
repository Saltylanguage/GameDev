# Scientific Data Economy — Future Work

Status: **approved product direction; deferred until the vertical-slice upgrade loop is proven**.

## Concept

Scientific data is the game's currency and connects simulation observation,
temporary Mutations, species mastery, and permanent Genome progression. The
player chooses between spending collected data during the current Species
Simulation or returning it to the Lab to unlock lasting Genome options.

## Data categories

- **Research Data:** general-purpose data earned from meaningful observations.
- **Plant Data:** earned from growth, propagation, resource recovery, and plant survival.
- **Herbivore Data:** earned from feeding, migration, reproduction, and predator avoidance.
- **Carnivore Data:** earned from hunting, population control, and predator survival.
- **Species Mastery Data:** species-specific progress earned through distinct accomplishments rather than repetition alone.

The first implementation should use the fewest categories needed by its content. Additional ecological or species-specific categories require a demonstrated gameplay purpose.

## Earning data

An expedition contains consecutive simulation phases in the same ecosystem.
Phase summaries and purchase/skip breaks do not reset unspent currency,
accomplishments or world state. Phase earnings and final settlement must be
identified separately and applied once; repeated Continue or viewing a summary
cannot award them again. The current survivor-count reward is a prototype,
not a validated continued-expedition economy. See the
[migration plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md); earning amounts, loss rules
and permanent Lab integration remain deferred.

Data should reward meaningful discoveries and accomplishments, not raw tick count. Candidate triggers include:

- first observation of a behavior or upgrade activation;
- reaching or recovering from a population threshold;
- successful feeding, hunting, propagation, migration, or predator avoidance;
- survival under an authored scenario pressure;
- rare behaviors and species-specific accomplishments.

Repeated common events need caps or diminishing returns so large populations cannot generate unlimited currency. Existing deterministic telemetry should be reused to evaluate rewards before adding new event infrastructure.

## Spending tension

During an expedition, data may purchase temporary Mutations that affect the
selected species for the rest of that expedition. Unspent data can instead be
returned to the **Lab** and used for permanent Genome research. Extinction may
cause some unbanked data to be lost, with the exact loss rule left for
playtesting.

This creates the central economic decision: **power now versus progress later**.

## The Lab

Every species has its own Genome tree. Purchasing a node unlocks it permanently;
the player may turn unlocked nodes on or off between simulations. The active
Genome is frozen at launch and applies to every population of that species,
including when the player is controlling another species. Candidate uses
include:

- improving a species stat within a controlled balance budget;
- unlocking a new species behaviour or ecological response;
- adding Mutations to that species' future reward pool;
- unlocking starting traits or alternate biological strategies;
- improving previews and revealing deeper simulation statistics;
- unlocking research objectives and accomplishment tracks;
- preserving more collected data after defeat;
- unlocking advanced observation tools.

Genome improvements need two views. Species Simulations may judge how they help
the focal species. Biome Simulations must also test their effect on resources,
predators, prey, competitors, biodiversity, stability, and recovery. A node may
be useful to its species and harmful to a biome. Highly developed Genome
libraries should offer legal cross-species active configurations that form a
richer and more resilient ecology; every possible configuration need not do so.

The exact active-Genome budget is still open. A reassignable capacity or point
limit could preserve meaningful builds without making unlocks reversible. The
starting bias is to allow reallocation between simulations and freeze it once a
simulation begins.

Biome projects are a possible separate use for banked data. These would improve
or reshape the habitat itself—such as water retention, habitat corridors, soil
recovery, or available niches—rather than act as hidden species buffs. Whether
a small restoration baseline gates later Biome progress remains an open design
decision.

Plant, Herbivore, and Carnivore remain useful data categories and navigation
filters. They are not permanent-buff targets: Genome changes belong to a stable
species identity. During a simulation, Mutations form branching build paths
that reset after the expedition. The relationship and balance requirements are
defined in [`UPGRADE_SYSTEM_DIRECTION.md`](UPGRADE_SYSTEM_DIRECTION.md) and
[`SG-005 — Upgrade and Ecology Balance`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md).

## Species mastery

A possible mastery progression is:

1. **Observed:** unlock the species profile and basic statistics.
2. **Studied:** reveal behavior details and upgrade affinities.
3. **Understood:** unlock species-specific upgrades.
4. **Mastered:** unlock an alternate starting trait, scenario, cosmetic, or build option.

Mastery objectives should require varied species behaviors. Repeating the same safe run should not be the optimal path.

## Suggested first implementation

- Research Data plus the fewest role or species data categories that have a
  proven purchase use.
- One mastery track for the vertical-slice player species.
- A choice to spend data during the run or bank it at the Lab.
- A simple, clearly communicated consequence for extinction.
- One small Hare Genome preview or purchase after its balance and persistence
  contracts are ready; broader Genome content remains later work.

## Questions to resolve before implementation

- Which data categories are currencies versus progress meters?
- Does spending category data also require general Research Data?
- How much unbanked data survives victory, narrow survival, and extinction?
- When is data awarded: immediately, at phase summaries, or at run results?
- Which rewards are repeatable, capped per run, or first-discovery only?
- How is Genome data earned without turning species collection or repetitive
  grinding into the game's main objective?
- What is the smallest Gene Lab Genome tree that demonstrates permanent unlocks
  and a reversible active configuration?
- Does active Genome capacity increase through mastery, research, Biome
  progress, or a fixed scenario rule?
- Are Biome projects a separate progression track, and does an introductory
  restoration project gate advanced Biome challenges?
