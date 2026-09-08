# Game feature roadmap triage

**Status:** Design triage baseline; proposed windows are not sprint commitments
**Date:** 2026-09-06
**Owner:** Josh
**Use:** Decide which player-facing feature to plan next, one feature at a time

## What this document is for

This is the game-design view of the roadmap. It asks what a feature gives the
player, why it belongs in the game, what must exist before it can work, and
what could stop it from landing. Technical tasks, research tasks and art tasks
only earn priority when they support a clear player outcome.

The feature rubric is:

1. **Player promise:** What will the player be able to feel, understand or do?
2. **Game value:** What decision, tension, expression or replay value does it
   add?
3. **Priority:** How necessary is it for a coherent first playable experience?
4. **Dependencies:** Which design decisions, systems, evidence or assets must
   exist first?
5. **Blockers:** What could make the feature unclear, invalid, too expensive or
   impossible to verify?
6. **Landing window:** When it belongs in the proposed roadmap, assuming its
   dependencies are ready.
7. **Proof:** What would convince us that the feature is working for players?

Priority meanings:

| Priority | Meaning |
| --- | --- |
| Now | Required to make the core expedition understandable and playable. |
| Next | Strengthens the first slice once the core loop is coherent. |
| Later | Valuable after the first slice proves its decisions and readability. |
| Research | Worth exploring, but not ready to become production scope. |

## Current design starting point

The same-world continuation seam now exists in the runtime through the generic
phase/checkpoint work. The player-facing expedition contract is ten phases.
The remaining cadence questions are normal phase duration, decision rhythm,
reward pacing, and terminal outcomes; the prototype currently shows 20-second
phases while the brief describes a longer viewing target.

The first upgrade catalog and boundary choices exist, but upgrade balance,
build identity, species counterplay and player comprehension are still
provisional. The simulation, upgrade rules and biome rules are still a tech
demo/POC; they need a deliberate depth pass before the game can promise a rich
ecology. Phase-aware telemetry and checkpoint infrastructure are available for
validation. EX-010's approved schedule has now been executed and reviewed; its
continued-world finding remains bounded to the tested scenario and upgrade
orders.

The approved progression language is now **Mutation** for temporary
Species-Simulation adaptations and **Genome** for permanently unlocked
per-species options with a configurable active set. Every scenario population
receives its species' active Genome even when it is not player-controlled;
Mutations never enter Biome Simulations. The two modes may use different goals,
while the long-term player objective includes building diverse, resilient
ecosystems. Balance work follows
[`SG-005`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md).

The GalapagOS desktop is also taking shape as the player's between-expedition
home base. Its current feature-set draft includes Overview, Species Collection,
Gene Lab, Biome Lab, History, Expedition Planner, Settings, personalization
and music. That is a player-facing destination, not a replacement for the
simulation or Developer Lab.

## Triaged feature docket

| Order | Feature | Player outcome | Priority | Proposed landing window | Dependencies | Potential blockers | Proof of success |
| ---: | --- | --- | --- | --- | --- | --- | --- |
| 1 | **Expedition decision loop** | The player understands what one expedition is, watches an ecosystem evolve, makes decisions at clear moments, and knows why it ended. | **Now** | M1 closeout / proposed S3 | Confirm ten-phase cadence, phase length, boundary reward timing, End/Restart, and terminal outcomes. | Unclear cadence, unclear reward rhythm, capacity for final design and playtest review. | A complete run can be explained from launch through result without calling a phase a new run. |
| 2 | **Phase decision moment** | At a frozen boundary the player can read what happened, take a Mutation, Skip, End, or Continue with confidence. | **Now** | M1 closeout / proposed S3 | Expedition loop; phase summary; Mutation eligibility; reward and currency meaning. | Too many choices, weak information hierarchy, launch-only effects appearing as dead options. | Players choose intentionally and can predict what will change on the next phase. |
| 3 | **Mutation grammar and build identity** | Choices create recognizable strategies with meaningful tradeoffs instead of a list of small bonuses. | **Now → Next** | Start in M1 closeout; deepen in S4 | Stable Mutation grammar, readable previews, same-world timing, shared capability map, seeded evidence. | Balance is provisional; some effects are initialization-only; art and copy may not communicate tradeoffs. | Trailblazer, Warren and Gardeners feel different and remain understandable over several phases. |
| 4 | **Mutation and Genome tree presentation** | The player can distinguish the temporary path being built during a Species Simulation from permanent Genome unlocks and the currently active Genome of each species. | **Next** | S4 design; S5 presentation pass | Economy and currency rules, stable IDs, species identities, active-capacity rules, UI and art direction. | Similar tree layouts can blur what resets; unlocked and active states can be confused; a tree can overwhelm the player or expose content before its balance and copy are ready. | Players can explain what they are Mutating now, which Genome options remain unlocked, which are active, and why they chose the configuration. |
| 5 | **Cause-and-effect phase summary** | The player can tell what helped, what hurt, and what the next decision is responding to. | **Now → Next** | M1 closeout, then S5 readability pass | Phase/expedition result meanings, event summaries, UI states, visual language. | Stat-Line semantics may be too dense; missing art for danger, recovery or rule activation. | A player can name the main cause of survival or collapse after seeing the summary. |
| 6 | **Economy and currency loop** | The player understands what survival, risk and choices earn, what can fund Mutations now, and what can fund a species' permanent Genome later. | **Now → Next** | Define with the expedition loop; implement with the home-base shell | Phase awards, expedition settlement, banked scientific data, Mutation costs, Genome costs and accomplishment rules. | Reward inflation, weak currency sinks, accidental mixing of temporary and permanent spending, or a reward rhythm that makes choices feel cosmetic. | Players can account for what they earned, choose a meaningful spend, and see why another expedition or species is worth playing. |
| 7 | **Species identity and counterplay** | Hare, Fox and Plant create different pressures and invite different responses. | **Next** | S4 species/scenario co-design | Upgrade vocabulary, Forest Edge evidence, readable ecological roles. | Current balance evidence is descriptive; predator/prey interactions can be seed-sensitive; Sim review capacity. | Each selected species changes which strategies are attractive, not just its starting numbers. |
| 8 | **Risk/reward events and adaptive pressure** | Positive and negative events, competitive species and adaptive responses make every choice feel consequential while preserving player agency. | **Next → Later** | S4 design; S7 validation and implementation | Expedition loop, economy, species counterplay, event vocabulary, explicit response rules and telemetry. | Randomness can hide agency; rubber-banding can feel unfair; event volume and balance require substantial seeded testing. | Players can name the choice that saved or endangered the expedition and understand why pressure changed. |
| 9 | **Species simulation and ecology depth slice** | The ecosystem feels alive through behavior and relationships: time of day, scent, hunting, hiding, ambush, burrows, abilities, cooperation and plant rules. | **Next (large foundational effort)** | S4 design slice; S5–S7 staged implementation | Explicit behavior contracts, scenario pressure, performance budget, telemetry, temporary behavior art and species identities. | The current POC architecture may not support interacting behaviors cleanly; interaction explosion, tuning capacity and art needs can overwhelm a broad feature list. | One focused predator/prey/plant slice produces readable, repeatable decisions before the roster or mechanic list expands. |
| 10 | **Biome mechanics expansion** | A biome supplies distinct rules, resources and hazards that shape the expedition rather than acting as a backdrop. | **Next → Later** | S4 design; S6–S7 implementation | Species identities, upgrade branches, scenario matrix, day/night or weather rules, art language and economy. | Content and balance cost, unclear differences between biomes, and biome rules that cannot be explained at the decision boundary. | A biome changes the viable plans and can be recognized from its pressures and opportunities. |
| 11 | **Scenario pressure** | The environment creates a problem to solve and supports more than one viable plan. | **Next** | S4 | Species identities, upgrade builds, Forest Edge pressure envelope, scenario authoring. | A scenario may reward one dominant build or hide its pressure from the player. | Fixed-seed runs show different viable responses with clear causes and no universal answer. |
| 12 | **Collection and Field Notes** | The player can browse the full collection of species, biomes and other unlocks, see owned and unowned entries, and enjoy learning what each discovery means. | **Next** | S5–S6 | Unlock taxonomy, profile data, History/Data Record, valid discovery evidence, art direction and writing capacity. | Content volume, missing art, invalid or overly technical evidence, and a catalog that feels like a checklist instead of a rewarding archive. | Players return to browse, understand how an entry was discovered, and can distinguish known, seen and still-secret content. |
| 13 | **Expedition setup and Lab context** | The player knows what they are taking into an expedition and why the result matters to the wider game. | **Next** | S5–S6 | Core loop, reward economy, upgrade trees, collection, UI information hierarchy, art direction. | Building the shell before the economy and unlock language are understood; UI capacity and visual acceptance. | Launch and return routes make scenario, species, build and outcome legible without developer controls. |
| 14 | **Readable visual language** | Terrain, species, danger, selection and upgrade consequences are readable at board scale. | **Next** | S5 | Confirmed UI states, board hierarchy, species roles, art direction and temporary asset plan. | Art direction is still being explored; graphics-capable Unity review is constrained by host state; art capacity. | A new player can identify role, threat and selected effect during normal-speed play. |
| 15 | **Audio and simulation feedback** | Frequent ecological events become a clear rhythm of warning, action, consequence and reward rather than noise. | **Next** | S5 | Event vocabulary, visual states, aggregation rules, temporary audio direction. | Noisy event volume, lack of final visual priorities, audio capacity. | Feedback reinforces important decisions and remains readable during dense simulation. |
| 16 | **Species Genome progression** | Completed expeditions permanently unlock Genome options that can be activated or deactivated between simulations without flattening Mutation choices. | **Later** | S6–S7 | Economy, Genome contract, balance reference panel, active-capacity rules, profile shape, settlement rules, stable species/node IDs, UI shell. | Applying Genome only to the player species, confusing unlocks with active effects, runaway active power, unclear data sinks, or save migration cost. | A purchase survives restart; its active state can change between simulations; when active it affects that species in player and background roles and creates an understandable ecological consequence. |
| 17 | **Profile, save/load and cloud sync** | Settings, accomplishments, collection progress and earned progression survive safely between sessions and across supported devices. | **Later (design now)** | Define alongside economy and Labs; implementation after the profile contract | Stable profile schema and IDs, settlement rules, platform account/cloud provider, offline behavior, conflict resolution, migration and corruption handling. | Cloud service or platform API availability, privacy and account edge cases, compatibility testing, and accidental scope creep into active-expedition resume. | A player can save, load, migrate, reset and recover a profile while permanent progress remains correct; cloud conflicts have an explicit player-facing rule. |
| 18 | **Additional species, scenarios and builds** | The game gains replay value through new strategic identities rather than more content of the same shape. | **Later** | S4 design; S7 integration; post-slice expansion | Proven upgrade grammar, species/scenario evidence, art and audio language. | Expanding before the first three builds are distinct; content and balance capacity. | New content creates a new decision pattern and survives representative seeded review. |
| 19 | **Reactive ecology and adaptive counterplay research** | The ecosystem responds to the player's success, creating tension and recovery without hidden catch-up rules. | **Research → Later** | S4 design spike; implementation after slice validation | Species counterplay, pressure telemetry, explicit response rules, evidence envelope. | Rubber-banding can feel unfair or mask balance problems; substantial design and evidence cost. | Players can identify the ecological response and recover through an understandable choice. |
| 20 | **Predictive AI as a design support tool** | Designers receive bounded forecasts about later outcomes without surrendering design authority to the model. | **Research** | After the core loop, economy and Stat-Line meaning are stable | EX-010 bounded evidence, phase-aware metrics, checkpoint replay, fresh validation panel. | General recommendation validity, cross-scenario calibration, insufficient independent evidence, human review capacity. | Forecasts are scored against declared windows and produce a bounded design insight, not a production mechanic. |

The larger **GalapagOS home base** spans features 6, 12, 13 and 16–17. Its scope is
called out here because the desktop should orient the
player, expose permanent progression and launch the next expedition, but it
should not become a second simulation or a collection of unrelated mini-games.

## Design guardrails for the new considerations

These are the distinctions to keep stable while the individual features are
planned:

- **Profile and cloud:** settings, collection, accomplishments and settled
  progression belong to the profile. Active-expedition save/resume is a
  separate player promise and must not appear accidentally through cloud sync.
- **Economy:** name the reward at the moment it is earned, the settlement at
  expedition end, the currency that can influence the current expedition and
  the currency that funds permanent Genomes. A single number should not hide
  four different jobs.
- **Mutation and Genome trees:** Mutations shape one Species Simulation and
  disappear at settlement. Genome nodes stay unlocked, but only the active
  configuration affects every future population of that stable species
  identity. The UI must distinguish locked, unlocked, active, and inactive
  states before a player spends or launches anything.
- **Balance:** use shared capabilities and Adaptation Value estimates to plan,
  then apply the correct Species or Biome scorecard under SG-005. Never use one
  mode's result as the sole approval measure for content in the other.
- **Collection and Field Notes:** an entry can be seen, discovered, owned,
  researched or still hidden. Those states should be intentional and should
  connect to valid evidence rather than merely to a developer toggle.
- **Risk and response:** a positive or negative event earns its place by
  creating a readable decision. Adaptive pressure may challenge a successful
  player, but it cannot quietly erase the meaning of a good choice.
- **Ecology depth:** treat day/night, scent, hunting, hiding, ambushes, burrows,
  abilities, cooperation and plant rules as a system-depth portfolio. Start
  with one vertical slice that proves the interaction language before adding a
  broad roster of mechanics.

## Capacity lens

| Feature group | Capacity shape | Planning implication |
| --- | --- | --- |
| Expedition loop, boundary decision, reward economy | Medium design and playtest effort | Resolve the player contract before spending capacity on content or polish. |
| Economy and currency | Medium-to-large design and balance effort | Define earning, spending, settlement and permanent sinks before adding more unlock content. |
| Upgrade identity and tree presentation | Large cross-system and content effort | Requires design, simulation evidence, UI copy, stable catalog IDs and readable presentation together. |
| Species identity, scenario pressure and reactive ecology | Large evidence and simulation effort | Protect time for fixed-seed playtests and iteration; treat ecology depth as a foundational effort, not a list of isolated mechanics. |
| Biome rules and scenario authoring | Large systemic and content effort | Prove one biome's distinct pressure before multiplying terrain, weather and resource rules. |
| Collection and Field Notes | Large UI, writing and art effort | Build the taxonomy and discovery contract before producing a complete catalog. |
| GalapagOS home base and expedition setup | Large UI/UX and content effort | Plan the smallest navigation, collection view and next-expedition promise first; defer optional apps until the core return path works. |
| Visual language and audio feedback | Medium-to-large art/audio effort | Art and sound need the approved player states and event priorities before production volume is justified. |
| Lab progression, profile, save/load and cloud sync | Large systems, compatibility and platform effort | Keep permanent profile persistence and active-run persistence separate; reserve migration, conflict and failure-handling time. |
| Additional roster and scenarios | Large content and balance effort | Add only after the first three builds create distinct decisions. |
| Predictive AI and research support | Research-heavy, review-limited effort | Schedule only with a frozen question, fresh panel and human review capacity. |

Capacity is a real dependency across the docket. The current plans already
combine design, evidence, UI, art and systems work; selecting one feature means
protecting its review and playtest time while explicitly carrying the others.

## Evidence, Stat-Line and telemetry implications

The new features change what counts as a meaningful result. Stat-Line work
should keep phase results and whole-expedition results separate: a phase window
has its own raw counters, acquisition ticks and partial/invalid states, while
the expedition report describes the accumulated same-world history. A phase
boundary is not a fresh population, fresh energy pool or fresh run unless a
future mechanic explicitly says so. Starting-population and starting-energy
effects remain launch-only until a live-state rule is deliberately designed.
Above-cap energy must retain its signed maximum-energy policy; a clamp or refill
cannot be hidden inside the migration.

The economy, events, ecology and collection features add evidence that the
current telemetry does not yet explain on its own. Future records need stable
identifiers for phase, expedition, species, biome, upgrade branch, currency
source/sink, event cause, collection state and profile settlement. Reports that
count every 20-second phase as a new run, compare every phase to a reset start
state, or treat a partial/invalid window as a completed result will be marked
legacy and excluded from balance claims. Preserve one fresh legacy report as a
versioned fixture so old readers and new continued-world readers can be checked
side by side.

The Stat-Line review should be repeated when the economy, upgrade tree,
adaptive pressure or ecology depth slice changes the player-facing meaning of a
metric. The predictive-AI research must use the same declared windows and
checkpoint semantics, and any schedule that assumes a restart between phases
is invalid. Adaptive events and richer behavior add confounders, so forecasts
remain design-support evidence until a fresh validation panel shows that their
inputs, targets and uncertainty are still understood. Cloud/profile work also
needs telemetry for save, load, migration, conflict and recovery outcomes, but
those records must not be mistaken for simulation outcomes.

## Cross-feature dependencies

Some dependencies affect nearly every feature and should be treated as shared
design constraints:

| Dependency | Features affected | Current state | What must be decided or supplied |
| --- | --- | --- | --- |
| Expedition shape | 1–6, 8, 11, 13, 16–20 | Same-world runtime exists and the ten-phase contract is established. | Choose the normal duration and decision rhythm. |
| Mutation / Genome grammar | 2–4, 6–11, 13, 16, 18 | Seven Hare Mutation assets are authored; active Genome configuration and scalable balance are planned. | Define shared capabilities, provisional value budgets, mode boundaries, effect contracts, stacking, timing, and the direct and ecological evidence needed for approval. |
| Reward/data economy | 1–6, 8, 12–13, 16–17 | Phase survivor data exists; permanent economy remains deferred. | Separate phase rewards, final settlement, temporary choice currency, banked data and permanent unlocks. |
| Profile identity and cloud contract | 12, 13, 16–17 | Profile persistence is not the active-expedition contract. | Decide stable IDs, account/offline behavior, conflict resolution, migration, reset and recovery before promising cross-device continuity. |
| Mutation / Genome tree language | 3–4, 6–7, 10, 12–13, 16 | Current Mutation catalog is authored; Genome activation and tree semantics are not final. | Define temporary versus permanent branches, prerequisites, exclusions, stack/order rules, active-capacity rules, per-species Genome application, and locked/unlocked/active language. |
| Collection and discovery taxonomy | 8, 10–13, 16–18 | History and data records exist as related concepts. | Define what counts as seen, discovered, owned, researched and secret, and which evidence can unlock an entry. |
| Ecology behavior contracts | 7–11, 14–15, 18–19 | Current species simulation is still POC-level. | Choose a depth slice, interaction vocabulary, performance budget and telemetry needed to explain behavior. |
| Cause-and-effect language | 2, 5, 7–11, 14–15, 20 | Phase-aware telemetry and Stat-Line output exist; final meaning review remains. | Choose the small set of facts the player must see and the deeper facts kept for the Lab. |
| Visual readability | 2–5, 7–15, 18 | Art/UI direction is active; graphics acceptance is not fully closed. | Lock board hierarchy, role recognition, danger, selection and upgrade feedback. |
| Capacity and review bandwidth | All | Current plans exceed a single small sprint when design, evidence and art are combined. | Protect one primary feature outcome per planning block and explicitly carry the rest. |
| Research boundary | 8–11, 19–20 | EX-010 is accepted as bounded continued-world evidence; broader predictive claims remain research-only. | Keep research questions separate from player-facing production promises. |

## Proposed landing sequence

This is a sequence of design outcomes, not a pre-filled sprint schedule:

1. **Define the Expedition Decision Loop.** Confirm the locked ten-phase
   cadence, boundary choices, rewards, End, Restart and terminal outcomes.
2. **Make the boundary decision satisfying.** Shape the phase summary, offer
   structure, Skip, upgrade previews and the player's next action.
3. **Define the economy and currency promise.** Decide what is earned during a
   phase, what settles at expedition end, what can fund Mutations immediately,
   and what funds permanent Genomes.
4. **Prove three Mutation identities and the balance method.** Use the existing
   catalog and Forest Edge to define the shared capability map, reference
   panel, and provisional value estimates; then test reachable paths,
   acquisition timing, matchups, and ecosystem consequences.
5. **Give species, biome and scenario pressure readable teeth.** Co-design
   Hare, Fox, Plant and the Forest Edge problem around those builds, starting
   with one focused ecology depth slice.
6. **Make risk and response feel earned.** Add positive/negative events and
   adaptive pressure only when players can identify the choice that caused the
   expedition to recover or fail.
7. **Make the experience readable.** Lock visual language, collection/Field
   Notes structure, UI hierarchy, feedback and audio priorities around the
   decisions that survived testing.
8. **Add the wider home-base shell.** Connect setup, settlement, per-species
   Genome progression, profile/cloud persistence and the next-expedition
   promise.
9. **Expand only after the slice proves itself.** Add more species, biomes,
   scenarios, reactive ecology and research-supported systems from evidence
   rather than from a desire to fill the roadmap.

## First feature to plan in detail

Start with **The Expedition Decision Loop**. Its feature brief should answer
the player's questions in order:

- What am I trying to keep alive?
- How long is one phase, and how do the ten phases make an expedition?
- What can change at a boundary, and what survives unchanged?
- What did I earn, and what can I spend it on?
- What does Skip, End or Restart mean?
- How do I know whether I won, narrowly survived or lost?
- Why should I start another expedition?

Do not detail the next feature until this brief has a stable player-facing
answer. The technical continuation, Stat-Line and predictive-AI work support
this feature; they do not decide its fantasy or pacing.

## What is deliberately outside the immediate feature queue

Active-expedition disk save/load and resume, a generalized modifier or plugin
framework, broad species/biome expansion, full reactive ecology, predictive
model promotion and final-volume art are all deferred until the earlier player
decisions have earned them a place in the slice. Profile save/load and cloud
sync remain on the roadmap, but they cover settled progression, collection and
settings; they do not silently define an active-expedition resume rule.

Related plans: [production roadmap](../ROADMAP.md),
[future sprint horizon](FUTURE_SPRINT_ROADMAP.md),
[continuous simulation plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md),
[Main Menu and Lab plan](MAIN_MENU_LAB_DELIVERY_PLAN.md), and
[Stat-Line/predictive-AI impact](CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md).
