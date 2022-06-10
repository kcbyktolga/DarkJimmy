using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using GooglePlayGames;

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
        public int WorldIndex { get; set; } = 0;
        public int LevelIndex { get; set; } = 0;
        public int Index { get; set; } = 0;

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

            if ( PlayService.Instance !=null && PlayService.Instance.signIn)
                await AuthenticationService.Instance.SignInWithGoogleAsync(((PlayGamesLocalUser)Social.localUser).GetIdToken());
            else
                await AuthenticationService.Instance.SignInAnonymouslyAsync();


            PlayerDatas = await RetrieveSpecificData<PlayerData>("PlayerData");

            Instance.UserId = PlayerDatas.PlayerId;

            // remote config.
            //SyncStages();



           // await SaveData();

           //  await ForceDeleteSpecificData("PlayerData");

           //await RetrieveEverything();

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
        public int GetCurrentCharacter()
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
        private int GetLockedFirstStage()
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
        private void SetDefualt()
        {
            PlayerDatas.Stages = systemData.Stages;
            PlayerDatas.Characters = systemData.CharacterDatas;
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
        private async Task SaveData()
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
                    return JsonUtility.FromJson<T>(value);
                }
                else
                {
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

   


