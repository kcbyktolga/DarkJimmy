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

            popupButton.OnClick(SystemManager.Instance.GemType,Go);
        }
        private void Go(GemType type )
        {
            GoBack();
            UIManager.Instance.PageIndex = SystemManager.Instance.GetShopPage(type.ToString());
            UIManager.Instance.OpenMenu(Menus.Shop);
        }
    }

}
