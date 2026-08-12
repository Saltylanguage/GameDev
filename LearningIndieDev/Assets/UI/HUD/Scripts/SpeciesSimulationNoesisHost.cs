using Noesis;
using UnityEngine;

namespace SaltyGame
{
    public sealed class SpeciesSimulationNoesisHost : MonoBehaviour
    {
        [SerializeField] NoesisXaml xaml;
        [SerializeField] bool enableNoesisUi = true;

        void Start()
        {
            if (!enableNoesisUi || xaml == null)
            {
                return;
            }

            var preview = FindAnyObjectByType<SpeciesSimulationPreview>();
            var camera = GetComponentInChildren<Camera>(true);
            if (preview == null || camera == null)
            {
                Debug.LogError("SpeciesSimulationNoesisHost requires a simulation preview and camera.", this);
                return;
            }

            var view = camera.GetComponent<NoesisView>();
            if (view == null)
            {
                view = camera.gameObject.AddComponent<NoesisView>();
            }

            view.enabled = false;
            view.Xaml = xaml;
            view.enabled = true;

            var viewModel = camera.GetComponent<SpeciesSimulationViewModel>();
            if (viewModel == null)
            {
                viewModel = camera.gameObject.AddComponent<SpeciesSimulationViewModel>();
            }

            viewModel.Initialize(preview);
            preview.NoesisUiEnabled = true;
            preview.LegacyUiEnabled = false;
        }
    }
}
