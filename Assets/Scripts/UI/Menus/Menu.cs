using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public abstract class Menu : MonoBehaviour
    {
        #region Collections
        public enum Menus
        {
            None,
            Lobby,
            Settings,
            Shop,
            Customize,
            Stages,
            Victory,
            Defeat,
            Play,
            Pause,
            PlayService,
            PrivacyPolicy,
            Languages,
            Credits,
            CharacterPrevious,
            Splash,
            Loading,
            LevelPrevious,
            Upgrade,

            // Popups
            PurchaseProcess,
            Waiting,
            StageUpProcess,
            ConnectedError,
            Disconnect,
            AppUpdate,
            StageLockOrientation,
            ShopOrientation,
            LuckySpin
 
        }
        public enum MenuRank
        {
            Primer,
            Seconder
        }
        public static Dictionary<Menus, string> MenuPaths = new Dictionary<Menus, string>
        {
             {Menus.Lobby, "Menus/Lobby"},
             {Menus.Settings, "Menus/Settings"},
             {Menus.Shop, "Menus/Shop"},
             {Menus.Customize, "Menus/Characters"},
             {Menus.Stages, "Menus/Stages"},
             {Menus.Victory, "Menus/Victory"},
             {Menus.Defeat, "Menus/Defeat"},
             {Menus.Pause, "Menus/Pause"},
             {Menus.Languages, "Menus/Languages"},
             {Menus.CharacterPrevious, "Menus/ProductInfo"},
             {Menus.Splash, "Menus/Splash"},
             {Menus.Loading, "Menus/Loading"},
             {Menus.LevelPrevious, "Menus/LevelPopup"},
             {Menus.LuckySpin, "Menus/LuckySpin"},

            //Popups
             {Menus.PurchaseProcess, "Popups/PurchaseProcessPopup"},
             {Menus.Waiting, "Popups/WaitingPopup"},
             {Menus.StageUpProcess, "Popups/StageUpProcessPopup"},
             {Menus.ConnectedError, "Popups/ConnectedErrorPopup"},
             {Menus.Disconnect, "Popups/DisconnectPopup"},
             {Menus.AppUpdate, "Popups/AppUpdatePopup"},
             {Menus.StageLockOrientation, "Popups/StageLockOrientationPopup"},
             {Menus.ShopOrientation, "Popups/ShopOrientationPopup"}
        };

        #endregion
        #region Fields       
        [Header("Components")]
        public Canvas canvas;
        public TMP_Text pageName;
        public RectTransform baseTransform;
        private float duration = 0.1f;
        [Header("Referances")]
        public Menus menuType;
        public MenuRank menuRank;
        #endregion
        #region virtual Methods
        public virtual void Start()
        {
            SetPageName();
            LanguageManager.onChangedLanguage += SetPageName;
        }
        public virtual void ActivateBase()
        {
            baseTransform.gameObject.SetActive(true);
        }
        public virtual void SetPageName()
        {
            if (pageName != null)
                pageName.text = LanguageManager.GetText(menuType.ToString());
        }
        public virtual void GoBack()
        {
            UIManager.Instance.GoBack();
        }
        public virtual void Cancel()
        {
            UIManager.Instance.Cancel();
        }
     
        public virtual IEnumerator Animation()
        {
            if (baseTransform == null)
                yield break;

            float time = 0;
            int step = 0;
            Vector2 start = baseTransform.localScale;
            Vector2 end = 1.04f * Vector2.one;

            while (step<2)
            {
                while (time <= 1)
                {
                    time += Time.deltaTime / (duration * 0.5f);
                    baseTransform.localScale = Vector2.Lerp(start, end, time);
                    yield return null;
                }

                step++;
                start = baseTransform.localScale;
                end = Vector2.one;
                time = 0;
            }
        }
        #endregion


        public virtual void OnEnable()
        {
            StartCoroutine(Animation());
        }
    }
}

