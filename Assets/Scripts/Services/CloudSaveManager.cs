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
      
        public PlayerData PlayerDatas;
        private SystemManager system;
       
        #region Properties
        public GemType GemType { get; set; }
        public int StageIndex { get; set; } = 0;
        public int LevelIndex { get; set; } = 0;
        public bool IsSignedIn { get; set; }
        public bool IsLoadedData { get; set; } = false;
        public string UserId { get; set; }

        public struct UserAttributes
        {
            
        }
        public struct AppAttributes
        {
           
        }

        public string AppVersion { get; set; } = string.Empty;
        public int EndGamePrice { get; set; }
        public int EndGamePayType { get; set; }
        public int IntersitialAdFrequency { get; set; }
        public int RewardAdFrequency { get; set; }
        public int SpinPrice { get; set; }
        public int SpinPayType { get; set; }

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
                       
            // Seviye senkronizasyonu remote config ile yapýlacak.. Daha sonra bak.
            //SyncStages();

            //await SaveData();
            //await ForceDeleteSpecificData("PlayerData");
            //await RetrieveEverything();
        }

        #region Player Data Methods
        public async void SignIn()
        {
            if (!Instance.IsSignedIn)
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
            return StageIndex < Instance.PlayerDatas.Stages.Count && LevelIndex < Instance.PlayerDatas.Stages[PlayerDatas.Stages.Count - 1].levels.Count;
        }
        public bool CanSpendGem(GemType type, int price)
        {
            return GetGemCount(type) >= price;
        }
        public bool CanSpendStone(Stones stone, int price)
        {
            return GetStoneCount(stone) >= price;
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
        public ref int GetGemCount(GemType type)
        {
            switch (type)
            {
                default:
                case GemType.Gold:
                    return ref Instance.PlayerDatas.Gold;
                case GemType.Diamond:
                    return ref Instance.PlayerDatas.Diamond;
            }
        }
        public ref int GetStoneCount(Stones stone)
        {
            switch (stone)
            {
                default:
                case Stones.Philosophy:
                    return ref Instance.PlayerDatas.Philistone;
                case Stones.LifeCrystal:
                    return ref Instance.PlayerDatas.Rockstone;
                case Stones.PowerCrystal:
                    return ref Instance.PlayerDatas.Powerstone;
                case Stones.Moonstone:
                    return ref Instance.PlayerDatas.Milestone;
            }
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
            return Instance.system.DefaultData;
        }
        public Level GetCurrentDefaultLevel()
        {
            return Instance.system.DefaultData.Stages[StageIndex].levels[LevelIndex];
        }
        public Level GetCurrentLevel()
        {
            return HasLevel() ? Instance.PlayerDatas.Stages[StageIndex].levels[LevelIndex] : Instance.system.DefaultData.Stages[StageIndex].levels[LevelIndex];
        }
        public string GetDefaultLevelName()
        {
          return LanguageManager.GetText(Instance.system.DefaultData.Stages[StageIndex].levels[LevelIndex].levelName);
        }
        public string GetDefaultStageName()
        {
            return LanguageManager.GetText(Instance.system.DefaultData.Stages[StageIndex].stageName);
        }
        private int GetStagesCount()
        {
            return Instance.PlayerDatas.Stages.Count;
        }
        private List<Stage> GetAllStage()
        {
            return Instance.PlayerDatas.Stages;
        }
        private int GetTotalMaxScore()
        {
            int maxScore = 0;
            
            for (int i = 0; i < GetStagesCount(); i++)
            {
                for (int j = 0; j < GetAllStage()[i].levels.Count; j++)
                {
                    if (GetAllStage()[i].stageIsLocked)
                        continue;

                    maxScore += GetAllStage()[i].levels[j].maxScore;
                }
            }
            return maxScore;
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
        public async void AddGem(GemType type, int amount)
        {
            GetGemCount(type)+=amount;

            if (Enum.TryParse(type.ToString(), out Stats stats))
                system.updateStats(stats, GetGemCount(type));

            if(stats.Equals(Stats.Gold))
                system.priceCalculate?.Invoke();

            AudioManager.Instance.PlaySound($"Gem");
            await SaveData();
        }
        public async void SpendGem(GemType type, int amount)
        {
            GetGemCount(type) -= amount;
          
            if (Enum.TryParse(type.ToString(), out Stats stats))
               system.updateStats(stats, GetGemCount(type));

            AudioManager.Instance.PlaySound($"Gem");

            await SaveData();
        }
        public async void AddStones(Stones type, int amount)
        {
            GetStoneCount(type) += amount;

            if (Enum.TryParse(type.ToString(), out Stats stats))
                system.updateStats(stats, GetStoneCount(type));

            system.priceCalculate?.Invoke();

            AudioManager.Instance.PlaySound($"Gem");
            await SaveData();
        }
        public async void SpendStones(Stones type, int amount)
        {
            GetStoneCount(type) -= amount;

            if (Enum.TryParse(type.ToString(), out Stats stats))
                system.updateStats(stats, GetStoneCount(type));

            AudioManager.Instance.PlaySound($"Gem");
            await SaveData();
        }
        public async void UnlockStage(int index, bool isOn)
        {
            Instance.PlayerDatas.Stages[index].stageIsLocked = isOn;
            await SaveData();
        }

        public async void SetLevelKey(int keyCount)
        {
            Instance.PlayerDatas.Stages[StageIndex].levels[LevelIndex].keyCount = keyCount;
            await SaveData();
        }
        public async void IAgree()
        {
            Instance.PlayerDatas.IsAccept = true;
            await  SaveData();
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
            if (GetStagesCount() != system.DefaultData.Stages.Count)
            {
                int startIndex = GetLockedFirstStage();
                int Count = GetStagesCount();

                for (int i = startIndex + 1; i < Count; i++)
                {
                    PlayerDatas.Stages.RemoveAt(startIndex + 1);
                }

                for (int i = startIndex + 1; i < Instance.system.DefaultData.Stages.Count; i++)
                {
                    Stage stage = Instance.system.DefaultData.Stages[i];
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
                Stages = system.DefaultData.Stages,
                Characters = system.DefaultData.CharacterDatas
            };

            await SaveData();
        }
        [ContextMenu("Set Level")]
        private void SetLevel()
        {
            Level level = Instance.system.DefaultData.Stages[StageIndex].levels[LevelIndex];

            SetLevel(level);
        }
        public async void SetLevel(Level level)
        {
            GetLevelList(StageIndex)[LevelIndex] = level;

            PlayService.Instance.AddScoreToLeaderboard(GetTotalMaxScore());

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

            // Set the user’s unique ID:
            RemoteConfigService.Instance.SetCustomUserID(Instance.UserId);

            // Set the environment ID:
            RemoteConfigService.Instance.SetEnvironmentID("e810f0f8-3a3c-427e-9b35-0f2e25feddc9");

            // Fetch configuration settings from the remote service:
            RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());

            
        }
        private void ApplyRemoteSettings(ConfigResponse configResponse)
        {
            // Conditionally update settings, depending on the response's origin:
            

            switch (configResponse.requestOrigin)
            {
                case ConfigOrigin.Default:
           
                    break;
                case ConfigOrigin.Cached:

                    break;
                case ConfigOrigin.Remote:
                   
                    AppVersion = RemoteConfigService.Instance.appConfig.GetString("App Version");
                
                    EndGamePayType = RemoteConfigService.Instance.appConfig.GetInt("End Game Pay Type", 0);
                    EndGamePrice = RemoteConfigService.Instance.appConfig.GetInt("End Game Price", 150);
                    IntersitialAdFrequency= RemoteConfigService.Instance.appConfig.GetInt("Intersitial Ad Frequency", 1);
                    RewardAdFrequency = RemoteConfigService.Instance.appConfig.GetInt("Reward Ad Frequency",90);
                    SpinPrice = RemoteConfigService.Instance.appConfig.GetInt("Spin Price", 15);
                    SpinPayType= RemoteConfigService.Instance.appConfig.GetInt("Spin Pay Type", 1);

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
        public int Gold;
        public int Diamond;
        public int Rockstone;
        public int Milestone;
        public int Powerstone;
        public int Philistone;
        public int Key;
        public int CurrentLevelIndex;
        public int CurrentCharacterIndex;
        public bool IsAccept;
        public bool HasRemoveAds;
        public List<CharacterData> Characters;      
        public List<Stage> Stages;

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

   


