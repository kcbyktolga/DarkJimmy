using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class AppUpdatePopup : Popup
    {
        public override void Start()
        {
            base.Start();

            if (popupButton != null)
                popupButton.Name = LanguageManager.GetText("Update");

            popupButton.OnClick(CloudSaveManager.Instance.AplicationURL,UIManager.Instance.OpenUrl);
        }
    }

}
