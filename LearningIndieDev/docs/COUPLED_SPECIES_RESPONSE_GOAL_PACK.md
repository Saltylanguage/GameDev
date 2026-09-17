# Coupled Hare/Fox Responses - implementation goal pack

Date: 2026-09-11  
Status: Draft for implementation; no gameplay or balance change is authorized by this document alone  
Requested by: Bevin  
Upgrade feature owner/reviewer: Josh  
Implementation owner: Assigned AI agent  
Scenario: Forest Edge  
Related design: [Reactive Species / Ecology Plan](REACTIVE_SPECIES_ECOLOGY_PLAN.md), [Hare + Fox treatment](Species%20Design/HARE_FOX_ITERATIVE_TREATMENT.md), [upgrade-system direction](UPGRADE_SYSTEM_DIRECTION.md), and [upgrade concerns](Planning%20Concerns/upgrade-system.md)

## Outcome

Add one explicit, default-off **Coupled Hare/Fox Responses** toggle under the
existing `bev-experimental` feature bundle. When enabled, purchasing a supported
Hare or Fox Mutation also grants one authored response level to the opposing
species at the same frozen phase boundary.

The feature should make an expedition feel like an ecological arms race without
adding random counter-picks, adaptive AI, plant tuning, or a general evolution
framework. When the toggle is off, the current simulation, rewards, reports,
random-number sequence, and balance must remain unchanged.

## Player-facing contract

- User-facing toggle: `Coupled Hare/Fox Responses`.
- Runtime setting: `CoupledSpeciesResponsesEnabled`.
- Batch argument: `-CoupledSpeciesResponses true|false`.
- Default: off.
- Availability: Forest Edge with both `hare` and `fox` present.
- The setting is chosen before an expedition starts and remains frozen until
  that expedition ends.
- Enabling it requires the `bev-experimental` feature bundle. A batch request
  that enables the response toggle without `-ExperimentalFeatures
  bev-experimental` must fail validation.
- The player pays only for the selected species' Mutation. The response is free.
- A purchased Mutation grants exactly one response level at the same boundary.
- Skip grants neither species an upgrade.
- Both changes remain active for later phases and reset when the expedition ends.
- The choice preview names both changes before the player commits.
- Missing, invalid, excluded, or capped responses must disable/reject the whole
  purchase before currency or rules change. Never silently apply only one side.

The Coupled Response is a separate experimental layer. It does not redefine a
Mutation, enter Biome Simulations, alter Genome ownership, or grant the
non-player species ordinary Mutation choices.

## Initial response map

Use exact stable species and upgrade IDs. Keep the map in the existing upgrade
catalog or the smallest nearby first-party surface; do not add a service,
reflection, discovery scan, or new ScriptableObject system.

| Player species | Purchased Mutation | Response species | Automatic response | Intended relationship |
| --- | --- | --- | --- | --- |
| Hare | `tough-hide` | Fox | `piercing-bite` | Defense is contested by attack accuracy. |
| Hare | `threat-exposure` | Fox | `relentless-pursuit` | Better escape creates stronger pursuit pressure. |
| Hare | `efficient-digestion` | Fox | `hunt-urgency` | Better-fed prey provoke earlier hunting. |
| Hare | `reproductive-drive` | Fox | `brood-drive` | More prey growth supports predator reproduction. |
| Hare | `crowding-tolerance` | Fox | `brood-drive` | Dense, persistent prey supports predator population pressure. |
| Fox | `piercing-bite` | Hare | `tough-hide` | Stronger attacks provoke stronger defense. |
| Fox | `relentless-pursuit` | Hare | `threat-exposure` | Stronger pursuit provokes better escape/avoidance. |
| Fox | `hunt-urgency` | Hare | `efficient-digestion` | Earlier hunting provokes better prey energy recovery. |
| Fox | `brood-drive` | Hare | `reproductive-drive` | Predator population growth provokes prey population growth. |

`brood-drive` appears twice on the Hare-to-Fox side because the retained Fox
catalog currently contains four accepted skills against five Hare skills. Do
not re-enable or present retired `keen-senses`; its Detection-versus-Avoidance
idea remains a separate future cross-species experiment.

## Ordered goals

All goals begin as Not Started. Complete each goal's acceptance gate before
depending on it. Preserve unrelated worktree files and report current evidence
instead of treating old handoffs as fresh verification.

### CSR-0 - Reconfirm the implementation boundary

**Purpose:** Ensure the new opt-in rule fits the current continuous expedition,
Mutation, Genome, and experimental-feature contracts.

**Work:**

- Recheck the branch, dirty worktree, recent Git history, current reward flow,
  `SpeciesExperimentalOptions`, upgrade catalog, `SpeciesProgression`, boundary
  continuation, report serialization, UI settings, and focused tests.
- Confirm that the current player path still treats `bev-experimental` as the
  umbrella and that this new flag is independently default-off.
- Update `UPGRADE_SYSTEM_DIRECTION.md` to name Coupled Response as a distinct
  Species-Simulation-only experimental layer before runtime implementation.
- Keep existing concerns UPG-C03, UPG-C04, UPG-C06, UPG-C07, and UPG-C08 active.

**Deliverable:** A short implementation note in the handoff stating the exact
baseline commit, relevant dirty files, accepted layer boundary, and files likely
to change.

**Acceptance gate:** The agent can explain how the flag travels from UI or batch
input into a frozen expedition, where both species' rules are replaced at a
boundary, and how both acquisitions remain replayable. No implementation starts
if the only proposed design is to put Fox responses inside the selected Hare's
Mutation progression.

### CSR-1 - Add the isolated feature toggle

**Purpose:** Make the behavior explicitly controllable without changing the
current default.

**Work:**

- Add `CoupledSpeciesResponsesEnabled` to `SpeciesExperimentalOptions`.
- Reject `true` unless the feature ID is `bev-experimental`.
- Add a default-off serialized preview setting and expose it through the existing
  ViewModel/settings application path.
- Add one `Coupled Hare/Fox Responses` checkbox near the other developer
  simulation settings. It may be changed only before a run starts.
- Include the flag in saved defaults without changing older saved-settings
  behavior when the field is absent.
- Add `-CoupledSpeciesResponses true|false` to the existing experiment runner.
- Record the resolved value in reports and manifests.

**Deliverable:** One flag flowing through the existing UI and batch paths. Do
not add a second feature-bundle framework or another settings screen.

**Acceptance gate:**

- Default construction and existing saved settings resolve the flag to false.
- Toggle off produces the same options, rules, fingerprints, and result as the
  pre-feature path for the same seed and choices.
- Toggle on without `bev-experimental` is rejected with a useful message.
- The setting cannot change an active expedition.

### CSR-2 - Resolve deterministic response pairs

**Purpose:** Convert a purchased Mutation into one explicit opponent response.

**Work:**

- Add one small catalog lookup keyed by player `SpeciesId` and purchased upgrade
  ID that returns the response species and response upgrade ID.
- Resolve both upgrades through the existing catalog and cap rules.
- Validate the complete pair before spending currency or mutating either rule
  set.
- Return no response for Skip. Reject unsupported species, absent response
  species, unknown IDs, exclusions, or caps without a partial change.
- Consume no random values while resolving a response.

**Deliverable:** A deterministic response lookup using the table in this pack.

**Acceptance gate:** Focused plain-C# tests cover every table row, both player
species, Skip, toggle off, missing Fox/Hare, unknown IDs, response caps, and
atomic rejection. Repeated resolution of the same inputs returns the same pair.

### CSR-3 - Apply both species changes at one phase boundary

**Purpose:** Continue the same ecosystem with new effective rules for both
species.

**Work:**

- Reuse `SpeciesProgression`, `SpeciesUpgradeSnapshot`, the species-rule
  dictionary, and `ContinueWithBoundaryState`.
- Track response levels separately from the selected species' paid progression;
  do not charge response costs or mix its levels into the player's currency UI.
- At purchase, validate both changes, build the next Hare and Fox rules, combine
  both snapshots into the effective ordered loadout, and install them at the
  same boundary.
- Preserve cells, resources, entity state, absolute tick, previous-perception
  state, metrics, seed, and remaining phase schedule.
- Keep the response active for subsequent phases and reset it only with a new
  expedition.

**Deliverable:** Toggle-on Hare and Fox purchases update both species without
restarting or reseeding the board.

**Acceptance gate:**

- A boundary fixture proves both modifiers first become effective on the same
  recorded tick.
- The pre-boundary grid and retained entities are unchanged by the application
  itself.
- The next ruleset fingerprint changes and includes both effective rules.
- Toggle off, Skip, and unsupported scenarios retain the old path.
- Same seed, choices, and toggle state reproduce the same final grid and report.

### CSR-4 - Explain and attribute the response

**Purpose:** Prevent the opponent's upgrade from feeling hidden or arbitrary.

**Work:**

- Extend each supported reward preview with `Fox responds:` or `Hare responds:`
  plus the response name and immediate rule effect.
- After purchase, show both selected and response changes in the phase summary.
- Record both target species, order, effective tick, acquisition source
  (`player-choice` or `coupled-response`), and the triggering upgrade ID.
- Preserve the existing resolved modifier values, snapshot fingerprints,
  ordered-loadout fingerprint, scenario fingerprint, and experimental flags.
- Keep report fields additive and version them if the existing schema requires
  it. Do not infer a historical response from a mutable catalog at read time.

**Deliverable:** A player and a report reader can identify what changed, why it
changed, which species received it, and when it became active.

**Acceptance gate:** UI-focused tests verify both preview directions and Skip.
Serialization round-trips the two linked acquisition records. A saved report is
self-contained even if the response map changes later.

### CSR-5 - Prove the first pair without tuning everything

**Question:** Does automatic `piercing-bite` restore some Fox pressure without
making the player's `tough-hide` choice meaningless?

**Work:**

1. Run a small deterministic wiring fixture first.
2. After the implementation and experiment contract are reviewed, run three
   matched Forest Edge arms on seeds 1-100 with identical phase timing:
   - no upgrade;
   - `tough-hide` with Coupled Responses off;
   - `tough-hide` with Coupled Responses on, producing `piercing-bite`.
3. Use opposed-roll combat and record the exact build, scenario/ruleset
   fingerprint, grid, seed set, phase schedule, toggle state, and both loadouts.

**Primary metric:** Fox successful-hit conversion against Hare under opposed
rolls. Keep Hare `PREY`/`pAVI`, Fox `HAT`/`KIL`/`hAVG`, populations, extinctions,
and recovery as diagnostics.

**Pass condition:** The matched aggregate should satisfy:

```text
Tough Hide only < Tough Hide + Piercing Bite <= no-upgrade baseline
```

This means Tough Hide still helps while the Fox response visibly contests it.
Fail if the response produces no measurable direct signal or fully reverses the
player's benefit. A better population average does not override a failed direct
mechanic check.

**Deliverable:** Validated reports, paired same-seed deltas, and a short factual
analysis that separates the mechanic result from any balance recommendation.

**Acceptance gate:** Exact seed coverage, report reconciliation, fingerprints,
acquisition attribution, and toggle identity all pass. The result is reviewed
before adding another response pair.

### CSR-6 - Expand one pair at a time and hand off

**Purpose:** Complete the initial Hare/Fox map without hiding weak interactions
inside a large combination sweep.

**Work:**

- Add the remaining mappings one at a time after the first pair passes its
  functional and evidence gates.
- Give each pair one direct-mechanic question and matched toggle-off/toggle-on
  comparison. Test the upgrade itself independently before combinations.
- Keep the feature experimental even if all mappings work. Production promotion
  is a separate human decision.
- Run the strongest relevant Unity EditMode and PlayMode suites. Perform a
  player-facing check at supported resolutions for checkbox, preview, phase
  summary, and disabled-during-run behavior.
- Write a handoff with changed files, commit/source identity, commands, result
  paths, failures, skipped checks, and remaining balance questions.

**Deliverable:** A complete experimental Hare/Fox response map with trustworthy
functional and evidence status for every pair.

**Acceptance gate:**

- All retained pairs have deterministic lookup and boundary coverage.
- Toggle-off regression, toggle-on replay, report serialization, and UI checks
  pass.
- No plant response, Biome Mutation, Genome shortcut, retired skill, random
  choice, or generic reaction framework entered the change.
- Human review decides whether to keep experimenting, retune a pair, or promote
  any part of the feature.

## Planning concerns

These concerns are accepted as part of this goal pack. The existing
`upgrade-system.md` record remains authoritative for shared upgrade concerns;
do not duplicate its full ledger here.

### CSR-C01 - Coupled Responses could silently redefine Mutations

- **Severity:** Extreme
- **Status:** Acknowledged
- **Trigger:** A non-player response is stored or presented as though it were an
  ordinary selected-species Mutation, is allowed into a Biome Simulation, or is
  confused with the species' active Genome.
- **Why it matters:** This violates the accepted ownership/lifetime contract and
  makes run provenance and balance evidence misleading.
- **Smallest mitigation:** Keep one named, Species-Simulation-only Coupled
  Response layer behind the default-off toggle; update the direction document;
  record both acquisition origins and target species.
- **Owner:** Josh
- **Recorded:** 2026-09-11, accepted in this goal-pack request.

### CSR-C02 - The response could erase the player's choice

- **Severity:** Mild
- **Status:** Acknowledged
- **Trigger:** A paired response fully cancels or reverses the selected Mutation
  across the declared direct-mechanic comparison.
- **Why it matters:** The upgrade would feel cosmetic or punitive instead of
  creating readable pressure.
- **Smallest mitigation:** Grant only one visible authored response level and
  require the three-arm, direct-mechanic gate before expanding the map.
- **Owner:** Josh
- **Recorded:** 2026-09-11, accepted in this goal-pack request.

## Failure handling

| Failure | Required response |
| --- | --- |
| Toggle requested without `bev-experimental` | Reject the settings or batch request; do not silently enable the bundle. |
| Forest Edge lacks Hare or Fox | Reject Coupled Responses for that run; do not substitute a role-wide species. |
| Pair lookup, prerequisite, exclusion, or cap fails | Leave currency, both rule sets, progression histories, and the active run unchanged. |
| Boundary continuation cannot install both rules | Remain at the decision boundary and report the error; never claim a successful purchase. |
| Toggle-off deterministic comparison differs | Stop feature expansion and isolate the regression before interpreting balance. |
| Report omits either acquisition or its origin | Treat the run as unfit for paired-response evidence. |
| Direct mechanic fails while population average improves | Report the mismatch; do not promote or hide it behind a composite score. |
| Unity tests, runtime validation, or visual checks cannot run | State exactly what is unverified; source changes alone are not completion. |

## Explicit non-goals

- Plant or Fern responses and tuning.
- More than the explicit Hare/Fox map above.
- Random, weighted, pressure-scored, rubber-band, or AI-selected responses.
- Mid-tick changes, population spawning, healing, or world resets.
- Scent, pack coordination, inherited traits, shelters, or a generalized
  evolution/modifier/event framework.
- Production promotion, permanent unlocks, active Genome changes, Biome
  Simulation Mutations, or economy redesign.
- Re-enabling retired `keen-senses`.
- Combining multiple response pairs before their independent mechanic checks.

## Copy-paste implementation prompt

```text
Work in F:\ForkBin\GameDev\LearningIndieDev and implement
docs/COUPLED_SPECIES_RESPONSE_GOAL_PACK.md as written.

Execute CSR-0 through CSR-6 in order and stop at each acceptance gate. Recheck
the real branch, dirty worktree, recent history, project guidance, relevant
handoffs, current UI/runtime flow, and active upgrade concerns before editing.
Preserve unrelated untracked files.

The required feature is one default-off "Coupled Hare/Fox Responses" toggle
inside the existing bev-experimental bundle. When enabled, a supported paid
Hare or Fox Mutation grants exactly one deterministic, authored response level
to the opposing species at the same frozen phase boundary. When disabled, the
current behavior and RNG sequence must remain unchanged. Skip grants no
response. Apply and record both sides atomically. Do not add plant work, random
selection, adaptive AI, a general reaction framework, Genome shortcuts, Biome
Mutations, or retired Keen Senses content.

Reuse the existing upgrade catalog, SpeciesProgression, immutable snapshots,
rules dictionary, boundary continuation, report provenance, settings UI, and
test conventions. Use the fewest files and smallest change that satisfies the
pack. Run focused checks after each goal and the strongest relevant Unity suites
before handoff. Do not call source edits, compilation, or old reports runtime
completion. Do not commit, push, launch a large balance sweep, or change shared
external state unless the assignment explicitly authorizes that action.

Finish with a handoff containing changed files, exact commands, test and report
paths, toggle-off parity evidence, toggle-on replay evidence, experiment status,
unverified boundaries, and the next human decision.
```
