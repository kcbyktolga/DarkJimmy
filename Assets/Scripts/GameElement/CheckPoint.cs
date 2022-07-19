using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Objects
{
    public class CheckPoint : Interactable
    {
        public bool IsPassed { get; set; }

        private GameSaveManager gsm;

        private BoxCollider2D boxCollider;
        private void Start()
        {
            gsm = GameSaveManager.Instance;
            boxCollider = GetComponent<BoxCollider2D>();
            activate += SaveData;
        }

        private void SaveData()
        {
            IsPassed = true;
            boxCollider.enabled = !IsPassed;
            gsm.RegisterCheckPoint(this);
            gsm.SetElements(IsPassed);

        }

        public void RewindCheckPoint()
        {
            IsPassed = false;
            boxCollider.enabled = !IsPassed;
        }
    }
}

