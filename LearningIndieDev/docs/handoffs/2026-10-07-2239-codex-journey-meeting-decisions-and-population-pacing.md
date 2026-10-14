# Journey meeting decisions and population pacing

[Working state](../WORKING_STATE.md) | Status: shared

- Handoff schema: 1
- Handoff ID: 2026-10-07-2239-codex-journey-meeting-decisions-and-population-pacing
- Owner: Codex
- Branch: BevBranch
- Baseline commit: ada3ef74
- Date: 2026-10-07
- Supersedes: none

## Summary

Bevin supplied the team's meeting outcomes after the October 7 integration checkpoint. Recorded them in the meeting brief and aligned the current product/journey context. This is a documentation and Trello update, not a new balance experiment or runtime change.

## Changes

- Added the six outcomes and remaining viability questions to the [meeting brief](../Meeting%20Briefs/2026-10-07-Journey-and-Ecology-Balance-Agenda.md#meeting-decisions), preserving the original agenda and historical results.
- Updated Working State, Project Context, and the journey contract so agents see approved inter-simulation upgrade carryover, the historical status of 600/40/25, and the deferred DNA work.
- Additive Trello comments communicate these decisions without replacing descriptions or marking balance work complete.

## Decisions and assumptions

- Success is intended to mean reaching tick 600 with an ecology viable through tick 700. The exact viability rule is an open design task.
- Upgrades carry into the next simulation run in the journey. This interprets "next simulation run" in the meeting's journey-node context; carryover into a newly started journey or permanent progression was not specified.
- 600 Plants / 40 Hares / 25 Foxes remains a historical comparison fixture. Seek a new starting population that makes kills, births, and other events occur at a more personal pace; no new counts are selected.
- SimMasterBev owns the next balance and population-search work. Salty's journey planning remains its existing workstream.
- DNA reward economy and the secondary long-running simulator are deferred for now.

## Validation

- Reviewed the live branch and the current meeting brief, Working State, Project Context, and journey contract before editing.
- Documentation diff/whitespace and relative-link checks are the relevant verification for this change. No code, scenes, authored assets, gameplay thresholds, or production population values were changed; no Unity tests or simulations were run for the meeting update.

## Risks and incomplete work

- Define viability: species floors or recovery requirements, actual continuation versus estimate, and the intervention/condition contract for ticks 600-700. The historic Plants > 0 / Hares >= 5 / Foxes >= 5 endpoint rule does not settle this question.
- The earlier 40-50% preference has not been revalidated or assigned a new scope under the revised success definition.
- A "personal" event pace still needs player observation and a measurable comparison; population count alone does not establish it. New fixture discovery is not yet completed.
- Approved continuity already matches the current prototype across nodes, but fresh-journey reset/permanent progression and upgrade vocabulary remain separate decisions.

## Next useful step

SimMasterBev should define a small review protocol for event pace and compare candidate starting populations, retaining kills, births, other events, population trends, and readable slashline evidence. Define tick-700 viability before claiming any candidate meets the new success goal. Preserve the historical 600/40/25 reports and use a distinct contract for the new search.
