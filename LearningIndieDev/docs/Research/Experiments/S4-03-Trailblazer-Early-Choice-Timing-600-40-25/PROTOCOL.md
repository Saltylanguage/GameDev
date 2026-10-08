# S4-03 Trailblazer early-choice timing diagnostic — 600/40/25 wrapped

**Status: Complete locally; human balance review pending.** Bevin approved the timing-only comparison on 2026-10-06. The one-seed pilot and 200-seed candidate arm passed validation on 2026-10-07.

## Question

Does moving Trailblazer's first Mutation choice before tick 0 reduce the opening Hare combat losses that happen before the current first choice at tick 100, without harming Fox persistence or all-species node clears?

## Hypothesis and scope

The existing 800-run screen showed identical phase-1 medians across strategies: Hare population fell from 40 to 21 with 45 deaths (39 combat, 6 starvation) before the first choice. An earlier first choice may reduce those opening combat deaths. It could also reduce Fox food access or fail to affect early losses; both are measured.

This is a simulation-only timing counterfactual. It does not change player-facing UX, skill values, species rules, population defaults, or the approved six-phase / five-choice product flow.

## Comparison

- **Reference:** existing Trailblazer arm from the approved strategy screen, with five policy choices at ticks 100, 200, 300, 400, and 500.
- **Candidate:** five choices at ticks 0, 100, 200, 300, and 400; automatically continue at tick 500 without a sixth choice.
- Both use the same 200 common seeds, 14200–14399, and identical Trailblazer policy, production three-card offer generation, lower-level-first rule, tie-break, skip rule, and five total choices. Since the offer generator depends on seed, offer rotation, previous choice, and legal levels rather than population, the candidate should receive the same offers and choose the same five skills as its reference run; the audit will verify this.

Both arms use production Forest Edge, 36x20 with wrapping, 600 Plants / 40 Hares / 25 Foxes, Hare player, 0.1-second steps, opposed-roll natural combat, six 100-tick phases, `bev-experimental`, coupled responses off, no reinforcements, and no added Genomes. The clear rule is tick 600 with Plants > 0, Hares >= 5, Foxes >= 5; runs ending early fail.

## Measures and interpretation

Primary: matched all-species clear difference and uncertainty versus the existing Trailblazer arm. Also report phase-1 Hare combat and starvation deaths, phase populations, Fox deaths/births/food/reproduction, completion to tick 600, and Hare slashlines with validity/denominators. Report exact matched outcomes and Wilson interval for the candidate rate. This single comparison is diagnostic; do not infer balance acceptance or make gameplay changes from it.

## Approval and evidence contract

Bevin approved this timing comparison on 2026-10-06. Freeze exact inputs, seed range, scenario hash, and relevant source hashes before the main run. Run a one-seed pilot first to verify five decisions at the stated ticks, identical offered/selected skill sequences, acquisition timing, automatic continuation after tick 500, and successful completion. If pilot and accounting checks pass, run the 200-seed candidate arm and compare to the existing matched reference. Keep raw histories and logs under ignored `artifacts/`; publish factual results, separate interpretation, and a handoff. Preserve all existing dirty worktree changes.

## Results

The candidate arm ran successfully for all 200 seeds and matched the frozen reference inputs. The pilot and full analyzer confirmed the candidate's five choices at 0/100/200/300/400, the same offers and selected skill sequence as the reference for every seed where both arms reached the decision, and correct acquisition timing. See the [factual report](REPORT.md), [interpretation](ANALYSIS.md), and [handoff](../../../handoffs/2026-10-07-0024-codex-s4-03-trailblazer-early-choice-timing.md).
