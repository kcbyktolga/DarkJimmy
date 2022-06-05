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

            infoText.text = $"Unity user id: {CloudSaveManager.Instance.userID}, GPGS user id:{((PlayGamesLocalUser)Social.localUser).GetIdToken()} , Sign in status: {PlayService.Instance.signIn}"; 
        }
    }
}

