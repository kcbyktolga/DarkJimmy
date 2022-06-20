using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class Disconnect : Popup
    {
        Loading loading;
        public override void Start()
        {
            base.Start();

            if (popupButton != null)
                popupButton.SetName(LanguageManager.GetText("Reconnect"));

            loading = FindObjectOfType<Loading>();

             popupButton.OnClick(Reconnect);
        }
        private void Reconnect()
        {
            CloudSaveManager.Instance.SignIn();
            string sentences = "Yeniden baðlanýlýyor..";
            loading.SetLoadingText(sentences);
            loading.Load(Menus.Lobby,1);
            GoBack();
        }

    }
}

