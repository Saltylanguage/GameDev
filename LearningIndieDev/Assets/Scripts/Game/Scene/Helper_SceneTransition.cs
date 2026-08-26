using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaltyGame
{
    public sealed class Helper_SceneTransition : MonoBehaviour
    {
        [SerializeField] string labSceneName = "Lab";
        [SerializeField] string mainMenuSceneName = "MainMenu";

        public string LabSceneName => labSceneName;
        public string MainMenuSceneName => mainMenuSceneName;

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
    }
}
