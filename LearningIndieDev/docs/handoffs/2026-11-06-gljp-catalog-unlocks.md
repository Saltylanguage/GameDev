# GLJP catalog unlocks - November 6, 2026

Local uncommitted work on the current branch. GLJP-01 is not closed. No push, and no Trello card was moved. The GLJP-01 card still showed unsaved description edits when this slice started.

Escape Instinct, Increased Minimum Litter, and Larger Litters are now purchasable Hare research nodes. Each costs 5 Hare Data, has no prerequisite, and is labeled as a test price and path in the Gene Lab. Buying one unlocks only its matching journey Mutation. Equipping it does not change natural Hare rules. A fresh profile still cannot receive those Mutations.

Faster Gestation stays CONCEPT · LOCKED. The old permanent preview reduces gestation by 15% per rank, but no journey Mutation changes gestation, so that formula was not copied into the shop.

A minimum-litter rank that would push the minimum above the current maximum is refused. Raising the maximum first makes the next minimum rank valid. Field Data offers the maximum-litter Mutation before the minimum-litter Mutation in the ordinary pool so a full automatic pass can apply both.

Keen Senses still tracks creature food, and the default Hare reproduction chance remains 100%. Neither was retuned.

## Evidence

- EditMode `FieldDataPermanent`: 29/29 (`artifacts/unity-tests-20261106-163531`).
- EditMode `FieldDataJourney`: 46/46 (`artifacts/unity-tests-20261106-163548`).
- New research test: 1/1 (`artifacts/unity-tests-20261106-163610`).
- Gene Lab PlayMode capture: 1/1 (`artifacts/visual-evidence-20261106-163739`). Larger Litters shows `BUY · 5 DATA (TEST)` and the provisional-path note. Faster Gestation remains locked in `genome-permanent-concept-locked.png`.

Full EditMode after these checks is 744 / 0 / 1 (`artifacts/unity-tests-20261106-163829`). The PlayMode half timed out when the Editor Pipeline disconnected and recorded no assertions. Resume from [the Codex pickup](2026-11-06-gljp-codex-pickup.md). Seed 10100 remains retained ecological evidence and was not tuned.
