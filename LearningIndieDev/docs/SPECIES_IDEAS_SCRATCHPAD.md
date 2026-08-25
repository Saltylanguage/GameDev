# Species simulation ideas scratchpad

Loose ideas are captured here before they become implementation commitments.
Promote an idea into a dated handoff or design note once it is ready for a
focused experiment.

## Cell and species ideas

- Visual direction: use compact, colorized geometric animal glyphs with flat
  fills and one unmistakable identifying feature per species. Preserve green,
  blue, and red role colors for plants, herbivores, and carnivores while using
  silhouette and small accents to distinguish individual species.

- Alpha offspring: an upgrade grants the species a chance to produce an alpha
  child. Alpha offspring require a special diet or other explicit conditions,
  similar in spirit to *The Isle*'s elder system, and receive significant stat
  bonuses when the requirements are met. The current prototype implements the
  chance and starting health/energy bonuses only; qualification is not yet part
  of the rule.
- Consider whether alpha status is inherited, earned by the child, or both.
- Consider whether alpha cells should influence nearby members of their species
  through pack behavior, reproduction priority, or territorial rules.
- Diet should eventually be a list of target species rather than one target.
  This enables food-web distinctions such as small carnivores eating insects
  while large carnivores do not, and grazers or browsers specializing in grass,
  fruit, nectar, or other plant resources.
- A species can define a play style or a scenario, not merely an NPC type. For
  example, bees could need hives and flower nectar to reproduce rapidly, while
  a flower-focused run could benefit from nearby bees.

## Upgrade ideas

- Alpha offspring unlock.
- Improved sight or a larger `VisionPattern`.
- Scent as a separate, periodically updated field that creatures query rather
  than a delayed version of sight. Revisit only as a focused diffusion/
  information-persistence experiment.
- Better food efficiency or slower starvation.
- Stronger crowding tolerance / larger sustainable pack size.
- Seed dispersal or improved plant propagation.
- Conditional attack or block patterns tied to cell state.

## Future species interaction stats

These are future candidates for richer species interactions. They are not a
request to add every stat at once; each should earn its place through a focused,
deterministic experiment.

### Perception, pursuit, and movement

- **Scent system:** add a second, distinct perception channel alongside sight.
  Scent could persist, diffuse, decay, or reveal recent activity, allowing the
  two channels to be layered for more dynamic pursuit and evasion. It should be
  treated as a separate field/state model, not as delayed vision.
- **Endurance:** distinguish short-term sprinting from sustained stalking or
  travel over longer distances. This creates a viable choice between a burst of
  speed and conserving energy for a prolonged pursuit.
- **Optional — sprint state, distance, and duration:** ambush predators could
  gain a short speed burst when close to prey. Nimble prey that recently escaped
  an encounter could receive a brief close-call escape advantage. The trigger,
  distance, duration, and energy cost would need to be explicit and bounded.

### Combat and escape

- **Attack versus Armor:** separate hit resolution from health loss. A seeded
  attack check against an Armor rating is a likely fit, followed by damage when
  the attack succeeds. This is a candidate D&D-like structure, not a locked dice
  formula. Attack reliability, Armor, damage, and HP should remain separately
  observable so a local combat effect is not mistaken for population fitness.
- **Dodge / Evasion:** give nimble species a way to avoid contact rather than
  relying only on HP or Armor. Evasion can interact with movement and escape;
  missed attacks may also create a species-specific delay before the next attack
  opportunity, reducing repeated-contact pressure.

### Mating and life history

- **Mating system improvements:** make mating a tracked interaction with legal
  partners, a pregnancy state, species-specific gestation measured in ticks,
  tunable litter sizes, and a clear birth event. Miscarriage remains optional
  until it proves useful for gameplay rather than adding noise.
- **Optional — maturity tiers:** infant, child, adolescent, adult, and elder
  phases could affect movement, reproduction, vulnerability, or lifespan. Some
  species may begin as adults when the intermediate phases are not gameplay-
  relevant. Tier durations must be tunable and should not exist only for
  simulation realism.
- **Significant-event age tracking:** record the age/tick of mating, pregnancy,
  birth, hunting, major injury, and death. This is primarily mutable state and
  telemetry rather than a species stat; it supports life-history rules,
  explainable reports, and later balance analysis.

## Candidate sequencing

Do not introduce these as one large species-data expansion. A plausible future
sequence is:

1. Attack/Armor/Dodge semantics and attack-delay telemetry.
2. Endurance and one explicit sprint interaction.
3. Mating, pregnancy, gestation, and event-age telemetry.
4. Scent as a separate perception-field experiment.
5. Optional maturity tiers and close-call buffs only if the earlier systems
   demonstrate a clear gameplay need.

Keep reusable species definitions, per-creature mutable state, and event
telemetry separate. Every new interaction must preserve fixed-seed replay and
have an isolated test before it is combined with the wider ecology.

## Emergent-system ideas

- Predator hunting-strategy ideation has been promoted into
  [`Species Design/HUNTING_STRATEGIES_IDEATION.md`](Species%20Design/HUNTING_STRATEGIES_IDEATION.md).
  Chase, ambush, and stalking remain experiments rather than committed rules.

- Future possible goal: geometry-directed colony construction through cellular
  automata. A species or cell type could receive a target shape plus a required
  material, then use local rules to approximate that geometry from resources
  available in the world. This could eventually support biologically motivated
  behavior such as ant colonies excavating tunnels and chambers or beavers
  gathering wood to construct dams that alter water flow.
- Start with deliberately simple viability proofs such as constructing a
  concrete cross or a wooden triangle from an anchored blueprint. Only expand
  toward decentralized gathering, transport, coordination, repair, and
  environmentally functional structures if the small experiment produces
  interesting variation. Open questions include how geometry is encoded, how
  cells coordinate without global placement control, how imperfect or
  incomplete builds are evaluated, and whether construction consumes, carries,
  or transforms nearby resources.
- Separate perception from movement: visible targets, priority selection, then a
  one-step move or later grid pathfinding.
- Food reserves should create carrying capacity rather than only acting as a
  binary requirement.
- Crowding can impose energy costs, force separation, reduce reproduction, or
  create territory rather than simply blocking births.
- Plant food could eventually come from moisture, soil quality, or drought
  events instead of a single fixed reserve.

## Technical design questions

- Define the smallest useful species-data model that permits distinctive
  behaviors and interactions without copying every rule into every species.
  Favor composable, data-driven rules with focused custom mechanics over a
  universal behavior-plugin framework.
- Determine which data belongs to reusable species definitions, which belongs
  to a scenario, and which is mutable state of an individual cell during a run.
- Generalize diet only when a second valid diet target makes the list valuable;
  preserve the simple single-target prototype until then.

## Analytics and player feedback ideas

- Develop a way to identify interactions that are fun, engaging, dynamic, and
  worth further design time as the content catalogue grows.
- Later analysis should combine simulation telemetry with player feedback to
  find patterns around engagement, experimentation, meaningful decisions, and
  satisfying emergent outcomes. Fun is a latent outcome that can be estimated
  and calibrated, not treated as unknowable.
- Do not optimize a single universal fun score. Use a profile of measurable
  signals: player agency, legibility, tension and recovery, novelty, strategic
  diversity, and replay intent.
- Attach player feedback to a precise run: scenario/ruleset fingerprint, seed,
  upgrades, timeline, and outcome. Useful early prompts include whether choices
  mattered, whether the result was understandable, and whether the player
  wanted another run.
- Use the combined evidence to rank candidate simulations for human design
  review. Automated search should identify promising hypotheses, while player
  evidence calibrates whether the measured signals correspond to enjoyment.
- Guard against Goodhart's Law: a generator can exploit any one target metric.
  Preserve multiple objectives, run diversity, and human review instead of
  letting an automated score declare a design fun on its own.

## AI and automation ideas

- Long-term: run bounded batches of experimental cellular-automata rulesets to
  surface interesting candidate simulations for human review.
- The experiment harness may eventually support controlled, injectable behavior
  modifications, but must keep seeds, ruleset fingerprints, and outcome reports
  reproducible. It should be isolated from the shipping simulation loop.
- Use automated search to generate and rank hypotheses, not to declare a
  simulation fun without design review and player evidence.

## Open questions

- What makes an alpha requirement interesting without making it mandatory for
  every successful run?
- Are alpha bonuses inherited by descendants or only applied to the qualifying
  offspring?
- Should alpha cells be unique, limited per pack, or allowed to compete?
- Should sight be a radius, an arbitrary pattern, or a line-of-sight pattern
  that can be upgraded independently?

## Experiment reminders

- Keep fixed seeds, rule values, grid dimensions, run duration, and step interval
  with every balance trial.
- Prefer one focused variable change per A/B comparison.
- Record population curves, extinctions, feeding events, reproduction events,
  and maximum local group size before deciding that a rule is better.
