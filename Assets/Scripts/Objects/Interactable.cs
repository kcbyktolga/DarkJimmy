using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Objects
{
    public abstract class Interactable : MonoBehaviour
    {
        public delegate void ActivateObject();
        public ActivateObject activate;
        public Animator animator;
 
        private void OnTriggerEnter2D(Collider2D collision)
        {
            activate();
        }

    }
}

