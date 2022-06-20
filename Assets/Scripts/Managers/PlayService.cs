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

        public void LoginGooglePlayGames()
        {
            Social.localUser.Authenticate(OnGooglePlayGamesLogin);
        }

        void OnGooglePlayGamesLogin(bool success)
        {
            signIn = success;

            if (success)
            {
                // Call Unity Authentication SDK to sign in or link with Google.
                Debug.Log("Login with Google Play Games done. IdToken: " + ((PlayGamesLocalUser)Social.localUser).GetIdToken());

            }
            else
            {
                Debug.Log("Unsuccessful login");
            }

            Debug.Log("Login with Google Play Games done. IdToken: " + ((PlayGamesLocalUser)Social.localUser).GetIdToken());
        }

    }
}

