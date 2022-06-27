using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace DarkJimmy
{
    public class SystemManager : Singleton<SystemManager>
    {
        public SystemProperty system;

        public delegate void UpdateStats(Stats stats, int value);
        public UpdateStats updateStats;
        public UpdateStats updateStatsCapacity;

  
        public GemType GemType { get; set; }
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
                GemType.Diamond => system.diamond,
                _ => system.gold,
            };
        }
        public Sprite GetProductBackground(TypeofProduct type)
        {
            return type switch
            {
                TypeofProduct.Gold => system.goldSprite,
                TypeofProduct.Diamond => system.diamondSprite,
                TypeofProduct.Premium => system.premiumSprite,
                TypeofProduct.Offers => system.offersSprite,
                _ => system.premiumSprite,
            };
        }
        public string StringFormat(int count)
        {
            return $"{string.Format("{0:N0}", count)}";
        }
        public int GetShopPage(string type)
        {
            if (Enum.TryParse(type, out TypeofProduct pageIndex))
                return (int)pageIndex;

            return 0;
        }
        public Color GetWhiteAlfaColor(bool isOn)
        {
            return isOn ? system.whiteAlfa255 : system.whiteAlfa0;
        }
        public Color GetBlackAlfaColor(bool isOn)
        {
            return isOn ? system.blackAlfa255 : system.blackAlfa0;
        }
        public string GetUrlAddress(string address)
        {
            return address switch
            {
                "YoutubeURL" => system.YoutubeeURL,
                "AppURL" => system.ApplicationURL,
                "PublisherURL" => system.PublisherURL,
                "WebsiteURL" => system.WebsiteURL,
                "Mail" => system.MailURL,
                "InstagramURL"=> system.InstagramURL,
                _=> string.Empty
            };
        }

    }
    public enum GemType
    {
        Gold,
        Diamond,
    }
    public enum RewardType
    {
        Gold,
        Diamond,
        LuckySpin,
        InGame,
        EndGame
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

        [Header("Product Background Sprite")]
        public Sprite goldSprite;
        public Sprite diamondSprite;
        public Sprite offersSprite;
        public Sprite premiumSprite;

        [Header("Colors")]

        public Color blackAlfa255;
        public Color blackAlfa0;
        public Color whiteAlfa255;       
        public Color whiteAlfa0;

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

