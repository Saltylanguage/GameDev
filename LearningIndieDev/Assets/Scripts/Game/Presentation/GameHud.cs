using UnityEngine;

namespace SaltyGame
{
    public sealed class GameHud : MonoBehaviour
    {
        GameRuntime runtime;
        GUIStyle title;
        GUIStyle body;
        GUIStyle summary;
        GUIStyle prompt;
        GUIStyle worldLabel;

        public void Initialize(GameRuntime runtime)
        {
            this.runtime = runtime;
        }

        void OnGUI()
        {
            if (runtime == null || runtime.Clock == null || runtime.Survival == null || runtime.Inventory == null || runtime.Camp == null || runtime.Objective == null || runtime.World == null || runtime.Interaction == null || runtime.Activities == null)
                return;

            title ??= new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold, normal = { textColor = new Color(1f, 0.88f, 0.62f) } };
            body ??= new GUIStyle(GUI.skin.label) { fontSize = 18, normal = { textColor = new Color(1f, 0.93f, 0.76f) } };
            summary ??= new GUIStyle(body) { fontSize = 16, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            prompt ??= new GUIStyle(body) { fontSize = 16, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            if (RuntimeDebugPanel.IsVisible)
                worldLabel ??= new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };

            GUI.Label(new Rect(24, 18, 500, 45), "Island Chores", title);
            GUI.Label(new Rect(26, 60, 900, 30), $"Day {runtime.Clock.Day} - {runtime.Clock.TimeOfDay} | Hunger: {runtime.Survival.Hunger}/100 | Energy: {runtime.Survival.Energy}/100", body);
            var toolText = runtime.Camp.CrudeAxeCrafted ? "Crude Axe" : "Hands";
            GUI.Label(new Rect(26, 92, 900, 30), $"Wood: {runtime.Inventory.Get(ResourceId.Wood)} | Berries: {runtime.Inventory.Get(ResourceId.Berries)} | Meals: {runtime.Inventory.Get(ResourceId.CookedMeal)} | Stone: {runtime.Inventory.Get(ResourceId.Stone)} | Tool: {toolText}", body);
            GUI.Label(new Rect(26, 124, 900, 30), runtime.Objective.StatusText(runtime.Inventory, runtime.Camp, runtime.Storm), body);
            if (RuntimeDebugPanel.IsVisible)
                DrawWorldLabels();

            if (runtime.MessageTimer > 0f)
            {
                if (runtime.DaySummary == null)
                    GUI.Label(new Rect(Screen.width / 2 - 180, 160, 700, 35), runtime.ActivityMessage, body);
                else
                {
                    var panel = new Rect(Screen.width / 2 - 280, 150, 560, 70);
                    DrawPanel(panel);
                    GUI.Label(new Rect(panel.x + 14, panel.y + 6, panel.width - 28, panel.height - 12), runtime.DaySummary, summary);
                }
            }

            if (runtime.Activities.IsActive)
            {
                var bar = new Rect(Screen.width / 2 - 180, Screen.height - 105, 360, 24);
                GUI.Box(bar, GUIContent.none);
                var active = runtime.Activities.CurrentActivity;
                if (active.RequiresTimingInput)
                {
                    GUI.color = new Color(0.2f, 0.8f, 0.25f);
                    GUI.Box(new Rect(bar.x + bar.width * 0.4f, bar.y, bar.width * 0.2f, bar.height), GUIContent.none);
                    GUI.color = Color.white;
                    GUI.Box(new Rect(bar.x + bar.width * runtime.Activities.Meter - 4, bar.y - 7, 8, bar.height + 14), GUIContent.none);
                }
                else
                {
                    GUI.color = new Color(0.95f, 0.48f, 0.12f);
                    GUI.Box(new Rect(bar.x, bar.y, bar.width * Mathf.Clamp01(runtime.Activities.Meter), bar.height), GUIContent.none);
                    GUI.color = Color.white;
                }

                var hint = active.RequiresTimingInput ? "[Space] hit green | [E]/[Esc] cancel" : "Cooking... | [Esc] cancel";
                DrawBottomPrompt($"{hint} | {runtime.Activities.CurrentActivity.StatusText}");
            }
            else
            {
                var target = runtime.Interaction.CurrentTarget;
                var actionPrompt = target switch
                {
                    CampfireInteractable campfire => campfire.GetActionPrompt(),
                    JungleEdgeInteractable jungleEdge => jungleEdge.GetActionPrompt(),
                    SurvivorInteractable survivor => survivor.GetActionPrompt(),
                    null => "Move with WASD",
                    _ => $"Press [E] to interact with {target.DisplayName}"
                };
                DrawBottomPrompt(actionPrompt);
            }
        }

        void DrawBottomPrompt(string text)
        {
            var panel = new Rect(0f, Screen.height - 54f, Screen.width, 54f);
            DrawPanel(panel);
            GUI.color = Color.white;
            GUI.Label(new Rect(18f, panel.y + 5f, panel.width - 36f, panel.height - 10f), text, prompt);
        }

        void DrawPanel(Rect panel)
        {
            GUI.color = new Color(0.76f, 0.55f, 0.25f, 0.95f);
            GUI.Box(panel, GUIContent.none);
            GUI.color = new Color(0.04f, 0.12f, 0.17f, 0.9f);
            GUI.Box(new Rect(panel.x + 2f, panel.y + 2f, panel.width - 4f, panel.height - 4f), GUIContent.none);
            GUI.color = Color.white;
        }

        void DrawWorldLabels()
        {
            foreach (var target in runtime.World.Targets)
            {
                if (!target.CanInteract && !(target is CampfireInteractable) && !(target is ShelterInteractable))
                    continue;

                var label = target switch
                {
                    TreeInteractable => "TREE",
                    BerryBushInteractable => "BERRIES",
                    RockInteractable => "ROCK",
                    CampfireInteractable => "CAMP",
                    ShelterInteractable => "SHELTER",
                    JungleEdgeInteractable => "JUNGLE EDGE",
                    SurvivorInteractable => "MARA",
                    _ => null
                };
                if (label == null)
                    continue;

                var camera = Camera.main;
                if (camera == null)
                    continue;

                var screen = camera.WorldToScreenPoint((Vector3)target.Position + Vector3.up * 0.85f);
                if (screen.z <= 0f)
                    continue;

                var rect = new Rect(screen.x - 64f, Screen.height - screen.y - 15f, 128f, 22f);
                GUI.color = new Color(0f, 0f, 0f, 0.55f);
                GUI.Box(rect, GUIContent.none);
                GUI.color = Color.white;
                GUI.Label(rect, label, worldLabel);
            }
        }
    }
}
