# S3-02 — Expedition contract

**Status:** In progress. Only the decisions marked agreed below are locked.
**Owner:** Josh · **Reviewer:** Sim · **Planned effort:** Josh 4h, Sim 2h

## Purpose

Set the player-facing rules for one Forest Edge expedition before S3-03 uses
them to build the full game-state flow. This is the new working record; earlier
S3-02 proposals are discarded.

## Agreed

- One expedition has **6 phases**.
- These six simulation phases are the six **rounds** of the expedition.
- Each phase lasts **10 seconds of simulation time**.
- One complete expedition therefore contains **1 minute of simulation time**.
- The player gets **5 upgrade decision moments per run**.
- At each moment, the player chooses from **3 Mutations** or chooses **Skip**.
- Mutations are temporary and last only for the current run.
- Permanent upgrades are bought with currency in the **Gene Lab** application,
  opened from the GalapagOS Desktop. These are **Genome Upgrades** and fill out
  a skill tree called a **Genome**.
- Mutation choices use a small visual cue and keyword (for example,
  **“hunting +1”**), not a full statistic breakdown. The player should be able
  to predict the effect at a glance, and its expected impact should be clear in
  the following phase.
- There is no Restart action. **End** abandons the current run after the player
  confirms Yes in a confirmation window. Ending before round 6 forfeits all
  rewards. **Pause** is available.
- Victory in any biome means the player species is not extinct at the end of
  round 6. Currency is awarded using a performance measure that is currently
  being developed, not simply the final population count. Additional
  bonus-event rewards are possible but not decided.
- If the player species goes extinct, the run ends immediately as a failure
  and gives no rewards.

## Six-round decision cadence

- Round 1 starts without an upgrade.
- After each of rounds 1–5, the player chooses one of three Mutations or
  chooses Skip, then continues to the next round.
- Finishing round 6 leads to the result; there is no additional upgrade.

## Still open or deferred, not blocking S3-02

- A possible Skip bonus is undecided. If adopted, it uses the same currency as
  Genome Upgrades. Its amount and limits are open; do not make this a blocker.
- Forest Edge board size is not authoritative yet. Defer the 42×20 versus
  36×20 choice; do not block the S3 contract on it.
- Playable plant species are on hold. Do not include Fern or plant identity in
  the current contract.
- No real-time duration target is set. “One minute” means simulation time;
  actual elapsed time varies with simulation speed and pauses.

## Other details to resolve during implementation

- Link the approved performance measure and its currency conversion when that
  work is ready; do not block this contract on the unfinished formula.

Keep unreviewed items open. Do not change runtime settings, scenario assets, or
other product documents until their contract decisions are agreed.
