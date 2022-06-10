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
                popupButton.SetName(LanguageManager.GetText("Update"));

            popupButton.OnClick(CloudSaveManager.Instance.AplicationURL,OpenUrl);

        }

        private void OpenUrl(string address)
        {
            UIManager.Instance.OpenUrl(address);

            if (Application.isEditor)
                GoBack();
            else
                Application.Quit();
        }
    }

}
