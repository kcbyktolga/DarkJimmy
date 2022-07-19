using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class ShopOreintation : Popup
    {
       private UIManager uıManager;
       private SystemManager system;
        public override void Start()
        {
            uıManager = UIManager.Instance;
            system = SystemManager.Instance;

            base.Start();
            
            if (popupButton != null)
                popupButton.SetName(LanguageManager.GetText("Go"));

            popupButton.OnClick(Go);
        }
        private void Go()
        {
            GoBack();

            //uıManager.PageIndex = system.GetShopPage(type.ToString());

            if (uıManager.GetCurrentMenu() is Shop)
                system.toPage(uıManager.PageIndex);
            else
                uıManager.OpenMenu(Menus.Shop);
        }
    }

}
