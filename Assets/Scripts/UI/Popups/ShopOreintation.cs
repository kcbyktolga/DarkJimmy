using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class ShopOreintation : Popup
    {     
        public override void Start()
        {
            base.Start();

            if (popupButton != null)
                popupButton.SetName(LanguageManager.GetText("Go"));

            popupButton.OnClick(CloudSaveManager.Instance.GemType,Go);
        }

        private void Go(GemType type )
        {
            GoBack();
            UIManager.Instance.PageIndex = GetPage(type);
            UIManager.Instance.OpenMenu(Menus.Shop);
        }
        private int GetPage(GemType gemType)
        {
            return gemType switch
            {
                GemType.Diamond => 3,
                _ => 2,
            };
        }

    }

}
