using UnityEngine;

namespace SaltyGame
{
    public sealed class CellularAutomataPrototypeRuntime : MonoBehaviour
    {
        public SpeciesSimulationPreview SpeciesPreview { get; private set; }

        void Awake()
        {
            CreateBackgroundCamera();
            SpeciesPreview = gameObject.AddComponent<SpeciesSimulationPreview>();
        }

        void CreateBackgroundCamera()
        {
            if (FindAnyObjectByType<Camera>() != null)
            {
                return;
            }

            var cameraObject = new GameObject("Prototype Camera");
            cameraObject.transform.SetParent(transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 0f, -10f);

            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.02f, 0.025f, 0.04f);
        }
    }
}
