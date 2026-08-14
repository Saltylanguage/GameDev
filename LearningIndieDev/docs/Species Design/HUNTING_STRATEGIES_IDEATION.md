# Hunting Strategies — Design Ideation

> Status: Exploratory future work | Updated: 2026-08-14

## Purpose

Explore how predators could hunt through distinct, readable strategies such as chasing, ambushing, and stalking. These should create different spatial problems for prey and different upgrade paths for predators—not merely different movement-speed or attack bonuses.

This note extends the Fox concepts in [`HARE_FOX_ITERATIVE_TREATMENT.md`](HARE_FOX_ITERATIVE_TREATMENT.md). It does not expand the current implementation scope or override that treatment's promotion gates.

## Shared hunt shape

Different strategies can use the same player-facing phases:

```text
Acquire prey -> Approach -> Commit -> Resolve -> Recover or abandon
```

A strategy changes how the predator moves through those phases:

- what information is needed before it acquires a target;
- how close it tries to get before committing;
- which terrain or prey behavior creates an opportunity;
- how much energy and time failure costs;
- when it gives up rather than pursuing indefinitely.

This vocabulary may be enough for early experiments. Do not create a universal hunt-state framework until at least two implemented strategies prove that they genuinely share the same contract.

## Initial strategies

### Chasing

**Fantasy:** identify exposed prey and overpower it through sustained pursuit.

- Best in open terrain where routes are direct.
- Acquires a visible target and commits quickly.
- Pays high movement/energy costs while pursuing.
- Succeeds through speed, endurance, target selection, or interception.
- Abandons when the prey escapes vision, reaches costly terrain, or the expected energy return becomes poor.

**Strength:** immediate and easy to read.

**Weakness:** expensive failures; prey can win through distance, cover, sharp route changes, or forcing difficult terrain.

**Good first species direction:** a pursuit predator such as a wolf, or a Fox experiment used as the simplest baseline.

### Ambushing

**Fantasy:** wait near a predictable route or valuable resource, then make one strong commitment.

- Selects a useful ambush area rather than tracking one prey indefinitely.
- Benefits from cover, terrain transitions, food patches, watering areas, or migration routes.
- Spends relatively little energy while waiting and a large amount during the attack burst.
- Loses value when prey disperses, changes routes, detects the threat, or avoids the contested location.

**Strength:** powerful near a prepared location and creates spatial tension around valuable areas.

**Weakness:** poor in open terrain or when prey behavior is unpredictable; time spent waiting can produce no return.

**Good first species direction:** the existing Fox Ambusher branch near Forest Edge transitions.

### Stalking

**Fantasy:** remain outside the prey's danger response, close distance gradually, and attack only from a favorable position.

- Requires awareness of both predator-to-prey distance and prey detection risk.
- Approaches slowly or indirectly while the target remains unaware.
- Commits when distance, terrain, facing, or prey isolation crosses a threshold.
- Breaks off when detected too early or when the target joins a safer group.

**Strength:** lower-risk approach and better attack opportunity than a direct chase.

**Weakness:** slow, vulnerable to vigilant prey, and likely to waste time if target information becomes stale.

**Design warning:** convincing stalking probably needs short-lived target memory or confidence, plus a readable prey-alert model. It should follow simpler chase and ambush experiments rather than introducing hidden omniscience.

## Possible later strategies

- **Interception:** predict a visible prey route and move toward a future crossing point instead of its current cell.
- **Pack pursuit:** multiple predators pressure different escape directions; defer until solo chase is readable.
- **Flush and strike:** one predator or environmental pressure drives prey toward another threat; requires coordination and strong causal feedback.
- **Aerial strike:** observe broadly, commit in a short burst, then recover; potentially useful for owl-like predators.
- **Opportunistic hunting:** attack only weakened, isolated, young, or distracted prey; useful as target-selection identity rather than a complete movement strategy.

These remain idea seeds. Each needs distinct prey counterplay before earning implementation work.

## Prey counterplay

A hunting strategy is only interesting if prey behavior and player upgrades can answer it.

| Predator strategy | Prey counterplay | Risk if missing |
| --- | --- | --- |
| Chase | Speed, endurance, dispersal, cover, terrain familiarity, route changes | Chase becomes a deterministic stat check. |
| Ambush | Vigilance, alternate routes, scouts, dispersed feeding, safer timing | Valuable terrain becomes an unavoidable death zone. |
| Stalk | Detection range, group alertness, unpredictable movement, breaking line of sight | Stalking becomes invisible guaranteed damage. |
| Interception | Route variation, decoys, rapid direction changes | Prediction becomes omniscience. |
| Pack pursuit | Group defense, terrain chokepoints, splitting the pack | More predators only multiply kill rate. |

Prey must receive enough warning to understand the threat before population collapse. Counterplay can be species behavior, run upgrades, terrain use, or a combination.

## Information and terrain considerations

Strategies should differ partly through the information they possess:

- **Visible location:** current sight; sufficient for the first chase experiment.
- **Recent location:** short-lived memory of where prey was last observed; a stalking/tracking candidate.
- **Route evidence:** repeated movement through cells or terrain transitions; useful for ambush placement.
- **Scent:** a separate stateful field and intentionally deferred experiment, not delayed sight under another name.

Terrain can create the strategic geometry:

- open ground favors long pursuit and visibility;
- dense or costly terrain helps prey break a chase;
- edge transitions and resource patches create ambush opportunities;
- narrow routes make interception or pack behavior stronger;
- cover should affect information or commitment conditions, not provide unexplained percentage bonuses.

## Energy and failure costs

Hunting needs an economy so the strongest behavior is not always active:

- chasing spends energy per pursuit step and needs a clear abandon rule;
- ambushing spends time and opportunity while conserving movement energy;
- stalking spends time and modest energy, then risks losing the setup when detected;
- attacks have a commitment/recovery cost even when they miss;
- successful feeding restores enough energy to continue, but should not automatically guarantee reproduction.

A failed hunt should produce a visible consequence for the predator and a readable success for the prey.

## Player-facing feedback

The player should be able to identify:

- which prey a predator is considering;
- whether it is searching, approaching, waiting, pursuing, attacking, or abandoning;
- why the predator gained an opportunity;
- whether terrain, detection, energy, or prey behavior ended the hunt;
- which upgrade affected the outcome.

Possible presentation includes a selected-predator intent label, short target/path overlays, alert indicators on threatened prey, ambush-area highlighting, and a compact hunt summary. Avoid permanent full-board lines that turn the ecosystem into unreadable debug visualization.

## Telemetry candidates

Record only what is needed to compare strategies:

- targets acquired and abandoned;
- time and distance spent in approach/pursuit;
- distance at commitment;
- attacks, successful hunts, and success rate;
- energy spent per attempt and gained per success;
- detection-before-commit events;
- time waiting in an ambush area;
- prey escape cause: distance, vision loss, terrain, detection, group response, or predator energy;
- terrain at acquisition, commitment, and resolution;
- upgrade attribution and strategy identifier.

Useful evaluation measures include hunts per predator, energy return per attempt, prey recovery after pressure, and whether players can explain a sampled hunt.

## Relationship to upgrades and mastery

Hunting strategies can become Carnivore run branches:

- **Pursuer:** speed, endurance, and target switching.
- **Ambusher:** preparation, cover use, and burst commitment.
- **Stalker:** approach control, detection avoidance, and isolated-prey selection.

Permanent Carnivore research should unlock or widen these possibilities rather than grant universal kill-rate bonuses. Species mastery can add distinctive variations: a Fox-specific edge ambush, Wolf interception/pack options, or Owl aerial commitment.

Mastery data should reward varied evidence such as successful hunts in favorable terrain, efficient abandonment of bad pursuits, or use of a species' defining strategy—not raw kill count alone.

## Suggested experiment order

1. **Chase baseline:** use current sight and movement to establish acquisition, pursuit cost, abandonment, escape, and hunt telemetry.
2. **Fox ambush:** add one authored terrain-transition opportunity and compare it with open-ground performance.
3. **Player comprehension:** verify that players can distinguish chase success, ambush success, and prey escape.
4. **Stalking feasibility:** only then test short-lived last-seen memory and prey alertness as one bounded experiment.
5. **Later strategies:** consider interception or coordination only if solo strategies remain strategically distinct.

Use fixed seeds and the same Hare/Fox fixture for A/B comparisons. Change one strategy rule at a time before adjusting global attack strength.

## Promotion questions

- Does each strategy create a different spatial pattern on the board?
- Does each have a visible strength, weakness, and prey response?
- Can a predator fail without appearing broken or random?
- Can a player explain why the hunt began, succeeded, failed, or was abandoned?
- Does the strategy change upgrade decisions rather than only final kill count?
- Is its extra state justified by a measurable gameplay difference?

If the answer is no, revise the interaction before adding another strategy or generalized behavior system.

