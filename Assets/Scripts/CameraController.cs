using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class CameraController : MonoBehaviour
    {
        void Start()
        {
            UIManager.Instance.MainCamera = GetComponent<Camera>();
        }

    }
}

