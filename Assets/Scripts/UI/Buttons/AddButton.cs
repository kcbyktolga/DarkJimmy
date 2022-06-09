using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class AddButton : BaseButton
    {
        public int pageIndex = 0;

        void Start()
        {
            OnClick(OpenPage);
        }

        public override void OpenPage()
        {
            UIManager.Instance.PageIndex = pageIndex;
            UIManager.Instance.OpenMenu(Menu.Menus.Shop);
        }   
    }
}

