using Unity;
using UnityEngine;
using System;
namespace DarkJimmy
{
    public static class SceneManager 
    {
        public delegate void OnChangedScene();
        public static OnChangedScene onChangedScene;

        //static SceneManager()
        //{
        //    UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ChangedScene;
        //}
        public static void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            
            //onChangedScene();
        }      
        public static string GetActiveSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
        //private static void ChangedScene(UnityEngine.SceneManagement.Scene a, UnityEngine.SceneManagement.Scene b)
        //{
        //    onChangedScene();
        //}
    }
}

