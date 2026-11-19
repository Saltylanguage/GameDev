# Rabbit + Fox Iterative Design Treatment

> Status: Experiment / vertical-slice design | Version: 0.2 | Scenario: Forest Edge

## Treatment premise

**Continuation scope (2026-09-04):** the 20-second fixture below remains a
bounded balance experiment. Target gameplay continues its evolved ecosystem
across decision breaks; it does not refill resources or replace Rabbit/Fox
populations. Existing single-window outcomes do not establish later-phase
recovery, long-run stability or sequential upgrade balance. The
[migration plan](../CONTINUOUS_SIMULATION_FLOW_PLAN.md) and
[retest register](../CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md) cover this gap.

The rabbit is not a complete gameplay unit by itself. Grass supplies the baseline
energy economy; the fox creates the readable threat that turns abundance into a
decision. We should therefore balance this pair as a small interaction system,
not as two independent species.

The player should be able to understand the causal chain:

> grass abundance -> rabbit growth -> fox opportunity -> rabbit movement/settlement choice -> population outcome

The intended challenge is not making grass scarce. Grass should usually be
plentiful. The challenge is maintaining a productive rabbit population while
preventing fox pressure from becoming either irrelevant or deterministic.

### First fixture values

- Grid: 32 x 20 cells
- Run: 20 seconds at a 0.1 second step interval
- Starting grass resource: 0.65 probability
- Starting rabbit: 0.15 probability
- Starting fox: 0.015 probability
- Rabbit: 2.2 movement speed, 16 starting/forage threshold, vision 5
- Fox: 0.8 movement speed, 32 starting/forage threshold, vision 6

This first fixture uses grass-resource cells and bare cells as the Forest Edge
proxy. A richer authored forest-edge terrain map is intentionally deferred until
the population interaction is readable; it should not be allowed to hide a
bad rabbit/fox economy.

## 1. Rabbit identity

**The rabbit is a mobile, fragile population that converts common grass into
growth, then survives by choosing where and when to expose itself.**

The rabbit should support two primary strategies:

- **Settlement:** build a stable population in a safe forest-edge pocket.
- **Migration:** follow grass and avoid fox concentration by moving between patches.

If the rabbit only wins by hiding in one safe corner, movement is decorative. If
it only wins by constantly fleeing, settlement and reproduction are decorative.
Both strategies need to be viable in the first scenario.

## 2. Rabbit treatment

### Dependency map

| Dependency | Treatment decision | Why it matters |
|---|---|---|
| Grass | Common, reliable baseline food | Keeps the species readable and prevents starvation from being the main challenge |
| Terrain | Forest edge is safer; open grass is more productive/exposed | Creates a meaningful safety-versus-growth geography |
| Foxes | Visible predator pressure, not constant contact | Turns population density and location into player decisions |
| Reproduction | Requires an energy reserve and available destination | Prevents instant exponential growth after every meal |
| Movement | Above-average mobility with terrain cost | Makes migration and escape real strategies |
| Population density | Crowding is productive until foxes can exploit it | Creates a predictable pressure curve |

### Minimal ruleset for the first experiment

- Rabbit eats grass or approved plant resources.
- Eating restores energy; it does not immediately reproduce the rabbit.
- A rabbit may reproduce only when above its reserve threshold and a legal cell is available.
- A rabbit that perceives a fox prioritizes escape over food.
- A rabbit without a visible fox prioritizes food when hungry, then mate/settlement behavior.
- Forest-edge terrain reduces exposure or improves escape reliability; open grass improves access to food.
- A rabbit occupies the creature layer while a seed/fruit resource may remain beneath it.
- Rabbits should prefer a destination that increases distance from visible foxes, not merely a random adjacent cell.

### Rabbit balance knobs

Keep the first pass small. Tune only these variables:

- movement speed
- vision range
- hunger threshold
- feeding energy
- reproduction reserve
- reproduction chance
- escape preference/range
- terrain preference
- fox detection/avoidance strength

Do not add shelter, scent, group intelligence, or inherited traits until this
small ruleset produces readable outcomes.

## 3. Fox preparation

### Fox identity

**The fox is a patient predator that converts rabbit concentration and predictable
routes into opportunity, but struggles to sustain itself when prey disperses.**

This gives the fox a complementary weakness: the rabbit fears exposure, while the
fox fears an empty or widely dispersed hunting ground.

### Minimal fox contract

- Fox seeks a visible rabbit before wandering toward food or mate behavior.
- Fox consumes a rabbit through a discrete attack/feeding interaction.
- A successful hunt restores enough energy to extend the fox's active period, not enough to reproduce immediately every time.
- Fox movement is slower or less flexible than the rabbit's on open ground.
- Fox has better prey perception but less ability to exploit ordinary grass.
- Fox reproduction requires sustained prey access, making fox population growth lag behind rabbit growth.
- Foxes should not coordinate in the first experiment; two foxes may create pressure through numbers alone.

### Fox balance knobs

- vision range
- movement speed
- attack range and success chance
- energy gained from a successful hunt
- energy drain per tick
- reproduction reserve
- preferred terrain
- prey priority and target selection

The fox should be reliable enough that the player can plan around it, but not so
efficient that every rabbit death is predetermined once a fox gets close.

## 4. Coupled population model

For the first balance pass, think in terms of three phases:

1. **Rabbit establishment:** grass supports rabbit growth while foxes are few.
2. **Predator opportunity:** rabbit density creates enough encounters for foxes to gain energy.
3. **Counterplay window:** rabbits migrate, split into pockets, or use an upgrade to reduce encounter quality.

The desired outcome is a repeating pressure cycle rather than a straight line:

> rabbit growth -> fox growth -> rabbit dispersal/decline -> fox starvation/decline -> rabbit recovery

### Initial target behavior

These are test targets, not final balance numbers:

- Grass should rarely be the direct cause of rabbit extinction.
- Foxes should become relevant because of rabbit density, not because they spawn overpowered.
- A successful rabbit run should survive at least one fox pressure wave.
- A successful fox population should require continued rabbit access.
- The player should have time to recognize pressure and respond before collapse.
- The same seed should support different outcomes when the player chooses different upgrades.

## 5. Upgrade pairing

Rabbit upgrades should create fox-facing decisions. Fox upgrades should create
new rabbit counterplay rather than simply raising kill rate.

### Rabbit: Trailblazer

- Effect direction: better movement through unfamiliar or costly terrain.
- New decision: migrate early or spend the run building a local population.
- Fox interaction: opens escape routes and makes dispersed settlement viable.
- Tradeoff: lower reproduction efficiency or weaker local food conversion.

### Rabbit: Warren

- Effect direction: improved survival/reproduction in a chosen local pocket.
- New decision: defend a home range or abandon it when fox pressure rises.
- Fox interaction: gives the fox a meaningful target while giving the rabbit a reason to tolerate pressure.
- Tradeoff: reduced value outside the established area.
- **1.0 identity decision (Josh, 2026-10-07):** Warren is the settled,
  stationary, territorial opposite of Trailblazer. Smaller litters and safer
  reproduction through burrows should make one home range reliable. Burrows is
  intended as one of the defining, high-impact upgrades in the later half of
  the Rabbit Genome tree, not an innate species rule. Crowding Tolerance may
  support this playstyle but is not its pillar. A working Burrows node is
  required for 1.0, not for the bounded S4 comparison. The node couples safer
  reproduction with smaller litters as its explicit tradeoff.

#### Burrows working behavior for 1.0

- When two Rabbits are ready to mate and find each other, they dig a burrow and
  stay inside through the birth. Foxes cannot hunt them while they are inside.
- After that birth, only the founding pair and their offspring can use that
  burrow to hide when hunted by Foxes. Other Rabbits cannot use empty slots.
- The burrow's maximum capacity is the litter size N from its first mating plus
  two parent slots (N+2 total). A later mating inside the same burrow resets
  capacity to that mating's litter size plus two.
- **Provisional litter tradeoff (Josh, 2026-10-08):** Burrows changes the
  Rabbit litter range from the current 2–5 kits to 1–3 kits. This is a 1.0
  design target pending balance evidence, not a change to the current Rabbit
  species asset.
- **Shelter energy (Josh, 2026-10-08):** Rabbits continue to pay their base
  energy cost while inside a burrow and must emerge to forage. The intended
  base cost is species-specific, alongside separate movement, mating, and
  crowding costs. Josh chose to preserve the current Rabbit effective rate of
  0.125 energy per tick, charged as 1 energy every 8 ticks. Burrow shelter is
  still a 1.0 design target, so this decision does not change the current
  species asset or introduce shelter behavior now.
- **Movement energy (Josh, 2026-10-08):** A completed move should carry a flat,
  species-configurable energy charge independent of terrain. Terrain or another
  feature may add a separate energy modifier later. The provisional Rabbit rate
  is 0.125 energy per completed move, accumulating as 1 energy after eight moves
  by that Rabbit. The provisional Fox rate is 0.5 energy per completed move, or
  1 energy after two moves by that Fox. Other species' movement rates remain
  open. The current terrain movement cost only affects travel speed; these
  energy charges are not yet in the simulation. Josh chose a separate measured
  balance pass after S4 before they are considered for production (2026-10-10).
- **Gestation baseline (Josh, 2026-10-07):** Animal reproduction now uses a
  provisional 48-tick interval between mating and birth in the game. A pending
  litter follows its parent and is lost if that parent dies before birth.
  Josh confirmed that the whole conceived litter waits together if too few
  birth cells are open at delivery. The parent retries each tick and retains
  the litter while it survives (2026-10-07). The current implementation caps
  the conceived litter by open birth cells at mating.
  Burrows will later shelter the founding pair during this interval; the
  shelter and family access rules are not part of this gestation change.
- Still to define: how a Fox can pressure a settled burrow. The provisional
  litter range and 48-tick interval remain open to balance tuning.

### Rabbit: Gardeners

- **S4 identity decision (Josh, 2026-10-10):** Gardeners maintain several
  nearby renewable food patches and move among them as those patches recover.
  Their range remains local, with more movement between resources than Warren's
  settled burrow and less map coverage than Trailblazer.
- Effect direction: feeding efficiency and seed dispersal renew a local network
  of food patches.
- New decision: invest in local food renewal or exploit abundance and move on.
- Fox interaction: creates attractive high-value rabbit areas that the fox can contest.
- **S4 immediate tradeoff (Josh, 2026-10-10):** Keep the current cost of one
  stored food reserve for each successful seed drop. Future food is gained by
  spending food held now; concentrated Rabbit activity around those patches also
  gives Foxes a place to hunt. The reserve cost is the first comparison input.

### Fox: Tracker

- Effect direction: better pursuit of recently observed rabbit movement.
- Rabbit response: migration must use terrain and route choice rather than simply running in a straight line.
- Tradeoff: weaker energy efficiency when no target is available.

### Fox: Ambusher

- Effect direction: stronger attack near forest-edge transition cells.
- Rabbit response: open-ground feeding becomes safer than forest-edge feeding in some situations.
- Tradeoff: poor performance in open terrain.

### Fox: Opportunistic breeder

- Effect direction: lower reproduction threshold after a successful hunt streak.
- Rabbit response: breaking up fox access becomes more important than merely preserving total rabbit count.
- Tradeoff: foxes remain fragile if the streak is interrupted.

## 6. Interaction experiments

### Experiment A: Baseline pressure

Use grass, rabbit, and fox with no upgrades. Compare several initial rabbit/fox
ratios while keeping grass plentiful.

Measure:

- time until first fox hunt
- rabbit population peak and trough
- fox population peak and trough
- number of recovery cycles
- percentage of rabbit deaths caused by foxes versus energy failure

### Experiment B: Settlement versus migration

Give the player Trailblazer or Warren. Test whether both can succeed in the
same Forest Edge layout with different routes and population histories.

### Experiment C: Fox target reliability

Test whether foxes consistently find dense rabbit pockets without instantly
tracking every rabbit across the map. The fox should create pressure through
opportunity, not omniscience.

### Experiment D: Recovery

Remove or reduce the rabbit population after a fox wave. Verify that grass and
the remaining rabbits can produce a comeback before the scenario timer ends.

## 7. Promotion gates

Promote the rabbit/fox pair to broader roster work only when:

- Players can explain why a fox attack succeeded.
- Grass abundance is not the primary source of tension.
- Settlement and migration are both viable.
- Foxes can decline naturally when prey access is lost.
- Rabbit and fox upgrades produce visibly different population histories.
- The pair remains readable with the existing terrain and occupancy layers.

If these gates fail, revise the interaction before adding another predator,
defensive system, scent layer, or species-specific exception.

## Next treatment step

Implement the minimal rabbit/fox rules as a coupled balance fixture, then run the
same treatment against the actual Forest Edge scenario. The execution details
are tracked in [`HARE_FOX_IMPLEMENTATION_PLAN.md`](HARE_FOX_IMPLEMENTATION_PLAN.md).

## Iteration 0.2 — Coupled playstyle pass

This pass keeps the 0.1 ruleset as the baseline and sharpens what the player is
actually choosing. The player does not command individual rabbits or foxes; the
player chooses which ecological pressure to improve at each reward break.

### Fun target

The pair is successful when the player can make a meaningful forecast, watch the
ecosystem respond, and recover from a setback through a different choice. The
core loop should feel like:

> identify pressure -> choose a counterplay -> observe the tradeoff -> adapt

### Locked identities for this iteration

- **Rabbit:** a mobile, fragile population that can either establish a safe pocket
  or keep moving between food patches.
- **Fox:** a patient pressure engine that converts rabbit concentration into
  temporary hunting strength, then weakens when the prey frontier disappears.
- **Grass:** a reliable baseline that keeps the conflict about exposure,
  settlement, and recovery rather than routine starvation.

### Interaction contract

1. Rabbit growth must precede meaningful fox growth. Foxes should not dominate
   before the player has time to recognize the first population wave.
2. Fox pressure must be spatially legible. A dense rabbit pocket, route, or edge
   transition should explain why encounters are increasing.
3. Rabbit dispersal must reduce encounter quality, but should carry a cost in food
   access, reproduction, or recovery time.
4. Foxes must lose momentum when rabbit access is interrupted. A successful hunt
   should extend the fox's active window, not guarantee indefinite growth.
5. Recovery must remain possible after a pressure wave. The player should be
   able to identify a comeback route rather than watch an irreversible collapse.

### Design hypotheses for the next evidence pass

| Hypothesis | Evidence required | Decision if false |
|---|---|---|
| Settlement and migration are both viable Rabbit builds | Compare final population, recovery cycles, and route snapshots for Warren vs Trailblazer | Adjust movement or local reproduction before adding mechanics |
| Foxes create tension through opportunity rather than omniscience | First-hunt timing, target distance, and hunts per fox across fixed seeds | Reduce target persistence/perception before changing attack strength |
| Gardeners creates a distinct risk/reward loop | Grass recovery, Rabbit density near cultivated patches, and fox encounter rate | Reduce the patch benefit or add a clearer exposure tradeoff |
| One fox wave is recoverable | Minimum surviving Rabbit population and time to positive growth after the trough | Tune fox energy gain and Rabbit recovery before adding defenses |

### Upgrade treatment for this pass

Use only one defining choice per branch. Do not combine multiple bonuses into a
single upgrade while the interaction is still being learned.

- **Trailblazer:** improves escape and migration; trades away local stability.
- **Warren:** improves survival and reproduction in a chosen pocket; loses value
  when the player must relocate.
- **Gardeners:** improves grass persistence or recovery; creates a valuable,
  contestable location for both species.
- **Tracker:** improves Fox pursuit of visible Rabbit movement; is inefficient
  when prey is dispersed.
- **Ambusher:** improves Fox pressure near edge transitions; is weaker in open
  ground.

Every upgrade must expose its affected rule, expected interaction, tradeoff, and
the telemetry that will show whether it mattered.

### Iteration promotion gate

Do not expand the roster or add scent, pack coordination, shelters, or inherited
traits until a fixed-seed comparison shows that:

- the three Rabbit branches produce visibly different population histories;
- Fox pressure is predictable without being deterministic;
- at least one Rabbit recovery occurs after a Fox peak;
- players can explain both a successful hunt and a successful escape; and
- the main tension comes from interacting populations and geography, not hidden
  tuning or random failure.

The next implementation pass should therefore change only balance values and
the minimum upgrade hooks needed to run these comparisons. New species rules
remain out of scope until the evidence passes this gate.
