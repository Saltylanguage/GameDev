# S4-03 Trailblazer spatial diagnostic — factual report

**Status:** Needs Josh/Sim review, 2026-10-11. **Scope:** five-arm exploratory replay on the existing 20 matched seeds. See the [predeclared plan](../S4-03-trailblazer-spatial-plan-20261011.md), [metadata](metadata.json), [compact metrics](summary.json), and [AI analysis](ai-analysis-v1.md). Raw Unity reports and test XML remain in ignored `LearningIndieDev/artifacts/s4-trailblazer-spatial-20261011/` in the attached worktree.

The observer records the Hare grid after each tick and never writes to simulation state or draws random values. `hareObservations` counts living Hare positions after ticks 1 onward. Distinct map cells include starting positions. Fox proximity means a Fox in any of the eight neighboring cells; it does not mean an attack opportunity. Crowding exposure uses the **current implemented rule**: the reproduction neighborhood, group size, and tolerance. The production Hare has a group size of seven and zero base tolerance, so an unmodified Hare becomes crowded at **seven Hare neighbors**. Josh's approved provisional density surcharge starts at **four neighbors** and remains unimplemented; these observed rates do not test that rule. Rates below divide by Hare observations, which differ substantially between arms because population and run length differ. Post-choice movement starts at phase 3, after both scheduled skills would be acquired.

## Observed outcomes

| Arm | Alive at 600 | Post-choice movement steps per Hare observation | Mean distinct cells per run / 720 | Paired coverage gains / losses / ties vs control | Fox-neighbor observations / 1,000 Hare observations | Crowded observations / 1,000 Hare observations |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Control | 6/20 | 1.382 | 524.4 | — | 59.1 | 1.26 |
| Movement alone | 7/20 | 1.852 | 541.3 | 11 / 9 / 0 | 51.1 | 2.99 |
| Threat Avoidance alone | 4/20 | 1.346 | 504.0 | 8 / 6 / 6 | 62.2 | 0.49 |
| Trailblazer: Movement first | 6/20 | 1.974 | 538.4 | 9 / 11 / 0 | 40.2 | 1.18 |
| Trailblazer: Avoidance first | 7/20 | 1.800 | 537.2 | 14 / 4 / 2 | 52.7 | 0.52 |

Across complete runs, Hare distance per observation was 1.055 cells in control, 1.186 with Movement first, and 1.140 with Avoidance first. Mean distinct cells per individual Hare were 33.1, 36.3, and 34.4 respectively. The two pair orders had 95,241 and 76,403 Hare observations versus 69,890 in control; direct totals are therefore not comparable as encounter rates. Full per-arm values are in [summary.json](summary.json). Distinct cell coverage is a union across the population and saturates toward the 720-cell grid limit.

## Player decision flow

The actual first-phase Hare mutation offer shows three rotating choices. Enumerating the offer algorithm for seeds 11100–11119 with every skill eligible yields Movement or Threat Avoidance in 12/20 first offers. All 20 control runs reached the first boundary. Once a player chooses either Trailblazer skill, the offer code inserts its partner into the next offer when that partner remains eligible. A focused PlayMode test selected Movement and then Threat Avoidance through the real phase decision UI model for seed 11100, verified both descriptions, and found both upgrades in the acquisition timeline. This tests the flow and text through code; it is not a human visual usability review.

## Validation and limits

- All 100 new run records were exactly equal to their archived S4-03 counterparts after removing only the new `spatial` field. The Hare-only observer guard added before commit was checked with a post-commit one-seed replay that exactly matched the earlier spatial smoke record.
- The focused Unity Editor test passed (1/1): known-grid distinct cells, Manhattan travel, crowding threshold, Fox proximity, and phase counts. The focused Unity PlayMode offer test passed (1/1).
- This replay has the original screen's 20-seed exploratory limit. It uses scheduled upgrades for the behavior comparison and cannot measure whether players notice or understand the build. The 400/25/15 stress fixture and frozen 200-seed confirmation are still separate work.
- Crowding under the current seven-neighbor threshold remains rare in this fixture. The approved four-neighbor density surcharge needs its own implementation and replay before Trailblazer crowding pressure can be judged. Fox-neighbor proximity is observational and does not itself measure Threat Avoidance success or prevented attacks.
