using System.Collections;
namespace DarkJimmy
{
    public static class SceneManager 
    {
        public static void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        } 
        
        public static string GetActiveSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
    }
}

