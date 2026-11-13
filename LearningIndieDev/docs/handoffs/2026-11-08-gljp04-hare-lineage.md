# GLJP-04 Hare lineage foundation

November 8, 2026. First implementation slice for [GLJP-04](https://trello.com/c/DQwFKLSF).

The regular runner records Hare offspring in a journey-owned ancestry table as soon as a continuous Hare run starts. Both immediate births and completed gestation record the offspring ID and the two parent IDs, using the mate ID captured at conception for gestation. This gives future burrows access to descendants born before Burrowing is acquired and preserves the relation if the mate dies. The record is independent of detailed birth telemetry. A checkpoint stores its own copy, restoration copies it again, and restarting a journey clears the ancestry from the discarded run.

The record grows with the number of Hare births in the journey and is discarded with the run. It uses cell entity IDs for lookup within that run. The shared standalone runner compiles the same source. The portable final-state digest still needs a stable ancestry representation when Burrowing makes those relations affect simulation outcomes.

## Validation

- The CellSim.Batch Release build finished with zero warnings and zero errors. Its existing portable self-test passed, including deterministic replay and the four-simulation journey.
- Both cases of `HareJourneyTracksBothParentsThroughBirthAndCheckpoint` passed when the exact NUnit test body was run against the standalone build. The cases cover immediate birth, a mate removed before gestation completes, ancestry with detailed telemetry disabled, an unrelated Hare, restart, a grandchild born after restore, and independence of the saved checkpoint.
- The focused Unity EditMode batch process timed out during startup after 150 seconds and produced no test report. See `artifacts/gljp04-lineage-20261108-114621/Unity.log`.

## Next implementation slice

The next implementation slice needs to own burrow locations, capacity, expiry and hidden occupants. It must include hidden Hares in population totals and prevent them from becoming combat targets. Use the approved 200-step lifetime as the first collapse trigger, keep occupants protected while exits are occupied, and leave other collapse triggers open. Creation, hiding and emergence, the journey Mutations, Born Burrower, and player presentation are still open.
