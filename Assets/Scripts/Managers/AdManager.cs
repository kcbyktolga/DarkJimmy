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
        //[SerializeField] 
        //private string androidGameId;
        //[SerializeField] 
        //private string iOSGameId;
        //[SerializeField] 
        //private bool testMode = true;
        [Header("Ads Units")]
        [SerializeField]
        private AdsUnit Banner;
        [SerializeField]
        private AdsUnit Interstitial;
        [SerializeField]
        private AdsUnit MREC;
        [SerializeField]
        private List<AdsUnit> Rewardeds;


        private bool isBannerShowing;
        private bool isMRecShowing;

        private int interstitialRetryAttempt;
        private int rewardedRetryAttempt;
        private int rewardedInterstitialRetryAttempt;

        //private string GameId
        //{
        //    get { return (Application.platform == RuntimePlatform.IPhonePlayer) ? iOSGameId : androidGameId; }
        //}

        private Dictionary<string, RewardType> GetRewardType = new Dictionary<string, RewardType>();
        public delegate void RewardOnEarned(RewardType reward);
        public RewardOnEarned rewardDelegate;
        public override void Awake()
        {
            base.Awake();

            InitializeAds();
           
        }
        public void InitializeAds()
        {

            MaxSdk.SetSdkKey(MaxSdkKey);
            MaxSdk.InitializeSdk();

            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                // AppLovin SDK is initialized, configure and start loading ads.
                Debug.Log("MAX SDK Initialized");

                InitializeInterstitialAds();
                InitializeRewardedAds();
                //InitializeRewardedInterstitialAds();
                InitializeBannerAds();
                //InitializeMRecAds();
            };

           
        }

        #region Banner Ad 
        private void InitializeBannerAds()
        {
            // Attach Callbacks
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
            // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
            MaxSdk.CreateBanner(Banner.AdUnit, MaxSdkBase.BannerPosition.TopCenter);

            // Set background or background color for banners to be fully functional.
            MaxSdk.SetBannerBackgroundColor(Banner.AdUnit, SystemManager.Instance.GetWhiteAlfaColor(false));
        }
        public void ToggleBannerVisibility()
        {
            if (!isBannerShowing)
                MaxSdk.ShowBanner(Banner.AdUnit);
            else
                MaxSdk.HideBanner(Banner.AdUnit);

            isBannerShowing = !isBannerShowing;
        }
        public void ToggleBannerVisibility(bool isOn)
        {
            if (isOn)
                MaxSdk.ShowBanner(Banner.AdUnit);
            else
                MaxSdk.HideBanner(Banner.AdUnit);
        }
        private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Banner ad is ready to be shown.
            // If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
            Debug.Log("Banner ad loaded");
        }
        private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Banner ad failed to load. MAX will automatically try loading a new ad internally.
            Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
        }
        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Banner ad clicked");
        }
        private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Banner ad revenue paid. Use this callback to track user revenue.
            Debug.Log("Banner ad revenue paid");

            // Ad revenue
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

           // TrackAdRevenue(adInfo);
        }
        #endregion

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
            if (MaxSdk.IsInterstitialReady(Interstitial.AdUnit))
                MaxSdk.ShowInterstitial(Interstitial.AdUnit);
        }
        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Interstitial loaded");

            // Reset retry attempt
            interstitialRetryAttempt = 0;
        }
        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

            Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);

            Invoke(nameof(LoadInterstitial), (float)retryDelay);
        }
        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            Debug.Log("Interstitial failed to display with error code: " + errorInfo.Code);
            LoadInterstitial();
        }
        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad
            Debug.Log("Interstitial dismissed");
            LoadInterstitial();
        }
        private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad revenue paid. Use this callback to track user revenue.
            Debug.Log("Interstitial revenue paid");

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
        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
            Debug.Log("Rewarded ad loaded");

            // Reset retry attempt
            rewardedRetryAttempt = 0;
        }
        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));

      
            Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);

            // Invoke(nameof(LoadRewardedAd(adUnitId)), (float)retryDelay);

            LoadRewardedAd(adUnitId);
        }
        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
            LoadRewardedAd(adUnitId);
        }
        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad displayed");
        }
        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad clicked");
        }
        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            Debug.Log("Rewarded ad dismissed");
            LoadRewardedAd(adUnitId);
        }
        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad was displayed and user should receive the reward
            Debug.Log("Rewarded ad received reward");

            Debug.Log(GetRewardType[adUnitId]);
            rewardDelegate(GetRewardType[adUnitId]);
        }
        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad revenue paid. Use this callback to track user revenue.
            Debug.Log("Rewarded ad revenue paid");

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

        #region MREC Ad Methods

        private void InitializeMRecAds()
        {

            // Attach Callbacks
            MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
            MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdFailedEvent;
            MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;

            // MRECs are automatically sized to 300x250.
            MaxSdk.CreateMRec(MREC.AdUnit, MaxSdkBase.AdViewPosition.BottomCenter);

            //MRECSetPosition(SystemManager.Instance.mrecPosition);

        }

        private void MRECSetPosition(Vector2 position)
        {
            //MaxSdk.UpdateMRecPosition(MREC.AdUnit, position.x, position.y);
            MaxSdk.CreateMRec(MREC.AdUnit, position.x, position.y);
        }
        public void ToggleMRecVisibility()
        {

            if (!isMRecShowing)
                MaxSdk.ShowMRec(MREC.AdUnit);
            else
                MaxSdk.HideMRec(MREC.AdUnit);

    
            isMRecShowing = !isMRecShowing;
        }
        private void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // MRec ad is ready to be shown.
            // If you have already called MaxSdk.ShowMRec(MRecAdUnitId) it will automatically be shown on the next MRec refresh.
            Debug.Log("MRec ad loaded");
        }
        private void OnMRecAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // MRec ad failed to load. MAX will automatically try loading a new ad internally.
            Debug.Log("MRec ad failed to load with error code: " + errorInfo.Code);
        }
        private void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("MRec ad clicked");
        }
        private void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // MRec ad revenue paid. Use this callback to track user revenue.
            Debug.Log("MRec ad revenue paid");

            // Ad revenue
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

           // TrackAdRevenue(adInfo);
        }

        #endregion

        private void OnDestroy()
        {
            Destroy(MaxSdkCallbacks.Instance);
        }

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

