using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using GooglePlayGames;
using Unity.Services.RemoteConfig;

namespace DarkJimmy
{
  
    public class CloudSaveManager : Singleton<CloudSaveManager>
    {
        [Header("References")]
        [SerializeField]
        private DefaultData systemData;
        public PlayerData PlayerDatas;
        private SystemManager system;
       
        #region Properties
        public GemType GemType { get; set; }
        public int WorldIndex { get; set; } = 0;
        public int LevelIndex { get; set; } = 0;
        public bool IsSignedIn { get; set; }
        public bool IsLoadedData { get; set; } = false;
        public string UserId { get; set; }

        public struct UserAttributes
        {
            //public bool expansionFlag;
        }
        public struct AppAttributes
        {
           
        }
        public struct FilterAttributes
        {
            // Optionally declare variables for attributes to filter on any of following parameters:
            public string[] key;
            public string[] type;
            public string[] schemaId;
        }

        public string AppVersion { get; set; } = string.Empty;

        #region URLs
        //public string AplicationURL { get; private set; } = "https://play.google.com/store/apps/details?id=com.rhombeusgaming.DarkJimmy";
        //public string WebsiteURL { get; private set; }    = "http://www.rhombeusgaming.com/";
        //public string InstagramURL { get; private set; }  = "https://www.instagram.com/rhombeusgaming/";
        //public string YoutubeeURL { get; private set; }   = "https://www.youtube.com/channel/UCgUMXl7bylbFv36oDuDgLtw";
        //public string PublisherURL { get; private set; }  = "https://play.google.com/store/apps/dev?id=8451792306383085358&hl=en_US&gl=US";
        //public string MailURL { get; private set; }       = "mailto:rhombeusgaming@gmail.com";
        #endregion

        #endregion

        #region Delegates
        public delegate void UpdateStage(Stage stage);
        public UpdateStage updateStage;
        #endregion

        public override async void Awake()
        {
            base.Awake();
            system = SystemManager.Instance;

            await UnityServices.InitializeAsync();
            SignIn();
            
            
            // Seviye senkronizasyonu remote config ile yap�lacak.. Daha sonra bak.
            //SyncStages();

            //await SaveData();
            //await ForceDeleteSpecificData("PlayerData");
            //await RetrieveEverything();
        }

        #region Player Data Methods
        public async void SignIn()
        {
            if (!IsSignedIn)
            {
                if (PlayService.Instance != null && PlayService.Instance.signIn)
                    await AuthenticationService.Instance.SignInWithGoogleAsync(((PlayGamesLocalUser)Social.localUser).GetIdToken());
                else
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                Instance.IsSignedIn = AuthenticationService.Instance.IsSignedIn;

            }

            Instance.PlayerDatas = await RetrieveSpecificData<PlayerData>("PlayerData");

            Instance.UserId = PlayerDatas.PlayerId;

            InitializeRemoteConfigAsync();

        }

        #region Check Methods
        private bool HasLevel()
        {
            return WorldIndex < Instance.PlayerDatas.Stages.Count && LevelIndex < Instance.PlayerDatas.Stages[PlayerDatas.Stages.Count - 1].levels.Count;
        }
        public bool CanSpendGem(GemType type, int price)
        {
            return GetGemCount(type) >= price;
        }
        public bool CanUnlockStage(int index)
        {
            return !Instance.PlayerDatas.Stages[index - 1].stageIsLocked;
        }
        #endregion

        #region Get Methods
        public int GetCurrentCharacterIndex()
        {
            return Instance.PlayerDatas.CurrentCharacterIndex;
        }
        public int GetGemCount(GemType type)
        {
            return type switch
            {
                GemType.Diamond => Instance.PlayerDatas.Diamond,
                _ => Instance.PlayerDatas.Gold,
            };
        }
        public int GetLockedFirstStage()
        {
            int index = 0;

            if (GetStagesCount() == 0)
                return 0;

            for (int i = 0; i < Instance.PlayerDatas.Stages.Count; i++)
            {

                if (Instance.PlayerDatas.Stages[i].stageIsLocked)
                    break;

                index = i;
            }

            return index;
        }
        public CharacterData GetCurrentCharacterData()
        {
            return Instance.PlayerDatas.Characters[PlayerDatas.CurrentCharacterIndex];
        }
        public DefaultData GetDefaultData()
        {
            return Instance.systemData;
        }
        public Level GetLevel()
        {
            return HasLevel() ? Instance.PlayerDatas.Stages[WorldIndex].levels[LevelIndex] : Instance.systemData.Stages[WorldIndex].levels[LevelIndex];
        }
        private int GetStagesCount()
        {
            return Instance.PlayerDatas.Stages.Count;
        }
        private List<Level> GetLevelList(int index)
        {
            return Instance.PlayerDatas.Stages[index].levels;
        }
        #endregion

        #region Set Methods
        public async void SetCharacterIndex(int index)
        {
            Instance.PlayerDatas.CurrentCharacterIndex = index;
            await SaveData();

        }
        public async void SetCharacterData(int index, CharacterData data)
        {
            Instance.PlayerDatas.Characters[index] = data;
            await SaveData();
        }
        public async void SetGem(GemType type, int amount)
        {
            switch (type)
            {
                case GemType.Gold:
                    Instance.PlayerDatas.Gold = amount;
                    break;
                case GemType.Diamond:
                    Instance.PlayerDatas.Diamond = amount;
                    break;
            }
            await SaveData();
        }
        public async void AddGem(GemType type, int amount)
        {
            switch (type)
            {
                case GemType.Gold:
                    Instance.PlayerDatas.Gold += amount;
                    break;
                case GemType.Diamond:
                    Instance.PlayerDatas.Diamond += amount;
                    break;
            }

            if (Enum.TryParse(type.ToString(), out Stats stats))
                system.updateStats(stats, GetGemCount(type));

            await SaveData();
        }
        public async void SpendGem(GemType type, int price)
        {
            switch (type)
            {
                case GemType.Gold:
                    Instance.PlayerDatas.Gold -= price;
                    break;
                case GemType.Diamond:
                    Instance.PlayerDatas.Diamond -= price;
                    break;
            }

            if (Enum.TryParse(type.ToString(), out Stats stats))
               system.updateStats(stats, GetGemCount(type));

            await SaveData();
        }
        public async void UnlockStage(int index, bool isOn)
        {
            Instance.PlayerDatas.Stages[index].stageIsLocked = isOn;
            await SaveData();
        }

        #endregion

        #region Common
        public void SetResetTime(DateTime resetTime, RewardType rewardType)
        {
            PlayerPrefs.SetString(rewardType.ToString(), resetTime.ToString());
        }
        public DateTime GetResetTime(RewardType rewardType)
        {
            string key = rewardType.ToString();

            if (PlayerPrefs.HasKey(key))
                return Convert.ToDateTime(PlayerPrefs.GetString(key));

            DateTime time = DateTime.Now;
            SetResetTime(time, rewardType);

            return time;
        }
        #endregion

        #endregion

        #region Context Methods

        [ContextMenu("Update Level")]
        private async void SyncStages()
        {
            if (GetStagesCount() != systemData.Stages.Count)
            {
                int startIndex = GetLockedFirstStage();
                int Count = GetStagesCount();

                for (int i = startIndex + 1; i < Count; i++)
                {
                    PlayerDatas.Stages.RemoveAt(startIndex + 1);
                }

                for (int i = startIndex + 1; i < Instance.systemData.Stages.Count; i++)
                {
                    Stage stage = Instance.systemData.Stages[i];
                    PlayerDatas.Stages.Add(stage);
                }

            }

            await SaveData();
        }
        [ContextMenu("Set Default")]
        private async void SetDefualt()
        {
            Instance.PlayerDatas = new PlayerData
            {
                PlayerId = AuthenticationService.Instance.PlayerId,
                Stages = systemData.Stages,
                Characters = systemData.CharacterDatas
            };

            await SaveData();
        }
        [ContextMenu("Set Level")]
        public void SetLevel()
        {
            Level level = Instance.systemData.Stages[WorldIndex].levels[LevelIndex];

            SetLevel(level);
        }
        private async void SetLevel(Level level)
        {
            GetLevelList(WorldIndex)[LevelIndex] = level;

            await SaveData();

        }
        #endregion

        #region Main Methods  
        [ContextMenu("Save")]
        public async Task SaveData()
        {
            await ForceSaveObjectData("PlayerData", Instance.PlayerDatas);
        }
        private async Task ListAllKeys()
        {
            try
            {
                var keys = await CloudSaveService.Instance.Data.RetrieveAllKeysAsync();

                Debug.Log($"Keys count: {keys.Count}\n" +
                    $"Keys: {String.Join(", ", keys)}");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }
        private async Task ForceSaveSingleData(string key, string value)
        {
            try
            {
                Dictionary<string, object> oneElement = new Dictionary<string, object>();

                // It's a text input field, but let's see if you actually entered a number.
                if (Int32.TryParse(value, out int wholeNumber))
                {
                    oneElement.Add(key, wholeNumber);
                }
                else if (Single.TryParse(value, out float fractionalNumber))
                {
                    oneElement.Add(key, fractionalNumber);
                }
                else
                {
                    oneElement.Add(key, value);
                }

                await CloudSaveService.Instance.Data.ForceSaveAsync(oneElement);

                Debug.Log($"Successfully saved {key}:{value}");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }
        private async Task ForceSaveObjectData(string key, PlayerData value)
        {
            try
            {
                // Although we are only saving a single value here, you can save multiple keys
                // and values in a single batch.
                Dictionary<string, object> oneElement = new Dictionary<string, object>
                {
                    { key, value }
                };

                await CloudSaveService.Instance.Data.ForceSaveAsync(oneElement);

                Debug.Log($"Successfully saved {key}:{value}");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }
        private async Task<T> RetrieveSpecificData<T>(string key)
        {
            try
            {
                var results = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { key });

                if (results.TryGetValue(key, out string value))
                {
                    Instance.IsLoadedData = !string.IsNullOrEmpty(value);

                    return JsonUtility.FromJson<T>(value);
                }
                else
                {
                    LanguageManager.DefaultLanguage();

                    SetDefualt();

                    await ForceSaveObjectData("PlayerData", Instance.PlayerDatas);

                    Instance.PlayerDatas = await RetrieveSpecificData<PlayerData>("PlayerData");
                    Instance.UserId = Instance.PlayerDatas.PlayerId;

                    return await RetrieveSpecificData<T>(key);

                }
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
                return await RetrieveSpecificData<T>(key);
            }

            return default;
        }
        private async Task RetrieveEverything()
        {
            try
            {
                // If you wish to load only a subset of keys rather than everything, you
                // can call a method LoadAsync and pass a HashSet of keys into it.
                var results = await CloudSaveService.Instance.Data.LoadAllAsync();
              
                Debug.Log($"Elements loaded!");

                foreach (var element in results)
                {
                    Debug.Log($"Key: {element.Key}, Value: {element.Value}");
                }
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }
        private async Task ForceDeleteSpecificData(string key)
        {
            try
            {
                await CloudSaveService.Instance.Data.ForceDeleteAsync(key);
                Debug.Log($"Successfully deleted {key}");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        //private async void OnDestroy()
        //{
        // //await  SaveData();
        //}
        #endregion

        #region RemetoConfig

        private  void InitializeRemoteConfigAsync()
        {          
            // Add a listener to apply settings when successfully retrieved:
            RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;

            // Set the user�s unique ID:
            RemoteConfigService.Instance.SetCustomUserID(Instance.UserId);

            // Set the environment ID:
            RemoteConfigService.Instance.SetEnvironmentID("e810f0f8-3a3c-427e-9b35-0f2e25feddc9");

            // Fetch configuration settings from the remote service:
            RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());
        }
        void ApplyRemoteSettings(ConfigResponse configResponse)
        {
            // Conditionally update settings, depending on the response's origin:
            switch (configResponse.requestOrigin)
            {
                case ConfigOrigin.Default:
           
                    break;
                case ConfigOrigin.Cached:

                    break;
                case ConfigOrigin.Remote:
                    AppVersion = RemoteConfigService.Instance.appConfig.GetString("AppVersion");
                    Debug.Log(AppVersion);

                    break;
            }
        }


        #endregion

        private void OnDestroy()
        {
            RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteSettings;
        }
    }

    

    [Serializable]
    public class PlayerData
    {
        public string PlayerId;        
        public int Gold { get; set; } = 0;
        public int Diamond { get; set; } =0;
        public int Key { get { return GetAllKeyCount(); } set { }}
        public int CurrentLevelIndex;
        public int CurrentCharacterIndex;
        public bool IsAccept;
        public bool HasPremium;
        public List<CharacterData> Characters;      
        public List<Stage> Stages;

        private int keyCount = 0;
      
        public int GetAllKeyCount()
        {
            int keyCount = 0;
            for (int i = 0; i < Stages.Count; i++)
            {
                if (Stages[i].stageIsLocked)
                    continue;

                for (int j = 0; j < Stages[i].levels.Count; j++)
                    keyCount += Stages[i].levels[j].keyCount;
            }

            return keyCount;
        }

    }

  
}

   


