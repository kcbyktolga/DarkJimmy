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
            OnClick(OpenPage);
            SetTouchButtonName();
            LanguageManager.onChangedLanguage += SetTouchButtonName;
        }
        public override void OpenPage()
        {
            if (menuType.Equals(Menu.Menus.None))
                UIManager.Instance.GoBack();
            else
                UIManager.Instance.Open(menuType);
        }           

        private void SetTouchButtonName()
        {
            string name = menuType.Equals(Menu.Menus.None) ? string.Empty : LanguageManager.GetText(menuType.ToString());

            SetTabButtonName(name);
        }
    }

}
