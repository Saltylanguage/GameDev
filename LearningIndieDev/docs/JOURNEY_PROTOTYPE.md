# Forest Edge journey prototype

**Field Data design update (2026-10-15):** Round-end income starts at the
surviving population of the player's species: 20 Hares earns 20 Field Data.
The balance carries across the journey for simulation population interventions
and journey upgrades. The reviewed first slice defines income modifiers,
shop reordering and node rewards; random challenges remain future work.
Failure loses all remaining
Field Data, while previously banked Desktop progress remains intact. Permanent
Genome/unlock currency is separate. The first slice discards leftover Field
Data on success-result exit and defers conversion. See the
[complete first-slice design](FIELD_DATA_JOURNEY_DESIGN.md) and
[current economy direction](SCIENTIFIC_DATA_ECONOMY.md#current-field-data-direction--2026-10-15).
The implementation/evidence entries below remain dated prototype records and
do not establish that this economy is implemented or balanced.

**Status (2026-10-06):** Integrated into the canonical Desktop player flow for the first two ecological cycles. Josh accepted this as a playable prototype and deferred further journey work until later in Sprint 4. The next pass aims to polish it for V1 ship readiness. The later map nodes are ScriptableObject placeholders. This implementation does not settle the final journey length, upgrade vocabulary, balance, or reward economy.

**Scrollable layout (2026-10-14):** Josh requested the accepted layout study in Unity, with all three opening choices playable. The authored graph now has 29 markers, 35 directed connections, and 13 encounter stages. The full-screen map scrolls horizontally across a 4,700 × 1,080 reference panorama. Circular icons and hover cards sit over live dirt-trail geometry and a quiet pixel-art forest placeholder. Chrono's final art will replace the panorama through one shared image resource. Later encounters remain non-playable placeholders.

**Design update (2026-10-07):** the team meeting approved upgrade carryover into the next simulation in the journey, retained 600/40/25 as historical research, and assigned the search for more personal event pacing to SimMasterBev. Success is intended to combine reaching tick 600 with viability through tick 700; that criterion remains undefined and is not implemented by this documentation update. DNA/secondary simulator work is deferred. See the [meeting decisions](Meeting%20Briefs/2026-10-07-Journey-and-Ecology-Balance-Agenda.md#meeting-decisions).

**Design update (2026-10-14):** Bevin will guide phase-clear criteria during
initial playtesting; no specific population or event-metric formula is locked.
Failure ends the whole journey with no cash-out. Currency and temporary
upgrades acquired in that run are lost; permanent Desktop upgrades and
unlocked upgrade or feature availability survive. An accomplishment during a
failed run can still unlock permanent availability once those rules are
designed. Field Data carries through its phases and nodes; other currency
types can leave a successful journey for Desktop Genome upgrades. Intended
Field Data purchases include removing Hares, changing Fox and Plant counts in
either direction, and buying a random event. Exact gates, prices, limits, and
the first implementation slice remain open. The 100-rabbit Burrow Genome
unlock is an illustrative example, not an approved condition. See the
[October 14 meeting decisions](Meeting%20Briefs/2026-10-14-Simulation-Focus-and-Journey-Economy-Notes.md#meeting-decisions).

## Player flow

1. Opening Simulation from the Desktop shows the full-screen Forest Edge journey map. The Back button returns to the Desktop. The map can also be reopened during a simulation; doing so pauses a running clock until the player closes it.
2. **Fern Glade**, **Creek Bend**, and **Pine Verge** are available at the start. Each opens a reward popup with the existing three Hare Mutation choices. Claiming one records that opening and makes only its connected first simulation available.
3. Choosing **Canopy Edge**, **Forest Edge**, or **Pine Trail** starts six phases in the same ecosystem. These first simulation nodes use the same scenario and cadence. After phases 1–5, the usual Mutation or Skip choice appears. The starting reward and phase Mutations remain in the run loadout.
4. At the end of phase 6, the cycle report appears. **Return to Journey Map** reveals the connected habitat condition choices. The upper opening leads to Seedfall, the middle opening can choose Seedfall or Fox Tracks, and the lower opening leads to Fox Tracks. Claiming a condition returns to the map and reveals its connected second simulation node.
5. Choosing the second simulation resumes the *same* `SimulationRunState` at phase 7. Cells, population history, seed, clock, and the accumulated Mutation loadout persist. The selected habitat condition changes the relevant species rules for the next tick. Phase Mutations or Skip appear after phases 7–11.
6. Phase 12 shows the final report. **Return to Journey Map** shows the completed route; **Start a New Journey** prepares a fresh expedition at the opening reward node.

The flow above describes the current prototype. It does not yet end the
journey on a failed phase or settle separate permanent currencies on success.

**Approved continuity, provisional vocabulary (2026-10-07):** upgrades acquired during a journey simulation persist into its next simulation. The prototype uses the existing Mutation system for the opening reward and phase choices, and already retains them across nodes until the expedition ends. The meeting approves that inter-simulation carryover; the category name and any persistence into a newly started journey or permanent progression remain separate decisions. The earlier simulation-boundary reset assumption does not apply to this journey flow.

## Authored map and UI

The 29-node, 13-stage graph lives in [`Journey`](../Assets/Data/ProductionData/CellularSimulation/Species/Journey/) ScriptableObjects. `ForestEdgeJourney` stores normalized positions, and `JourneyNodeAsset` stores kind, description, branch connections, availability, condition effect, data reward, and optional Mutation reward IDs. Ten nodes are playable: three starting Rewards, three first Simulations, two Conditions, and two second Simulations. The remaining nineteen nodes are visible placeholders. Each complete visual route reaches 13 nodes; the implemented gameplay still ends after its second simulation.

The map uses the full screen width for its horizontally scrolling trail. The mouse wheel moves along the panorama; the native horizontal scrollbar is also available on hover. The header and footer stay fixed. Nodes show only their type icon, with a gold ring for an available choice and a pulse for the current selection. Hover shows kind, title, state, and description in a card and highlights connected trails. The hover card changes sides near a viewport edge. Encounter popups use the selected node's description and current reward choices.

### Replacing the placeholder artwork

- Author a continuous 5,760 × 1,080 panorama, matching Chrono's art brief. The 1920 × 1080 viewport shows overlapping portions of it. The older 4,700 × 1,080 placeholder stretches across this area until final art arrives.
- The UI uses a 1280 × 720 logical canvas and a 3840 × 520 logical scrolling route area. The panorama is displayed at 3840 × 720 with its top offset by −95 inside that area; the fixed header and footer cover the outer bands.
- The stable resource key is `GalapagOS.Image.Journey.Panorama` in `Assets/UI/ImageResources.xaml`. Its current source is `Assets/UI/GalapagOS/Art/Journey/JourneyPanoramaPlaceholder.png`. Replace its source through the dictionary and preserve asset identity and approved pixel-art import settings.
- Node positions and route geometry remain live UI. Keep icons, labels, state highlights, and hover cards out of the painting. The current trails are drawn by XAML; align any authored trail texture to the same node and connection guide before replacing that layer.
- For a placement from `ForestEdgeJourney`, reference-art coordinates are `x = normalizedCenter.x × 5760` and `y = 142.5 + normalizedCenter.y × 780`.

| Habitat choice | Effect for cycle 2 | Immediate reward |
| --- | --- | --- |
| Seedfall | Plant seed drop chance +0.10 | None |
| Fox Tracks | Fox movement speed +0.5 | 20 field data |

Both condition effects are captured as runtime snapshots when the expedition is prepared. They are examples of environmental pressure, not balanced production numbers.

## Implementation and evidence

Forest Edge/Hare uses 12 phase windows in one run. The boundary at phase 6 holds the run in `AwaitingDecision` while the report, map, and condition popup are shown. Choosing the connected simulation resumes through `ContinueWithBoundaryState`. Non-Journey scenarios retain their six-phase flow, and direct test scenes can still auto-start.

The focused Unity Editor asset test validates the graph, placement, three reward starts, and connected simulation/condition sequence. Play Mode tests verify each trailhead, the Desktop Noesis map and popup, report-to-map transition, same run/cells/history through cycle 2, persistent Mutations, and final reset. Graphics checks cover wheel travel, opening/middle/right views, hover details, and returning to the trailheads after exploration or reset. Current evidence is indexed in the [scrolling map handoff](handoffs/2026-10-14-2105-codex-scrollable-forest-journey-map.md). These tests do not establish ecological balance or final pacing. The current default of 100 ticks per phase yields 1,200 ticks for the two-cycle Forest Edge run.

## Next content decisions

- Name the temporary upgrade category that carries between journey simulations and clear it when that run ends. Keep permanent Desktop upgrades and unlock availability in a separate profile progression category.
- Let Bevin choose and revise the initial phase-clear gates from playtest
  evidence. Record the chosen gate with each test; do not treat the historical
  600/40/25 fixture as a production rule.
- Define the journey failure transition and provisional reward ledger, then
  implement loss of run-earned currency and temporary upgrades on failure,
  preserve permanent Desktop progression, and deposit eligible currency once
  when the journey succeeds.
- Specify Field Data earning, purchase costs and limits, and the first subset
  of approved population controls and random events to make playable.
- Replace the later placeholder nodes with real effects, costs, events, and rewards; extend the runtime node handler beyond the first two cycles.
- Review map readability, choice pacing, and survival across matched seeds before setting a production run length.
- Add explicit transition provenance to exported reports before interpreting changes across cycles as effects of one decision.
