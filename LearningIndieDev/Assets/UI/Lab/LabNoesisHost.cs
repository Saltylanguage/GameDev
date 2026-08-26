using Noesis;
using UnityEngine;

namespace SaltyGame
{
    public sealed class LabNoesisHost : MonoBehaviour
    {
        [SerializeField] NoesisView view;
        [SerializeField] VM_Lab viewModel;
        [SerializeField] NoesisXaml xaml;

        void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (view == null || viewModel == null || xaml == null)
            {
                Debug.LogError("LabNoesisHost requires serialized NoesisView, VM_Lab, and Lab XAML references.", this);
                return;
            }

            view.enabled = false;
            view.Xaml = xaml;
            view.enabled = true;

            if (view.Content == null)
            {
                Debug.LogError("LabNoesisHost could not load its XAML content.", this);
                return;
            }

            view.Content.DataContext = viewModel;
        }
    }
}
