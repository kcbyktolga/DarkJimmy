using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public abstract class Menu : MonoBehaviour
    {
        public enum Menus
        {
            None,
            Lobby, 
            Settings,
            Shop,
            Characters,
            Stages,
            Victory,
            Defeat,
            Play,
            Pause,
            PlayService,
            PrivacyPolicy,
            Language,
            Credits
        }
        public static Dictionary<Menus, string> MenuPaths = new Dictionary<Menus, string>
        {
             {Menus.Lobby, "Menus/Lobby"},
             {Menus.Settings, "Menus/Settings"},
             {Menus.Shop, "Menus/Shop"},
             {Menus.Characters, "Menus/Characters"},
             {Menus.Stages, "Menus/Stages"},
             {Menus.Victory, "Menus/Victory"},
             {Menus.Defeat, "Menus/Defeat"},
             {Menus.Pause, "Menus/Pause"},
        };

        #region Fields       
        [HideInInspector]
        public Animator animator;
        [HideInInspector]
        public int onParamId;

        [Header("Components")]
        public Canvas canvas;
        public CanvasScaler canvasScaler;
        public TMP_Text pageName;

        [Header("Referances")]
        public Menus menuType;

        #endregion
        #region virtual Methods
   
        public virtual void Awake()
        {
            animator = GetComponent<Animator>();
            onParamId = Animator.StringToHash("On");   
        }
        public virtual void Start()
        {
            canvas.renderMode = menuType.Equals(Menus.Lobby) || menuType.Equals(Menus.Play) ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;

            if (menuType.Equals(Menus.Lobby) || menuType.Equals(Menus.Play))
                canvas.worldCamera = UIManager.Instance.MainCamera;

            canvas.planeDistance = 10f;
            canvasScaler.referenceResolution = UIManager.Instance.GetReferenceResolotion();
        }
        public virtual void GoBack()
        {
            UIManager.Instance.GoBack();
        }
        #endregion
    }
}

