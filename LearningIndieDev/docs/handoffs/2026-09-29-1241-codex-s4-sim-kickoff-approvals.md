# S4 planning handoff — Sim kickoff decisions

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-09-29-1241-codex-s4-sim-kickoff-approvals
- Owner: codex
- Branch: codex/forest-edge-visual-pass
- Baseline commit: 25642440
- Date: 2026-09-29
- Supersedes: none

## Summary

S4 is still Proposed. Seven draft cards are staged in Trello's `🎯 Upcoming
Work` list for planning; none is started or complete. Card owners and estimates
are not assigned or confirmed yet. Sim, please review the proposed work and
reply with approval or changes to the decisions below so the team can finalize
the S4 plan and decide whether it is ready to kick off.

This proposed lane follows your recent Forest Edge reporting and validation,
readable Hare Mutation choices, phase-boundary Reinforcements, reproduction
telemetry, and Forest Edge starting/balance work. It asks you to lead the
bounded configuration and evidence work while Josh owns the player-facing
choices and local profile flow.

## Proposed work and capacity

| Task | Proposed contribution from Sim | Draft Trello card |
| --- | --- | --- |
| S4-01 — Define build identities and counterplay | Co-review strategy matrix, 2h | [Open card](https://trello.com/c/74VBPME5/112-s4-01-define-forest-edge-build-identities-and-counterplay) |
| S4-02 — Implement bounded configurations | Lead implementation, 8h | [Open card](https://trello.com/c/j22jDrAu/113-s4-02-implement-three-bounded-forest-edge-build-configurations) |
| S4-03 — Matched-seed comparison and in-game review | Lead evidence, 4h | [Open card](https://trello.com/c/xyDDiiRC/114-s4-03-run-matched-seed-comparisons-and-complete-in-game-review) |
| S4-04 — Local profile contract | Review simulation inputs/freeze boundary, 1h | [Open card](https://trello.com/c/JiVmlHhh/115-s4-04-define-the-local-profile-save-restore-contract) |
| S4-05 — Save and restore local profile fields | Review, 2h | [Open card](https://trello.com/c/78M83hds/116-s4-05-save-and-restore-approved-local-profile-fields) |
| S4-07 — Integration and review reserve | Joint reserve, up to 3h | [Open card](https://trello.com/c/e5xd0K7c/117-s4-07-integration-defects-and-review-reserve) |

The draft allocates Sim 17h for feature work and 3h from the protected reserve.
Josh's draft allocation is 15h feature work and 5h reserve. Together this is
32h of feature work plus 8h of reserve (40h total). These are planning numbers,
not approved commitments.

P1-032, the Desktop launch-context migration, is planned before S4-01 but is
not included in that 40h: its estimate and capacity trade are unresolved.
Josh will resolve the estimate and trade before kickoff. Please flag any
dependency that would prevent the Forest Edge work from starting until it is
complete. [P1-032 draft card](https://trello.com/c/Enm8Avxu/111-p1-032-desktop-launch-context-migration)

## Decisions requested from Sim

Please reply to each item with **approve**, **change** (and the correction), or
**defer**:

1. **Role, availability, and estimates:** Do you accept the proposed split:
   co-review S4-01, lead S4-02/03, review the simulation boundary in S4-04/05,
   and share the S4-07 reserve? Can you take 17h of feature work plus up to 3h
   of reserve? Are the 2h / 8h / 4h allocations for S4-01/02/03 realistic,
   are 1h and 2h enough for profile review, and is 3h a reasonable reserve
   allocation?
2. **Build feasibility and identity:** Can Trailblazer, Warren, and Gardeners
   be made meaningfully distinct using supported species and Mutation rules?
   Should those names represent species, builds, or loadouts? What is the
   smallest set of supported input differences that makes their choices
   understandable? The draft excludes new mechanics and a new species roster.
3. **Evidence gate:** Is the matched-seed approach sufficient if scenario,
   starting state, seed set, and comparison windows are held constant, with
   configuration fingerprints, outcomes, observed decisions, and limitations
   retained? Recommend the seed count and any essential report fields or
   measurements. The draft limits conclusions to the seeds tested.
4. **Balance scope:** The roadmap describes a wider capability map, reference
   panel, Adaptation Value, and reachable nine-Mutation-path analysis. The
   current 40h draft focuses on three bounded builds and matched evidence.
   Can that wider work stay out of S4, or is a specific small piece required
   for this outcome? If required, name it and its estimate so capacity can be
   traded explicitly.
5. **Profile boundary:** Is it safe for S4 to save only local player choices,
   with selected inputs frozen at launch and no effect on an already-started
   run or deterministic simulation? The draft leaves active-expedition resume,
   cloud sync, settlement, progression migration, and production Genome
   persistence out. Josh leads the contract and implementation; your proposed
   role is a focused simulation-boundary review.

S4-06, a 4h Genome contract spike, is optional and has no draft card. It stays
out of the baseline unless the team explicitly trades other work for it.

## Kickoff path

After your review, Josh will settle the P1-032 estimate and feature-cap trade,
confirm the local profile field set and restart behavior, and update the cards
with the agreed scope, ownership, and estimates. S4 remains Proposed until
those capacity and acceptance decisions are recorded and a separate kickoff
is confirmed.

## Linked records

- [S4 control record](../Sprints/S4-control-record.md)
- [Working state](../WORKING_STATE.md)
