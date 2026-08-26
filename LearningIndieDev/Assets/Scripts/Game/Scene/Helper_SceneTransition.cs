using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaltyGame
{
    public sealed class Helper_SceneTransition : MonoBehaviour
    {
        [SerializeField] string labSceneName = "Lab";
        [SerializeField] string mainMenuSceneName = "MainMenu";
        [SerializeField] string simulationSceneName = "CellularAutomataPrototype";

        static SimulationLaunchRequest pendingSimulationLaunch;

        public string LabSceneName => labSceneName;
        public string MainMenuSceneName => mainMenuSceneName;
        public string SimulationSceneName => simulationSceneName;

        public bool LoadLab(ProfileSessionSnapshot profile)
        {
            if (profile == null || !profile.HasLoadedProfile || string.IsNullOrEmpty(labSceneName))
            {
                return false;
            }

            SceneManager.LoadScene(labSceneName, LoadSceneMode.Single);
            return true;
        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        public bool LoadMainMenu()
        {
            if (string.IsNullOrEmpty(mainMenuSceneName))
            {
                return false;
            }

            SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
            return true;
        }

        public bool LoadSimulation(SimulationLaunchRequest launch)
        {
            if (launch == null || string.IsNullOrEmpty(simulationSceneName))
            {
                return false;
            }

            pendingSimulationLaunch = launch;
            SceneManager.LoadScene(simulationSceneName, LoadSceneMode.Single);
            return true;
        }

        public static bool TryConsumeSimulationLaunch(out SimulationLaunchRequest launch)
        {
            launch = pendingSimulationLaunch;
            pendingSimulationLaunch = null;
            return launch != null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetPendingLaunch()
        {
            pendingSimulationLaunch = null;
        }
    }
}
