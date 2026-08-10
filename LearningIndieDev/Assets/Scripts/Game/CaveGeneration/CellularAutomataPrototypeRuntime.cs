using UnityEngine;

namespace SaltyGame
{
    public sealed class CellularAutomataPrototypeRuntime : MonoBehaviour
    {
        public CavePreview Preview { get; private set; }
        public LifeSimulationPreview LifePreview { get; private set; }

        void Awake()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 0f, -10f);

            var sceneCamera = cameraObject.AddComponent<Camera>();
            sceneCamera.orthographic = true;
            sceneCamera.orthographicSize = 4.5f;
            sceneCamera.clearFlags = CameraClearFlags.SolidColor;
            sceneCamera.backgroundColor = new Color(0.035f, 0.045f, 0.065f);

            Preview = gameObject.AddComponent<CavePreview>();
            Preview.Initialize();
            LifePreview = gameObject.AddComponent<LifeSimulationPreview>();
        }
    }
}
