using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

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
        public PlayerData playerData;
        public override async void Awake()
        {
            base.Awake();
            // Cloud Save needs to be initialized along with the other Unity Services that
            // it depends on (namely, Authentication), and then the user must sign in.

            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            playerData = await RetrieveSpecificData<PlayerData>("PlayerData");

            //await ForceDeleteSpecificData("PlayerData");
        }
        public async void SetGem(GemType type, int amount)
        {
            switch (type)
            {
                case GemType.Gold:
                    Instance.playerData.Gold = amount;
                    break;
                case GemType.Token:
                    Instance.playerData.Token = amount;
                    break;
                case GemType.Key:
                    Instance.playerData.Key = amount;
                    break;
            }
            await SaveData();
        }
        public async void AddGem(GemType type , int amount)
        {
            switch (type)
            {
                case GemType.Gold:
                    Instance.playerData.Gold += amount;
                    break;
                case GemType.Token:
                    Instance.playerData.Token += amount;
                    break;
                case GemType.Key:
                    Instance.playerData.Key += amount;
                    break;            
            }
            await SaveData();
        }
        public async void SpendGem(GemType type, int price)
        {
            switch (type)
            {
                case GemType.Gold:
                    Instance.playerData.Gold -= price;                   
                    break;
                case GemType.Token:
                    Instance.playerData.Token -= price;
                    break;
                case GemType.Key:
                    Instance.playerData.Key -= price;
                    break;
            }

            await SaveData();
        }
        public bool CanSpendGem(GemType type, int price)
        {
            return GetGemCount(type) >= price;
        }
        public int GetGemCount(GemType type )
        {
            return type switch
            {
                GemType.Token => Instance.playerData.Token,
                GemType.Key => Instance.playerData.Key,
                _ => Instance.playerData.Gold,
            };
        }
        public int GetCurrentCharacter()
        {
            return Instance.playerData.CurrentCharacter;
        }
        public CharacterData GetCurrentCharacterData()
        {
            return Instance.playerData.Characters[Instance.playerData.CurrentCharacter];
        }


        [ContextMenu("Save")]
        private async Task SaveData()
        {
            await ForceSaveObjectData("PlayerData", playerData);
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
                    playerData.PlayerId = AuthenticationService.Instance.PlayerId;
                    await ForceSaveObjectData("PlayerData", playerData);
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

    }

    public enum GemType
    {
        Gold,
        Token,
        Key
    }

    [System.Serializable]
    public class PlayerData
    {
        public string PlayerId;        
        public int Gold = 0;
        public int Token = 0;
        public int Key = 0;
        public int CurrentLevel;
        public int CurrentCharacter;
        public bool IsAccept;
        public bool IsRemoveAds;
        public List<CharacterData> Characters;
    }

    [System.Serializable]
    public class CharacterData
    {
        public string Id;
        public int Level;
        public int Energy;
        public int Mana;
        public float ERR;
        public float MMR;
    }

}
