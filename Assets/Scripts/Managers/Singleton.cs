using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkJimmy
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {     
        public static T Instance;
    }
}


