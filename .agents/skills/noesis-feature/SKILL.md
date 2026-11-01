---
name: noesis-feature
description: Build or review player-facing Noesis/XAML features with correct wiring, image resources, import checks, and visual verification.
---

# Noesis Feature

Build the smallest complete player-facing UI slice that fits the game's current Noesis architecture. Use this skill for implementation or focused review; a review-only request does not authorize edits.

## Establish the current boundary

Read the relevant part of the [UI/MVVM guideline](../../../LearningIndieDev/docs/Studio%20Guidelines/SG-003-UI-MVVM-ARCHITECTURE.md), the active [Noesis image-resource guideline](../../../LearningIndieDev/docs/Studio%20Guidelines/SG-006-NOESIS-IMAGE-RESOURCE-PIPELINE.md), [Unity engineering standards](../../../LearningIndieDev/docs/UNITY_ENGINEERING_STANDARDS.md), and the feature's current design/acceptance record. SG-003 is proposed for review; use the accepted architecture and nearby first-party code to resolve a conflict rather than treating its status as approval for a broad migration.

Inspect the existing XAML, ViewModel, helper/controller, domain owner, composition host, scene/prefab references, and tests before choosing files. Preserve Unity `.meta` files, GUIDs, serialized fields, and other developers' in-progress changes. Do not introduce a Unity UI framework as the player UI or rename legacy assets as incidental cleanup.

## Implement the feature slice

- Keep XAML in presentation: layout, bindings, resources, and visual states. The ViewModel exposes display state and commands; the helper coordinates Unity-facing work; the domain owns simulation, progression, persistence, and other authoritative rules. Wire the scene and data context explicitly. Follow the feature's existing conventions where they are stable, and use the `V_Panel_*`, `VM_*`, and `Helper_*` conventions for new parts when applicable.
- When authored images are needed, add stable semantic keys to `Assets/UI/ImageResources.xaml` and consume them through `{StaticResource ...}` in each view/template state. Check resource merges, import settings, and all consumers. Do not add duplicate keys or direct image paths to consuming XAML.
- Add focused domain or interaction tests for changed behavior. Keep visual-only work scoped to the affected screen and states. Do not add a framework, global event bus, or extra state owner to avoid a clear serialized reference.

## Verify and report

Use `unity-cli` and the [project Unity workflow](../../../LearningIndieDev/docs/UNITY_PLUGIN_WORKFLOWS.md) for import, PlayMode, and visual evidence. Confirm Noesis loads without missing-resource or binding errors, exercise the relevant enabled/disabled and transition states, and inspect captures at the target resolutions when appearance changes. A C# compile or passing screenshot-capture test alone does not prove the screen works or reads well. If an Editor or visual run is unavailable, report the exact unverified states.

Report the player-visible behavior, authoritative state owner, files changed, tests and visual states actually inspected, screenshot/artifact paths, and remaining limits. Keep the distinction between implementation, observed runtime behavior, and human visual acceptance explicit.
