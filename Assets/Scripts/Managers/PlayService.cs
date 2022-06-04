using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames;

namespace DarkJimmy
{
    public class PlayService : Singleton<PlayService>
    {
        public bool signIn;

        void Start()
        {
            InitializePlayGamesLogin();
            LoginGooglePlayGames();
        }
        void InitializePlayGamesLogin()
        {
            var config = new PlayGamesClientConfiguration.Builder()
                // Requests an ID token be generated.  
                // This OAuth token can be used to
                // identify the player to other services such as Firebase.
                .RequestIdToken()
                .Build();

            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

  
        }

        void LoginGooglePlayGames()
        {
            Social.localUser.Authenticate(OnGooglePlayGamesLogin);
        }

        void OnGooglePlayGamesLogin(bool success)
        {
            if (success)
            {
                signIn = true;
                // Call Unity Authentication SDK to sign in or link with Google.
                Debug.Log("Login with Google Play Games done. IdToken: " + ((PlayGamesLocalUser)Social.localUser).GetIdToken());

                //CloudSaveManager.Instance
            }
            else
            {
                Debug.Log("Unsuccessful login");
            }
        }

    }
}

