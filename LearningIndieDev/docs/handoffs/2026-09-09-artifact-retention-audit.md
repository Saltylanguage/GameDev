# Artifact retention audit — 2026-09-09

Purpose: make Unity evidence quick to load without throwing away the raw
material needed to investigate or replay a result.

Status: audit and conservative cleanup completed. Thirteen explicitly approved
empty directories and 41 approved raw log/report files were removed after
verification. The inventory counts below describe the pre-cleanup state.

## What was checked

The audit covered `LearningIndieDev/artifacts`, the project documentation that
references those artifacts, and the V1 Artifact Summarization output.

| Evidence type | Observed |
| --- | ---: |
| Artifact directories | 335 |
| Total artifact data | 5.95 GB |
| CellSim `report.json` files | 72 / 2.08 GB |
| CellSim summaries generated | 72 / 7.64 MB JSON + 0.65 MB Markdown |
| Unity log files | 455 / 3.69 GB |
| NUnit result XML files | 90 / 27.7 MB |
| Empty artifact directories at audit time | 14 |

All 72 CellSim reports summarized successfully. Sixty-nine parsed as clean
JSON. Three legacy reports were recovered by removing only trailing commas and
are marked with a parse warning in their summaries:

- `cellular-experiment-20260905-063539`
- `cellular-experiment-20260905-063730`
- `cellular-experiment-20260905-063858`

The recovery preserves the source report untouched. It is not permission to
delete the source without checking the clean representative first.

## Retention rule

`report-summary.json` preserves decision-useful aggregates, provenance,
per-seed/per-phase outcomes, upgrade identity, stat lines, behavior, deaths,
combat distributions, opportunity controls, and baseline deltas. It does not
preserve raw event order or replay detail. The original `report.json` remains
the primary forensic/replay evidence.

The audit therefore uses three states:

- **Keep raw:** the report is cited by a research package, is part of a paired
  reproducibility/held-out comparison, or is active upgrade evidence.
- **Candidate to archive/remove:** a clean summary exists, the report is not
  cited, and an equivalent representative or retained result covers the same
  semantic run.
- **Do not remove yet:** the report has a parse warning, failed-run diagnostics,
  unresolved research value, or no trustworthy replacement.

## Retain as raw evidence

These groups are explicitly cited by current documentation or define active
research/balance evidence:

- The paired EX-001/EX-001B bundles from `20260815`.
- The EX-007/EX-008/EX-009 adapter-backed bundles from `20260903` and
  `20260904`.
- The EX-010 development and held-out bundles from `20260906`, plus its paired
  comparison artifact.
- The current upgrade capability panel from `20260909` (control and the nine
  scheduled upgrade arms).
- Explicitly cited historical/support bundles such as
  `cellular-experiment-20260811-223303`,
  `cellular-experiment-20260811-225332`,
  `cellular-experiment-20260812-061503`,
  `cellular-experiment-20260812-211950`,
  `cellular-experiment-20260812-212058`,
  `cellular-experiment-20260905-035353`,
  `cellular-experiment-20260905-040741`,
  `cellular-experiment-20260905-040925`, and
  `cellular-experiment-20260905-064045`.

The paired EX-001/EX-001B reports may have semantically equal payloads, but the
documentation records their separate raw hashes and pair roles. Do not merge
or delete one of those representatives.

## Semantic duplicate candidates

The summarizer compared experiment configuration, upgrade contract, and all
retained run payloads while ignoring generated creation timestamps. These are
semantic duplicates, not byte-identical files:

| Retain | Candidate duplicate(s) | Approx. removable bundle data |
| --- | --- | ---: |
| `cellular-experiment-20260811-223303` | `cellular-experiment-20260811-222638` | 2.39 MB |
| `cellular-experiment-20260905-035353` | `cellular-experiment-20260905-035528` | 0.45 MB |
| `cellular-experiment-20260905-064045` | `cellular-experiment-20260905-063539`, `...-063730`, `...-063858` | 6.33 MB |

Potential savings from these five unreferenced duplicate bundles is about
9.17 MB. The three `0635xx` candidates carry the trailing-comma recovery
warning; their clean `064045` representative is the reason they are only
candidates, not automatic deletions.

## Lowest-risk cleanup candidates

These directories were empty and contained no report, log, or metadata. They
were removed without losing file content; the names remain here as the cleanup
record:

- `cellular-experiment-20260815-151611`
- `cellular-experiment-20260815-160010`
- `cellular-experiment-20260815-164853`
- `cellular-experiment-20260815-184014`
- `cellular-experiment-20260815-184117`
- `unity-preflight-20260903-181738`
- `unity-tests-20260811-221643`
- `unity-tests-20260811-221959`
- `unity-tests-20260812-034903`
- `unity-tests-20260812-042051`
- `unity-tests-20260815-193018`
- `unity-tests-manual`
- `windows-build-20260908-065929`

`cellular-experiment-20260815-203653` is also empty, but current EX-002
documentation names it as a failed startup attempt. Leave that path marker in
place unless the historical reference is deliberately revised. It is the only
empty directory from this audit that remains after the approved cleanup.

## Logs and test reports

The largest avoidable cost is not CellSim JSON; it is Unity test logging.

- Forty-three test folders have result XML with no failed/error test case. Their
  logs occupy about 2.4 GB. Eleven of those folders are explicitly referenced
  by current documentation; retain those logs until the references are revised.
  The other thirty-two folders account for about 1.6 GB and are reasonable
  archive/remove candidates after retaining their XML.
- Eleven test folders contain failed/error cases and their logs occupy about
  0.57 GB. Keep those logs until the failure is either resolved or its useful
  diagnostic evidence is recorded elsewhere.
- Nine test folders have no result XML. Keep their logs until it is clear they
  are failed startup attempts with no remaining diagnostic value.
- Keep the seven explicitly cited preflight/license logs. Other old preflight
  folders are small cleanup candidates, with the same “no unresolved diagnostic
  value” check.
- Keep NUnit XML result files. They are only about 27.7 MB and preserve test
  outcomes more compactly than Unity logs.

## V1 diagnostics compression

The new diagnostics index is a quick-reading layer for the logs and Unity test
result XML (NUnit):

- `unity-diagnostics-summary-v1.json` covers 455 Unity logs (3,957,675,119
  bytes) and 90 NUnit result XML files (29,045,643 bytes).
- The two compact outputs are about 0.93 MB total, a 99.98% size reduction
  for triage/context loading.
- The index keeps source hashes, byte/line counts, Unity/version/date/command
  identity, process-exit evidence, bounded redacted signal samples,
  memory-leak totals, and failed test names/messages.
- It omits raw event order and full stack/log replay. Raw logs and XML remain
  the source of truth for investigation; a summary is not proof of causality
  or permission to delete anything.

The first pass found 198 logs where Unity reported success, 50 where it
reported failure, and 207 without a clear exit marker. Fourteen of the 90 test
result files contain failed/error cases. The successful, clean-result folders
are the best later archive candidates, but only after checking current
documentation references and preserving a recoverable archive. Logs without a
clear exit marker should stay until their startup/termination meaning is
recorded.

The generated files are local and ignored by Git:

- `LearningIndieDev/artifacts/unity-diagnostics-summary-v1.json`
- `LearningIndieDev/artifacts/unity-diagnostics-summary-v1.md`

After cleanup, the index contains 416 remaining Unity logs and all 90 NUnit
result XML files. It was filtered from the hash-verified pre-cleanup index so
the deleted rows are not presented as current evidence; no remaining log was
rewritten.

## Safe next cleanup sequence

1. Review the five semantic duplicate bundle candidates above.
2. Move approved candidates to a recoverable local archive, then rerun the
   summarizer and confirm the retained representative summaries have no parse
   warnings.
3. Use the diagnostics index to classify test folders; the first conservative
   cleanup pass is recorded in the companion execution handoff.
4. Re-run the DirtyBoy snapshot and the smallest relevant bundle/test checks.
5. Delete only after the archive/retention decision is recorded in the next
   handoff.

This sequence preserves the evidence needed for current upgrade work while
avoiding a false claim that an aggregate summary can replace replay data.
