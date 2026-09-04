# Stat-line and Predictive AI — working session with Sim

- **Scope:** How the species stat-line work and Predictive AI work can share
  data, help each other, or get in each other's way. This does not approve
  changes to simulation behavior, player UI, or the stat-line contract.
- **Canonical plans:** [Sprint 1 species stat line](../SPRINT_1_SPECIES_STAT_LINE_TICKETS.md)
  and [AI-assisted ecology lab](../Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md)
- **Human owners:** Josh and Sim
- **Status:** Active

## Short version

The two workstreams fit together well:

- Sim's stat line gives us clear, repeatable facts about what happened.
- Predictive AI can use those facts to make forecasts and check how well those
  forecasts hold up.
- Prediction failures can also expose unclear stat names, missing context, or
  weak instrumentation.

The clean boundary is:

> The simulation produces the stat line. Predictive AI reads it after the run.
> Scoring does not change the simulation, the stat meanings, or the original
> evidence.

## Where we can help each other

- Clear stat definitions give the AI something fair and stable to predict.
- The AI work gives the stat line another practical use beyond display and
  manual comparison.
- Shared seed, scenario, time-window, and ruleset details make both sides easier
  to reproduce.
- Prediction mistakes can show us where a stat name or explanation is unclear.
- One trusted report format means neither side needs its own version of the same
  calculation.
- Better forecasts could eventually help us choose useful follow-up experiments,
  without turning the AI into the final design decision-maker.

## Active concerns

### SPAI-C01 — Keep scoring out of the simulation

- **Severity:** Extreme
- **Status:** Open
- **Trigger:** Prediction scoring is added to simulation stepping, metric
  collection, or other code that can affect a run.
- **Why it matters:** The evaluator could change the behavior or timing of the
  thing it is meant to measure. That would weaken determinism and trust in the
  results.
- **Evidence:** The project treats deterministic replay and unchanged simulation
  results as core stat-line requirements.
- **Smallest mitigation:** Score completed report files in Editor or research
  tooling. Do not write scoring results back into run state.
- **Owner:** Josh and Sim

### SPAI-C02 — Agree on what every scored stat means

- **Severity:** Extreme
- **Status:** Open
- **Trigger:** A forecast uses a stat whose name, unit, sign, time window, or
  denominator is unclear or has changed.
- **Why it matters:** We could score the wrong outcome and still produce a result
  that looks official. The earlier `PREY` mix-up is the concrete example.
- **Evidence:** [EX-007 analysis](../Research/Experiments/EX-007-Predictive-Statline-Interventions/AI_ANALYSIS.md)
  records that `PREY` was first read as a food/resource event instead of Hare
  deaths caused by carnivores.
- **Smallest mitigation:** Give each scored stat a stable name, plain-English
  meaning, unit, sign convention, time window, and contract version.
- **Owner:** Sim for stat meaning; Josh for how the prediction refers to it

### SPAI-C03 — Do not rewrite old predictions or reports

- **Severity:** Extreme
- **Status:** Open
- **Trigger:** We add new probabilities, interval meanings, baselines, or metric
  interpretations to an old prediction after seeing its outcome, or edit a
  completed report in place.
- **Why it matters:** That would turn a test into a post-hoc explanation and
  damage the historical evidence trail.
- **Evidence:** EX-007 was preregistered, but its broad confidence values and
  effect bands were not written as a formal scoring contract.
- **Smallest mitigation:** Keep EX-007 as a pilot. Start formal scoring with new
  preregistered forecasts and save evaluation as a separate versioned file.
- **Owner:** Josh

### SPAI-C04 — Compare the same kind of result

- **Severity:** Extreme
- **Status:** Open
- **Trigger:** We compare different seeds, scenarios, run lengths, stat versions,
  or different targets such as a per-seed result versus a five-seed average.
- **Why it matters:** We could mistake an easier test case for a better forecast,
  or seed variation for an intervention effect.
- **Evidence:** EX-009 exists because the earlier upgrade-order runs did not use
  the same held-out seeds.
- **Smallest mitigation:** Each prediction states whether it targets a raw value,
  a same-seed change from baseline, a per-seed range, or a panel average. Direct
  comparisons use the same cases.
- **Owner:** Josh

### SPAI-C05 — Do not let one score become the verdict

- **Severity:** Extreme
- **Status:** Open
- **Trigger:** One combined number is used to decide that the AI is good, that a
  species is strong, that an intervention caused an outcome, or that a change is
  good for the game.
- **Why it matters:** A single number can hide which stats failed. Prediction
  accuracy also does not prove cause, balance, or fun.
- **Evidence:** The research plan keeps simulation evidence, causal claims,
  design judgment, and human decisions separate.
- **Smallest mitigation:** Show results by stat first. Keep any combined score
  secondary, and keep causal and design decisions in their existing review path.
- **Owner:** Josh and Sim

### SPAI-C06 — Report missing and invalid results honestly

- **Severity:** Extreme
- **Status:** Open
- **Trigger:** `NotApplicable`, invalid, unreconciled, or zero-denominator results
  are quietly changed to zero or dropped after we see them.
- **Why it matters:** Hard cases would disappear and make the AI look more
  accurate than it is.
- **Evidence:** The current experimental stat line already carries explicit
  validity states for derived metrics.
- **Smallest mitigation:** Decide the handling before the run, preserve the stat
  status, and list every excluded result with its reason.
- **Owner:** Sim for validity states; Josh for evaluation handling

### SPAI-C07 — Do not claim calibration from one experiment

- **Severity:** Extreme
- **Status:** Open
- **Trigger:** We say the AI's confidence is calibrated from EX-007 alone, or we
  keep tuning against the same held-out seeds.
- **Why it matters:** A few checks cannot show that a stated 60% confidence means
  60%, and reused held-out data stops being genuinely held out.
- **Evidence:** The EX-007 report already calls its confidence result an early
  observation rather than proof of calibration.
- **Smallest mitigation:** Build calibration history from many new,
  preregistered forecasts and rotate in fresh seed or scenario panels.
- **Owner:** Josh

### SPAI-C08 — Keep the first scoring pass small

- **Severity:** Mild
- **Status:** Open
- **Trigger:** The first pass grows into new dashboards, player UI, several score
  families, large data stores, or a broad statistics framework.
- **Why it matters:** It could delay the stat-line work and leave both sides with
  more code to maintain.
- **Evidence:** The stat-line plan calls for the smallest useful review surface,
  and the AI research plan warns against turning one experiment into a general
  framework.
- **Smallest mitigation:** Start with one prediction file, one evaluation file,
  Brier score for yes/no events, CRPS for numeric outcomes, and focused tests.
- **Owner:** Josh

### SPAI-C09 — Keep one source of truth

- **Severity:** Mild
- **Status:** Open
- **Trigger:** The UI, report writer, and prediction evaluator each calculate
  their own version of the same stat.
- **Why it matters:** The numbers could disagree depending on where we look.
- **Evidence:** The stat-line plan says JSON remains the machine-readable source
  and presentation should stay a thin adapter.
- **Smallest mitigation:** Predictive AI reads the serialized stat and status. It
  does not rebuild Sim's formulas.
- **Owner:** Josh and Sim

### SPAI-C10 — Keep research scoring out of player UI for now

- **Severity:** Mild
- **Status:** Open
- **Trigger:** Forecast losses, confidence ranges, or an AI-quality score are
  added beside the player-facing stat line without a separate design decision.
- **Why it matters:** Players could read a research diagnostic as a species
  rating or build recommendation.
- **Evidence:** The current stat-line and research plans keep presentation,
  simulation evidence, and design decisions separate.
- **Smallest mitigation:** Keep the first scoring output in developer/research
  reports only.
- **Owner:** Josh and Sim

### SPAI-C11 — Keep ownership clear

- **Severity:** Mild
- **Status:** Open
- **Trigger:** Sim's stat-line work becomes responsible for prediction policy,
  calibration, baselines, or research reporting.
- **Why it matters:** The stat-line scope would grow and ownership could become
  muddy.
- **Evidence:** The two plans currently describe separate delivery and research
  lanes.
- **Smallest mitigation:** Sim owns stat meaning and trustworthy telemetry. Josh
  owns forecasts, scoring, and evaluation reports. Shared contract changes are
  discussed together.
- **Owner:** Josh and Sim

### SPAI-C12 — Use fair baselines and keep context visible

- **Severity:** Mild
- **Status:** Open
- **Trigger:** The AI is compared only with a weak baseline, or results from very
  different ecological situations are averaged into one number.
- **Why it matters:** The AI could look useful without beating a simple forecast,
  and one good scenario could hide a bad one.
- **Evidence:** Current research reports already preserve scenario, seed, and
  held-out context.
- **Smallest mitigation:** Compare against no-effect and simple historical
  forecasts, and show results by scenario and intervention before any overall
  summary.
- **Owner:** Josh

### SPAI-C13 — Leave room for informal ideas

- **Severity:** Mild
- **Status:** Open
- **Trigger:** Every early hypothesis is required to include a complete formal
  probability forecast.
- **Why it matters:** Useful exploration could become slow or feel too expensive
  to record.
- **Evidence:** The research plan separates candidate ideas from promoted,
  held-out predictions.
- **Smallest mitigation:** Keep two lanes: informal hypotheses for exploration
  and preregistered forecasts for formal scoring.
- **Owner:** Josh and Sim

## Existing overlap to settle: RFS and APS

The experimental herbivore stat line already calculates and displays `RFS` and
`APS`, which act like combined scores. The newer Sprint 1 plan says the first
general stat line should not introduce an MVP/WAR-style overall species rating.

This does not mean `RFS` and `APS` must be removed. We should agree whether they
are:

- temporary experimental fields;
- useful derived stats with narrow, explicit meanings;
- intended overall species ratings; or
- fields to leave out of the future general stat-line contract.

The Predictive AI score should not quietly become another species rating while
that question is open.

## Suggested first pass

1. Leave Sim's current runtime behavior and stat formulas unchanged.
2. Freeze the meaning and version of the stats we want to predict.
3. Write future predictions in a small preregistration file.
4. Score completed held-out reports in a separate tool.
5. Save a separate evaluation file that points back to the original evidence.
6. Show per-stat results and simple reference forecasts.
7. Keep the output in research reports until we make a separate player-facing
   decision.

## Decisions for us to make together

- [ ] Who owns the shared stat-definition contract?
- [ ] Which current stats are stable enough to forecast?
- [ ] Are `RFS` and `APS` staying, changing, or remaining experimental only?
- [ ] Is the forecast target usually a same-seed change from baseline, a raw
      value, or both?
- [ ] What belongs in the stat-line report versus the separate evaluation file?
- [ ] Which simple baselines should every AI forecast have to beat?
- [ ] How many new predictions do we want before discussing calibration?
- [ ] What, if anything, should eventually be visible to players?

## Meeting notes

Use this section for the decisions we actually make. Update concern status rather
than deleting old concerns so the reasoning stays easy to follow.
