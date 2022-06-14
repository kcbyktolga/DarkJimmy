using UnityEngine;
using UnityEngine.Advertisements;

namespace DarkJimmy
{
    public class UnityAdsManager: AdManager, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [Header("Unity Ads")]
        [SerializeField]
        private AdsUnit InterstitialAd;
        [SerializeField]
        private AdsUnit RewardedAd;

        public override void InitializeAds()
        {            
            Advertisement.Initialize(GameId, TestMode, this);
        }

        private void Start()
        {
            LoadInterstitialAd();
        }
        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");
        }
        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error} - {message}");
        }
        public void OnUnityAdsAdLoaded(string placementId)
        {
            throw new System.NotImplementedException();
        }
        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            throw new System.NotImplementedException();
        }
        public void OnUnityAdsShowClick(string placementId)
        {
            throw new System.NotImplementedException();
        }
        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            throw new System.NotImplementedException();
        }
        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            throw new System.NotImplementedException();
        }
        public void OnUnityAdsShowStart(string placementId)
        {
            throw new System.NotImplementedException();
        }

        public void LoadInterstitialAd()
        {
            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
            Debug.Log("Loading Ad: " + InterstitialAd.AdUnit);
            Advertisement.Load(InterstitialAd.AdUnit, this);

            Invoke(nameof(ShowAd),5);
        }

        // Show the loaded content in the Ad Unit:
        public void ShowAd()
        {
            // Note that if the ad content wasn't previously loaded, this method will fail
            Debug.Log("Showing Ad: " + InterstitialAd.AdUnit);
            Advertisement.Show(InterstitialAd.AdUnit, this);
        }

    }

 


}

