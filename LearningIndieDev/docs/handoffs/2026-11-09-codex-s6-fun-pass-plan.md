# Sprint 6 Fun Pass plan publication

[Working state](../WORKING_STATE.md) | Status: planned

- Handoff schema: 1
- Handoff ID: 2026-11-09-codex-s6-fun-pass-plan
- Owner: codex
- Branch: codex/s6-fun-pass-plan
- Baseline commit: 4f0ace88
- Date: 2026-11-09
- Supersedes: none

## Summary

Josh approved [S6 — The Fun Pass](../Sprints/S6-control-record.md) for November 16–29 as the next sprint direction. Its outcome is one enjoyable, understandable, replayable Hare journey, supported by three distinct builds and a lightweight balance-comparison loop. The roadmap now forecasts this work instead of the older S6 persistence/Genome bucket. S5 remains active through November 15 and needs a separate closeout and kickoff decision.

## Decisions and evidence to preserve

- Bevin made the November implementation canonical for GLJP-06. October runs are historical, and the current runner rejects the October snapshot's source hash; do not treat them as paired current evidence.
- [GLJP-06](https://trello.com/c/eo0iqoRL/154-gljp-06-diagnose-route-seed-survival-mismatch-before-further-development) reports plant exhaustion before the authored seed-10100 route ends at tick 1210. The stable-Hare fixture reaches successful results at tick 2400 but does not validate authored ecology.
- November GLJP-01/04/05 checks and their remaining player/content decisions are recorded on their Trello cards. They are not closed by this planning publication.
- `ProjectMain` baseline `4f0ace88` predates the local November GLJP work. Reconcile Sim's source, handoffs, and artifacts before S6 implementation or comparisons. Do not assume a new branch from ProjectMain contains them.

## Validation and limits

The roadmap HTML generator now resolves code-formatted link labels, and the regenerated page has no unresolved inline tokens. Its `-Check` mode and `git diff --check` passed. The handoff validator reported 71 errors in older notes and sparse-checkout artifact references, with none in this new note; it is not a repository-wide pass. No Unity test, ecology run, gameplay change, S5 closeout, or player acceptance is claimed by this documentation update.

## Next useful step

At the S5-to-S6 boundary, verify source and evidence, close S5 separately, assign owners and estimates, and confirm the smallest playable route. Then begin with the Hare journey design and playtesting discussion before implementation.
