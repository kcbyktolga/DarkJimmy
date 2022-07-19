using TMPro;
using UnityEngine;
using System;

namespace DarkJimmy.UI
{
    public class RateGamePopup : Popup
    {
        [SerializeField]
        private TouchButton noThanksButton;
        [SerializeField]
        private TMP_Text rateDescText;

        private SystemManager system;
        public override void Start()
        {
            base.Start();
            system = SystemManager.Instance;
            popupButton.SetName(LanguageManager.GetText("ILoveGame"));
            noThanksButton.SetName(LanguageManager.GetText("NoThanks"));

            popupButton.OnClick(OnRating);
            noThanksButton.OnClick(OnNoThanks);
        }

        public override void SetPageName()
        {
            // base.SetPageName();
            pageName.text = LanguageManager.GetText("EnjoyingThisGame");
            rateDescText.text = LanguageManager.GetText("RateGame");
        }
        private void OnRating()
        {
            UIManager.Instance.OpenUrl(system.GetUrlAddress("AppURL"));
            LocalSaveManager.Save("Rate", true);
            GoBack();
        }

        private void OnNoThanks()
        {
            DateTime time = DateTime.Now.AddMinutes(1);
            LocalSaveManager.Save("ResetRate", time);
            GoBack();
        }
    }
}

