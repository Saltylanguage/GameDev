using Noesis;
using UnityEngine;

namespace SaltyGame
{
    public sealed class MainMenuNoesisHost : MonoBehaviour
    {
        [SerializeField] NoesisView view;
        [SerializeField] VM_MainMenu viewModel;

        void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (view == null || viewModel == null || view.Content == null)
            {
                Debug.LogError("MainMenuNoesisHost requires serialized NoesisView and VM_MainMenu references.", this);
                return;
            }

            view.Content.DataContext = viewModel;

            string primaryActionName = viewModel.ContinueEnabled
                ? "ContinueButton"
                : "ProfileSelectionButton";

            Button primaryAction = view.Content.FindName(primaryActionName) as Button;
            if (primaryAction == null)
            {
                Debug.LogWarning($"Main menu could not focus '{primaryActionName}'.", this);
                return;
            }

            view.Content.Dispatcher.BeginInvoke(() =>
            {
                if (!primaryAction.Focus())
                {
                    Debug.LogWarning($"Main menu could not focus '{primaryActionName}'.", this);
                }
            });
        }
    }
}
