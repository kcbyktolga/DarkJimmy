using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class ShopOreintation : Popup
    {
       private UIManager u�Manager;
       private SystemManager system;
        public override void Start()
        {
            u�Manager = UIManager.Instance;
            system = SystemManager.Instance;

            base.Start();
            
            if (popupButton != null)
                popupButton.SetName(LanguageManager.GetText("Go"));

            popupButton.OnClick(Go);
        }
        private void Go()
        {
            GoBack();

            //u�Manager.PageIndex = system.GetShopPage(type.ToString());

            if (u�Manager.GetCurrentMenu() is Shop)
                system.toPage(u�Manager.PageIndex);
            else
                u�Manager.OpenMenu(Menus.Shop);
        }
    }

}
