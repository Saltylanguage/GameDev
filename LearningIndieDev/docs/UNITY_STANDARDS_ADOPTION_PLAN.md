# Unity Engineering Standards Adoption Plan

Status: proposed phased adoption  
Date: 2026-08-02  
Authority: [`UNITY_ENGINEERING_STANDARDS.md`](UNITY_ENGINEERING_STANDARDS.md)

## Current compliance summary

The project already has a useful vertical slice: an explicit `GameRuntime` composition root, plain C# activity/inventory/clock rules, a runtime assembly boundary, NUnit tests, text serialization, and a bootstrap validator. It is not yet a production-scale architecture: there are no save-data definitions, profiling budgets/captures, Play Mode test assembly, first-party ScriptableObject definitions, or stable UI/branch/large-file policies.

The audit found no missing `.meta` files for current Assets, no runtime-to-Editor assembly reference, no Addressables installation, no first-party DOTS code, and no existing `.editorconfig` or analyzer configuration. `Assets/UI` and `Assets/TutorialInfo` retain inconsistent/template conventions and are excluded from immediate cleanup.

## Highest-value problems

1. Keep bootstrap and generated-object ownership explicit as the world grows; teardown and scene transition behavior are not yet defined.
2. Add tests at the narrowest boundary when new domain rules are added; the current Edit Mode coverage is good for the first slice but has no Play Mode coverage.
3. Establish a measured target hardware/frame-time budget before optimizing.
4. Decide whether future authored activity data needs ScriptableObjects; do not create assets until shared authoring is a real need.
5. Keep the optional Noesis/UI experiment isolated until its package path is portable and its conventions are deliberately adopted.

## Low-risk immediate improvements

- Use the authoritative stylesheet in code reviews and AI-assisted changes.
- Run `Salty > Validate Bootstrap Scene` after scene/bootstrap edits.
- Add a focused Edit Mode test with each new activity or domain rule.
- Use the new `.editorconfig` for touched first-party code; do not reformat the whole repository.
- Keep `.meta` files paired and inspect serialized diffs for any Unity asset change.
- Record a short manual validation scenario for prototype-only changes until a test is practical.

## Phased adoption

### Phase 0 - policy and checks

Apply the stylesheet, keep the current assembly boundaries, and run existing tests plus the bootstrap validator. Add repository checks for `.meta` parity, forbidden generated files, asmdef cycles, and `git diff --check`/equivalent Plastic diff hygiene where the team can execute them.

### Phase 1 - touched-code consistency

When a file is changed for feature work, use explicit access modifiers, current `SaltyGame` naming, braces, named constants, and visible dependencies. Add or update the smallest relevant Edit Mode test. Do not sweep legacy UI/template code.

### Phase 2 - runtime growth boundary

Before adding a second major gameplay loop, document ownership for initialization/reset/shutdown and decide whether world construction remains code-authored or moves to prefabs/ScriptableObjects. If moving assets, create a separate Unity Editor migration with reference validation and rollback evidence.

### Phase 3 - production evidence

Choose representative hardware and scenarios, set target budgets, capture baseline CPU/GPU/memory data, and add performance tests or repeatable profiling instructions only where useful. Any pooling, scheduler, Burst/Jobs, ECS, or data rewrite waits for this evidence.

### Phase 4 - persistence and UI decisions

Define versioned save DTOs/migrations before persistence becomes gameplay-critical. Decide UI Toolkit/uGUI/portable Noesis direction from actual screen complexity and package availability. Add Play Mode and serialization/migration tests after those decisions.

## Changes requiring serialized asset migration

- Moving/renaming scenes, prefabs, ScriptableObjects, or folders.
- Converting code-authored world objects to prefabs.
- Introducing serialized configuration assets referenced by scenes.
- Changing serialized field names/types or replacing runtime scene references.

These require a Unity Editor migration, `.meta`/GUID validation, scene/prefab load validation, and a separate reviewable change. None are performed in this task.

## Changes requiring architectural decisions

Save-data schema/versioning; additive scene loading; prefab ownership; UI technology; event-channel policy; target hardware/budgets; Plastic branch workflow; large-file policy; and a second runtime assembly boundary.

## Explicit non-goals

No gameplay refactor, asset move/rename, scene/prefab modification, package/Unity upgrade, DOTS addition, DI framework, analyzer package, broad legacy formatting pass, or performance claim is part of this adoption plan.

## Known exceptions and debt not to fix without context

- `Assets/UI/DelegateCommand.cs` has legacy naming and an unused `canExcute` parameter; it is optional/experimental and should not be changed as collateral cleanup.
- `Assets/UI/HUD/Scripts/BaseViewModel.cs` is guarded by `NOESIS` and contains legacy naming/debug text; keep it isolated until the Noesis path is portable and selected.
- `Assets/TutorialInfo` is template content and may follow Unity's template conventions.
- `WorldRuntime` creates placeholder sprites and uses hard-coded prototype values; authored data/prefab conversion needs a design decision, not an opportunistic cleanup.
- `RuntimeDebugPanel.visible` is intentionally static development state; it is not approved as a general global-state pattern.
- The misspelled `Boostrap` scene path is serialized and must not be renamed casually.

## Recommended first implementation slice

Add one new small activity using the existing `IActivity`/`IActivityTarget` boundary, with a plain C# Edit Mode test for its rule and one controller/inventory integration assertion. Run the existing bootstrap validator and current test assembly, then review the diff for `.meta`/serialization safety. This is behavior-preserving to existing activities, proves the stylesheet's dependency/test rules, and avoids asset migration, new abstractions, and unmeasured optimization.

