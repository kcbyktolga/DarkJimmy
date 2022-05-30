using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace DarkJimmy.UI
{
    public class TouchButton : BaseButton
    {
        [Header("Touch Button Property")]
        [SerializeField]
        private Menu.Menus menuType;
        [SerializeField]
        private TMP_Text buttonName;

        private void Start()
        {
            buttonName.text =  menuType.Equals(Menu.Menus.None) ? string.Empty : menuType.ToString();
            OnClick(Open);
        }
        private void Open()
        {
            if (menuType.Equals(Menu.Menus.None))
                UIManager.Instance.GoBack();
            else
                UIManager.Instance.Open(menuType);
        }      
       
    }

}
