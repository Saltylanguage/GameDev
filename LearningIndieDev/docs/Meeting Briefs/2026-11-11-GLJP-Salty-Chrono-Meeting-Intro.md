# GLJP working session - Salty and Chrono

**Prepared for:** November 11, 2026, 9:00 PM Eastern
**Status:** Discussion brief. No meeting decisions are recorded here until the group agrees.

## Meeting aim

Bring Salty and Chrono up to date on the November GLJP slice, review the player-facing flow, and leave with clear owners and next steps for verification and integration.

## Opening summary

The GLJP slice now connects permanent Gene Lab research to temporary journey choices, ranked progression, population controls, and Burrowing. Bevin approved the catalog diagram and accepted the current Gene Lab screen. The remaining work is final validation, review of the journey upgrade screens and Burrow marker, ticket evidence, and integration of the local `BevBranch` work into the Sprint 6 branch.

## Current state as of November 11

- **Catalog:** Approved prices and prerequisite paths are implemented in runtime definitions and authored assets. Research ownership gates new purchases and journey Mutation eligibility; equipping remains a separate choice. Gene Lab research does not directly apply a journey Mutation.
- **Gene Lab screen:** Bevin reviewed the current locked/active capture and said it looks correct. Current captures: [`gene-lab-locked.png`](../../artifacts/visual-evidence-20261111-diagram-update/gene-lab-locked.png) and [`gene-lab-permanent-node-active.png`](../../artifacts/visual-evidence-20261111-diagram-update/gene-lab-permanent-node-active.png).
- **Journey and Burrowing:** Focused captures exist for linked Heightened Senses choices, shop progression, and the Burrow lifecycle. Review of the journey upgrade screen and brown-circle Burrow marker remains.
- **Slice values:** Faster Gestation remains a provisional 15% reduction per rank, up to three ranks. Open-ground Sprint remains a provisional +0.5 movement for one rank over current exposed Bare/Grass terrain; an equipped Born Burrower excludes it. Burrowing costs 24 energy, holds three Hares by default, and lasts 200 steps.
- **Route finding:** The authored Forest Edge seed 10100 loses its last Hare at tick 1210. Bevin approved tracking this as a route finding rather than making unaided survival a GLJP closeout gate.
- **Branch and publication:** The GLJP changes are being published from local `BevBranch`, based on `a2c38b3`. The complete live Unity run on the final source completed in `artifacts/unity-tests-20261111-190817`: EditMode **770 passed, 0 failed, 1 skipped**; PlayMode **100 passed, 1 failed, 2 skipped**. The one PlayMode failure is `FieldDataTestRouteCompletesThroughAllShopsOnFixedSeed`: authored Forest Edge seed 10100 loses its last Hare at tick 1210. This remains the accepted route finding, not a GLJP acceptance gate. Two screenshot-dependent PlayMode cases skipped because this was not a visual-runner capture session; focused Gene Lab and GLJP PlayMode checks passed. Commit/push and Trello read-back are part of this work session.
- **Ticket snapshot:** Last recorded Trello status on November 11: GLJP-02/03 Done, GLJP-01/06 In Progress, GLJP-04/05 Current Work. Recheck statuses and add/read back evidence after publication.

## Suggested 30-minute discussion

1. **Status and scope - 5 min**
   Confirm the GLJP tickets included in this slice and the current state of the local `BevBranch` implementation.

2. **Player-facing walkthrough - 10 min**
   Review the accepted Gene Lab screen, then the journey Mutation choice flow and Burrow marker. Check that the permanent research unlock, temporary journey choice, ranked states, exclusions, and population-control wording are clear.

3. **Verification and open issues - 7 min**
   Review the current full-suite results, required replay-parity evidence, visual checks still needing acceptance, and the documented seed-10100 outcome.

4. **Integration and owners - 5 min**
   Agree who will integrate `BevBranch` into the Sprint 6 working branch and how conflicts or follow-up fixes will be handled.

5. **Commitments - 3 min**
   Record each remaining task, one owner, and a target date. Keep GLJP tickets open until their evidence and acceptance criteria are recorded.

## Questions to resolve together

- Does the journey upgrade screen communicate the choices and their temporary effects clearly?
- Is the Burrow marker readable at normal gameplay scale?
- Who owns the final full-suite rerun and exact-source replay-parity artifact after this commit?
- Who owns integration into the Sprint 6 branch, and what handoff details would make that safe?
- Which GLJP cards can close after the remaining visual review and validation evidence are attached?

## Proposed action log

| Action | Owner | Due |
|---|---|---|
| Review journey upgrade and ranked Gene Lab states | TBD | TBD |
| Review Burrow marker presentation | TBD | TBD |
| Attach final validation and replay-parity evidence to the relevant cards | TBD | TBD |
| Integrate the published GLJP change into the Sprint 6 working branch | TBD | TBD |

## References

- [GLJP finish plan](../GLJP_FINISH_PLAN.md)
- [GLJP process design](../GENE_LAB_JOURNEY_PROGRESSION_PROCESS_DESIGN.md)
- [GLJP system design](../GENE_LAB_JOURNEY_PROGRESSION_SDD.md)
- [November 9 Salty handoff](../handoffs/2026-11-09-gljp-salty-handoff.md)
- [November 11 GLJP-01 handoff](../handoffs/2026-11-11-1400-codex-gljp01-open-ground-sprint.md)
