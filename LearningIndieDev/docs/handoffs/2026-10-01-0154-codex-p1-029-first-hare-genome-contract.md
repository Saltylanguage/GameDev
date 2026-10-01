# P1-029 first Hare Genome contract — design direction

- Date: 2026-10-01
- Status: Contract accepted by Josh on 2026-10-01; implementation and balance evidence remain follow-up work.
- Owner: Josh, with design and simulation review.
- Related item: [P1-029 in Loose Ends](../LOOSE_ENDS.md)

## Decisions from Josh

- The first Genome task is to define one production Hare node before expanding
  the tree.
- The first Hare path should help with famine. Fertile droppings that renew
  grass are the leading direction; better metabolism remains an alternative or
  later path.
- Fertile droppings should expose Hares to more tracking by Foxes and other
  predators. A smaller stomach is an optional balancing idea, not a requirement.
- The Gene Lab should open after the player's first few runs. The first time the
  player selects Rabbit Genome, a short tutorial should explain that paths
  support different playstyles and benefits.
- Rabbit Data purchases permanent access to the full Rabbit Genome tree.
  Subsequent upgrades are resettable and a reset costs generic data.
- Stat rewards stack by default unless a node explicitly says otherwise.
- Player-facing copy should be brief, clear, practical, and qualitative rather
  than a stat list.
- Each proposed node should be reviewed for species/playstyle fit, intended
  effect, and a drawback another species in its food web can exploit.

## Accepted progression defaults

- Gene Lab access unlocks after three completed runs; wins and losses count.
- Rabbit Data buys permanent access to the full Rabbit tree and funds active
  node upgrades.
- Resetting clears the active allocation, returns the Rabbit Data spent on
  that allocation, and charges a flat Research Data fee. The fee amount and
  individual node prices are balance values to set later.
- Keep the existing eight-point active capacity. Freeze the configured
  Genome at simulation launch.
- The Rabbit Genome tile only buys permanent tree access; it has no separate
  gameplay effect.
- The first selection of Rabbit Genome opens the brief path/playstyle tutorial.

## Recommended first-node contract

**Working name:** Fertile Droppings
**Player copy:** “Help new grass grow, but leave a trail Foxes can follow.”

When an active Hare with food in reserve succeeds at dispersing a seed, it
creates grass in a nearby valid empty cell and leaves a short-lived scent trail
that a predator which hunts Hare can follow beyond normal visual contact. A
predator still needs to see a live Hare before making an attack. The trail's
range, lifetime, and exact pursuit rule need a bounded implementation design;
they are not balance values approved by this note.

This is a better first-node fit than better metabolism because it supports the
Gardeners playstyle by renewing the food supply and gives Foxes a readable way
to exploit that choice. It is an indirect famine buffer: a Hare must already
have food in reserve for the existing seed-drop resolver to run, and each
successful drop consumes one reserve. The effect may improve later food access
but should not be described as guaranteed famine prevention.

Do not add a smaller stomach to the first version. It would be a second penalty
on a node that already spends carried food and exposes the Hare to hunting; it
would also make famine relief harder to read. Better metabolism can be a later
alternate path with a more direct effect on Hare food demand. Revisit stomach
size only if evidence shows stored food creates an exploit.

The Genome tree tile should grant permanent access only. Giving the access tile
its own gameplay effect would blur tree access with an upgrade choice.
Repeated stat levels should add together by default, within valid stat bounds;
any exclusive or non-stacking behavior must be declared on that node.

## Pricing and implementation follow-up

The accepted model is a permanent Rabbit tree license plus a resettable active
upgrade allocation. Rabbit Data buys the license and active node upgrades;
resetting refunds the active allocation and costs a flat amount of Research
Data. Exact prices are balance tuning, not unresolved contract rules.

## Evidence required before balance approval

Use a matched control and candidate with the same scenario, seed set, starting
state, and simulation step. Attribute successful seed drops and their food
reserve cost. Review new grass/plant persistence, Hare starvation and survival,
Fox pursuit and successful catches, and run-to-run variation. Evaluate the
node both as a Hare benefit and as a food-web change. Confirm the short player
copy through an in-game explanation review. Set the numeric seed chance and
scent parameters from that evidence; this note approves no values.

## Current implementation boundary

`SpeciesSimulation.ResolveSeedDrops` already creates grass in an available
movement-pattern cell for a creature with `FoodReserve >= 1` and a successful
`SeedDropChance` roll, then consumes one reserve. Production Hare currently
authors `seedDropChance: 0`. `SpeciesPerception.TryFindThreatTarget` searches a
predator's vision pattern, and there is no scent-trail behavior. The first
Genome effect therefore combines an existing, inactive Hare seed-dispersal
path with new predator-tracking behavior.

The current Genome profile implementation stores permanently unlocked node IDs
separately from active node IDs. That data model does not match a permanent
whole-tree license and resettable active allocation; update the save/profile
contract and migration plan before implementing persistence. P1-029 closes the
design-contract gap only. The optional four-hour S4 Genome spike remains outside
the active sprint; runtime implementation and balance evidence need separate
capacity and tracking.
