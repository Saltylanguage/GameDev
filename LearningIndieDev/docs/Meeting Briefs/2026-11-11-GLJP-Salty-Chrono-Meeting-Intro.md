# GLJP working session - Salty and Chrono

**Prepared for:** November 11, 2026, 9:00 PM Eastern
**Status:** Discussion brief. No meeting decisions are recorded here until the group agrees.

## Meeting aim

Bring Salty and Chrono up to date on the November GLJP slice, review the player-facing flow, and leave with clear owners and next steps for verification and integration.

## Opening summary

The GLJP slice now connects permanent Gene Lab research to temporary journey choices, ranked progression, population controls, and Burrowing. Bevin approved the catalog diagram and accepted the current Gene Lab screen. On November 11, Bevin clarified that a Gene Lab Mutation must be researched **and equipped** before it can appear in the journey offer pool. That rule is implemented and published in commit `eb6aae5`; Unity recompiled without errors, and runtime verification remains open.

## Current state as of November 11

- **Catalog:** Approved prices and prerequisite paths are implemented in runtime definitions and authored assets. Research permanently unlocks a node; equipping that node enables its associated journey Mutation; selecting the Mutation applies it for that journey. Owned but unequipped nodes do not add their Mutations to the journey offer pool. Starter genes remain explicit launch grants.
- **Gene Lab screen:** Bevin reviewed the current locked/active capture and said it looks correct. Current captures: [`gene-lab-locked.png`](../../artifacts/visual-evidence-20261111-diagram-update/gene-lab-locked.png) and [`gene-lab-permanent-node-active.png`](../../artifacts/visual-evidence-20261111-diagram-update/gene-lab-permanent-node-active.png).
- **Journey and Burrowing:** Focused captures exist for linked Heightened Senses choices, shop progression, and the Burrow lifecycle. Review of the journey upgrade screen and brown-circle Burrow marker remains.
- **Live Gene Lab demo aid:** In the Unity Editor or a Development Build, load a profile and open Gene Lab. The left panel's Developer Wallet Tools accept a whole-number grant amount (default 1,000) and add it to either Hare Data or Research Data. Grants save to the loaded profile and are hidden in regular release builds.
- **Slice values:** Faster Gestation remains a provisional 15% reduction per rank, up to three ranks. Open-ground Sprint remains a provisional +0.5 movement for one rank over current exposed Bare/Grass terrain; an equipped Born Burrower excludes it. Burrowing costs 24 energy, holds three Hares by default, and lasts 200 steps.
- **Route finding:** The authored Forest Edge seed 10100 loses its last Hare at tick 1210. Bevin approved tracking this as a route finding rather than making unaided survival a GLJP closeout gate.
- **Branch and publication:** The equip-gated Mutation eligibility change is published on `origin/BevBranch` at `eb6aae521247b9b78a8ef08bc6cafd97059b6fdd`; the worktree is clean and local/remote heads match. Unity recompiled without errors; no tests were run for this change. The complete Unity run before this change completed in `artifacts/unity-tests-20261111-190817`: EditMode **770 passed, 0 failed, 1 skipped**; PlayMode **100 passed, 1 failed, 2 skipped**. The one PlayMode failure is the approved seed-10100 route finding; two screenshot-dependent PlayMode cases require the visual runner. Re-run appropriate verification on the final source before marking GLJP complete.
- **Ticket snapshot:** Trello evidence was posted and read back on GLJP-04/05/06. Statuses remain: GLJP-02/03 Done, GLJP-01/06 In Progress, GLJP-04/05 Current Work. Cards remain open pending their remaining acceptance evidence.
- **Effort estimate:** About **2–4 focused hours** remain for journey/ranked Gene Lab/Burrow visual review, exact-source replay evidence, and closeout. The route finding is not a gate by itself; if the group changes that acceptance decision, estimate additional investigation and reruns separately.

## Suggested 30-minute discussion

1. **Status and scope - 5 min**
   Confirm the GLJP tickets included in this slice and the published `BevBranch` revision.

2. **Player-facing walkthrough - 10 min**
   Review the accepted Gene Lab screen, then the journey Mutation choice flow and Burrow marker. Check that the permanent research unlock, temporary journey choice, ranked states, exclusions, and population-control wording are clear.

3. **Verification and open issues - 7 min**
   Review the current full-suite results, exact-source replay-parity evidence still needed, visual checks still needing player acceptance, and the documented seed-10100 finding.

4. **Integration and owners - 5 min**
   Agree whether and when to integrate the published revision into the Sprint 6 working branch and how follow-up fixes will be handled.

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
