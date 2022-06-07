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
    public enum DataType
    {
        PlayerData,
        GameData,
        SystemData
    }
    public class CloudSaveManager : Singleton<CloudSaveManager>
    {
        [Header("References")]
        [SerializeField]
        private LevelData levelData;
        public PlayerData PlayerDatas;
        public string userID;
        public int WorldIndex = 0;
        public int LevelIndex = 0;
        
        

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


            Instance.PlayerDatas = await RetrieveSpecificData<PlayerData>("PlayerData");

            Instance.PlayerDatas.PlayerId = AuthenticationService.Instance.PlayerId;

            SyncStages();



           // await SaveData();

            //await ForceDeleteSpecificData("PlayerData");

            //await RetrieveEverything();

        }

        private bool HasLevel()
        {
            return WorldIndex < PlayerDatas.Stages.Count && LevelIndex < PlayerDatas.Stages[PlayerDatas.Stages.Count-1].levels.Count;
        }
   
        public bool CanSpendGem(GemType type, int price)
        {
            return GetGemCount(type) >= price;
        }
        public int GetCurrentCharacter()
        {
            return Instance.PlayerDatas.CurrentCharacterIndex;
        }
        public int GetGemCount(GemType type )
        {
            return type switch
            {
                GemType.Token => Instance.PlayerDatas.Token,
                GemType.Key => Instance.PlayerDatas.Key,
                _ => Instance.PlayerDatas.Gold,
            };
        }
        public CharacterData GetCurrentCharacterData()
        {
            return Instance.PlayerDatas.Characters[Instance.PlayerDatas.CurrentCharacterIndex];
        }
        public Level GetLevel()
        {
            return HasLevel() ? PlayerDatas.Stages[WorldIndex].levels[LevelIndex] : levelData.stages[WorldIndex].levels[LevelIndex];
        }
        public void SetGem(GemType type, int amount)
        {
            switch (type)
            {
                case GemType.Gold:
                    Instance.PlayerDatas.Gold = amount;
                    break;
                case GemType.Token:
                    Instance.PlayerDatas.Token = amount;
                    break;
                case GemType.Key:
                    Instance.PlayerDatas.Key = amount;
                    break;
            }
        }
        public void AddGem(GemType type , int amount)
        {
            switch (type)
            {
                case GemType.Gold:
                    Instance.PlayerDatas.Gold += amount;
                    break;
                case GemType.Token:
                    Instance.PlayerDatas.Token += amount;
                    break;
                case GemType.Key:
                    Instance.PlayerDatas.Key += amount;
                    break;            
            }
        }
        public void SpendGem(GemType type, int price)
        {
            switch (type)
            {
                case GemType.Gold:
                    Instance.PlayerDatas.Gold -= price;                   
                    break;
                case GemType.Token:
                    Instance.PlayerDatas.Token -= price;
                    break;
                case GemType.Key:
                    Instance.PlayerDatas.Key -= price;
                    break;
            }
        }

        [ContextMenu("Update Level")]
        private void SyncStages()
        {
            if (GetStagesCount() != levelData.stages.Count)
            {
                int startIndex = GetLockedFirstStage();

                int _count = GetStagesCount() - startIndex;
                int Count = GetStagesCount();

                // Instance.PlayerDatas.Stages.RemoveRange(startIndex, Count);

                for (int i = startIndex+1; i < Count; i++)
                {
                    int index= startIndex + 1;
                    Instance.PlayerDatas.Stages.RemoveAt(index);
                }
               
                Debug.Log("Seviyeler S�f�rland�");

                for (int i = startIndex + 1; i < Instance.levelData.stages.Count; i++)
                {
                    Debug.Log(i);
                    Stage stage = Instance.levelData.stages[i];
                    Instance.PlayerDatas.Stages.Add(stage);                    
                }

                Debug.Log("Seviyeler g�ncellendi");

                return;
            }

            Debug.Log("Seviyeler g�ncel");
        }

        private int GetLockedFirstStage()
        {
            int index = 0;

            if (GetStagesCount() == 0)
                return 0;

            for (int i = 0; i < Instance.PlayerDatas.Stages.Count; i++)
            {

                if (Instance.PlayerDatas.Stages[i].stageIsLocked)
                    break;

                index =i; 
            }
              
            Debug.Log( $"Ba�lat�c� {index}");
            return index;
        }

        [ContextMenu("Set Default")]
        public void SetDefualt()
        {
            PlayerDatas.Stages = levelData.stages;
        }
        [ContextMenu("Sett Level")]
        public void SetLevel()
        {
            Level level = levelData.stages[WorldIndex].levels[LevelIndex];

            SetLevel(level);
        }

        public void SetLevel(Level level)
        {
            GetLevelList(WorldIndex)[LevelIndex] = level;

            Debug.Log($"{level.levelId} li level g�ncellendi");
        }

        private int GetStagesCount()
        {
            return Instance.PlayerDatas.Stages.Count;
        }
       
        private List<Level> GetLevelList(int index)
        {
            return Instance.PlayerDatas.Stages[index].levels;
        }

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
                    await ForceSaveObjectData("PlayerData", PlayerDatas);
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
        private async void OnDestroy()
        {
          await  SaveData();
        }
        #endregion

    }

    public enum GemType
    {
        Gold,
        Token,
        Key
    }

    [Serializable]
    public class PlayerData
    {
        public string PlayerId;        
        public int Gold = 0;
        public int Token = 0;
        public int Key = 0;
        public int CurrentLevelIndex;
        public int CurrentCharacterIndex;
        public bool IsAccept;
        public bool IsRemoveAds;
        public List<CharacterData> Characters;      
        public List<Stage> Stages;  
        
        public int GetAllCharacterCount
        {
            get { return Characters.Count; }
        }
    }

    [Serializable]
    public class CharacterData
    {
        public string Id;
        public float Level;
        public float Energy;
        public float Mana;
        public float ERR;
        public float MMR;
        public float Speed;

        public float GetCharacterProperty(CharacterProperty property)
        {
            return property switch
            {
                CharacterProperty.Mana => Mana,
                CharacterProperty.Speed => Speed,
                CharacterProperty.MMR => MMR,
                CharacterProperty.ERR => ERR,
                _ => Energy,
            };
        }
    }
    public enum CharacterProperty
    {
        Energy,
        Mana,
        Speed,
        MMR,
        ERR
    }

}

   


