using UnityEngine;

namespace SaltyGame
{
    public sealed class GameHud : MonoBehaviour
    {
        GameRuntime runtime;
        GUIStyle title;
        GUIStyle body;

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

            GUI.Label(new Rect(24, 18, 500, 45), "Island Chores", title);
            GUI.Label(new Rect(26, 60, 300, 30), $"Day {runtime.Clock.Day} - {runtime.Clock.TimeOfDay} | Wood: {runtime.Inventory.Get(ResourceId.Wood)}", body);

            if (runtime.Activities.IsActive)
            {
                var bar = new Rect(Screen.width / 2 - 180, Screen.height - 105, 360, 24);
                GUI.Box(bar, GUIContent.none);
                GUI.color = new Color(0.2f, 0.8f, 0.25f);
                GUI.Box(new Rect(bar.x + bar.width * 0.4f, bar.y, bar.width * 0.2f, bar.height), GUIContent.none);
                GUI.color = Color.white;
                GUI.Box(new Rect(bar.x + bar.width * runtime.Activities.Meter - 4, bar.y - 7, 8, bar.height + 14), GUIContent.none);
                GUI.Label(new Rect(Screen.width / 2 - 185, Screen.height - 72, 500, 30), $"[Space] hit green | [E]/[Esc] cancel | {runtime.Activities.Active.StatusText}", body);
            }
            else
            {
                var target = runtime.Interaction.CurrentTarget;
                GUI.Label(new Rect(Screen.width / 2 - 170, Screen.height - 70, 500, 35), target == null ? "Move with WASD" : $"Press [E] to interact with {target.DisplayName}", body);
            }
        }
    }
}
