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
            OnClick(pageIndex,OpenPage);
        }

        void OpenPage(int index)
        {
            UIManager.Instance.PageIndex = pageIndex;
            UIManager.Instance.Open(Menu.Menus.Shop);
        }   
    }
}

