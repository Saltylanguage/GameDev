# GLJP-01 — profile research gates journey Mutations

Date: November 5, 2026. Local changes on BevBranch; no push.

**Human acceptance, November 5:** Bevin tested the change and confirmed, "okay yes that is working as intended now." This accepts the research-gate behavior described below. The broader GLJP-01 catalog redesign and previously recorded runtime failures remain open.

Bevin found Mutation choices on a fresh profile, `Bev Test 11/5/2026`, and requested the missing GLJP-01 gate. Live inspection confirmed zero research/species wallets, no Genome licenses and no Genome profiles. The old authored offer pools were not connected to permanent ownership.

## Implemented

- `CaptureMutationResearch` copies owned research IDs from the selected profile. Equipment is not required for offer eligibility. Separate profiles and later research changes cannot mutate an already captured policy.
- The Desktop profile launch binds this policy to the preview's progression. Opening map rewards, phase/legacy offers, authored offers and Field Data choices filter by that policy. Reward-application checks also reject unresearched Mutations.
- Empty opening rewards have a clear Continue Without a Mutation action and still unlock the next simulation node. Empty Field Data rewards display an explanation and allow Skip; the empty card header is hidden.
- Gene Lab details explain that owned research unlocks the associated journey choices even while unequipped.
- Existing profileless developer/scientific harnesses remain unrestricted; player-profile launches always bind the captured policy.

## Current catalog mapping and limits

The current FD2 researchable nodes unlock Tough Hide, movement through Long Stride, Efficient Digestion, and Keen Senses through the current Long Vision node. Legacy POC research IDs map to their existing capabilities. Unresearched or unauthored capabilities are excluded, rather than made universally available.

This completes the gate requested here, not the entire GLJP-01 catalog redesign. Heightened Senses naming and a separate Long Vision Mutation, new research nodes/prices, and removal of the existing permanent stat bonuses remain catalog work. The current permanent bonuses are retained during this scoped change. No profile migration or currency grant was needed for the gate.

## Verification

- Final full EditMode: 708 passed, 0 failed, 1 skipped, `artifacts/unity-tests-20261105-141827`. Includes new ownership-versus-equipment, frozen policy/profile isolation, rejected unresearched application, and empty/eligible Field Data pool tests. Earlier passing run `141139` is retained.
- Real Main Menu profile entry, empty research opening reward: 1/1 passed, `artifacts/unity-tests-20261105-141509`.
- Real Main Menu profile entry, purchasing Tough Hide on the Desktop before opening the journey: 1/1 passed, `artifacts/unity-tests-20261105-142047` (earlier run `141523` retained). Only Tough Hide is offered at the opening. Research is refreshed when opening an untouched journey so no scene reload is required; once the opening node has been entered, its policy remains frozen.
- Empty research Field Data round, Skip and population purchase rendering: 1/1 passed, `artifacts/visual-evidence-20261105-141712`. Inspected `gljp-journey-mutation.png`: no Mutation cards, readable research explanation and Skip action. The test verifies no choices, then Skip and successful Hare purchase.
- Initial failed tests are retained: one fixture used the legacy `hare` dictionary key, and the first opening test entered the profileless developer scene. Corrected fixtures use the authored species rules and actual Main Menu profile entry, including its timed transition.
- The named user profile remains at zero currencies with no Genome ownership/license after test backup restoration; no research or Mutation was granted to it.

The historical full-PlayMode eight failures from November 1 are not reclassified or hidden by these focused passes. No balance or player-build acceptance is claimed.

## Ticket status

GLJP-01's research-gate subtask is implemented and verified. The full card stays open for the remaining catalog work above. GLJP-02 ranked research/migration and GLJP-04 burrows are independent follow-ups. Updated the PDD companion/SDD and morning list to resolve research-versus-equipment ownership semantics.
