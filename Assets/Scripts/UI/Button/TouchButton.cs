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
            buttonName.text = menuType.ToString();
            OnClick(Open);
        }
        private void Open()
        {
            UIManager.Instance.Open(menuType);
        }      
       
    }

}
