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
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // LocalSaveManager.AddCollectable(stats,amount);
            //CloudSaveManager.Instance.AddCollectable(state,amount);

            //if(Enum.TryParse(stats.ToString(), out GemType gemType))
            //    CloudSaveManager.Instance.AddGem(gemType,this.amount);

            LocalSaveManager.Instance.AddCollectable(stats, this.amount);

        }
    }
}



