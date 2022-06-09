using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class Popup : Menu
    {
        public TouchButton popupButton;
  
        public override void Start()
        {
            SetPageName();
        }

    }
}

