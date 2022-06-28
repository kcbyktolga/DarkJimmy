using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace DarkJimmy
{
    public class Collectable : MonoBehaviour
    {
        [SerializeField]
        private Stats stats;
      
        [SerializeField]
        private int amount;

        SystemManager system;
        private void Start()
        {
            system = SystemManager.Instance;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            system.updateGMStats(stats,amount);         
        }
    }

    
}



