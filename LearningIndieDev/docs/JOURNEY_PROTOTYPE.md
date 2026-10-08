# Forest Edge journey prototype

**Status (2026-10-06):** Integrated into the canonical Desktop player flow for the first two ecological cycles. The later map nodes are ScriptableObject placeholders. This implementation does not settle the final journey length, upgrade vocabulary, balance, or reward economy.

## Player flow

1. Opening Simulation from the Desktop shows the full-screen Forest Edge journey map. The Back button returns to the Desktop. The map can also be reopened during a simulation; doing so pauses a running clock until the player closes it.
2. The only available opening node is **First Discovery**. Clicking it opens a reward popup with three authored Hare Mutation choices. Claiming one returns to the map. The first simulation node then becomes available.
3. Choosing **Forest Edge** starts six phases in the same ecosystem. After phases 1–5, the usual Mutation or Skip choice appears. The starting reward and phase Mutations remain in the run loadout.
4. At the end of phase 6, the cycle report appears. **Return to Journey Map** reveals the two habitat condition branches. Clicking either opens a popup with its effect and field-data reward. Claiming it returns to the map and reveals the connected second simulation node.
5. Choosing the second simulation resumes the *same* `SimulationRunState` at phase 7. Cells, population history, seed, clock, and the accumulated Mutation loadout persist. The selected habitat condition changes the relevant species rules for the next tick. Phase Mutations or Skip appear after phases 7–11.
6. Phase 12 shows the final report. **Return to Journey Map** shows the completed route; **Start a New Journey** prepares a fresh expedition at the opening reward node.

**Temporary Mutation rule:** For this prototype, the opening node reward and all phase choices use the existing Mutation system. Mutations last until the expedition ends, including across simulation nodes. This differs from the intended glossary in which phase Mutations reset after a simulation and Genome upgrades persist between runs. The durable *within-run* upgrade category and its inheritance rules still need a separate design decision.

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

- Name and implement the within-run upgrade category, then decide whether and how phase Mutations reset at each simulation boundary.
- Replace the later placeholder nodes with real effects, costs, events, and rewards; extend the runtime node handler beyond the first two cycles.
- Review map readability, choice pacing, and survival across matched seeds before setting a production run length.
- Add explicit transition provenance to exported reports before interpreting changes across cycles as effects of one decision.
