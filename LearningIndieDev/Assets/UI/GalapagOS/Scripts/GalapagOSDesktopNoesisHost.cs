using Noesis;
using UnityEngine;

namespace SaltyGame
{
    public sealed class GalapagOSDesktopNoesisHost : MonoBehaviour
    {
        [SerializeField] NoesisView view;
        [SerializeField] VM_GalapagOS_Desktop viewModel;
        [SerializeField] NoesisXaml xaml;

        void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (view == null || viewModel == null || xaml == null)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost requires serialized NoesisView, VM_GalapagOS_Desktop, and desktop XAML references.", this);
                return;
            }

            view.enabled = false;
            view.Xaml = xaml;
            view.enabled = true;

            if (view.Content == null)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost could not load its XAML content.", this);
                return;
            }

            view.Content.DataContext = viewModel;
        }
    }
}
