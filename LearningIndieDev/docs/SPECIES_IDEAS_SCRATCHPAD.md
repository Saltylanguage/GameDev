# Species simulation ideas scratchpad

Loose ideas are captured here before they become implementation commitments.
Promote an idea into a dated handoff or design note once it is ready for a
focused experiment.

## Cell and species ideas

- Alpha offspring: an upgrade grants the species a chance to produce an alpha
  child. Alpha offspring require a special diet or other explicit conditions,
  similar in spirit to *The Isle*'s elder system, and receive significant stat
  bonuses when the requirements are met. The current prototype implements the
  chance and starting health/energy bonuses only; qualification is not yet part
  of the rule.
- Consider whether alpha status is inherited, earned by the child, or both.
- Consider whether alpha cells should influence nearby members of their species
  through pack behavior, reproduction priority, or territorial rules.

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

## Emergent-system ideas

- Separate perception from movement: visible targets, priority selection, then a
  one-step move or later grid pathfinding.
- Food reserves should create carrying capacity rather than only acting as a
  binary requirement.
- Crowding can impose energy costs, force separation, reduce reproduction, or
  create territory rather than simply blocking births.
- Plant food could eventually come from moisture, soil quality, or drought
  events instead of a single fixed reserve.

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
