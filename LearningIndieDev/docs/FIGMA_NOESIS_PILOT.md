# Figma to Noesis pilot

This branch tests the smallest useful design-to-implementation loop without
changing a live scene or replacing existing UI.

- Figma file: [GalapagOS UI Design System Pilot](https://www.figma.com/design/IKbgAmYVhzjGOWVc4FY7we)
- Noesis preview: `Assets/UI/DesignSystem/FigmaNoesisPilot.xaml`
- Noesis resources: `Assets/UI/DesignSystem/FigmaNoesisPilotResources.xaml`

## What the pilot proved

The connected Figma Starter plan can create local variables, aliases, text
styles, effect styles, and editable design content. It is limited to three pages
and the Figma MCP tool-call quota was reached before component creation and
visual validation could finish. The current Figma file therefore contains a
Cover and Foundations work-in-progress; its Components page is intentionally
unfinished.

Native Code Connect is not part of this pilot. Publishing Code Connect mappings
requires a compatible paid Figma plan and published components. The table below
is the temporary human- and agent-readable contract; it is not presented as a
published Code Connect mapping.

## Token contract

| Figma variable | Figma code syntax | Noesis resource |
| --- | --- | --- |
| `color/bg/canvas` | `var(--ui-color-canvas)` | `Ui.Brush.Canvas` |
| `color/bg/header` | `var(--ui-color-header)` | `Ui.Brush.Header` |
| `color/bg/window` | `var(--ui-color-window)` | `Ui.Brush.Window` |
| `color/bg/window-raised` | `var(--ui-color-window-raised)` | `Ui.Brush.WindowRaised` |
| `color/text/primary` | `var(--ui-color-text-primary)` | `Ui.Brush.TextPrimary` |
| `color/text/secondary` | `var(--ui-color-text-secondary)` | `Ui.Brush.TextSecondary` |
| `color/text/accent` | `var(--ui-color-text-accent)` | `Ui.Brush.TextAccent` |
| `color/status/success` | `var(--ui-color-success)` | `Ui.Brush.Success` |
| `color/status/danger` | `var(--ui-color-danger)` | `Ui.Brush.Danger` |
| `spacing/100` | `var(--ui-spacing-100)` | `Ui.Space.100` |
| `spacing/150` | `var(--ui-spacing-150)` | `Ui.Space.150` |
| `spacing/200` | `var(--ui-spacing-200)` | `Ui.Space.200` |
| `spacing/300` | `var(--ui-spacing-300)` | `Ui.Space.300` |
| `radius/control` | `var(--ui-radius-control)` | `Ui.Radius.Control` |
| `radius/window` | `var(--ui-radius-window)` | `Ui.Radius.Window` |

Composite XAML values such as `Ui.Padding.Button.Medium` are derived from the
scalar spacing tokens. They are implementation helpers, not additional Figma
source tokens.

## Component contract

Use PascalCase component names, named properties, and short variant axes:

| Figma concept | Noesis implementation |
| --- | --- |
| `Window` | `Border` using `Ui.Brush.Window`, `Ui.Radius.Window`, and `Ui.Padding.Window` |
| `Window.Title` text property | `TextBlock.Text` or a normal `Text` binding |
| `Window/Slot/Content` | normal XAML child content; do not flatten it into an image |
| `Button.Style=Primary` | `Style="{StaticResource Ui.Button.Primary}"` |
| `Button.Style=Secondary` | `Style="{StaticResource Ui.Button.Secondary}"` |
| `Button.Size=Small` | `Ui.Padding.Button.Small`, `MinHeight="36"` |
| `Button.Size=Medium` | `Ui.Padding.Button.Medium`, `MinHeight="44"` |
| `Button.Size=Large` | `Ui.Padding.Button.Large`, `MinHeight="52"` |
| `Button.Label` text property | `Content` or `{Binding ...}` |
| disabled state | `IsEnabled`; the shared template controls its visual state |

The Figma `Elevation/Window` style does not yet have a Noesis resource mapping.
No shadow effect was added without first verifying the exact effect support and
cost in NoesisGUI 3.2.13.

## Low-friction working loop

1. Design with local components and semantic variables in Figma.
2. Keep component names, variant axes, and layer roles aligned with this contract.
3. Have Codex inspect the selected Figma nodes and update only the matching XAML
   resources or component markup.
4. Parse the XAML, import it in Unity, inspect Noesis errors, and render the
   representative preview before touching a live scene.
5. Promote reviewed resources into application dictionaries or production views
   only after the pilot visually matches.

## Next quota-window test

When Figma MCP access resets, finish the `Window` component, create the six
`Button` variants (`Primary|Secondary` by `Small|Medium|Large`), compose one
preview instance, and capture metadata plus screenshots. That evidence is the
gate for deciding whether this mapping should become a permanent project
workflow.
