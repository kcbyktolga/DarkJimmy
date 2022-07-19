using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

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
            LuckySpin,

            //Social Platform
            Share,
            InstagramURL,
            YoutubeURL,
            AppURL,
            PublisherURL,
            Mail,

            Preparation,
            TapToStart,
            Policy,
            CharacterPurchase,
            RateGame,
            RewardPopup,
        }
        public enum MenuRank
        {
            Primer,
            Seconder
        }
        public static readonly Dictionary<Menus, string> MenuPaths = new Dictionary<Menus, string>
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
             {Menus.LevelPrevious, "Menus/LevelPrevious"},
             {Menus.LuckySpin, "Menus/LuckySpin"},
             {Menus.Play, "Menus/GameDisplay"},
             {Menus.Preparation, "Menus/Preparation"},
             {Menus.TapToStart, "Menus/Starter"},
             {Menus.Upgrade, "Menus/Upgrade"},

            //Popups
             {Menus.PurchaseProcess, "Popups/PurchaseProcessPopup"},
             {Menus.Waiting, "Popups/WaitingPopup"},
             {Menus.StageUpProcess, "Popups/StageUpProcessPopup"},
             {Menus.ConnectedError, "Popups/ConnectedErrorPopup"},
             {Menus.Disconnect, "Popups/DisconnectPopup"},
             {Menus.AppUpdate, "Popups/AppUpdatePopup"},
             {Menus.StageLockOrientation, "Popups/StageLockOrientationPopup"},
             {Menus.ShopOrientation, "Popups/ShopOrientationPopup"},
             {Menus.CharacterPurchase, "Popups/CharacterPurchasePopup"},
             {Menus.PrivacyPolicy, "Popups/PrivacyPolicy"},
             {Menus.Policy, "Popups/Policy"},
             {Menus.RateGame, "Popups/RateGamePopup"},
             {Menus.RewardPopup, "Popups/RewardPopup"}
        };

        #endregion
        #region Fields       
        [Header("Components")]
        public Canvas canvas;
        public TMP_Text pageName;
        public RectTransform baseTransform;
        
        public  float Duration { get;} = 0.1f;
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
   
        public virtual void ScaleAnimation()
        {
            if (baseTransform == null)
                return;

            baseTransform.localScale = 0.95f * Vector2.one;
            baseTransform.DOScale(Vector2.one, Duration).SetEase(SystemManager.Instance.GetMenuCurve()).OnComplete(AfterScaleAnimation);     
        }

        public virtual void AfterScaleAnimation()
        {
            baseTransform.DOKill();
        }
   
        #endregion

        public virtual void OnEnable()
        {         
            ScaleAnimation();
        }
        public virtual void OnDestroy()
        {
            LanguageManager.onChangedLanguage -= SetPageName;
        }

        public virtual void SetVolume(bool isOn)
        {
            if (AudioManager.Instance == null)
                return;

            //audioManager.SourceFadeVolume(SoundGroupType.Music, isOn);
            // audioManager.SourceFadeVolume(SoundGroupType.Ambient, isOn);

            AudioManager.Instance.FadeVolume(isOn);
        }
    }
}

