using Noesis;
using UnityEngine;
using UnityEngine.U2D;

namespace SaltyGame
{
    public sealed class SpeciesSimulationNoesisHost : MonoBehaviour
    {
        [SerializeField] NoesisXaml xaml;
        [SerializeField] SpriteAtlas animalAtlas;
        [SerializeField] SpriteAtlas terrainAtlas;
        [SerializeField] Sprite foxSprite;
        [SerializeField] Sprite rabbitSprite;
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

            viewModel.Initialize(preview, animalAtlas, terrainAtlas, foxSprite, rabbitSprite);
            viewModel.BindToView(view);
        }
    }
}
