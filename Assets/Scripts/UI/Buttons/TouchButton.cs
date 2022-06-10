using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class TouchButton : BaseButton
    {
        [Header("Touch Button Property")]
        [SerializeField]
        private Menu.Menus menuType;

        private void Awake()
        {
            OnClick(OpenPage);
            SetTouchButtonName();
            LanguageManager.onChangedLanguage += SetTouchButtonName;
        }
       
        public override void OpenPage()
        {
            if (menuType.Equals(Menu.Menus.None))
            {
                if (SceneManager.GetActiveSceneName().Equals(Menu.Menus.Stages.ToString())&&UIManager.Instance.GetStackCount().Equals(1))
                    SceneManager.LoadScene(Menu.Menus.Lobby.ToString());
                else
                    UIManager.Instance.GoBack();
            }             
            else if (menuType.Equals(Menu.Menus.PlayService))
                PlayService.Instance.LoginGooglePlayGames();
            else if (menuType.Equals(Menu.Menus.Stages))
                SceneManager.LoadScene(menuType.ToString());
            else
                UIManager.Instance.OpenMenu(menuType);
        }           

        private void SetTouchButtonName()
        {
            string name = menuType.Equals(Menu.Menus.None) ? string.Empty : LanguageManager.GetText(menuType.ToString());

            SetTabButtonName(name);
        }

        public void SetName(string name)
        {
            if (buttonName != null)
                buttonName.text = name;
        }

    }

}
