# Planning concerns — Simulation Window

**Scope:** GalapagOS-styled Noesis presentation of the cellular simulation,
including window chrome, board composition, player flow, developer-surface
separation, and the Unity/Noesis composition seam. This does not change
simulation rules, continuation mechanics, telemetry meaning, persistence, or
the Lab's feature ownership.
**Canonical plans:** [`../UNITY_MVVM_ARCHITECTURE_PLAN.md`](../UNITY_MVVM_ARCHITECTURE_PLAN.md),
[`../UNITY_MVVM_UI_CONTRACTS.md`](../UNITY_MVVM_UI_CONTRACTS.md), and
[`../CONTINUOUS_SIMULATION_FLOW_PLAN.md`](../CONTINUOUS_SIMULATION_FLOW_PLAN.md)
**Human owner:** Josh
**Status:** Active

## Active concerns

### SIMWIN-C01 — Unfinished expedition close policy

- **Severity:** Mild
- **Status:** Accepted for first production slice
- **Trigger:** The window close affordance becomes enabled for an unfinished
  expedition without an explicit confirmation and End contract.
- **Why it matters:** Active expeditions are not restored from player disk save;
  an accidental close could abandon a run, while a no-op close button would make
  the production shell misleading.
- **Smallest mitigation:** Disable close during Running, Paused, and
  AwaitingDecision; Ready and Results may return to the Lab. A later
  confirmation path must use explicit End semantics rather than Stop or Restart.
- **Owner:** Josh
- **Recorded:** 2026-09-05, Josh accepted the disabled-while-active policy for
  the first production slice. The decision is covered by the Lab-to-Simulation
  Play Mode flow test.

### SIMWIN-C02 — Duplicate Noesis composition ownership

- **Severity:** Mild
- **Status:** Open
- **Trigger:** The host and ViewModels both bind views, discover simulation
  objects, or locate the board through `Find*`/`FindName` on the normal path.
- **Why it matters:** Binding order becomes implicit and the same feature can
  silently acquire more than one composition path.
- **Smallest mitigation:** Make `SpeciesSimulationNoesisHost` the sole normal
  composition root, keep serialized references authoritative, remove the
  ViewModel discovery fallbacks after focused scene tests pass, and leave
  compatibility code only when its reason is documented.
- **Owner:** Josh
- **Recorded:** 2026-09-05, user-confirmed plan.

### SIMWIN-C03 — Window chrome must not own simulation lifecycle

- **Severity:** Extreme
- **Status:** Open
- **Trigger:** Opening, closing, hiding, resizing, or visual-state changes
  rebuild the runner, replace the retained world, advance a tick, settle a
  reward twice, or otherwise make XAML the authority for continuation state.
- **Why it matters:** The same-world continuation and deterministic evidence
  objective would fail even if the window looked correct.
- **Smallest mitigation:** Keep lifecycle ownership in the simulation manager
  and helper; route all player commands through explicit APIs; retain the same
  run through phase decisions; and cover window-state transitions with the
  existing same-run, one-decision, and no-phantom-tick tests.
- **Owner:** Josh
- **Recorded:** 2026-09-05, user-confirmed plan; aligned with CF-C01 and CF-C03
  in the continuous-simulation plan.

### SIMWIN-C04 — Developer and research diagnostics must not leak into player UI

- **Severity:** Mild
- **Status:** Open
- **Trigger:** Raw tuning fields, experimental Stat-Line output, predictive
  scoring, or research-only controls become part of the normal player window.
- **Why it matters:** The player surface would imply unsupported balance or
  evidence claims and the first slice would grow into an analytics framework.
- **Smallest mitigation:** Keep developer controls explicitly gated or move
  them to a separate developer surface. The player window consumes read-only
  run, phase, upgrade, and result projections; research scoring remains outside
  the player UI.
- **Owner:** Josh and Sim for stat meaning
- **Recorded:** 2026-09-05, user-confirmed plan; aligned with SPAI-C08 through
  SPAI-C10.

## Accepted decision

**Disabled while active:** the simulation close affordance is disabled during
Running, Paused, and AwaitingDecision. It is enabled only when the preview is
Ready or Results and the loaded profile can return to the Lab.

If a confirmation path is added later, it must be a separate explicit End flow;
active close must never map directly to Stop or Restart.
