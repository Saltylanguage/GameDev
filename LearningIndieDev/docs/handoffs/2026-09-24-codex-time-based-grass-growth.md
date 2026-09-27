# Time-Based Grass Growth

**Date:** 2026-09-24  
**Status:** Implemented locally; unverified and provisional

## Decision

Grass spread is now timer-based. Each grass resource tile accumulates elapsed
simulation time and becomes ready to spread into one adjacent open tile after
17.4 seconds. The timer pauses while an animal occupies the tile and resets
after a successful spread. If no adjacent tile is available, the plant remains
ready and retries on later ticks. Plants present at scenario start receive
seeded staggered timer phases so they do not all spread on the same tick.

The interval was chosen to preserve the previous expected wait: Forest Edge
used a 1.15% spread chance per 0.2-second tick, or about 87 eligible ticks
(17.4 simulation seconds) per successful spread for one tile. The current
Forest Edge scenario sets its step interval to 0.2 seconds; the interval is
converted to ticks from that scenario value so other step sizes use the same
simulation-time duration.

This replaces chance-based **spreading**. Passive grass food-reserve regrowth
remains unchanged.

## Implementation

- Grass terrain carries a `GrowthIntervalSeconds` value of 17.4.
- Each terrain resource cell stores elapsed growth ticks in its simulation
  state.
- Plant reproduction uses the interval when the terrain defines one; species
  reproduction chances remain available for other species and for resource
  terrains without timed growth.
- The plant asset's old 1.15% chance is set to zero so it no longer suggests
  that this plant's spread is chance-based.
- The cellular simulation data fingerprint advances to v9 and includes the
  terrain growth interval.

## Verification and follow-up

No tests or matched seeded balance runs were run for this change. Run a focused
growth behavior check and a matched Forest Edge comparison before treating
17.4 seconds as balanced. In particular, inspect plant births, Hare starvation,
and whether later generations still spread in synchronized waves.

This implementation interprets “time-based growth” as spreading into new
tiles. If the intended behavior is for grazed tiles to refill their food
reserve over time, that is a separate mechanic.
