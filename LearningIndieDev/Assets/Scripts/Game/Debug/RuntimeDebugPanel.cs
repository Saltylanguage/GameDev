using UnityEngine;

namespace SaltyGame
{
    public sealed class RuntimeDebugPanel : MonoBehaviour
    {
        static bool visible = true;
        GameRuntime runtime;
        GUIStyle body;

        public void Initialize(GameRuntime runtime)
        {
            this.runtime = runtime;
        }

        public static void Toggle()
        {
            visible = !visible;
        }

        void OnGUI()
        {
            if (!visible || runtime == null)
                return;

            body ??= new GUIStyle(GUI.skin.label) { fontSize = 14, normal = { textColor = Color.white } };

            var target = runtime.Interaction.CurrentTarget;
            var activity = runtime.Activities.Active;
            var text = $"DEBUG [F3]\nState: {runtime.State}\nDay: {runtime.Clock.Day} / {runtime.Clock.TimeOfDay}\nTarget: {(target == null ? "none" : target.DisplayName)}\nActivity: {(activity == null ? "none" : activity.DisplayName)}\nWood: {runtime.Inventory.Get(ResourceId.Wood)}\nBerries: {runtime.Inventory.Get(ResourceId.Berries)}\nStone: {runtime.Inventory.Get(ResourceId.Stone)}";
            GUI.Box(new Rect(Screen.width - 250, 18, 230, 165), GUIContent.none);
            GUI.Label(new Rect(Screen.width - 238, 28, 210, 150), text, body);
        }
    }
}
