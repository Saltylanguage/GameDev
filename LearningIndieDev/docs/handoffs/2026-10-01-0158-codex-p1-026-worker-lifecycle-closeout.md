# P1-026 Worker Lifecycle Closeout

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-01-0158-codex-p1-026-worker-lifecycle-closeout
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 2e11ce5b
- Date: 2026-10-01
- Supersedes: none

## Summary

P1-026 is resolved. The shared `codex/cellsim-worker` branch now contains the
Unity lifecycle and preflight safety backport. The worker is documented as a
closed-Editor batch runner, so the live Pipeline state-routing path used by
developer workflows is not required for this worker.

## Changes

- Shared worker branch includes `a227f118` (lifecycle/preflight safety) and
  `0adb76cb` (handoff status correction).
- Batch tooling no longer deletes Unity lockfiles. It cleans up the launched
  Unity process tree and newly observed Package Manager/licensing helpers.
- Updated the Loose Ends ledger to remove P1-026 from open items.

## Decisions and assumptions

- The worker's documented contract requires Unity to be closed while jobs run.
  Its existing batch execution path is appropriate; adopting the developer
  `Auto`/`Live`/`Clean` routing is outside this lifecycle defect.
- “Mini PC” is generic wording in the worker documentation, not an identified
  host or connection. No host-specific test is claimed.
- Queue job `20260903-033240-b1b43c58` remains pending and was not run or changed.

## Validation

- PowerShell parser and `git diff --check` passed on the worker tooling change.
- A synthetic Windows parent/child process tree was terminated using the
  worker branch's cleanup function with normal host permissions.
- Synthetic `Invoke-UnityBatch` exit 0 and exit 7 paths passed; the original
  nonzero exit was reported.
- A mock project lockfile remained present after the new guard rejected it.
- No Unity Editor invocation or queued simulation was performed.

## Risks and incomplete work

- The tests exercise the worker branch's PowerShell code on this Windows host,
  but do not claim validation on a separate worker machine.
- The worker branch remains intentionally behind `ProjectMain`; synchronizing
  unrelated gameplay and simulation changes is not part of P1-026.

## Next useful step

Before starting the worker, review its pending job as a separate experiment
request. Do not treat this tooling closeout as approval to run that job.