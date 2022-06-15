using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using GooglePlayGames;
using TMPro;

namespace DarkJimmy
{
   

    public class CloudSaveManager : Singleton<CloudSaveManager>
    {
        [Header("References")]
        [SerializeField]
        private SystemData systemData;
        public PlayerData PlayerDatas;
        [SerializeField]
        private SystemProperty system;

        #region Properties
        public GemType GemType { get; set; }

        public int WorldIndex { get; set; } = 0;
        public int LevelIndex { get; set; } = 0;
        public int Index { get; set; } = 0;

        public bool IsSignedIn { get; set; }
        public bool IsLoadedData { get; set; } = false;

        public string UserId { get; set; }
        public string AplicationURL { get; set; } = "https://play.google.com/store/apps/details?id=com.rhombeusgaming.DarkJimmy";
        public string WebsiteURL { get; set; } = "http://www.rhombeusgaming.com/";
        public string InstagramURL { get; set; } = "https://www.instagram.com/rhombeusgaming/";
        public string YoutubeeURL { get; set; } = "https://www.youtube.com/channel/UCgUMXl7bylbFv36oDuDgLtw";
        public string PublisherURL { get; set; } = "https://play.google.com/store/apps/dev?id=8451792306383085358&hl=en_US&gl=US";
        public string MailURL { get; set; } = "mailto:rhombeusgaming@gmail.com";
        #endregion

        public delegate void UpdateStage(Stage stage);
        public UpdateStage updateStage;
        public override async void Awake()
        {
            base.Awake();
            // Cloud Save needs to be initialized along with the other Unity Services that
            // it depends on (namely, Authentication), and then the user must sign in.

            await UnityServices.InitializeAsync();

            SignIn();

            // remote config.
            //SyncStages();
            //await SaveData();
            //await ForceDeleteSpecificData("PlayerData");
            //await RetrieveEverything();
        }

        public async void SignIn()
        {
            if (!IsSignedIn)
            {
                if (PlayService.Instance != null && PlayService.Instance.signIn)
                    await AuthenticationService.Instance.SignInWithGoogleAsync(((PlayGamesLocalUser)Social.localUser).GetIdToken());
                else
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                IsSignedIn = AuthenticationService.Instance.IsSignedIn;

                Debug.Log(IsSignedIn);
            }
          
            PlayerDatas = await RetrieveSpecificData<PlayerData>("PlayerData");

            Instance.UserId = PlayerDatas.PlayerId;
        }

        #region Player Data Methods
        private bool HasLevel()
        {
            return WorldIndex < PlayerDatas.Stages.Count && LevelIndex < PlayerDatas.Stages[PlayerDatas.Stages.Count - 1].levels.Count;
        }
        public bool CanSpendGem(GemType type, int price)
        {
            return GetGemCount(type) >= price;
        }
        public int GetCurrentCharacterIndex()
        {
            return PlayerDatas.CurrentCharacterIndex;
        }
        public int GetGemCount(GemType type)
        {
            return type switch
            {
                GemType.Diamond => PlayerDatas.Diamond,
                _ => PlayerDatas.Gold,
            };
        }
        public CharacterData GetCurrentCharacterData()
        {
            return PlayerDatas.Characters[PlayerDatas.CurrentCharacterIndex];
        }
        public Level GetLevel()
        {
            return HasLevel() ? PlayerDatas.Stages[WorldIndex].levels[LevelIndex] : systemData.Stages[WorldIndex].levels[LevelIndex];
        }

        public async void SetCharacterIndex(int index)
        {
            PlayerDatas.CurrentCharacterIndex = index;
            await SaveData();

        }
        public async  void SetCharacterData(int index , CharacterData data)
        {
            PlayerDatas.Characters[index] = data;
            await SaveData();
        }    
        public async void SetGem(GemType type, int amount)
        {
            switch (type)
            {
                case GemType.Gold:
                    PlayerDatas.Gold = amount;
                    break;
                case GemType.Diamond:
                    PlayerDatas.Diamond = amount;
                    break;
            }
            await SaveData();
        }
        public async void AddGem(GemType type, int amount)
        {
            switch (type)
            {
                case GemType.Gold:
                    PlayerDatas.Gold += amount;
                    break;
                case GemType.Diamond:
                    PlayerDatas.Diamond += amount;
                    break;
            }

            if (Enum.TryParse(type.ToString(), out Stats stats))
                UIManager.Instance.updateState(stats, GetGemCount(type));

           await SaveData();
        }
        public async void SpendGem(GemType type, int price)
        {
            switch (type)
            {
                case GemType.Gold:
                    PlayerDatas.Gold -= price;
                    break;
                case GemType.Diamond:
                    PlayerDatas.Diamond -= price;
                    break;
            }

            if (Enum.TryParse(type.ToString(), out Stats stats))
                UIManager.Instance.updateState(stats, GetGemCount(type));

            await SaveData();
        }
        public async void UnlockStage(int index, bool isOn)
        {
            PlayerDatas.Stages[index].stageIsLocked = isOn;
            await SaveData();
        }
    
        #endregion
        #region Common
        public string StringFormat(int count)
        {
            return $"{string.Format("{0:N0}", count)}";
        }

        public void SetResetTime(DateTime resetTime,RewardType rewardType)
        {
            PlayerPrefs.SetString(rewardType.ToString(),resetTime.ToString());
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
        //public void TimeUpdate(RewardType rewardType, out string description, out PayType payType)
        //{
        //    description = string.Empty;         
        //    DateTime resetTime = GetResetTime(rewardType);
        //    payType = PayType.Paid;

        //    while (DateTime.Now <= resetTime)
        //    {
        //        TimeSpan diffTime = resetTime.Subtract(DateTime.Now);

        //        string timeText = diffTime.Hours > 0 ? $"{diffTime.Hours}{LanguageManager.GetText("sa")} : {diffTime.Minutes}{LanguageManager.GetText("d")}" : $"{diffTime.Minutes}{LanguageManager.GetText("dk")} : {diffTime.Seconds}{LanguageManager.GetText("sn")}";

        //        description= $"{LanguageManager.GetText("RemainingTime")}: {timeText}";
        //    }
            
        //    payType = PayType.Free;

        //}

        #endregion

        [ContextMenu("Update Level")]
        private async void SyncStages()
        {
            if (GetStagesCount() != systemData.Stages.Count)
            {
                int startIndex = GetLockedFirstStage();
                int Count = GetStagesCount();

                for (int i = startIndex+1; i < Count; i++)
                {
                    PlayerDatas.Stages.RemoveAt(startIndex+1);
                }

                for (int i = startIndex + 1; i < Instance.systemData.Stages.Count; i++)
                {
                    Stage stage = Instance.systemData.Stages[i];
                    PlayerDatas.Stages.Add(stage);                    
                }

            }

         await  SaveData();
        }

        public bool CanUnlockStage(int index) 
        {
            return !PlayerDatas.Stages[index - 1].stageIsLocked;
        }
        public int GetLockedFirstStage()
        {
            int index = 0;

            if (GetStagesCount() == 0)
                return 0;

            for (int i = 0; i < PlayerDatas.Stages.Count; i++)
            {

                if (PlayerDatas.Stages[i].stageIsLocked)
                    break;

                index =i; 
            }
              
            Debug.Log( $"Baþlatýcý {index}");
            return index;
        }

        [ContextMenu("Set Default")]
        private async void SetDefualt()
        {
            PlayerDatas.Stages = systemData.Stages;
            PlayerDatas.Characters = systemData.CharacterDatas;

           await SaveData();
        }
        [ContextMenu("Sett Level")]
        public void SetLevel()
        {
            Level level = systemData.Stages[WorldIndex].levels[LevelIndex];

            SetLevel(level);
        }
        public async void SetLevel(Level level)
        {
            GetLevelList(WorldIndex)[LevelIndex] = level;

            await SaveData();
   
        }
        public SystemData GetSystemData()
        {
            return systemData;
        }
        private int GetStagesCount()
        {
            return PlayerDatas.Stages.Count;
        }    
        private List<Level> GetLevelList(int index)
        {
            return PlayerDatas.Stages[index].levels;
        }

        #region System 
        public Sprite GetLevelSprite(LevelStatus status)
        {
            return status switch
            {
                LevelStatus.Active => system.active,
                LevelStatus.Passive => system.passive,
                _ => system.passed,
            };
        }
        public Sprite GetPaySprite(GemType type)
        {
            return type switch
            {
                GemType.Diamond => system.token,
                _ => system.gold,
            };
        }
        public Sprite GetGridProductSprite(PayType type)
        {
            return type switch
            {
                PayType.Free => system.freeSprite,
                _ => system.paidSprite,
            };
        }
        #endregion

        #region Main Methods  
        [ContextMenu("Save")]
        public async Task SaveData()
        {
            await ForceSaveObjectData("PlayerData", PlayerDatas);
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
                    IsLoadedData = !string.IsNullOrEmpty(value);

                    Debug.Log(IsLoadedData);

                    return JsonUtility.FromJson<T>(value);

                }
                else
                {
                    LanguageManager.DefaultLanguage();
                    PlayerDatas.PlayerId = AuthenticationService.Instance.PlayerId;
                    SetDefualt();
                    await ForceSaveObjectData("PlayerData", PlayerDatas);

                    PlayerDatas = await RetrieveSpecificData<PlayerData>("PlayerData");
                    Instance.UserId = PlayerDatas.PlayerId;

                    Debug.Log("Burdayýýým aaq");

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

            Debug.Log("Burda deðilim");
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

    }

    public enum GemType
    {
        Gold,
        Diamond,
    }

    [Serializable]
    public class PlayerData
    {
        public string PlayerId;        
        public int Gold = 0;
        public int Diamond = 0;
        public int Key = 0;
        public int CurrentLevelIndex;
        public int CurrentCharacterIndex;
        public bool IsAccept;
        public bool HasPremium;
        public List<CharacterData> Characters;      
        public List<Stage> Stages;  
      
        public int GetAllCharacterCount
        {
            get { return Characters.Count; }
        }
    }

    [Serializable]
    public class SystemProperty
    {
        [Header("Button Sprites")]
        public Sprite passed;
        public Sprite active;
        public Sprite passive;

        [Header("Gem Sprites")]
        public Sprite gold;
        public Sprite token;
        public Sprite key;

        [Header("Grid Product Sprite")]
        public Sprite paidSprite;
        public Sprite freeSprite;

    }

}

   


