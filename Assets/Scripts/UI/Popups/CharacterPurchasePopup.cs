using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class CharacterPurchasePopup : Popup
    {
        public override void Start()
        {
            base.Start();

            if (popupButton != null)
                popupButton.SetName(LanguageManager.GetText("Okay"));
        }
  
    }

}
