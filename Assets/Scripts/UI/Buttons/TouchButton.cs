using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DarkJimmy.Manager;


namespace DarkJimmy.UI
{
    public class TouchButton : BaseButton
    {
        [Header("Touch Button Property")]
        [SerializeField]
        private Menu.Menus menuType;
 
        private void Start()
        {
            buttonName.text =  menuType.Equals(Menu.Menus.None) ? string.Empty : LanguageManager.GetText(menuType.ToString());
            OnClick(OpenPage);
        }
        public override void OpenPage()
        {
            if (menuType.Equals(Menu.Menus.None))
                UIManager.Instance.GoBack();
            else
                UIManager.Instance.Open(menuType);
        }           
    }

}
