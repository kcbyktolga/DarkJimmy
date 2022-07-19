using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace DarkJimmy.Objects
{
    public class Platform : MonoBehaviour
    {     
        private CinemachineConfiner confiner;

        private void Start()
        {
            confiner = FindObjectOfType<CinemachineConfiner>();
            confiner.m_BoundingShape2D = GetComponent <PolygonCollider2D>();
        }
    }

}
