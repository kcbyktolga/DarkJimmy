using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace DarkJimmy
{
    public class SystemManager : Singleton<SystemManager>
    {
        [SerializeField]
        private SystemProperty systemProperty;
        [SerializeField]
        private DefaultData defaultData;

        #region Delegates
        public delegate void UpdateStats(Stats stats, int value);
        public UpdateStats updateStats;
        public UpdateStats updatePowerUp;
        public UpdateStats updateGameDisplay;
        public UpdateStats updateGMStats;
        public delegate void UpdateStatsMax(Stats stats, int value, int maxValue);
        public UpdateStatsMax updateStatsMax;
        

        public delegate void AddGameElement(GameObject obj);
        public AddGameElement addGameElement;

        public delegate void UpdateTransform(Vector2 transform);
        public UpdateTransform updateTransform;

        public delegate void OpenShopPage(int pageIndex);
        public OpenShopPage toPage;
        public OpenShopPage onChangedPage;

        public delegate void PlayParticle(Material material, Vector3 position);
        public PlayParticle particle;

        public delegate void SetBackground(BackgroundsType type);
        public SetBackground onChangedBackground;

        public delegate void DoSomething();
        public DoSomething cameraShake;
        public DoSomething priceCalculate;
        #endregion

        #region Properties
        public RewardProduct Reward { get; set; }
        public Level SelectedLevel { get; set; }
        public GemType GemType { get; set; }
        public DefaultData DefaultData { get { return defaultData; } }
        private int Seed { get { return UnityEngine.Random.Range(0,100); } }
       
        #endregion
        #region Collections
        private List<int> luckyFactor = new List<int>();
        #endregion
        private void Start()
        {
            GenerateReward();
        }

        private void GenerateReward()
        {
            for (int i = 0; i < defaultData.Catalog.GetProductLuckySpin.Count; i++)
            {
                int factor=defaultData.Catalog.GetProductLuckySpin[i].luckyFactor;
                for (int j = 0; j < factor; j++)
                    luckyFactor.Add(i);
            }
        }

        #region Get Sprites
        public Sprite GetChracterPropSprite(CharacterProperty property)
        {
            return property switch
            {
                CharacterProperty.Mana => systemProperty.manaProp,
                CharacterProperty.Speed => systemProperty.speedProp,
                _ => systemProperty.hpProp,
            };
        }
        public Sprite GetStatsIcon(Stats stats)
        {
            return stats switch
            {
                Stats.Diamond => systemProperty.diamond,
                Stats.Key => systemProperty.key,
                Stats.Time => systemProperty.time,
                Stats.Philosophy => systemProperty.philosphy,
                Stats.LifeCrystal => systemProperty.elegantStone,
                Stats.PowerCrystal => systemProperty.Powercrystal,
                Stats.Moonstone => systemProperty.moonStone,
                Stats.HP => systemProperty.hp,
                Stats.Mana => systemProperty.mana,
                Stats.Speed => systemProperty.speed,
                _ => systemProperty.gold,
            };
        }
        public Sprite GetResultSprite(UI.Result result)
        {
            return result switch
            {
                UI.Result.Gold => systemProperty.gold,
                UI.Result.Key => systemProperty.key,
                UI.Result.Score => systemProperty.thorpy,
                _ => systemProperty.time,
            };
        }
        public Sprite GetRewardAdSprite(bool isOn)
        {
            return isOn ? systemProperty.readyAd : systemProperty.notReadyAd;
        }
        public Sprite GetLevelSprite(LevelStatus status)
        {
            return status switch
            {
                LevelStatus.Active => systemProperty.active,
                LevelStatus.Passive => systemProperty.passive,
                _ => systemProperty.passed,
            };
        }
        public Sprite GetPaySprite(GemType type)
        {
            return type switch
            {
                GemType.Diamond => systemProperty.diamond,
                _ => systemProperty.gold,
            };
        }
        public Sprite GetProductBackground(TypeofProduct type)
        {
            return type switch
            {
                TypeofProduct.Gold => systemProperty.goldSprite,
                TypeofProduct.Diamond => systemProperty.diamondSprite,
                TypeofProduct.Stones => systemProperty.stoneSprite,
                TypeofProduct.Offers => systemProperty.offersSprite,
                _ => systemProperty.offersSprite,
            };
        }
        #endregion

        #region Get Colors
        public Color GetWhiteAlfaColor(bool isOn)
        {
            return isOn ? systemProperty.whiteAlfa255 : systemProperty.whiteAlfa0;
        }
        public Color GetBlackAlfaColor(bool isOn)
        {
            return isOn ? systemProperty.blackAlfa255 : systemProperty.blackAlfa0;
        }

        public Color GetLevelColor(LevelStatus status)
        {
            return status switch
            {
                LevelStatus.Active => systemProperty.orange,
                LevelStatus.Passive => systemProperty.black,
                _ => systemProperty.green,
            };
        }
        #endregion

        #region Get String
        public string StringFormat(int count)
        {
            return $"{string.Format("{0:N0}", count)}";
        }
        public string GetUrlAddress(string address)
        {
            return address switch
            {
                "YoutubeURL" => systemProperty.YoutubeeURL,
                "AppURL" => systemProperty.ApplicationURL,
                "PublisherURL" => systemProperty.PublisherURL,
                "WebsiteURL" => systemProperty.WebsiteURL,
                "Mail" => systemProperty.MailURL,
                "InstagramURL" => systemProperty.InstagramURL,
                _ => string.Empty
            };
        }

        #endregion

        public void ClaimReward()
        {
            if (Enum.TryParse(Reward.typeOfProduct.ToString(), out GemType _gemType))
                CloudSaveManager.Instance.AddGem(_gemType, Reward.amount);

            else if (Reward.typeOfProduct.Equals(TypeofProduct.Stones))
                CloudSaveManager.Instance.AddStones(Reward.stoneType, Reward.amount);
        }
        public RewardProduct GetReward()
        {
            return defaultData.Catalog.GetProductLuckySpin[GetRandomRewardIndex()];
        }
        public AnimationCurve GetMenuCurve()
        {
            return systemProperty.menuCurve;
        }
        public int GetRandomRewardIndex()
        {
            //Shuffle Element...
            luckyFactor = new List<int>(Utility.ShuffleArray(luckyFactor.ToArray(), Seed));
            // pick up random element..
            int index = UnityEngine.Random.Range(0, luckyFactor.Count);
            return luckyFactor[index];
        }
    }
    public static class Utility
    {
        public static T[] ShuffleArray<T>(T[] array, int seed)
        {
            System.Random prng = new System.Random(seed);

            for (int i = 0; i < array.Length - 1; i++)
            {
                int randomIndex = prng.Next(i, array.Length);
                T tempItem = array[randomIndex];
                array[randomIndex] = array[i];
                array[i] = tempItem;
            }

            return array;
        }

    }
    public enum GemType
    {
        Gold,
        Diamond,
    }
    public enum Stones
    {
        Philosophy,
        LifeCrystal,
        PowerCrystal,
        Moonstone
    }
    public enum RewardType
    {
        Gold,
        Diamond,
        LuckySpin,
        StartGame,
        EndGame,
        DoubleReward
    }

    [Serializable]
    public struct SystemProperty
    {
        [Header("Animation Curves")]
        public AnimationCurve menuCurve;
        [Header("Button Sprites")]
        public Sprite passed;
        public Sprite active;
        public Sprite passive;

        [Header("Gem Sprites")]
        public Sprite gold;
        public Sprite diamond;
        public Sprite key;

        [Header("Stats Sprite")]
        public Sprite philosphy;
        public Sprite moonStone;
        public Sprite elegantStone;
        public Sprite Powercrystal;
        public Sprite time;
        public Sprite hp;
        public Sprite mana;
        public Sprite speed;
        [Header("Character Porperty Sprite")]
        public Sprite hpProp;
        public Sprite manaProp;
        public Sprite speedProp;
        [Header("Product Background Sprite")]
        public Sprite goldSprite;
        public Sprite diamondSprite;
        public Sprite offersSprite;
        public Sprite stoneSprite;

        [Header("Colors")]
        public Color blackAlfa255;
        public Color blackAlfa0;
        public Color whiteAlfa255;       
        public Color whiteAlfa0;
        public Color green;
        public Color orange;
        public Color black;

        [Header("Reward AD Sprites")]
        public Sprite readyAd;
        public Sprite notReadyAd;

        [Header("Score")]
        public Sprite thorpy;
     

        [Header("URLs")]
        #region URLs
        public string ApplicationURL;
        public string WebsiteURL;
        public string InstagramURL;
        public string YoutubeeURL;
        public string PublisherURL;
        public string MailURL;
        #endregion
    }
}

