# Scientific Data Economy — Future Work

Status: **approved product direction; deferred until the vertical-slice upgrade loop is proven**.

## Concept

Scientific data is the game's currency and connects simulation observation, in-run evolution, species mastery, and permanent progression. The player chooses between spending collected data during the current run or returning it to the Lab for lasting research.

## Data categories

- **Research Data:** general-purpose data earned from meaningful observations.
- **Plant Data:** earned from growth, propagation, resource recovery, and plant survival.
- **Herbivore Data:** earned from feeding, migration, reproduction, and predator avoidance.
- **Carnivore Data:** earned from hunting, population control, and predator survival.
- **Species Mastery Data:** species-specific progress earned through distinct accomplishments rather than repetition alone.

The first implementation should use the fewest categories needed by its content. Additional ecological or species-specific categories require a demonstrated gameplay purpose.

## Earning data

Data should reward meaningful discoveries and accomplishments, not raw tick count. Candidate triggers include:

- first observation of a behavior or upgrade activation;
- reaching or recovering from a population threshold;
- successful feeding, hunting, propagation, migration, or predator avoidance;
- survival under an authored scenario pressure;
- rare behaviors and species-specific accomplishments.

Repeated common events need caps or diminishing returns so large populations cannot generate unlimited currency. Existing deterministic telemetry should be reused to evaluate rewards before adding new event infrastructure.

## Spending tension

During a run, data can purchase temporary evolutionary upgrades that affect the current ruleset. Unspent data can instead be returned to the **Lab** and used for permanent research. Extinction may cause some unbanked data to be lost, with the exact loss rule left for playtesting.

This creates the central economic decision: **power now versus progress later**.

## The Lab

Permanent research should primarily widen future choices rather than produce unlimited stat inflation. Candidate uses include:

- adding upgrades to future reward pools;
- unlocking species, scenarios, starting loadouts, or alternate traits;
- improving previews and revealing deeper simulation statistics;
- unlocking research objectives and accomplishment tracks;
- preserving more collected data after defeat;
- unlocking advanced observation tools.

Small permanent numerical bonuses may be tested, but they must not trivialize early scenarios or erase meaningful simulation pressures.

Permanent upgrades are organized into Plant, Herbivore, and Carnivore research
trees. During a simulation, temporary upgrades instead form branching build
paths that reset after the run. The relationship between these layers is
defined in [`UPGRADE_SYSTEM_DIRECTION.md`](UPGRADE_SYSTEM_DIRECTION.md).

## Species mastery

A possible mastery progression is:

1. **Observed:** unlock the species profile and basic statistics.
2. **Studied:** reveal behavior details and upgrade affinities.
3. **Understood:** unlock species-specific upgrades.
4. **Mastered:** unlock an alternate starting trait, scenario, cosmetic, or build option.

Mastery objectives should require varied species behaviors. Repeating the same safe run should not be the optimal path.

## Suggested first implementation

- Research Data plus Plant, Herbivore, and Carnivore Data.
- One mastery track for the vertical-slice player species.
- A choice to spend data during the run or bank it at the Lab.
- A simple, clearly communicated consequence for extinction.
- Lab purchases limited to upgrade unlocks, diagnostics, and one persistent content unlock.

## Questions to resolve before implementation

- Which data categories are currencies versus progress meters?
- Does spending category data also require general Research Data?
- How much unbanked data survives victory, narrow survival, and extinction?
- When is data awarded: immediately, at phase summaries, or at run results?
- Which rewards are repeatable, capped per run, or first-discovery only?
- What is the smallest Lab screen and research tree that demonstrates the loop?
