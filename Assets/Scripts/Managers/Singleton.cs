using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkJimmy
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public SingletonType type;
        public static T Instance;

        public virtual void Awake()
        {
            if (type.Equals(SingletonType.DontDestroy))
            {
                if (Instance == null)
                {
                    Instance = GetComponent<T>();
                    DontDestroyOnLoad(Instance);
                }
                else
                    Destroy(this);
            }
            else
                Instance = GetComponent<T>();

        }
    }

    public enum SingletonType
    {
        DontDestroy,
        Destroy
    }

}


