using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GooglePlayGames;

namespace DarkJimmy.UI
{
    public class Lobby : Menu
    {
        [SerializeField]
        private TMP_Text infoText;
        public override void Start()
        {
            base.Start();

            infoText.text = $"Unity user id: {CloudSaveManager.Instance.UserId}, GPGS user id:{((PlayGamesLocalUser)Social.localUser).GetIdToken()} , Sign in status: {PlayService.Instance.signIn}";

          //  AdManager.Instance.ToggleBannerVisibility(true);
           // AdManager.Instance.ToggleMRecVisibility();

           // Invoke(nameof(ShowInterstitial), 5);
        }

        void ShowInterstitial()
        {
            AdManager.Instance.ShowInterstitial();
        }
    }
}

