# ProjectMain consolidation - GLJP, M2 and mandatory wrapping

[Working state](../WORKING_STATE.md) | Status: published to ProjectMain; existing ecology and M2 findings remain open

- Handoff schema: 1
- Handoff ID: 2026-11-16-1712-codex-projectmain-gljp-m2-wrapping-unification
- Owner: Codex for Bevin
- Date: 2026-11-16
- Integration candidate: codex/projectmain-unify-20261116
- Intended shared working branch: ProjectMain
- Original ProjectMain: dac3bfa5a8b43acadf4cb7e94c5c906c15df0d14
- GLJP closeout: b38de80 (BevBranch, already contained in original ProjectMain)
- Mandatory wrapping: 1ef3f178adf8fc372315f64007e7d7e06f0599ef (previously local-only)
- M2 implementation, review and startup fix: 3f9f726e16e5bfa366d8a79aff3111305d44f206
- Candidate merge: 472375b6acb65b1ea5cfdb5873b008bae078371d; catalog alignment 85d585a; isolated offer fixtures 21b3c71.

## Why the visual review lacked GLJP and wrapping

The M2 review branch forked before the final GLJP closeout. It did not contain cf5b771 through b38de80, including approved Gene Lab prices/prerequisites, rank persistence, equipment gating and later UI support. The final GLJP closeout was already on ProjectMain, but the later M2 branch did not merge that integration. Wrapping was committed separately as 1ef3f17 on a local worktree branch and had never been published. Work remained in Git; the reviewed checkout omitted those lines.

The isolated candidate combines these lines. An ancestry audit found every current local and remote branch tip is contained in the candidate (excluding the candidate's own future documentation commits). Historical branches/worktrees are retained as recovery references; ProjectMain is the intended single current shared work branch. The unrelated default main branch is unchanged.

## Conflict resolutions

Seven files had textual conflicts:

- EscapeInstinct.asset and ReproductiveDrive.asset: preserve approved GLJP prices/prerequisites and GUIDs; descriptions explain research/equipment outside the introductory Forest Edge exception, without resurrecting obsolete test prices.
- SpeciesProgression.cs: retain GLJP research/equipment policies and rank limits; combine them with the M2 introductoryHareMutations flag. Only Escape Instinct, Tough Hide and Reproductive Drive bypass research/equipment on the declared introductory Hare route. Other upgrades retain their gates; ordinary/Field Data routes do not receive this exception.
- FieldDataPermanentProgressionDefinition.cs: preserve the approved complete catalog, Sprint/Gestation nodes, ranks and prerequisite paths; retain the M2 Quick Escape mapping to Escape Instinct research/equipment. No price or ecology tuning.
- SpeciesSimulationPreview.cs and FieldDataJourneySession.cs: carry combined access policies through offer generation and reward application, retain topology/wrapping, GLJP captured rank/equipment policies and the legacy saved-health fix.
- WORKING_STATE.md: retain dated history from both sides and add the current integrated entry point.

An authored/runtime Tough Hide description mismatch was caught by the full suite and aligned. Two Seed Dispersal fixtures depended on deterministic selection from a larger eligible pool. They now use the public reset/arrange APIs to equip only Seed Dispersal while retaining researched prerequisites; this isolates the offer under test without weakening eligibility assertions. A new two-case test proves the introductory exception does not expose other unequipped research and does not apply to ordinary progression.

## Validation and baseline separation

- Both standalone Release builds passed with zero errors/warnings: CellSim.Batch and CellSim.Workbench.
- Batch self-test passed, including topology cells/seams/gaps/provenance JSON round-trip, hidden/burrow digests and complete eighteen-round replay invariants.
- Original M2 full live EditMode baseline: 780 passed, 0 failed, 1 skipped (`F:/ForkBin/GameDev/LearningIndieDev/artifacts/unity-tests-20261116-164825`). Its PlayMode run was polluted by background CLI/MCP commands requesting unsupported `editor status` from the older Pipeline package; those log failures are not used as game regression evidence.
- Initial candidate live EditMode: 808 passed, 1 failed, 1 skipped (`artifacts/unity-tests-20261116-165154`); the failure identified the Tough Hide description mismatch, subsequently corrected. The accompanying live PlayMode run was cancelled to correct the catalog and switch to clean execution.
- Clean combined EditMode after catalog correction: **809 passed, 0 failed, 1 skipped** (`artifacts/unity-tests-20261116-170635/EditMode-results.xml`). Covers GLJP authored/runtime catalog parity, approved prices/ranks/prerequisites, profile schema 10, equipment/rank policies, introductory exceptions and topology tests. The skip requires a newly retained exact-source Field Data parity manifest; portable self-tests do not replace that test.
- First clean PlayMode: 99 passed, 3 failed, 7 skipped. Two failures were the isolated-offer fixtures described above; one was the known authored ecology failure.
- Final clean combined PlayMode after fixture correction: **101 passed, 1 failed, 7 skipped** (`artifacts/unity-tests-20261116-171409/PlayMode-results.xml`). The one failure is `FieldDataAuthoredRouteReachesFirstShopAndResetsNode`, seed 10101 at tick 288. Its complete failure message and recent population history exactly match the retained pre-integration wrapping-source result at `C:/Users/bevin/.codex/worktrees/bev-ticket-1-map-wrapping/LearningIndieDev/artifacts/unity-tests-20261114-121541/PlayMode-results.xml`.
- Dedicated clean graphics-capable Gene Lab rank-five check: **1 passed, 0 failed, 0 skipped**, at 1920x1080 (`artifacts/visual-evidence-20261116-171723`). Both captures were inspected: [Gene Lab rank-five ownership/equipment and current catalog](../Reviews/ProjectMain-Integration-20261116/gljp-population-reserves-rank-5.png), and [rank-five Journey Hare allowance](../Reviews/ProjectMain-Integration-20261116/gljp-rank-5-journey-allowance.png). This verifies the integrated source renders the GLJP functionality; it is not new human visual acceptance.

The clean results replace the noisy live PlayMode run as the merge check. The existing authored-route failure is preserved as an unresolved ecology issue; no tests are disabled, no failed criteria are called passed, and no ecology values are changed to manufacture a green suite. The two incoming Unity-generated icon meta files retain pre-existing trailing whitespace; resolution changes pass targeted whitespace checks and GUIDs are preserved.

## Retained scope and open work

- Full GLJP closure source and later M2 implementation/review remain in the combined history. Profile schema 10, permanent research/equipment/ranks, newer art/feedback/results and startup fix coexist.
- All scenario maps wrap; generic bounded cave/test grids remain supported. Sparse topology and portable serialization are included. Named shape selection and procedural irregular authoring controls remain the separate map tickets, not a completed feature claim.
- Wrapping changes spatial ecology relative to a bounded map. The seed-10101 tick-288 outcome is already present in the wrapping source; this is not new tuning or a full Journey success claim.
- M2's silent audio, poor event/strategy readability, progression blocked by early failure, keyboard/reduced-motion discoverability and navigation feedback remain open in [Sim's independent review](../Reviews/M2-Sim-20261116/README.md).
- No player build, fresh retained exact-source parity manifest, new balance experiment, full authored-route success or M2 milestone acceptance is claimed.

## Publication and recovery

The validated source is 21b3c71, following merge 472375b and catalog alignment 85d585a. Integration plus retained documentation/images was pushed to origin/ProjectMain at **e5f475642f7cd64027cba8b7878f9d861ebd709c**. A fresh fetch/ancestry audit immediately before publication found no local/fetched remote branch tip outside the candidate. The normal F:/ForkBin/GameDev checkout was clean, switched to ProjectMain and fast-forwarded; the remote exact SHA was read back. No force-push or historical branch/worktree deletion occurred. Default main remains b3ea53f543465d9ca6178558185274960a725584.

The required [M2 Trello integration handoff](https://trello.com/c/PoxE1gpz/19-m2-vertical-slice#comment-6afb8309f78ad492441eee74) was posted under the user-confirmed Nick Nevins identity and read back successfully. It links the shared integration evidence and preserves the open milestone/ecology status. Local screenshot proof is artifacts/projectmain-integration-20261116/trello-consolidation-confirmation.png. This subsequent publication-record commit changes documentation only.

Use ProjectMain in F:/ForkBin/GameDev/LearningIndieDev for subsequent reviews. Earlier M2/GLJP/wrapping branches remain recovery/history references; their old checked-out worktrees should not be mistaken for the unified source.

The normal Unity 6000.4.6f1 Editor reopened ready after publication, and the compilation check returned up-to-date with zero errors. A final recovery-worktree audit left existing dirty files untouched: the old FD2 rehearsal contains already-integrated per-round wallet/label/test edits, an obsolete 400/20/10 scenario draft and historical working-state edits; its scenario rollback was deliberately not applied over the current 400/55/20 source. ProBuilder settings edits also remain in that rehearsal and the original wrapping worktree. All committed branch work is contained; these preserved obsolete/local drafts are not additional feature merges. FD2-T01 and the earlier ProjectMain integration worktrees are clean.
