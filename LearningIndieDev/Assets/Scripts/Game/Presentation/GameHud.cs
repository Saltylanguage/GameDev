using UnityEngine;

namespace SaltyGame
{
    public sealed class GameHud : MonoBehaviour
    {
        GameRuntime runtime;
        GUIStyle title;
        GUIStyle body;
        GUIStyle summary;

        public void Initialize(GameRuntime runtime)
        {
            this.runtime = runtime;
        }

        void OnGUI()
        {
            if (runtime == null)
                return;

            title ??= new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
            body ??= new GUIStyle(GUI.skin.label) { fontSize = 18, normal = { textColor = Color.white } };
            summary ??= new GUIStyle(body) { alignment = TextAnchor.MiddleCenter, wordWrap = true };

            GUI.Label(new Rect(24, 18, 500, 45), "Island Chores", title);
            GUI.Label(new Rect(26, 60, 900, 30), $"Day {runtime.Clock.Day} - {runtime.Clock.TimeOfDay} | Hunger: {runtime.Survival.Hunger}/100 | Energy: {runtime.Survival.Energy}/100", body);
            GUI.Label(new Rect(26, 92, 900, 30), $"Wood: {runtime.Inventory.Get(ResourceId.Wood)} | Berries: {runtime.Inventory.Get(ResourceId.Berries)} | Meals: {runtime.Inventory.Get(ResourceId.CookedMeal)} | Stone: {runtime.Inventory.Get(ResourceId.Stone)}", body);
            GUI.Label(new Rect(26, 124, 900, 30), runtime.Objective.StatusText(runtime.Inventory, runtime.Camp, runtime.Storm), body);

            if (runtime.MessageTimer > 0f)
            {
                if (runtime.DaySummary == null)
                    GUI.Label(new Rect(Screen.width / 2 - 180, 160, 700, 35), runtime.ActivityMessage, body);
                else
                {
                    var panel = new Rect(Screen.width / 2 - 320, 160, 640, 86);
                    GUI.Box(panel, GUIContent.none);
                    GUI.Label(new Rect(panel.x + 16, panel.y + 8, panel.width - 32, panel.height - 16), runtime.DaySummary, summary);
                }
            }

            if (runtime.Activities.IsActive)
            {
                var bar = new Rect(Screen.width / 2 - 180, Screen.height - 105, 360, 24);
                GUI.Box(bar, GUIContent.none);
                var active = runtime.Activities.Active;
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
                GUI.Label(new Rect(Screen.width / 2 - 185, Screen.height - 72, 600, 30), $"{hint} | {runtime.Activities.Active.StatusText}", body);
            }
            else
            {
                var target = runtime.Interaction.CurrentTarget;
                var prompt = target is CampfireInteractable campfire ? campfire.GetActionPrompt() : target == null ? "Move with WASD" : $"Press [E] to interact with {target.DisplayName}";
                GUI.Label(new Rect(Screen.width / 2 - 260, Screen.height - 70, 800, 35), prompt, body);
            }
        }
    }
}
