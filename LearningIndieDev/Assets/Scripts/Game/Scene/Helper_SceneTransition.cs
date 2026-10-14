using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaltyGame
{
    public sealed class Helper_SceneTransition : MonoBehaviour
    {
        [SerializeField] string labSceneName = "Lab";
        [SerializeField] string desktopSceneName = "GalapagOSDesktopTest";
        [SerializeField] string mainMenuSceneName = "MainMenu";
        [SerializeField] string simulationSceneName = "CellularAutomataPrototype";

        static SimulationLaunchRequest pendingSimulationLaunch;
        static string pendingDesktopProfileId;
        static bool pendingProfileSelection;

        public string LabSceneName => labSceneName;
        public string DesktopSceneName => desktopSceneName;
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

        public bool LoadDesktop(ProfileSessionSnapshot profile)
        {
            if (profile == null || !profile.HasLoadedProfile || string.IsNullOrEmpty(desktopSceneName))
            {
                return false;
            }

            pendingDesktopProfileId = profile.ProfileId;
            SceneManager.LoadScene(desktopSceneName, LoadSceneMode.Single);
            return true;
        }

        public bool ReturnToProfileSelection()
        {
            if (string.IsNullOrEmpty(mainMenuSceneName))
            {
                return false;
            }

            pendingProfileSelection = true;
            SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
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

        public static bool TryConsumeDesktopProfile(out string profileId)
        {
            profileId = pendingDesktopProfileId;
            pendingDesktopProfileId = null;
            return !string.IsNullOrEmpty(profileId);
        }

        public static bool ConsumeProfileSelectionRequest()
        {
            var requested = pendingProfileSelection;
            pendingProfileSelection = false;
            return requested;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetPendingLaunch()
        {
            pendingSimulationLaunch = null;
            pendingDesktopProfileId = null;
            pendingProfileSelection = false;
        }
    }
}
