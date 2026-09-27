# Hunter patrol and solo stalking — feature plan

**Status:** Proposed feature plan for review (2026-09-27)

**First fixture:** Forest Edge, Fox hunting Hare

**Scope:** Patrol and solo stalking; corralling and pack coordination deferred

**Schedule:** Unassigned; this plan does not add work to the active sprint

## Intended player experience

A Fox searches a recognizable part of the board when it has no prey in sight. If
it finds a Hare, it can approach cautiously, but the Hare has a readable chance
to notice and escape. Failed searches and approaches cost the Fox time and
energy. Hare dispersal and terrain can break the hunt, allowing recovery after
a Fox pressure wave.

Patrol answers **where and in what pattern the hunter searches**. Stalking
answers **how it approaches one prey animal**. Neither behavior implies shared
targets, pack roles, or control over another hunter.

This is a proposed follow-on to the [Hare/Fox implementation
plan](HARE_FOX_IMPLEMENTATION_PLAN.md), not a replacement for its baseline and
promotion gates. The [hunting strategies note](HUNTING_STRATEGIES_IDEATION.md)
remains the wider idea set. Start this feature's implementation only after the
current chase/escape behavior has a fixed-seed baseline and a readable outcome.

## Current seam to build on

- `SpeciesBehaviorSystem` selects `Hunting`, `Wandering`, and `Threatened`;
  `SpeciesSimulation` resolves attacks before movement and processes movement in
  shuffled order with destination claims.
- `SpeciesPerception` finds visible prey and threats. Vision uses a range
  pattern, without terrain occlusion. A Hare becomes `Threatened` when a Fox is
  visible and tries to increase its distance from that Fox.
- `SpeciesNavigation` finds a first step toward interaction range on the grid.
  Current prey pursuit routes toward the prey's current position. The
  remembered-target lookup can locate a live entity anywhere on the board while
  its tracking timer remains positive; this is unsuitable as stalking evidence
  without an explicit information rule.
- Fox hunting currently has an energy threshold; no-prey behavior can lead to
  mating or ordinary movement. Patrol must fit that priority rather than making
  every Fox search constantly.
- Current behavior transitions, movement, encounters, attacks, kills, and
  population outcomes are partly measurable. Search effort, target loss,
  detection, and approach outcome need focused telemetry.

Relevant runtime files: `SpeciesSimulation.cs`, `SpeciesPerception.cs`,
`SpeciesNavigation.cs`, `SpeciesCell.cs`, and `SpeciesSimulationMetrics.cs`.
Changes to per-creature state must also be checked through movement copying,
phase continuation, checkpoints, ruleset fingerprints, and reports.

## Behavior contracts

### 1. Patrol

- **Entry:** a hunt-eligible Fox has no currently visible or legitimately
  remembered Hare and is not resolving a higher-priority survival or mating
  action. Decide the exact hunger/mating precedence against the current rules
  before writing code.
- **Search area:** choose a bounded local area from information available to that
  Fox, such as its previous prey sighting or a local terrain feature. A stable
  fallback is needed for a Fox that has never seen prey. Do not use the live
  location or density of unseen Hares to choose a patrol point.
- **Pattern:** support two tunable search shapes in the first patrol slice:
  a serpentine sweep through adjacent lanes of a bounded area, and a direct
  shuttle that travels between two reachable nodes and reverses. The shuttle
  nodes can first be generated from known local positions; they do not require
  landmark discovery. A later authored or discovered landmark can supply one
  of those nodes without changing the movement rule.
- **Movement:** the active pattern proposes the next destination cell. Move
  toward it using the existing grid movement and terrain costs, briefly
  inspect or change direction on arrival, then advance the sweep or reverse
  the shuttle. A blocked destination causes a bounded replan or skips that
  node, not a permanent stall.
- **Tuning:** compare pattern, search radius or route span, sweep lane spacing,
  time spent at a node, and total search budget. Start with a small set of
  scenario/fixture values; promote only useful knobs into immutable authored
  species or scenario data. Each Fox retains only its current route progress.
- **Exit:** seeing prey hands control to an approach or chase decision; loss of
  hunt eligibility, a higher-priority need, or search timeout returns to the
  existing behavior priorities.
- **Prey response:** patrol itself grants no attack bonus. A Hare that sees the
  Fox keeps its normal warning and escape opportunity.

The source of patrol nodes is separate from the pattern that traverses them.
"Watering hole to burrow and back" is a good future shuttle route, but neither
landmark type nor a mechanic for identifying it currently exists. Using such a
route later requires the world to represent those places and an explicit rule
for how this Fox learned about them: scenario-known, observed, or visited.
Unseen landmarks must not become hidden map knowledge. Until then, use local
generated nodes and test fixtures; keep a deterministic fallback when fewer
than two valid nodes are known. A board-wide prey heatmap, territory ownership,
and scent are outside this pass.

### 2. Solo stalking

- **Entry:** a hunt-eligible Fox has directly observed a Hare and there is a
  feasible cautious approach. A failed feasibility check falls back to the
  existing chase or abandonment rule; it must not freeze the hunter.
- **Information:** store the last observed prey position and age of that
  observation. Refresh it only through direct perception. Once sight is lost,
  the Fox can search that last known position for a short, authored duration;
  it cannot query the prey's live position through the tracking entity ID.
- **Approach:** choose a *destination cell* separately from the prey target.
  Score a small set of reachable candidate cells by approach distance, terrain
  cost, and the agreed detection rule. This permits cautious positioning now
  and, later, a different source of destination suggestions for interception
  or flank positions. It does not require a general strategy framework.
- **Prey detection:** define a visible rule for when the Hare notices the Fox
  and how long that alert lasts. Preserve the current Hare escape behavior once
  alerted. Terrain may affect detection only if its effect is explicit in the
  simulation and visible enough for a player to understand.
- **Commit:** attack or switch to the existing chase only at a specified
  opportunity, such as reaching a useful distance or position before alert.
  Record what triggered the commitment. Attacks still use the normal combat
  resolver; stalking is not an unexplained damage multiplier.
- **Abort:** early detection, stale information, unreachable positioning,
  excessive energy cost, or the target's death ends the setup. The Fox pays a
  bounded cost and returns to patrol, chase, or other existing priorities.

The current circular vision and immediate Hare threat response may leave no
useful approach on some boards. Prove a fair approach and a viable Hare escape
in a small fixture before committing to a specific cover or alert model.

## Work packages and gates

| Work | Deliverable | Gate to continue |
| --- | --- | --- |
| PS-0 — Chase baseline | Fixed seeds and compact board examples for visible pursuit, Hare escape, no-prey behavior, Fox energy, and prey recovery; record the current ruleset and current unverified balance changes. | The team can explain a sampled hunt and escape without patrol or stalking. |
| PS-1 — Patrol contract | Specify hunt/mate priority, search-area information, timeout, and fallback; implement Fox-only serpentine sweep and two-node shuttle in small fixtures, using generated local nodes. Expose only tuning knobs that change their measured behavior. | Both patterns produce distinct, reproducible routes; Foxes reach or skip blocked nodes, react to visible prey, leave fruitless searches, and retain existing occupancy and reproduction behavior. |
| PS-2 — Stalking feasibility | Prototype last-seen memory and one explicit detection/alert rule in small layouts: open space, edge transition, obstructed route, and dispersed Hares. Compare with chase under the same seeds. | At least one layout produces a legible cautious approach, and at least one lets an alerted Hare escape. If not, revise the interaction before production integration. |
| PS-3 — Solo stalking | Implement destination selection, commit/abort costs, state propagation, and focused telemetry; connect only the Fox/Hare fixture first. | Stalking uses no hidden live target information, cannot run indefinitely, survives continuation/checkpoint, and does not break fixed-seed replay. |
| PS-4 — Readability and ecology | Add a selected-hunter intent/last-seen cue and concise hunt outcome in existing presentation/report paths; compare sweep, shuttle, and baseline under matched seeds for search effort, encounters, kills, escapes, starvation, and Hare recovery. | Players can distinguish patrol, stalk, chase, abandonment, and a successful Hare response; the patterns create different search paths without making ecological pressure unrecoverable. |

Implement each package as a focused change. Runtime behavior tests should cover
priority transitions, target loss/death, blocked routes, destination conflicts,
energy limits, and identical results for identical seeds. Run matched Forest
Edge experiments only after the local contracts pass. Do not choose final
balance values from a single seed or report population improvement without a
matched comparison.

## Compatibility boundary for deferred corralling

Corralling remains deferred. Its later form is undecided: independent hunters
may create enough spatial pressure, or a genuine pack behavior may need shared
target and flank assignments. The first two features should preserve these
options through a few concrete boundaries:

1. Keep **observed prey**, **desired destination**, and **resolved move** as
   separate concepts. A hunt target is not automatically the cell to enter.
   Patrol pattern and node source likewise stay separate: a later known
   landmark can supply a node without defining a new movement system.
2. Make destination choice from a consistent read-only board state, then use
   the existing movement resolver for legal occupancy and destination claims.
   This leaves room for later multi-hunter planning without allowing two Foxes
   to occupy the same cell.
3. Keep patrol and stalking knowledge local to the individual hunter. Do not
   silently share prey coordinates, invent pack membership, or have one Fox
   move another. A later coordination rule can supply destination suggestions
   through an explicit boundary if evidence justifies it.
4. Preserve deterministic target selection and tie-breaking. Record enough
   intent and outcome to tell whether multiple Foxes blocked an escape route
   by chance or through a future coordination rule.
5. Keep prey threat evaluation replaceable at the *decision point*. Today's
   nearest-threat escape can be extended to evaluate several visible Foxes
   without changing how legal movement is resolved. Do not add multi-threat
   behavior until a corralling experiment is approved.

These are constraints on the patrol/stalking implementation, not a request for
pack data structures, role enums, shared blackboards, or coordination code now.

## Decisions still needed before implementation

- What is the Fox's patrol priority relative to mating when it is above or
  below its hunting energy threshold?
- Which first search anchor is legible on Forest Edge: last sighting, a local
  terrain edge, or another bounded location derived without hidden prey data?
- Which patrol settings produce a meaningful difference between sweep and
  shuttle without adding more authoring controls than a designer can tune?
- What counts as Hare detection during a stalk, and does the existing terrain
  offer a meaningful way to approach without immediate alert?
- What exact condition makes a Fox commit, switch to chase, or give up?
- Which small board cue best communicates a search, cautious approach, and
  alerted Hare without filling the whole board with paths?

Resolve these through PS-0 and PS-2 fixtures before adding broad authored
species fields or a generalized hunting strategy API.
