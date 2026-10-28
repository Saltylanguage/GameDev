# fd2-integration-and-gameplay-validation

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-10-28-1310-codex-fd2-integration-and-gameplay-validation
- Owner: codex
- Branch: BevBranch
- Baseline commit: 9f9ed1153d805b980ee84ac00231a2a297c5494f
- Date: 2026-10-28
- Supersedes: none

## Integration

User authorized review and integration of completed FD2, followed by normal
play and balance checks. Local BevBranch fast-forwarded to candidate
`d42f3201c6987cf85f256c9fa0aeeec9a8e3a809`, a descendant of the accepted FD2
branch `0ae492e` and the then-current remote ProjectMain `7ac2a54`. Remote
BevBranch was `9f9ed11` before integration; nothing was pushed.

Before integration, all 43 modified/untracked files were byte-snapshotted and
hash-verified. Backup: `LearningIndieDev/artifacts/fd2-integration-20261028/original-snapshot/` (also retained in the FD2 worktree).
A recoverable Git stash `520cc9cbdbf4b25ad5dd01c41539888ba4622999` retains the
entire original dirty state; preexisting stashes are recorded in the snapshot.
The five original `_Recovery` files and preexisting `WORKING_STATE.md` content
were restored byte for byte; this current-focus entry was added afterward.

The four locally modified T02 profile/test files matched the original T02
manifest exactly; their work had already been reconciled into the FD2 branch
at `03cd29e` and validated by migration cases. Original working-state history
and recovery files remain local uncommitted/untracked. Eighteen previously
untracked handoff/design files were byte-identical to the incoming tracked
versions and are now retained by the integrated history. The two locally
edited direction documents were reconciled in `d42f320`: their FD2 settings and
override survive, with stale pre-T01 placeholder language updated.

The ten original flourish importer edits were structurally preserved in the
incoming metadata, which additionally contains default WebGL entries. The
incoming history also sets `runInBackground` from 0 to 1 in Player Settings;
that branch behavior was retained. No terrain assets or `.meta` GUIDs were
rewritten by the integration.

## Authored starting population

The incoming history changed Forest Edge from the previously approved 400 Plants,
55 Hares and 20 Foxes to the 400/20/10 research fixture. Restored the approved
asset exactly in `a2035b35fb1f45411977d9c4de0892746bef7e32`, preserving its
metadata. Updated the stale default expectations in the population override test
in `28de090`; its override assertions remain intact.

## Validation

- Corrected rehearsal full EditMode: 702 total, 701 passed, zero failed, one
  existing parity skip. Corrected focused graphics PlayMode: 8/8. The actual
  integrated checkout also passed the earlier focused graphics set 8/8.
- The remaining 25 related PlayMode cases were rerun with an isolated temporary
  PlayerPrefs namespace: 21 passed and four failed. All four test names had
  failed in the prior baseline; three messages are identical. The fixed-seed
  success test now fails at tick 237, versus tick 882 historically, with the
  approved starting population restored. This is not a full PlayMode pass.
- Portable Release build and self-test passed in the rehearsal. The temporary
  product name was restored; the real profile preference keys and all five
  recovery files matched their snapshots after the earlier integrated test.
- XML results, logs and comparison are in
  `artifacts/fd2-integration-20261028/validation/`.

## Approved balance screen

Bevin approved 200 matched seeds for no active Genome versus Tough Hide, Long
Vision, Long Stride and Efficient Digestion. All 400 headless natural journeys
completed, with independently checked output hashes. Authored Forest Edge
400/55/20 populations and rules came from `a2035b3`; the later `28de090` change
only adjusts a test expectation. No free rewards or purchases were applied.
The exact T12 live-selected loadout differs and is not represented as equivalent.

No-Genome succeeded 31/200, and the four-node build succeeded 30/200. Both
succeeded on five matched seeds; baseline alone succeeded on 26 and the build
alone on 25. Living Hares at the tick-600 boundary: 71 versus 42. Median
terminal ticks: 329 versus 216. These are exploratory results without an agreed
survival threshold, not balance approval or a tuning decision. The frozen
four-node snapshot was baked using shared progression rules because the replay
command ignores scientific arguments; four final-digest Unity references per arm,
one actual journey-session comparison per arm, and a repeated seed matched.
The first exported 400/20/10 snapshot was excluded before cohort runs.

Factual report, interpretation, per-seed data, audit, frozen runner and
provenance: `artifacts/fd2-integration-20261028/natural-screen-200x2/`.
Single-node/incremental comparisons and early-loss investigation are possible
follow-ups; the four-node comparison cannot isolate the cause.

## Next steps

The integrated original Unity project is open in Edit mode. Interactive normal
play remains pending Bevin's choice of a new test profile or an existing profile;
no manual gameplay or fun acceptance is claimed for this integration. No tuning,
player-build, production balance or publication approval is implied by the
screen. The integration is local and has not been pushed.
