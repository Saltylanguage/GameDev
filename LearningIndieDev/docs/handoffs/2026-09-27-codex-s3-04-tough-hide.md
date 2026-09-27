# S3-04 — Tough Hide implementation and first review

**Date:** 2026-09-27
**Status:** Implementation and bounded comparison complete; player-facing copy and generic card markers accepted for this slice.
**Branch:** `BevBranch`, source commit `7e6ed38c029bb263e0e96d7c556d34fbc8f6b5f8` with uncommitted work.

## Implemented player flow

- At each Hare phase decision, the existing five experimental Mutations feed
  three distinct options; the fourth action is Skip. A previously selected
  Mutation returns in later offers and applies its next level. Selection costs
  no Field Data; Skip changes nothing.
- Offers use concise qualitative copy: Tough Hide — “Block more incoming
  attacks”; Efficient Digestion — “Gain more energy from food”; Crowding
  Tolerance — “Lose less energy when crowded”; Reproductive Drive — “Make
  successful reproduction more likely”; Threat Exposure — “Evade predator
  attacks more often.” The cards omit modifier rows, currency costs, and
  availability diagnostics.
- The following summary reports the ending Hare population. When Tough Hide is
  in the completed phase loadout, it also reports the number of opposed-roll
  misses whose target was the Hare. The metric is computed from target species
  and roll result; `CombatBlocked` activity is attributed to the attacker and
  would answer the wrong question here.
- Stat-Line and upgrade-count diagnostics are hidden in normal player mode and
  refresh when Developer Mode changes.
- Existing generic A/B/C card markers remain. Candidate-specific icon
  recognition was not separately accepted in this implementation review.

## Unity validation

All checks used the locally installed Unity 6000.4.6f1 Editor in Clean mode.

- `PhaseMutationOffersShowThreeDistinctHerbivoreMutations`: EditMode 1/1,
  `artifacts/unity-tests-20260926-235645/EditMode-results.xml`.
- `FiveHerbivoreSkillsRetainLevelsAndAcquisitionsAcrossDecisionBoundaries`:
  PlayMode 1/1, `artifacts/unity-tests-20260927-001300/PlayMode-results.xml`.
- `ExperimentalDiagnosticsStayHiddenUntilDeveloperModeEnabled`: PlayMode
  1/1, `artifacts/unity-tests-20260927-000553/PlayMode-results.xml`.
- `ContinuousPhaseDecisionResumesTheSamePreviewRun`: PlayMode 1/1,
  `artifacts/unity-tests-20260927-000634/PlayMode-results.xml`.

The phase-flow test covers three distinct choices, free application, same-run
continuation, repeated level/stack application, phase ordering, the factual
Tough Hide summary, and no double-charge on repeated clicks. Its test grid now
uses the authored 36×20 Forest Edge dimensions so the configured starting
population fits.

## Bounded Forest Edge/Hare comparison

The initial five-seed phase-2 sample had only one seed with incoming Fox
attacks, so it was expanded under the plan's variance rule. The final matched
panel used seeds 10100–10119, the production Forest Edge asset, 36×20 grid,
opposed-roll combat, natural attack opportunities, and six 50-tick phases at
0.2 seconds per tick. The baseline used no Mutation. The treatment selected
Tough Hide after round 1 and retained it through rounds 2–6; its recorded
acquisition is tick 50, phase index 1, order 0, targeting Hare, with the
`combat.block` modifier +2.

| Round 2 measure | No Mutation | Tough Hide |
| --- | ---: | ---: |
| Incoming Fox-to-Hare opposed rolls | 33 | 39 |
| Rolls the Hare blocked | 14 (42.4%) | 20 (51.3%) |
| Hare predation deaths | 19 | 19 |
| Mean Hare population at round end | 49.2 | 49.5 |

Across the full six-round panel, mean final Hare population was 220.2 in the
baseline and 223.5 with Tough Hide; no runs ended with Hare extinction. These
whole-run differences are descriptive and are not attributed to Tough Hide.
The narrow next-round roll measure supports the directional copy, while the
predation-death and population results do not establish a broader survival or
balance benefit.

Both report validators returned `VALIDATED_WITH_LIMITATIONS`: FPO and the
independently supported counters reconciled, while HPS, EHS, and ECN lack raw
event lists in this report schema for independent recalculation. The report
schedule records the legacy catalog's cost of 5; the research runner did not
spend currency. Zero-cost selection is verified separately in the preview
PlayMode test. Both experiment manifests record the source tree as dirty, and
retain source commit, scenario GUID, Unity executable, seed range, and metric
dictionary hashes.

- Baseline: `artifacts/cellular-experiment-20260927-001351/report.json` and
  `report-summary.md`.
- Tough Hide and paired analysis: `artifacts/cellular-experiment-20260927-001508/report.json`,
  `analysis.md`, and `report-summary.md`.
- Initial five-seed pilot: `artifacts/cellular-experiment-20260927-000821/`
  and `artifacts/cellular-experiment-20260927-000903/`.

## Review decision

The player-facing copy “Block more incoming attacks” and the existing generic
A/B/C card markers are accepted for this slice. The five-seed extension
supports that direct defensive direction, but it is one small
scenario-specific review, not production balance or player-fun evidence.
Mutation-specific artwork remains outside this slice.
