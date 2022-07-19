using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace DarkJimmy.UI
{
    public class Shop : Menu
    {
        public override void Start()
        {
            base.Start();
        }
     
        public override void OnDestroy()
        {
            base.OnDestroy();

            SetVolume(true);          
        }

        public override void OnEnable()
        {
            base.OnEnable();

            SetVolume(false);
        }
    }

}

