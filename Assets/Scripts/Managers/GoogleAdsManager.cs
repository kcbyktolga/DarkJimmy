using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

namespace DarkJimmy
{
    public class GoogleAdsManager : Singleton<GoogleAdsManager>
    {
        [Header("Test Mod")]
        [SerializeField] 
        private bool testMode;
        [SerializeField]
        private string testDeviceId = string.Empty;    
        [Header("IDs")]
        [SerializeField]
        private AdsUnit InterstitialAd;
        [SerializeField]
        private AdsUnit LocalAdvancedAd;
        [SerializeField]
        private AdsUnit AppOpenAd;
        [SerializeField]
        private List<AdsUnit> RewardedAds;
        [Header("Other Settings")]
        [SerializeField]
        private bool targetForChildren = false;

        private float interstitialRrequestTimeout;
        private float rewardedRrequestTimeout;
        private float localAdvancedRrequestTimeout;
        private float appOpenRrequestTimeout;

        private IEnumerator showInterstitialCoroutine;
        private IEnumerator showRewardedVideoCoroutine;

        public delegate void RewardOnEarned(RewardType reward);
        public RewardOnEarned rewardDelegate;

        private string testIntersititialAdUnit = "ca-app-pub-3940256099942544/1033173712";
        private string testRewardedAdUnit      = "ca-app-pub-3940256099942544/5224354917";
        private string testLocalAdvancedAdUnit = "ca-app-pub-3940256099942544/2247696110";
        private string testAppOpendAdUnit      = "ca-app-pub-3940256099942544/3419835294";


        private InterstitialAd interstitial;
        private RewardedAd rewarded;
        private AppOpenAd appOpen;
        
        public bool TestMode { get { return testMode; } }

        private float interstitialAutoReloadTime  = float.PositiveInfinity;
        private float rewardedAutoReloadTime      = float.PositiveInfinity;
        private float localAdvancedAutoReloadTime = float.PositiveInfinity;
        private float appOpenAutoReloadTime       = float.PositiveInfinity;

        private RewardType TypeOfReward { get; set; }
        public override void Awake()
        {
            base.Awake();
            InitializeAds();
        }

        public void InitializeAds()
        {
            testDeviceId = testDeviceId.Trim();

            MobileAds.Initialize(Ad => { });

            RequestConfiguration.Builder adConfig = new RequestConfiguration.Builder();

            if (TestMode && !string.IsNullOrEmpty(testDeviceId))
                adConfig.SetTestDeviceIds(new List<string>() { testDeviceId });

            if (targetForChildren)
                adConfig.SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.True);

            MobileAds.SetRequestConfiguration(adConfig.build());


            LoadInterstitialAd();

            for (int i = 0; i < Enum.GetNames(typeof(RewardType)).Length; i++)
                LoadRewardedAd((RewardType)i);

        }

        #region InterstitialAd
        private void LoadInterstitialAd()
        {
            if (!TestMode && string.IsNullOrEmpty(InterstitialAd.AdUnit))
                return;

            if (interstitial != null && interstitial.IsLoaded())
                return;

            if (interstitial != null)
                interstitial.Destroy();

            string adUnit = TestMode ? testIntersititialAdUnit : InterstitialAd.AdUnit;

            interstitial = new InterstitialAd(adUnit);
            interstitial.OnAdClosed += InterstitialDelegate;
            interstitial.OnAdFailedToLoad += InterstitialOnFailedToLoad;
            interstitial.LoadAd(AdRequest());

        }
        private void InterstitialDelegate(object sender, EventArgs args)
        {
            LoadInterstitialAd();
        }
        private void InterstitialOnFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log(args.LoadAdError.ToString());
            interstitialAutoReloadTime = Time.realtimeSinceStartup + 30f;

            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }
        }
        private IEnumerator InsterstitialShowCoroutine()
        {
            float requestTimeoutMoment = Time.realtimeSinceStartup + 2.5f;

            while (!interstitial.IsLoaded())
            {
                if (Time.realtimeSinceStartup > requestTimeoutMoment)
                    yield break;

                yield return null;

                if (interstitial == null)
                    yield break;
            }

            interstitial.Show();
        }

        public bool InterstitialIsReady()
        {
            if (Instance == null)
                return false;

            if (Instance.interstitial == null)
                return false;

            return Instance.interstitial.IsLoaded();
        }
        public void LoadInterstitial()
        {
            if (Instance == null)
                return;

            Instance.LoadInterstitialAd();
        }
        public void ShowInterstitialAd()
        {
            if (Instance == null)
                return;

            if (Instance.interstitial == null)
            {
                Instance.LoadInterstitialAd();
                if (Instance.interstitial == null)
                    return;
            }

            if (Instance.showInterstitialCoroutine != null)
            {
                Instance.StopCoroutine(Instance.showInterstitialCoroutine);
                Instance.showInterstitialCoroutine = null;
            }

            if (Instance.interstitial.IsLoaded())
                Instance.interstitial.Show();
            else
            {
                if (Time.realtimeSinceStartup >= Instance.interstitialRrequestTimeout)
                    Instance.LoadInterstitialAd();

                Instance.showInterstitialCoroutine = Instance.InsterstitialShowCoroutine();
                Instance.StartCoroutine(Instance.showInterstitialCoroutine);
            }
        }

        #endregion

        #region RewardedAd
        private void LoadRewardedAd(RewardType type)
        {
            TypeOfReward = type;
            string adUnit = RewardedAdUnit(type);

            if (!TestMode && string.IsNullOrEmpty(adUnit))
                return;

            if (rewarded != null && rewarded.IsLoaded())
                return;

            if (rewarded != null)
                rewarded.Destroy();

            string _adUnit = TestMode && (string.IsNullOrEmpty(testDeviceId) || string.IsNullOrEmpty(adUnit)) ? testRewardedAdUnit : adUnit;

            rewarded = new RewardedAd(_adUnit);
            rewarded.OnAdClosed += RewardedDelegate;
            rewarded.OnAdFailedToLoad += RewardOnFailedToLoad;
            rewarded.OnUserEarnedReward += RewardOnEarnedUser;
            rewarded.LoadAd(AdRequest());
        }

        public void ShowRewardedAd(RewardType type,RewardOnEarned reward)
        {
            if (Instance == null)
                return;

            if (Instance.rewarded == null)
            {
                Instance.LoadRewardedAd(type);
                if (Instance.rewarded == null)
                    return;
            }

            if (Instance.showRewardedVideoCoroutine != null)
            {
                Instance.StopCoroutine(Instance.showRewardedVideoCoroutine);
                Instance.showRewardedVideoCoroutine = null;
            }

            Instance.rewardDelegate = reward;

            if (Instance.rewarded.IsLoaded())
                Instance.rewarded.Show();
            else
            {
                if (Time.realtimeSinceStartup >= Instance.rewardedRrequestTimeout)
                    Instance.LoadRewardedAd(type);

                Instance.showRewardedVideoCoroutine = Instance.ShowRewardedCoroutine();
                Instance.StartCoroutine(Instance.showRewardedVideoCoroutine);
            }
        }
        private IEnumerator ShowRewardedCoroutine()
        {
            float requestTimeoutMoment = Time.realtimeSinceStartup + 10f;
            while (!rewarded.IsLoaded())
            {
                if (Time.realtimeSinceStartup > requestTimeoutMoment)
                    yield break;

                yield return null;

                if (rewarded == null)
                    yield break;
            }

            rewarded.Show();
        }
        private void RewardedDelegate(object sender, EventArgs args)
        {
            LoadRewardedAd(TypeOfReward);
        }
        private void RewardOnFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log(args.LoadAdError.ToString());
            rewardedAutoReloadTime = Time.realtimeSinceStartup + 30f;

            if (rewarded != null)
            {
                rewarded.Destroy();
                rewarded = null;
            }
        }
        private void RewardOnEarnedUser(object sender, Reward reward)
        {
            if (rewardDelegate != null)
            {
                rewardDelegate(TypeOfReward);
                rewardDelegate = null;
            }
        }



        #endregion


        private AdRequest AdRequest()
        {
            return new AdRequest.Builder().Build();
        }
        private string RewardedAdUnit(RewardType type)
        {
           return RewardedAds[(int)type].AdUnit;
        }

    }
    public enum AdType
    {
        InterstitialAd,
        LocalAdvancedAd,
        AppOpenAd,
        RewardedAd
    }
    public enum RewardType
    {
        Gold,
        Diamond,
        LuckySpin,
        StartGame,
        EndGame
    }
}

