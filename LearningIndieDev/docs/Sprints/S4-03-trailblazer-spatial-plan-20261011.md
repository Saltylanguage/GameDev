# S4-03 Trailblazer spatial diagnostic plan

Status: exploratory diagnostic, declared before replay on 2026-10-11.

## Question

Does the current Trailblazer pair cover more of Forest Edge and spend less time near other Hares and Foxes than the no-skill control? Compare both acquisition orders and the two single-skill arms. This is a behavior review, not a balance acceptance test.

## Fixed replay

Use the existing S4-03 Forest Edge setup: 36×20, 400 Plants / 20 Hares / 10 Foxes, six 100-tick phases, 0.1-second step, opposed-roll combat, natural opportunities, `bev-experimental`, and seeds 11100–11119. Replay control, Movement, Threat Avoidance, Trailblazer Movement first, and Trailblazer Avoidance first. Keep the original skill levels and cumulative phase schedules. Use new output filenames so the archived screen remains intact.

## Observations

- **Map coverage:** distinct cells occupied by any Hare, including initial cells; also distinct cells within each phase. Compare per seed with its matched control, noting that shorter runs have less opportunity to explore.
- **Individual travel:** total Manhattan cell distance between successive observations of the same Hare, and mean distinct cells per Hare observed. Divide distance by Hare observations to compare exposure; review phase activity movement steps as a cross-check.
- **Crowding:** Hare-tick observations above the *active* crowding threshold, using the game's reproduction neighborhood, group size, and tolerance. Report as a share of Hare observations. This measures exposure, not energy lost.
- **Fox proximity:** Hare-tick observations with at least one Fox in the eight neighboring cells. Report as a share of Hare observations. Compare with existing Fox combat attempts and hits; proximity is not an attack opportunity.
- Keep early extinctions and missed choice boundaries visible. Segment every observation by the phase active during that tick.

The observer only reads the resulting grid. It does not feed the simulation or consume random numbers. No skill or scenario balance values change. Use the same seeds for paired descriptive comparisons; reserve fresh seeds and acceptance thresholds for the later confirmation panel. The in-game player offer review is separate because scheduled runs bypass that UI.
