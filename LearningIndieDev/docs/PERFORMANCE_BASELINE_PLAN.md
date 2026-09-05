# Performance baseline plan

Status: **measurement protocol proposed; no product budget accepted**

## Purpose

The [consecutive-phase migration](CONTINUOUS_SIMULATION_FLOW_PLAN.md) adds a
required measurement case: a complete five-phase expedition retaining history,
events, tracked entities and prior-grid state across decision breaks. Compare
tick cost, boundary/report latency and memory across the whole supported horizon.
An old 20-second fresh-window profile is not a continued-expedition budget.

Establish a reproducible Windows player baseline before optimization or new
profiling dependencies. Until minimum supported hardware is selected, results
are machine baselines rather than product requirements.

## Provenance

Every capture records CPU, GPU, RAM, OS, power mode, display resolution, Unity
version, Git commit, graphics settings, build configuration, scenario asset and
fingerprint, seed, simulation speed, and capture duration. Use a Development
Player connected to Unity Profiler; do not use deep profiling for recorded runs.

## Capture matrix

After a 30-second warmup, capture three 120-second samples for each case:

1. Forest Edge 32x32 at normal speed and 1920x1080.
2. The same scenario and seed at 1280x720.
3. A 128x128 high-population characterization run at the fastest supported
   simulation speed.

The first two represent the vertical slice. The stress case identifies scaling
pressure and is not a pass/fail gate.

## Metrics

- P50, P95, and P99 frame time.
- Main-thread and render-thread time; CPU/GPU-bound classification.
- GC allocation per frame and allocation spikes.
- Total and managed memory, including ten-minute growth behavior.
- Simulation tick duration.
- Noesis and board-render duration when separable.

Use Frame Debugger only when render submission is implicated. Add
`ProfilerMarker`s only after a baseline leaves an important region opaque; the
first likely boundaries are `SpeciesSimulationRunner.AdvanceOneTick` and
`SpeciesSimulationBoard.OnRender`.

## Provisional review thresholds

These require product/technical approval before becoming gates:

- Representative P95 frame time at or below 16.67 ms.
- Simulation-tick P95 at or below 8 ms.
- No recurring steady-state allocation on unchanged presentation frames;
  report actual allocation on simulation and render-update frames.
- No monotonic memory growth during a ten-minute representative run.

Do not add performance-test, analysis, or memory packages until baseline
variance demonstrates that automation or a specialized tool will answer a
specific recurring question.
