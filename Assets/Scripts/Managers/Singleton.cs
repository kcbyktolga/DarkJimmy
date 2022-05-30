using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkJimmy
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {     
        private enum SingletonType
        {
            Destroy,
            DontDestroy
        }
        [SerializeField]
        private SingletonType type;
        public static T Instance;


        //private void Awake()
        //{
        //    //if (Instance == null)
        //    //    SceneManager.activeSceneChanged += Test;

        //    if (type.Equals(SingletonType.DontDestroy))
        //    {
        //        if (Instance == null)
        //            Instance = GetComponent<T>();
        //        else
        //            Destroy(gameObject);
        //    }
        //    else
        //        Instance = GetComponent<T>();
           
        //}
    }
}


