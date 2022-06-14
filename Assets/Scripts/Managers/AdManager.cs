using UnityEngine;

namespace DarkJimmy
{
    public abstract class AdManager : Singleton<AdManager>
    {
        [Header("AdManager")]
        [SerializeField] string androidGameId;
        [SerializeField] string iOSGameId;
        [SerializeField] bool testMode = true;
   
        public string AndroidGameId { get { return androidGameId; } }
        public string IOSGameId { get { return iOSGameId; } }
        public bool TestMode { get { return testMode; } }
        public string GameId 
        { 
            get { return (Application.platform == RuntimePlatform.IPhonePlayer)? IOSGameId : AndroidGameId; }
        }

        public override void Awake()
        {
            base.Awake();

            InitializeAds();
        }
        public abstract void InitializeAds();
  
    }
    [System.Serializable]
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

