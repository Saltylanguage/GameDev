# What We Have Learned About AI-Assisted Game Development

**Status:** Draft retrospective for human review<br>
**Date:** 2026-09-12<br>
**Scope:** The current `LearningIndieDev` workflow, with emphasis on Unity
development, simulation research, project continuity, evidence handling, and
human–AI collaboration.<br>
**Decision boundary:** This document describes observed practice and proposes
next steps. It does not approve a new studio policy, production feature, or
research claim.

## Executive conclusion

We are no longer using AI only as a faster way to write code. We are building a
small, evidence-oriented development system around it.

The strongest part of the workflow is the separation between what the game did,
what an AI thinks it means, and what a human decides to do next. Deterministic
runs, stable identities, fingerprints, report bundles, held-out comparisons,
bounded claims, and explicit human decision records give the work a level of
traceability that is uncommon in ordinary prototype development.

The largest weakness is that we have measured the simulation and its evidence
more carefully than we have measured the AI-assisted workflow itself. We can
show that bounded experiments reproduce and that some AI predictions can be
scored. We cannot yet show that AI consistently saves time, improves design
decisions, reduces defects, or produces better player experiences. The amount
of documentation and raw evidence has also grown faster than the mechanisms for
keeping it current and easy to navigate.

The most interesting opportunity is the combination of three layers:

```text
AI-assisted development
    -> auditable simulation research
    -> player-facing ecology explanation and experimentation
```

None of those ingredients is new by itself. The potentially distinctive idea is
that the same evidence spine could support all three without allowing AI output
to become the source of truth. That is a credible research and production
direction. It is not yet a proven novelty or intellectual-property claim.

## Evidence snapshot

This retrospective is based on the repository state on 2026-09-12, including:

| Observed repository evidence | Current snapshot | What it suggests |
| --- | ---: | --- |
| Dated handoff notes | 114 | Continuity and traceability are taken seriously, but retrieval cost is becoming material. |
| Versioned experiment packages | 7 | The research loop has moved beyond a one-off demo. |
| Repository-local workflow skills | 7 | Repeated failure modes are beginning to become reusable operating practice. |
| Active Studio Guidelines | 6 | Human expectations are being turned into durable, reviewable policy. |
| Pre-cleanup artifact directories | 335 / 5.95 GB | The evidence system is real enough to create operational data-lifecycle problems. |
| CellSim reports summarized in the retention audit | 72 | Compact context can be derived without pretending it replaces raw evidence. |
| EX-010 runs | 50 total across two sequences and two seed panels | Continued-world upgrade timing can be compared under an approved, reproducible contract. |

The current working tree is also a useful warning about status semantics. It
contains active Genome implementation, UI, test, documentation, scene, and
handoff changes that are not represented by the current `HEAD` commit. A handoff
can accurately describe local work while the branch is still not shareable.
“Documented,” “verified,” “committed,” “pushed,” “reviewed,” and “accepted” must
continue to mean different things.

## What we are doing well

### 1. We treat the repository as shared memory, not the conversation

The project has a deliberate memory hierarchy:

- [Project context](PROJECT_CONTEXT.md) stores durable product and architecture
  direction.
- [Working state](WORKING_STATE.md) provides a current entry point.
- [Handoff notes](handoffs/README.md) preserve task-sized historical state.
- Git and the checked-out code remain authoritative for implementation.
- Experiment bundles preserve runtime evidence.

This is a strong answer to one of the hardest practical problems in AI-assisted
development: an agent's conversation is temporary, partial, and often different
from another contributor's context. The workflow extracts decisions,
constraints, ownership, verification, and next actions into artifacts that a new
human or AI session can inspect.

The [collaboration workflow](COLLABORATION_WORKFLOW.md) also defines how to
resolve disagreement between those layers. Product intent comes from the project
context; implementation state comes from code and Git; a handoff is a dated
claim that may lag. That explicit precedence prevents a plausible summary from
quietly overriding reality.

### 2. We separate facts, interpretation, and authority

The strongest studio pattern is:

```text
Experiment -> Run -> Report -> Analysis -> Human Decision
```

[SG-001](Studio%20Guidelines/AI_GENERATED_REPORTS.md) assigns a different purpose
to each stage. Runs are immutable evidence. Reports state what happened. AI
analysis is derived and versioned separately. A named human decides what the
result authorizes.

This separation survived a real mistake. In EX-007, the AI correctly predicted
several directional effects but incorrectly predicted reduced starvation and
misread the `PREY` metric. The project did not relabel the outcome afterward.
The metric-definition error was excluded from causal scoring and preserved in
the [P3 cohesive report](Research/Experiments/P3-Predictive-AI-Cohesive-Report.md).
That is excellent practice: the workflow made an AI error inspectable instead of
allowing a polished narrative to erase it.

The human decisions for [EX-009](Research/Experiments/EX-009-Same-Heldout-Order-Comparison/HUMAN_DECISION.md)
and [EX-010](Research/Experiments/EX-010-Sequential-Upgrade-Continuation/HUMAN_DECISION.md)
are equally careful. They accept only the declared scenario, species, seed
panels, upgrade values, schedule, and time window. They explicitly do not claim
balance, fun, generality, or production approval.

### 3. We use determinism as an experimental tool

The simulation has been designed for comparison rather than merely made
repeatable after the fact. It includes seeded execution, immutable snapshots,
stable species and terrain identities, ruleset fingerprints, phase windows,
checkpoint restore, replay manifests, and structured telemetry.

That foundation enabled two particularly useful results:

- EX-009 showed exact equality between two launch-time orders for the tested
  additive upgrades and same held-out seeds.
- EX-010 showed that reversing six acquisitions during one continuing world did
  change later-phase and final outcomes, even though there was no defensible
  basis for calling either order generally better.

The distinction is valuable. It demonstrates that “the same upgrades commute
when applied at launch” and “upgrade timing does not matter in a stateful run”
are different claims requiring different experiments.

### 4. We preserve useful failures and limitations

Failed startup attempts, dirty working-tree state, validator limitations,
missing result XML, intentional test skips, generated-art review status, and
scope gaps are recorded rather than converted into success language.

Examples include:

- the failed first EX-001 batch startup being retained separately from the
  successful rerun;
- EX-010 reporting `VALIDATED_WITH_LIMITATIONS` where an independent Stat-Line
  validator cannot fully cross-check accumulated counters;
- the current baseline handoff refusing to claim a new Unity result when the
  batch could not run;
- generated visual assets remaining “Needs Review” rather than becoming
  production direction by default.

This makes the work slower to summarize but much safer to trust.

### 5. We are converting recurring judgment into lightweight tools

The repository-local skills cover correctness under uncertainty, loose-end
triage, sprint rollover, documentation clarity, artifact summarization, feature
concerns, and worktree hygiene. The [skills plan](AI_WORKFLOW_SKILLS_PLAN.md)
correctly resists turning every idea into a skill. It waits for a workflow to
stabilize, prefers existing scripts, and requires representative prompts.

The alert-only policy hook follows the same philosophy. Six machine-readable
rules point back to human-authored Studio Guidelines and warn about destructive
Git actions, external side effects, shared Unity assets, network/dependency
changes, communication, and possible secrets. A validator checks the link
between prose policy and executable alerts. Keeping this fail-open during the
prototype is a sensible way to measure warning quality before granting it
blocking authority.

### 6. We are learning to compress evidence without confusing compression with truth

The artifact-retention work is quietly important. The
[retention audit](handoffs/2026-09-09-artifact-retention-audit.md) reduced roughly
3.96 GB of Unity diagnostics to about 0.93 MB for triage, while retaining source
hashes, result identities, failure signals, and explicit loss boundaries. It
also states that summaries omit event order and cannot prove causality or
authorize deletion.

This is a practical pattern for AI work: give agents a compact reading layer,
but preserve a verifiable path back to raw evidence.

## What can be improved

### 1. Measure whether AI improves the work

This is the biggest gap. The project has evidence that the simulation workflow
can support AI-assisted predictions. It does not have comparative evidence that
the AI workflow is better than a careful human-only workflow.

For a small sample of real tasks, record:

- elapsed time from question to reviewable evidence;
- human review time;
- number of incorrect or unsupported AI claims caught;
- number of reruns or rework cycles;
- defects found before and after review;
- false-positive and false-negative impact predictions;
- which context artifacts the agent actually used;
- model, reasoning effort, and approximate usage cost;
- the human's usefulness rating and final decision.

This should be a small observational ledger first, not a new dashboard. Ten to
twenty representative tasks would reveal whether the current rigor is saving
time, moving effort into review, or merely producing more artifacts.

### 2. Reduce context and documentation entropy

The project has 114 handoff notes and a large Working State index after roughly
one month of concentrated work. That is evidence of discipline, but also a
retrieval and staleness problem. More documentation is not automatically more
memory.

Improvements should begin with cheap checks:

- add consistent status, branch, commit, owner, supersedes, and affected-system
  metadata to new handoffs;
- validate links, missing commits, duplicated IDs, and references to deleted
  artifacts;
- generate a short “current by workstream” index from handoff metadata;
- distinguish task handoffs from checkpoints so every small change does not
  receive equal documentary weight;
- archive or mark superseded notes without rewriting their history;
- keep `WORKING_STATE.md` as a doorway instead of allowing it to become a second
  project history.

The goal is faster trustworthy retrieval, not fewer records for its own sake.

### 3. Make evidence lifecycle a property of creation

The artifact audit found 335 directories and 5.95 GB only after evidence had
accumulated. New run manifests should declare a retention class when they are
created:

- decision evidence;
- regression baseline;
- failed-run diagnostic;
- temporary smoke evidence;
- replaceable duplicate candidate.

The run should also record which document or decision cites it. This would make
cleanup a normal lifecycle transition instead of a forensic project.

### 4. Generalize the research before increasing automation

The accepted predictive results remain concentrated on Forest Edge, Hare, a
small set of upgrades, and narrow seed panels. The next valuable proof is not a
more autonomous recommender. It is one clean transfer study across a different
species, scenario, or class of mechanic with a pre-registered prediction and
held-out evaluation.

The project should also build a versioned metric dictionary before asking AI to
interpret more telemetry. The EX-007 `PREY` error shows that schema-valid data
can still be semantically misunderstood.

### 5. Treat visual and experiential validation as first-class evidence

Game development is multimodal. Compile results and deterministic metrics cannot
establish readability, feel, pacing, art cohesion, or fun. The project already
has visual-capture tests and graphics acceptance, but those remain more manual
and less consistently indexed than simulation evidence.

A practical next step is a small visual evidence contract: target resolution,
scene and state, input sequence, reference or acceptance criteria, capture
paths, reviewer, and decision. Avoid pixel-perfect regression where animation
or rendering makes it noisy; the point is repeatable review, not false
precision.

This priority is consistent with current benchmark evidence: GameDevBench found
that game-development agents still struggle with multimodal tasks and that even
simple image/video feedback improved completion rates in its tested setups.

### 6. Validate model and effort routing empirically

SG-002 now defines risk-based model routing, but the repository does not yet
show whether those routing choices are cost-effective. Record the initial risk
class, selected model/effort, escalations, outcome, and review burden for the
same small task sample used to measure workflow value. The policy can then be
revised from observed failures and costs rather than model reputation.

### 7. Improve coordination before adding a messaging transport

Putting the Discord bridge on hold was the right decision. The difficult part
is not posting messages; it is ownership, idempotency, status, branch identity,
conflict handling, and source-of-truth discipline.

The existing [transport-agnostic protocol](DISCORD_AGENT_COLLABORATION_PROTOCOL.md)
is more valuable than a premature bot. The next coordination improvement should
be a locally validated task/handoff envelope with stable IDs and named owners.
Transport can remain manual until two real contributors can complete a full
handoff and recovery exercise without ambiguity.

### 8. Close the loop from evidence to player value

The workflow is currently strongest at producing developer knowledge. The game
concept becomes more interesting if some of that knowledge can be translated
into player-facing explanations, choices, and consequences.

Choose one accepted bounded finding and create one player-facing explanation of
it. Then test whether a player can:

1. understand what changed;
2. predict a plausible tradeoff;
3. make a meaningful choice; and
4. explain the outcome afterward.

That test would connect the research program to actual game design without
mistaking simulation validity for fun.

## What appears distinctive or potentially novel

### A careful claim

The individual ingredients are established:

- mixed-initiative game-design tools combine human judgment with automated
  suggestions;
- AI systems and multi-agent pipelines can generate game code, scenes, and
  prototypes;
- automated and AI-assisted game testing is an active research area;
- deterministic simulations, sensitivity analysis, held-out tests, and change
  impact analysis are established methods;
- human–AI teams and repository-grounded coding agents are under active study.

In a limited comparison pass, no reviewed source matched this project's exact
combination of repository memory, deterministic ecological simulation,
pre-registered AI predictions, immutable evidence bundles, explicit claim
bounds, separate human decisions, and a planned path to expose the same
evidence as player-facing ecology knowledge.

That supports calling the approach **distinctive and researchable**. It does
not support calling it technically novel, academically novel, patentable, or
unique. Those claims require a systematic literature and prior-art review and,
for intellectual property, qualified legal advice.

### The most interesting patterns

#### 1. Evidence-native game development

The simulation is not only the thing being built. It is also an instrument for
testing design changes. Telemetry, checkpoints, fingerprints, and experiment
contracts are being designed alongside mechanics rather than bolted on at the
end.

The larger implication is a game architecture in which every meaningful design
change can carry its own evidence trail: what changed, which worlds were tested,
what moved, what remained stable, what the AI predicted, and what the designer
accepted.

#### 2. Claims that carry their own scope

Most AI workflows produce an answer and leave the reader to remember its
conditions. This workflow tries to make scenario, seed panel, ruleset,
telemetry schema, time window, intervention, confidence, and human decision
travel with the claim.

That pattern could generalize beyond games to digital twins, economic models,
operations research, safety testing, and any domain where an AI explanation is
only valid inside a measured regime.

#### 3. AI as an accountable hypothesis partner

The AI is not merely asked to summarize a completed run. It can make a bounded
prediction before execution, be scored against held-out outcomes, have semantic
mistakes preserved, and propose the next discriminating experiment. This turns
“AI insight” from a persuasive paragraph into a falsifiable work product.

The important next implication is calibration: over time, the team could learn
which classes of question the assistant predicts well, where it should abstain,
and how much human review each class deserves.

#### 4. One evidence spine for developer and player understanding

The same causal stories that help a designer understand extinction, crowding,
movement, or upgrade timing may eventually help a player understand the living
system they are manipulating. If successful, observability stops being only a
debug feature and becomes part of the game's learning and decision loop.

This is probably the project's most compelling larger opportunity: a game whose
development laboratory and player fantasy reinforce each other.

#### 5. Governance that can be tested like software

The alert-only policy prototype ties machine-readable warnings to stable human
guideline IDs and validates that connection. This creates a path from prose
norms to observable tooling without pretending that regular expressions can
replace judgment.

For small studios, this could become a reusable way to encode safeguards around
shared assets, external actions, secrets, provenance, and destructive commands
while keeping humans in control of enforcement.

#### 6. Loss-aware context compression

The summarization workflow states what each compact artifact preserves and what
it loses. That is more important than the compression ratio. It makes context
reduction an explicit transformation with provenance rather than an invisible
AI memory trick.

This could become a general agent pattern: every summary should identify its
source hash, supported questions, omitted detail, and path back to primary
evidence.

## Larger potential

If the next validation steps succeed, the work could become several things:

1. **A better internal studio workflow.** The immediate value is faster,
   safer iteration in a complex Unity project with fewer stale assumptions and
   more reviewable evidence.
2. **A reusable evidence toolkit for simulation-heavy games.** The experiment,
   provenance, comparison, decision, and retention contracts are more portable
   than the particular ecology rules.
3. **A publishable case study.** The strongest research contribution would not
   be “AI made our game.” It would be measured evidence about when an AI
   hypothesis partner helps, misleads, abstains, or increases review burden in a
   real game-production loop.
4. **A player-facing ecology laboratory.** Accepted developer evidence could be
   transformed into explainable forecasts, field notes, upgrade tradeoffs, and
   post-run narratives that deepen the core game fantasy.
5. **A foundation for bounded design-risk prediction.** With enough fresh,
   diverse, well-labeled evidence, the system might flag likely affected
   metrics, tests, scenarios, or player explanations before a change is made.
   This remains a research target, not a current capability.
6. **A model for accountable agent collaboration.** Repository memory,
   claim-scoped evidence, human decisions, and testable policy could apply to
   other creative or simulation-driven teams.

## Recommended next moves

The priorities describe order, not importance:

- **P0 — Build the foundation now.** These steps make later conclusions safer
  and easier to evaluate.
- **P1 — Prove the next capabilities.** These steps test whether the workflow
  transfers to new situations and creates visible game-development value.
- **P2 — Learn from operation.** These steps should use evidence collected by
  the earlier work rather than assumptions.

### P0 — Build the foundation now

1. **Measure whether AI actually helps.**

   **Progress:** In progress. The versioned ledger is live, with 3 of the 10
   minimum representative tasks recorded. Human ratings and the final summary
   remain intentionally pending.

   **Goal and ability unlocked:** Learn where AI saves time, where it moves work
   into human review, and where it makes a result worse. This gives us the
   evidence needed to improve the workflow instead of adding process because it
   sounds useful.

   **Work and completion:** Record 10–20 representative tasks, including time,
   corrections, rework, cost, review effort, and outcome. Complete the step when
   a small versioned dataset and human-written summary exist. Keep every
   conclusion within the sampled tasks.

2. **Make handoffs easier to trust and retrieve.**

   **Progress:** Complete on 2026-09-12. New handoffs use schema 1, and the
   validator checks required identity/status fields while treating historical
   notes as legacy evidence rather than rewriting them.

   **Goal and ability unlocked:** Let a human or AI quickly find the current
   owner, branch, commit, status, replacement note, and next action. The same
   check should expose broken links and stale evidence before they become
   accepted context.

   **Work and completion:** Add consistent handoff metadata and a lightweight
   validator. Complete the step when new handoffs validate owner, status,
   branch, commit, and `supersedes` fields, and when broken links and stale
   artifact references are reported without rewriting historical notes.

3. **Give every experiment metric one shared meaning.**

   **Progress:** Complete on 2026-09-12. Metric dictionary version 1 defines
   all 20 herbivore Stat-Line fields. Report schema 26 and manifest schema 2
   identify and bundle the exact dictionary, and a seeded Unity bundle passed
   end-to-end validation.

   **Goal and ability unlocked:** Give code, reports, AI analysis, and human
   reviewers a common vocabulary. This reduces the chance that valid data is
   interpreted incorrectly, as happened when `PREY` was misunderstood in
   EX-007.

   **Work and completion:** Add a versioned metric dictionary to experiment
   bundles. Complete the step when every scored metric has a stable ID,
   plain-language definition, unit, direction, source, and valid time window,
   and each report records the dictionary version it used.

### P1 — Prove the next capabilities

4. **Test whether a finding transfers beyond Forest Edge and Hare.**

   **Progress:** EX-011's bounded result was accepted on 2026-09-17. The
   combined movement-and-crowding intervention moved to Open Range/Deer and the
   preregistered FPO direction passed on all 20 development and all 5 held-out
   pairs. This single transfer case does not establish general transfer or
   production validity.

   **Goal and ability unlocked:** Begin learning when prior findings can be
   reused and when the assistant should abstain or request new evidence. This is
   the first move from a successful case study toward bounded general
   capability.

   **Work and completion:** After the metric dictionary is ready, run one
   human-approved, pre-registered experiment using a different species,
   scenario, or class of mechanic. Complete the step with development and
   held-out evidence, separate AI analysis, and a human decision that states
   exactly what transferred and what did not.

5. **Make visual review repeatable.**

   **Progress:** The home-screen-only capture test and
   [review contract](AI%20Workflow/VISUAL_REVIEW_PILOT_001.md) are prepared.
   The September 7 screenshots show an earlier setup state and are not current
   acceptance evidence. Fresh captures are pending because Unity is
   open. This pilot avoids app-window transitions covered by SIMWIN-C03 and
   captures desktop home only; Josh's visual decision will follow the captures.

   **Goal and ability unlocked:** Review presentation qualities that compilation
   and simulation metrics cannot judge, including readability, layout, focus,
   art cohesion, and visible regressions.

   **Work and completion:** Pilot one visual evidence contract for a defined
   scene, state, input sequence, and target resolution. Complete the step when
   another contributor can reproduce the state and captures, inspect the
   acceptance criteria, and find a named human review decision.

6. **Turn one developer insight into player value.**

   **Goal and ability unlocked:** Learn whether the evidence system can improve
   the game itself, not only internal analysis. The desired ability is to help a
   player understand a tradeoff, make a meaningful choice, and explain the
   consequence afterward.

   **Work and completion:** Translate one accepted simulation finding into a
   player-facing explanation and run a small comprehension and decision test.
   Record what the player understood, predicted, chose, and misunderstood. Do
   not claim fun or broad usability from one test.

### P2 — Learn from operation

7. **Tune policy warnings from real use.**

   **Goal and ability unlocked:** Create a safer, less distracting guardrail and
   gain evidence for deciding whether each warning should stay advisory, be
   revised, or eventually block an action.

   **Work and completion:** Review representative alert-only warnings and record
   which were useful, noisy, or missing. Complete the step with a short report
   covering outcomes, false positives, and missed risks. Do not consider a
   blocking mode until this evidence exists.

8. **Make the workflow explainable and reusable outside this project.**

   **Goal and ability unlocked:** Create a clear account of what was attempted,
   what worked, what failed, and which parts another team could reproduce. This
   becomes the foundation for a workshop paper, public write-up, or reusable
   toolkit without overstating novelty.

   **Work and completion:** After the workflow-value sample has been reviewed,
   prepare a case-study outline. Complete the step when every proposed claim
   maps to repository evidence, limitations are explicit, and observed results
   remain separate from future potential.

In practical sequence: begin the three P0 steps together; require the metric
dictionary before the transfer experiment; run the visual and player-facing
pilots when their target slices are reviewable; evaluate policy after enough
warnings have accumulated; and outline a case study only after the workflow
value ledger has a human-reviewed result.

## What we can responsibly say today

We can say:

- the project has a functioning, auditable AI-assisted experiment loop;
- deterministic game simulation can support bounded predictions, paired
  comparisons, held-out checks, and human decisions;
- the workflow has caught and preserved AI interpretation errors;
- context, provenance, failure evidence, and ownership are handled more
  deliberately than in an ad hoc chat-driven workflow;
- the combination appears distinctive enough to justify further study.

We cannot yet say:

- AI makes development faster or cheaper overall;
- AI recommendations are generally reliable or calibrated;
- the workflow improves balance, fun, retention, or commercial outcomes;
- the findings generalize across the game, other teams, or other projects;
- the approach is technically or academically novel;
- the system is ready for autonomous design or production decisions.

That boundary is not a weakness in the story. It is evidence that the workflow
is behaving as intended.

## External comparison used for the novelty assessment

This was a focused orientation pass, not a systematic review:

- [GameDevBench: Evaluating Agentic Capabilities Through Game Development](https://arxiv.org/abs/2602.11103)
  shows that integrated, multimodal game-development tasks remain difficult for
  current agents and that visual feedback can materially improve results.
- [Human-AI Collaborative Game Testing with Vision Language Models](https://arxiv.org/abs/2501.11782)
  reports benefits from AI assistance and detailed project knowledge, while
  also showing that AI errors can mislead human testers.
- [ChatCollab: Exploring Collaboration Between Humans and AI Agents in Software Teams](https://arxiv.org/abs/2412.01992)
  studies role-based human–AI teamwork using an interactive game-development
  task.
- [Level Building Sidekick](https://ojs.aaai.org/index.php/AIIDE/article/view/27535)
  demonstrates a mixed-initiative design tool integrated into Unity and built
  with small independent studios.
- [Towards AI as a Creative Colleague in Game Level Design](https://ojs.aaai.org/index.php/AIIDE/article/view/21957)
  shows that increasing AI agency can create frustration and strain, reinforcing
  the need to design initiative and control deliberately.
- [DreamGarden](https://arxiv.org/abs/2410.01791) and
  [AutoUE](https://arxiv.org/abs/2603.07106) explore semi-autonomous or
  multi-agent generation of game environments and complete prototypes. Their
  primary emphasis is generation; this project's most distinctive emphasis is
  traceable evidence and bounded human decisions across ongoing development.

## Internal evidence reviewed

- [AI-Assisted Development](Studio%20Guidelines/AI_ASSISTED_DEVELOPMENT.md)
- [AI Generated Reports](Studio%20Guidelines/AI_GENERATED_REPORTS.md)
- [Conversation Scope and Continuity](Studio%20Guidelines/SG-004-CONVERSATION-SCOPE-AND-CONTINUITY.md)
- [Collaboration Workflow](COLLABORATION_WORKFLOW.md)
- [AI Workflow Skills Plan](AI_WORKFLOW_SKILLS_PLAN.md)
- [AI-Assisted Ecology Laboratory Research Plan](Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md)
- [Predictive AI research paper](Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PAPER.md)
- [P3 gate decision](Research/P3_GATE_DECISION_2026-09-06.md)
- [P3 cohesive results](Research/Experiments/P3-Predictive-AI-Cohesive-Report.md)
- [EX-009 report](Research/Experiments/EX-009-Same-Heldout-Order-Comparison/REPORT.md)
- [EX-010 report](Research/Experiments/EX-010-Sequential-Upgrade-Continuation/REPORT.md)
- [Artifact retention audit](handoffs/2026-09-09-artifact-retention-audit.md)
- `.codex/studio-policy.json`, `.codex/hooks.json`, and the alert-only policy hook
