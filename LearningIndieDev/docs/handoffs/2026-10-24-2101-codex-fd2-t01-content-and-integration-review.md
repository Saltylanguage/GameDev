# fd2-t01-content-and-integration-review

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-24-2101-codex-fd2-t01-content-and-integration-review
- Owner: codex
- Branch: BevBranch
- Baseline commit: 9f9ed11
- Date: 2026-10-24
- Supersedes: none

## Summary

Prepared a concrete FD2-T01 content definition for review against Salty's now-
pushed mechanics. The tuning sheet proposes five supported additive modifiers
and truthful display names while retaining stable IDs, graph, economy and
permanent ownership. The isolated incoming portable build and 36-assertion
review pass. T01 remains open for effect/name approval and a validated
integrated definition; no production source/asset or user save changed.

## Changes

- Added [T01 review](../FIELD_DATA_PERMANENT_PROGRESSION_T01_REVIEW.md), with
  incoming/local rule differences, SG-005 review, evidence and integration order.
- Updated the [single tuning sheet](../FIELD_DATA_PERMANENT_PROGRESSION_TUNING_SHEET.md)
  to proposal v0.2, preserving approved economy/IDs/graph/capacity terms.
- Proposed display/effect pairs: Tough Hide/block +1; Long Vision/vision +2;
  Long Stride/movement +0.25; Efficient Digestion/digestion +0.25; Population
  Reserves/pool capacity +10. Source counterparts are incoming POC assets;
  the proposal does not copy their POC IDs, graph, prices or refundable ownership.
- Aligned PDD/SDD with review status and owned prerequisites, removed stale
  unresolved conversion-research prices, and corrected contradictory rounding
  prose to the canonical two-stage floor rule.
- Updated Working State and ticket draft with the review checkpoint.

## Decisions and assumptions

- Preserve approved FD2 permanent node ownership and no-refund reset, documenting
  its scoped exception to SG-005's other Rabbit allocation/refund contract.
- Prerequisites gate permanent purchases; they do not also consume active
  capacity. The incoming POC resolver requires active parents, so preserve its
  own semantics and adapt the FD2 validation/freeze seam explicitly.
- Effects and display names are proposed content choices, not approved assets.
- Incoming uses a different schema-v3 wallet/progression shape. A compatible
  migration is a prerequisite for merging the profile implementations; do not
  infer migration solely from the shared version number.

## Validation

- Fetched origin. Incoming ref:
  `7ac2a5456bf0c1f6b8e3c106533cd555f78ba5dc`; local baseline `9f9ed11`.
- Retained source archive and source manifest with Git blob IDs and SHA-256
  content hashes under `artifacts/fd2-t01-reconciliation-20261024/`.
- Built the incoming portable project from that isolated Git archive in Release:
  passed, zero warnings/errors. No dependency or project configuration change.
- Ran `dotnet run --project artifacts/fd2-t01-reconciliation-20261024/ContractReview.csproj
  -c Release -- <absolute docs/FIELD_DATA_PERMANENT_PROGRESSION_TUNING_SHEET.md>`:
  36 assertions passed. Covers five modifier applications, catalog terms, all
  32 candidate loadout subsets under incoming capacity/prerequisite validation,
  and eight canonical arithmetic examples. Incoming permits six loadouts,
  zero with Warren Network. Once all nodes are owned, 31 capacity subsets are
  possible under proposed ownership-only prerequisites, 15 with Warren Network.
  These latter counts are enumeration, not a patched runtime test.
- `git merge-file --stdout --diff3` on isolated copies: both profile files and
  GenomeContractTests each have one text conflict; MainMenuPlayModeTests is
  text-clean. All original source hashes stayed unchanged. No Git index or
  branch mutation. Text cleanliness does not prove semantic integration.
- Focused diff whitespace checks pass. Review links resolve; the link scan also
  found 14 pre-existing Working State links to absent historical ignored artifacts,
  all present in HEAD already. They were left unchanged and recorded in
  `artifacts/fd2-t01-reconciliation-20261024/link-check.json`.
- All 22 inspected incoming source snapshots match recorded hashes, and all
  four reviewed local source files still match their pre-preview hashes.

## Risks and incomplete work

- No new Unity test/compile, balance simulation, scene/asset authoring, profile
  migration execution, merge, commit, push or Trello change in this review.
- Supported modifiers are not balanced content. SG-005 outcome measurements and
  AV/tier estimates remain unmeasured; no new research panel/gate was invented.
- Two-stage rounding checks validate the specification only; T03 calculator
  implementation remains separate.
- Existing local T02 code/tests, unrelated metadata and recovery artifacts are
  preserved. Prior T02 26 focused passes do not validate incoming/merged code.

## Next useful step

Bevin reviews the five proposed effect/display-name mappings in the tuning
sheet. Then prepare the isolated integration and compatible save migration,
apply the reviewed definition through existing authoring/runtime seams and
run the tests listed in the T01 review. T01 remains open until the approved
definition and integrated validation are complete.
