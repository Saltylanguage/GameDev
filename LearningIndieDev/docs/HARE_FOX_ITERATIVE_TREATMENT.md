# Hare + Fox Iterative Design Treatment

> Status: Experiment / vertical-slice design | Version: 0.1 | Scenario: Forest Edge

## Treatment premise

The hare is not a complete gameplay unit by itself. Grass supplies the baseline
energy economy; the fox creates the readable threat that turns abundance into a
decision. We should therefore balance this pair as a small interaction system,
not as two independent species.

The player should be able to understand the causal chain:

> grass abundance -> hare growth -> fox opportunity -> hare movement/settlement choice -> population outcome

The intended challenge is not making grass scarce. Grass should usually be
plentiful. The challenge is maintaining a productive hare population while
preventing fox pressure from becoming either irrelevant or deterministic.

### First fixture values

- Grid: 32 x 20 cells
- Run: 20 seconds at a 0.1 second step interval
- Starting grass resource: 0.65 probability
- Starting hare: 0.15 probability
- Starting fox: 0.015 probability
- Hare: 2.2 movement speed, 16 starting/forage threshold, vision 5
- Fox: 0.8 movement speed, 32 starting/forage threshold, vision 6

This first fixture uses grass-resource cells and bare cells as the Forest Edge
proxy. A richer authored forest-edge terrain map is intentionally deferred until
the population interaction is readable; it should not be allowed to hide a
bad hare/fox economy.

## 1. Hare identity

**The hare is a mobile, fragile population that converts common grass into
growth, then survives by choosing where and when to expose itself.**

The hare should support two primary strategies:

- **Settlement:** build a stable population in a safe forest-edge pocket.
- **Migration:** follow grass and avoid fox concentration by moving between patches.

If the hare only wins by hiding in one safe corner, movement is decorative. If
it only wins by constantly fleeing, settlement and reproduction are decorative.
Both strategies need to be viable in the first scenario.

## 2. Hare treatment

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

- Hare eats grass or approved plant resources.
- Eating restores energy; it does not immediately reproduce the hare.
- A hare may reproduce only when above its reserve threshold and a legal cell is available.
- A hare that perceives a fox prioritizes escape over food.
- A hare without a visible fox prioritizes food when hungry, then mate/settlement behavior.
- Forest-edge terrain reduces exposure or improves escape reliability; open grass improves access to food.
- A hare occupies the creature layer while a seed/fruit resource may remain beneath it.
- Hares should prefer a destination that increases distance from visible foxes, not merely a random adjacent cell.

### Hare balance knobs

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

**The fox is a patient predator that converts hare concentration and predictable
routes into opportunity, but struggles to sustain itself when prey disperses.**

This gives the fox a complementary weakness: the hare fears exposure, while the
fox fears an empty or widely dispersed hunting ground.

### Minimal fox contract

- Fox seeks a visible hare before wandering toward food or mate behavior.
- Fox consumes a hare through a discrete attack/feeding interaction.
- A successful hunt restores enough energy to extend the fox's active period, not enough to reproduce immediately every time.
- Fox movement is slower or less flexible than the hare's on open ground.
- Fox has better prey perception but less ability to exploit ordinary grass.
- Fox reproduction requires sustained prey access, making fox population growth lag behind hare growth.
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
efficient that every hare death is predetermined once a fox gets close.

## 4. Coupled population model

For the first balance pass, think in terms of three phases:

1. **Hare establishment:** grass supports hare growth while foxes are few.
2. **Predator opportunity:** hare density creates enough encounters for foxes to gain energy.
3. **Counterplay window:** hares migrate, split into pockets, or use an upgrade to reduce encounter quality.

The desired outcome is a repeating pressure cycle rather than a straight line:

> hare growth -> fox growth -> hare dispersal/decline -> fox starvation/decline -> hare recovery

### Initial target behavior

These are test targets, not final balance numbers:

- Grass should rarely be the direct cause of hare extinction.
- Foxes should become relevant because of hare density, not because they spawn overpowered.
- A successful hare run should survive at least one fox pressure wave.
- A successful fox population should require continued hare access.
- The player should have time to recognize pressure and respond before collapse.
- The same seed should support different outcomes when the player chooses different upgrades.

## 5. Upgrade pairing

Hare upgrades should create fox-facing decisions. Fox upgrades should create
new hare counterplay rather than simply raising kill rate.

### Hare: Trailblazer

- Effect direction: better movement through unfamiliar or costly terrain.
- New decision: migrate early or spend the run building a local population.
- Fox interaction: opens escape routes and makes dispersed settlement viable.
- Tradeoff: lower reproduction efficiency or weaker local food conversion.

### Hare: Warren

- Effect direction: improved survival/reproduction in a chosen local pocket.
- New decision: defend a home range or abandon it when fox pressure rises.
- Fox interaction: gives the fox a meaningful target while giving the hare a reason to tolerate pressure.
- Tradeoff: reduced value outside the established area.

### Hare: Gardeners

- Effect direction: hare activity improves grass recovery or preserves a small resource patch.
- New decision: cultivate a stable ecosystem or exploit abundance and move on.
- Fox interaction: creates attractive high-value hare areas that the fox can contest.
- Tradeoff: slower immediate mobility or population growth.

### Fox: Tracker

- Effect direction: better pursuit of recently observed hare movement.
- Hare response: migration must use terrain and route choice rather than simply running in a straight line.
- Tradeoff: weaker energy efficiency when no target is available.

### Fox: Ambusher

- Effect direction: stronger attack near forest-edge transition cells.
- Hare response: open-ground feeding becomes safer than forest-edge feeding in some situations.
- Tradeoff: poor performance in open terrain.

### Fox: Opportunistic breeder

- Effect direction: lower reproduction threshold after a successful hunt streak.
- Hare response: breaking up fox access becomes more important than merely preserving total hare count.
- Tradeoff: foxes remain fragile if the streak is interrupted.

## 6. Interaction experiments

### Experiment A: Baseline pressure

Use grass, hare, and fox with no upgrades. Compare several initial hare/fox
ratios while keeping grass plentiful.

Measure:

- time until first fox hunt
- hare population peak and trough
- fox population peak and trough
- number of recovery cycles
- percentage of hare deaths caused by foxes versus energy failure

### Experiment B: Settlement versus migration

Give the player Trailblazer or Warren. Test whether both can succeed in the
same Forest Edge layout with different routes and population histories.

### Experiment C: Fox target reliability

Test whether foxes consistently find dense hare pockets without instantly
tracking every hare across the map. The fox should create pressure through
opportunity, not omniscience.

### Experiment D: Recovery

Remove or reduce the hare population after a fox wave. Verify that grass and
the remaining hares can produce a comeback before the scenario timer ends.

## 7. Promotion gates

Promote the hare/fox pair to broader roster work only when:

- Players can explain why a fox attack succeeded.
- Grass abundance is not the primary source of tension.
- Settlement and migration are both viable.
- Foxes can decline naturally when prey access is lost.
- Hare and fox upgrades produce visibly different population histories.
- The pair remains readable with the existing terrain and occupancy layers.

If these gates fail, revise the interaction before adding another predator,
defensive system, scent layer, or species-specific exception.

## Next treatment step

Implement the minimal hare/fox rules as a coupled balance fixture, then run the
same treatment against the actual Forest Edge scenario. The next design review
should use population histories and a few saved snapshots, not intuition alone.
