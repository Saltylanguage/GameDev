using System.ComponentModel;
using Noesis;
using UnityEngine;
using UnityEngine.U2D;

namespace SaltyGame
{
    public sealed class GalapagOSDesktopNoesisHost : MonoBehaviour
    {
        [Header("Desktop Composition")]
        [SerializeField] Camera desktopCamera;
        [SerializeField] NoesisView view;
        [SerializeField] VM_GalapagOS_Desktop viewModel;
        [SerializeField] NoesisXaml xaml;

        [Header("Simulation Composition")]
        [SerializeField] Camera simulationCamera;
        [SerializeField] NoesisView simulationView;
        [SerializeField] NoesisXaml simulationXaml;
        [SerializeField] SpeciesSimulationPreview simulationPreview;
        [SerializeField] VM_SimulationShell simulationViewModel;
        [SerializeField] VM_SimulationBoard simulationBoardViewModel;
        [SerializeField] SpriteAtlas animalAtlas;
        [SerializeField] SpriteAtlas terrainAtlas;
        [SerializeField] Sprite foxSprite;
        [SerializeField] Sprite rabbitSprite;

        SpeciesSimulationBoard simulationBoard;
        bool simulationCompositionReady;

        void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (desktopCamera == null || view == null || viewModel == null || xaml == null)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost requires serialized desktop camera, NoesisView, VM_GalapagOS_Desktop, and XAML references.", this);
                return;
            }

            desktopCamera.enabled = true;
            view.enabled = true;
            if (simulationCamera != null)
            {
                simulationCamera.enabled = false;
            }

            if (simulationView != null)
            {
                simulationView.enabled = false;
            }

            if (view.Content == null)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost could not load the desktop XAML content.", this);
                return;
            }

            view.Content.DataContext = viewModel;
            viewModel.BindSimulationLauncher(OpenSimulation);
            simulationCompositionReady = TryInitializeSimulation();
        }

        bool TryInitializeSimulation()
        {
            if (simulationCamera == null
                || simulationView == null
                || simulationXaml == null
                || simulationPreview == null
                || simulationViewModel == null
                || simulationBoardViewModel == null)
            {
                Debug.LogError(
                    "GalapagOSDesktopNoesisHost requires simulation XAML, preview, shell VM, and board VM references.",
                    this);
                return false;
            }

            simulationViewModel.Initialize(simulationPreview, animalAtlas, terrainAtlas, foxSprite, rabbitSprite);
            simulationViewModel.BindDesktopClose(CloseSimulation);
            simulationBoardViewModel.Initialize(simulationPreview);
            return true;
        }

        void OpenSimulation()
        {
            if (!simulationCompositionReady)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost cannot open Simulation because its composition is not ready.", this);
                return;
            }

            desktopCamera.enabled = false;
            view.enabled = false;
            simulationCamera.enabled = true;
            simulationView.enabled = true;

            if (simulationView.Content == null)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost could not load the simulation XAML content.", this);
                CloseSimulation();
                return;
            }

            simulationView.Content.DataContext = simulationViewModel;
            simulationBoard = FindSimulationBoard(simulationView.Content);
            if (simulationBoard == null)
            {
                Debug.LogError("GalapagOSDesktopNoesisHost could not find the SimulationBoard control in its XAML content.", this);
                CloseSimulation();
                return;
            }

            simulationBoard.SetSpriteVisuals(
                simulationViewModel.AnimalSprites,
                simulationViewModel.GrassTerrainTiles,
                simulationViewModel.DesertTerrainTiles);
            simulationBoardViewModel.PropertyChanged -= HandleBoardPropertyChanged;
            simulationBoardViewModel.PropertyChanged += HandleBoardPropertyChanged;
            ApplyBoardSnapshot();
        }

        void CloseSimulation()
        {
            simulationBoardViewModel.PropertyChanged -= HandleBoardPropertyChanged;
            simulationBoard = null;
            simulationView.enabled = false;
            simulationCamera.enabled = false;
            desktopCamera.enabled = true;
            view.enabled = true;
        }

        void OnDestroy()
        {
            if (simulationBoardViewModel != null)
            {
                simulationBoardViewModel.PropertyChanged -= HandleBoardPropertyChanged;
            }
        }

        void HandleBoardPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(VM_SimulationBoard.Snapshot))
            {
                ApplyBoardSnapshot();
            }
        }

        void ApplyBoardSnapshot()
        {
            simulationBoard?.SetSnapshot(simulationBoardViewModel?.Snapshot);
        }

        static SpeciesSimulationBoard FindSimulationBoard(FrameworkElement root)
        {
            if (root == null)
            {
                return null;
            }

            var board = root.FindName("SimulationBoard") as SpeciesSimulationBoard;
            if (board != null)
            {
                return board;
            }

            var window = root.FindName("SimulationWindow") as HeaderedContentControl;
            var windowContent = window?.Content as FrameworkElement;
            return windowContent?.FindName("SimulationBoard") as SpeciesSimulationBoard;
        }
    }
}
