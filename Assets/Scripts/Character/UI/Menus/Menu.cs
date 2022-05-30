using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DarkJimmy.UI
{
    public abstract class Menu : MonoBehaviour
    {
        public enum Menus
        {
            None,
            MainMenu,
            Settings,
            Shop,
            Game,
            Pause
        }
        public static Dictionary<Menus, string> MenuPaths = new Dictionary<Menus, string>
        {
             {Menus.MainMenu, "Menus/MainMenu"},
             {Menus.Settings, "Menus/SettingsMenu"},
             {Menus.Shop, "Menus/ShopMenu"},
             {Menus.Pause, "Menus/PauseMenu"},
             {Menus.Game, "Menus/GameDisplay"},

        };

        #region Fields       
        [HideInInspector]
        public Animator animator;
        [HideInInspector]
        public int onParamId;

        [Header("Components")]
        public Transform[] buttonContainer;
        public Canvas canvas;
        public TMP_Text header;

        [Header("Referances")]
        //public MenuStruct menuStruct;
        public Menus menuType;

        #endregion
        #region virtual Methods
        public abstract void GenerateButtons();
        public virtual void Awake()
        {
            animator = GetComponent<Animator>();
            onParamId = Animator.StringToHash("On");

        }
        public virtual void GoBack()
        {
            UIManager.Instance.GoBack();
        }
        #endregion

    }
}

