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
        }
    }
}
