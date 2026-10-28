# fd2-integration-and-gameplay-validation

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-10-28-1310-codex-fd2-integration-and-gameplay-validation
- Owner: codex
- Branch: BevBranch
- Baseline commit: 9f9ed1153d805b980ee84ac00231a2a297c5494f
- Date: 2026-10-28
- Supersedes: none

> **Correction, October 28:** The population restoration described below was an
> integration mistake. A later October 22 decision selected 400 Plants / 20
> Hares / 10 Foxes for launch; `be98b03` authored it. The 400/55/20 screen
> remains valid for that exact tested setup, not for the intended launch setting.
> Bevin chose to stop the live 55/20 run and correct the setting.
> Later the same day, Bevin explicitly chose 55/20 for continued playtesting
> after rapid Hare collapse on 20/10; see the later choice below.

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
55 Hares and 20 Foxes to 400/20/10. Mistakenly restored the older
asset in `a2035b35fb1f45411977d9c4de0892746bef7e32`, preserving its
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
  older starting population restored. This is not a full PlayMode pass.
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

## Live-play correction and banked Field Data

On October 28 Bevin observed the first checkpoint showing 16 FD awarded but
0 available. The actual session ledger had banked 16; the October 22 decision
withholds first-simulation earnings until the end of six rounds. The previous
display did not explain this. The breakdown now distinguishes available from
banked Field Data and states when the bank becomes available. Gameplay rules
and payout calculations are unchanged.

Bevin chose to stop the incorrectly configured 55/20 run. The 400/20/10
October 22 scenario and matching test expectations were restored locally.
Isolated population configuration EditMode 41/41 and targeted UI PlayMode 1/1
passed; the integrated Editor compiled without errors. The restarted live
20/10 run reached round 1 and shows 9 banked, 0 available, matching the live
ledger. Bevin's clarity response and the full normal-play outcome remain
pending. Correction posted on T12:
https://trello.com/c/wxVgh3Q6/148-fd2-t12-verify-migration-and-end-to-end-permanent-progression#comment-6ae25bb53963925ac37fe55a

## Updated Field Data timing decision

After reviewing the clarified display, Bevin confirmed on October 28 that Field
Data should be spendable at each round-end pause, including the first simulation,
superseding the October 22 delay for the Field Data test route. The session now
exposes settled ledger funds immediately, while failed and closed journeys
continue to expose zero; mutation choices still must be resolved before paid
population controls and controls still cost 10 FD. The temporary banked-until-
Simulation-1 explanation is removed. Focused journey/session and population
control EditMode: 41/41; focused route UI PlayMode: 1/1; original integrated
Editor compiled without errors. These were isolated from the user's profile.
Live restart and manual first-round purchase verification remain pending. The
legacy POC journey is separate and was not changed. No balance approval.

## Later playtest population choice

After restarting on 400/20/10, Bevin reported that Hare collapse occurred too
quickly to test or play and explicitly chose to return to 400 Plants / 55 Hares /
20 Foxes for continued playtesting on October 28. This supersedes the earlier
launch setting for this current playtest; it is not balance acceptance. The
new per-round Field Data spending rule remains in place. The previous 400-run
55/20 screen still applies only to its exact no-purchase/no-reward-choice
setup and cannot establish the outcome of this interactive playthrough.

At the restarted 55/20 round-one pause, 16 FD was available. Bevin bought a
population control and said it worked; the live session confirmed 6 FD left
and one intervention. The journey continued; full playthrough and balance
acceptance remain pending.
