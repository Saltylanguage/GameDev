# Simulation focus and journey economy — notes for Josh and Sim

Prepared October 14, 2026. The currency points below are **Josh's stated
direction for the meeting**, not a record of Sim's agreement. Open questions
remain questions until the team records a decision.

**Meeting completed:** Bevin, Chrono, and Salty met October 14 at 9 pm. The
decisions below supersede the preparation notes where they differ. The
preparation notes remain as a record of questions and proposals.

## Meeting decisions

1. **Phase clear during initial playtesting:** Bevin will guide and adjust the
   criteria as the ecology is tested. The team did not lock population totals
   or a births, deaths, and kills formula. The October 7 tick-600/tick-700
   direction remains context, not a fixed production phase gate.
2. **Journey failure (clarified October 15):** Failure ends the whole journey
   without a cash-out. All currency earned during that run and all temporary
   upgrades acquired during it are lost. Permanent Desktop upgrades and
   unlocked upgrade or feature availability remain on the profile, including
   permanent unlocks earned through an accomplishment during the run. The
   conditions for those unlocks are not designed yet. For illustration only,
   mating 100 rabbits might unlock a Burrow Genome upgrade; 100 is not an
   approved threshold, and this is not an implemented feature.
3. **Currency boundary:** Field Data carries between phases and nodes during
   one journey. Other currency types can leave a successful journey for the
   Desktop and Genome upgrades. The exact permanent wallet categories and
   reward amounts were not settled by this answer.
4. **Within-journey spending:** The intended options include removing Hares,
   adding or removing Foxes, adding or removing Plants, and buying a random
   event as a gamble. Costs, limits, timing, targeting, and which options enter
   the first playable slice remain to be designed.

These decisions describe the intended game. The current prototype does not
implement journey failure settlement or the complete spending menu.

## Aim of the meeting

Agree what work to park while Forest Edge simulation content and balance take
priority, align on the playable roguelike journey, and settle the smallest
currency contract needed to build it.

## 1. Focus the next work block

- Put ecology viability, starting populations and event pacing, visible build
  differences, and simulation-node goals first. The October 12 confirmation
  review found useful choice effects but no reliable tick-600 ecology baseline.
- Park broad Desktop/Lab polish, final-volume art and audio, additional species
  and scenarios, full Genome purchase implementation, cloud/active-run resume,
  and the secondary DNA simulator. Keep only presentation and tooling needed to
  play, understand, and measure the current journey.
- Decide which of the later journey placeholders must become playable for the
  next milestone. Their existence on the map does not approve every encounter
  type or a production route length.

**Decision to capture:** the immediate simulation/content outcome, the work
explicitly deferred, and any essential exception.

## 2. Re-align on the roguelike journey

The working pitch is **cellular automata as a roguelike**. The current journey
prototype has three opening trailheads, a reward choice, two playable
simulation cycles, a habitat-condition branch, and an evolving ecosystem that
continues between those cycles. Upgrades acquired in one journey simulation
carry into the next; that carryover was approved October 7. The reset point for
a newly started journey, the final route length, and the later encounter
content remain open. See [Journey Prototype](../JOURNEY_PROTOTYPE.md).

- A *run* is the whole journey. Define its exact success/failure boundary and
  what happens in play after a failed simulation node.
- Make the tick-600 clear plus viability through tick 700 test precise: which
  populations and recovery behavior count, and how is viability checked?
- Decide what a branch changes in the ecosystem or player's decisions, and
  what happens after a failed simulation node or journey.
- Choose the minimum complete route and node types for the next playable slice.
  Reconcile the older six-round roadmap/product brief once this is agreed.

**Decision to capture:** one player-facing run flow, success/failure rule,
upgrade reset boundary, and minimum route.

## 3. Journey rewards and permanent currency

**Josh's direction to take into the meeting:**

1. **Field Data** is spent only during a journey and disappears when the run
   completes.
2. Permanent progression has a separate currency for **each species** and
   **each biome**. Rabbit Data buys Rabbit Genome upgrades; Forest Data buys
   Forest biome upgrades.
3. Each simulation node has explicit requirements and a **set reward** earned
   when those requirements are met.
4. A *run* is the whole journey. Qualifying simulation-node rewards remain
   provisional during it. Deposit their permanent currency only when the
   journey succeeds; a failed journey pays none.
5. For now, Mutation choices are free and Skip gives no reward.

**Still to define:** the exact journey success/failure gate, each node's clear
requirements and reward amount, and what the player does after a failed node.

**Feature questions for Sim and Josh:**

- What is the minimum useful Field Data earning/spending loop within the
  journey? The current Reinforcement purchase and example condition reward are
  prototype behavior, not a complete economy.
- Does each node reward one named species or biome currency, or can it reward
  several? How does the player know its requirement and set reward before
  entering?
- What is the first Forest biome upgrade, how is it configured between runs,
  and what evidence would show a meaningful ecological effect?
- How will the profile store separate wallets? Its current generic `Currency`
  field is a foundation, not this approved multi-currency economy.

**Decision to capture:** a one-page node contract (requirements, clear/fail,
fixed reward, currency type) and a journey settlement rule. Set owners for the
next balance experiment and the economy/profile design follow-up.

## References

- [October 7 journey and ecology decisions](2026-10-07-Journey-and-Ecology-Balance-Agenda.md)
- [October 12 Forest Edge confirmation review](../handoffs/2026-10-12-1609-codex-forest-edge-fresh-seed-confirmation-results-review.md)
- [Profile persistence foundation](../handoffs/2026-10-14-1424-codex-profile-persistence-foundation.md)
- [Scientific Data Economy](../SCIENTIFIC_DATA_ECONOMY.md)
