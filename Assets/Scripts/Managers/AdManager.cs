using System;
using UnityEngine;
using System.Collections.Generic;

namespace DarkJimmy
{
    public class AdManager : Singleton<AdManager>
    {
        [Header("AdManager Property")]
        [SerializeField]
        private string MaxSdkKey;
        [Header("Ads Units")]
        [SerializeField]
        private AdsUnit Banner;
        [SerializeField]
        private AdsUnit Interstitial;
        [SerializeField]
        private List<AdsUnit> Rewardeds;

        private bool isBannerShowing;

        private int interstitialRetryAttempt;
        private int rewardedRetryAttempt;
        private readonly int rewardedInterstitialRetryAttempt;

        private readonly Dictionary<string, RewardType> GetRewardType = new Dictionary<string, RewardType>();
        public delegate void RewardOnEarned(RewardType reward);
        public RewardOnEarned rewardDelegate;

        public delegate void OnClosedAd();
        public OnClosedAd onClosed;
        public override void Awake()
        {
            base.Awake();
            InitializeAds();
        }

     
        public void InitializeAds()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                InitializeInterstitialAds();
                InitializeRewardedAds();
            };

            MaxSdk.SetSdkKey(MaxSdkKey);
            MaxSdk.InitializeSdk();
        }
        private bool CanShowInstersititalAd()
        {
            if (CloudSaveManager.Instance.PlayerDatas.HasRemoveAds)
                return false;

            if (DateTime.Now > LocalSaveManager.GetResetTime("Interstitial"))
                return true;
            else
                return false;

        }

        #region Interstitial Ad
        private void InitializeInterstitialAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

            // Load the first interstitial
            LoadInterstitial();
        }
        void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(Interstitial.AdUnit);
        }
        public void ShowInterstitial()
        {
            if (!CanShowInstersititalAd())
            {
                onClosed();
                return;
            }
               
            if (MaxSdk.IsInterstitialReady(Interstitial.AdUnit))
                MaxSdk.ShowInterstitial(Interstitial.AdUnit);

            DateTime time = DateTime.Now.AddMinutes(CloudSaveManager.Instance.IntersitialAdFrequency);
            LocalSaveManager.Save("Interstitial", time);
        }
        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
           // Debug.Log("Interstitial loaded");

            // Reset retry attempt
            interstitialRetryAttempt = 0;
        }
        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

           // Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);

            Invoke(nameof(LoadInterstitial), (float)retryDelay);
        }
        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            //Debug.Log("Interstitial failed to display with error code: " + errorInfo.Code);
            LoadInterstitial();
        }
        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad
            //Debug.Log("Interstitial dismissed");
            onClosed();
            LoadInterstitial();
        }
        private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad revenue paid. Use this callback to track user revenue.
           // Debug.Log("Interstitial revenue paid");          
            // Ad revenue
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        }

        #endregion

        #region Rewarded Ad
        private string GetRewardUnitID(RewardType type)
        {
           return Rewardeds[(int)type].AdUnit;
        }
        private void InitializeRewardedAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

            // Load the All RewardedAd
            LoadAllRewardedAds();
        }
        private void LoadAllRewardedAds()
        {
            for (int i = 0; i < Rewardeds.Count; i++)
            {
                string adUnitId = Rewardeds[i].AdUnit;
                LoadRewardedAd(adUnitId);

                if (GetRewardType.ContainsKey(adUnitId))
                    continue;

                GetRewardType.Add(Rewardeds[i].AdUnit,(RewardType)i);
            }              
        }
        private void LoadRewardedAd(string adUnit)
        {
            if (string.IsNullOrEmpty(adUnit))
                return;

            MaxSdk.LoadRewardedAd(adUnit);
        }
        public void ShowRewardedAd(RewardType rewardType, RewardOnEarned reward)
        {
            string adUnitId = GetRewardUnitID(rewardType);

            if (string.IsNullOrEmpty(adUnitId))
                return;

            Instance.rewardDelegate = reward;

            if (MaxSdk.IsRewardedAdReady(adUnitId))
                MaxSdk.ShowRewardedAd(adUnitId);

        }

        public bool RewardAdReady(RewardType rewardType)
        {
            string adUnitId = GetRewardUnitID(rewardType);

            if (string.IsNullOrEmpty(adUnitId))
                return false;

            return MaxSdk.IsRewardedAdReady(adUnitId);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
            //Debug.Log("Rewarded ad loaded");

            // Reset retry attempt
            rewardedRetryAttempt = 0;
        }
        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));

      
            //Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);

            // Invoke(nameof(LoadRewardedAd(adUnitId)), (float)retryDelay);

            LoadRewardedAd(adUnitId);
        }
        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            //Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
            LoadRewardedAd(adUnitId);
        }
        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            //Debug.Log("Rewarded ad displayed");
        }
        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            //Debug.Log("Rewarded ad clicked");
        }
        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
          //  Debug.Log("Rewarded ad dismissed");
            LoadRewardedAd(adUnitId);
        }
        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad was displayed and user should receive the reward
            //Debug.Log("Rewarded ad received reward");

            rewardDelegate(GetRewardType[adUnitId]);
        }
        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad revenue paid. Use this callback to track user revenue.
          //  Debug.Log("Rewarded ad revenue paid");

            // Ad revenue
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

            //TrackAdRevenue(adInfo);
        }

        #endregion
    }
    [Serializable]
    public class AdsUnit
    {
        public string AndroidAdUnitId = string.Empty;
        public string IOSAdUnitId = string.Empty;
        public string AdUnit
        {
            get
            {            
                return ((Application.platform == RuntimePlatform.IPhonePlayer) ? IOSAdUnitId : AndroidAdUnitId).Trim();
            }
        }
    }
}

