# Forest Edge journey prototype

**Status (2026-10-06):** Integrated into the canonical Desktop player flow for the first two ecological cycles. The later map nodes are ScriptableObject placeholders. This implementation does not settle the final journey length, upgrade vocabulary, balance, or reward economy.

**Design update (2026-10-07):** the team meeting approved upgrade carryover into the next simulation in the journey, retained 600/40/25 as historical research, and assigned the search for more personal event pacing to SimMasterBev. Success is intended to combine reaching tick 600 with viability through tick 700; that criterion remains undefined and is not implemented by this documentation update. DNA/secondary simulator work is deferred. See the [meeting decisions](Meeting%20Briefs/2026-10-07-Journey-and-Ecology-Balance-Agenda.md#meeting-decisions).

## Player flow

1. Opening Simulation from the Desktop shows the full-screen Forest Edge journey map. The Back button returns to the Desktop. The map can also be reopened during a simulation; doing so pauses a running clock until the player closes it.
2. The only available opening node is **First Discovery**. Clicking it opens a reward popup with three authored Hare Mutation choices. Claiming one returns to the map. The first simulation node then becomes available.
3. Choosing **Forest Edge** starts six phases in the same ecosystem. After phases 1–5, the usual Mutation or Skip choice appears. The starting reward and phase Mutations remain in the run loadout.
4. At the end of phase 6, the cycle report appears. **Return to Journey Map** reveals the two habitat condition branches. Clicking either opens a popup with its effect and field-data reward. Claiming it returns to the map and reveals the connected second simulation node.
5. Choosing the second simulation resumes the *same* `SimulationRunState` at phase 7. Cells, population history, seed, clock, and the accumulated Mutation loadout persist. The selected habitat condition changes the relevant species rules for the next tick. Phase Mutations or Skip appear after phases 7–11.
6. Phase 12 shows the final report. **Return to Journey Map** shows the completed route; **Start a New Journey** prepares a fresh expedition at the opening reward node.

**Approved continuity, provisional vocabulary (2026-10-07):** upgrades acquired during a journey simulation persist into its next simulation. The prototype uses the existing Mutation system for the opening reward and phase choices, and already retains them across nodes until the expedition ends. The meeting approves that inter-simulation carryover; the category name and any persistence into a newly started journey or permanent progression remain separate decisions. The earlier simulation-boundary reset assumption does not apply to this journey flow.

## Authored map and UI

The 18-node, 12-stage graph lives in [`Journey`](../Assets/Data/ProductionData/CellularSimulation/Journey/) ScriptableObjects. `ForestEdgeJourney` stores normalized positions, and `JourneyNodeAsset` stores kind, description, branch connections, availability, condition effect, data reward, and optional Mutation reward IDs. The first six nodes are playable: one starting Reward, one opening Simulation, two Conditions, and two connected Simulation nodes. The remaining twelve nodes are visible placeholders, not selectable content. Each complete visual route reaches 12 nodes.

The full Desktop canvas has seasonal and continuous-summer backdrops. Node icons and curved route lines are XAML overlays over reusable artwork. Labels appear on hover, and hover focuses the connected route and fills the right field journal from the node asset. Selecting a playable node changes the journal and adds a pulsing highlight. The popup uses the selected node's authored description and current reward choices. Its copy and styling are still prototype quality.

| Habitat choice | Effect for cycle 2 | Immediate reward |
| --- | --- | --- |
| Seedfall | Plant seed drop chance +0.10 | None |
| Fox Tracks | Fox movement speed +0.5 | 20 field data |

Both condition effects are captured as runtime snapshots when the expedition is prepared. They are examples of environmental pressure, not balanced production numbers.

## Implementation and evidence

Forest Edge/Hare uses 12 phase windows in one run. The boundary at phase 6 holds the run in `AwaitingDecision` while the report, map, and condition popup are shown. Choosing the connected simulation resumes through `ContinueWithBoundaryState`. Non-Journey scenarios retain their six-phase flow, and direct test scenes can still auto-start.

The focused Unity Editor asset test validates the graph, placement, reward start, and connected simulation/condition sequence. Play Mode tests verify the Desktop Noesis map and popup, report-to-map transition, same run/cells/history through cycle 2, persistent Mutations, and final reset. Game-view captures of the map and opening popup are in the 2026-10-06 visual-evidence artifacts. These tests do not establish ecological balance or final pacing. The current default of 100 ticks per phase yields 1,200 ticks for the two-cycle Forest Edge run.

## Next content decisions

- Name the upgrade category that carries between journey simulations. Preserve the approved carryover; separately decide any reset at a newly started journey or permanent progression boundary.
- Replace the later placeholder nodes with real effects, costs, events, and rewards; extend the runtime node handler beyond the first two cycles.
- Review map readability, choice pacing, and survival across matched seeds before setting a production run length.
- Add explicit transition provenance to exported reports before interpreting changes across cycles as effects of one decision.
