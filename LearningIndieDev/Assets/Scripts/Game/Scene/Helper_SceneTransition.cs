using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaltyGame
{
    public sealed class Helper_SceneTransition : MonoBehaviour
    {
        [SerializeField] string labSceneName = "Lab";

        public string LabSceneName => labSceneName;

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
    }
}
