using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using DG.Tweening;

namespace DarkJimmy
{
    public class StatsUI : MonoBehaviour
    {
        [SerializeField]
        private StatsType type;
        [SerializeField]
        private  Stats stats;
        [SerializeField]
        private Image statsIcon;
        [SerializeField]
        private Image background;
        [SerializeField]
        private Image frame;
     

        public TMP_Text amount;  
        public Stats Stats
        {
            get { return stats; }
            set { stats = value; }
        }
        public StatsType Type
        {
            get { return type; }
        }

        [HideInInspector]
        public CloudSaveManager csm;
        [HideInInspector]
        public SystemManager system;

        protected int defaultValue;
        protected int minValue;

        public int Value
        {
            get { return defaultValue; }
            set { defaultValue = value < 0 ? minValue : minValue + value; }
        }

        public virtual void Awake()
        {
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;

            system.updateStats += SyncStateValue;
            // system.updatePowerUp += AddPowerUp;
            system.updateGameDisplay += UpdateGameDisplay;

        }
        public virtual void Start()
        {        
            Initialize();
            SetStatsIcon();

           // system.updateStats += SyncStateValue;
           //// system.updatePowerUp += AddPowerUp;
           // system.updateGameDisplay += UpdateGameDisplay;

        }

        public virtual void SetStatsIcon()
        {
            if (statsIcon != null)
                statsIcon.sprite = system.GetStatsIcon(Stats);
        }
        public virtual void SyncStateValue(Stats stats,int value)
        {
            if (Stats != stats)
                return;
               SyncAmount(value);  
        }
        public virtual void UpdateGameDisplay(Stats stats, int value)
        {
            if (Stats != stats)
                return;

           // defaultValue += value;
            amount.text = system.StringFormat(value);
        }        
     
        public virtual void SyncAmount(int value)
        {
            minValue=defaultValue = value;
            amount.text = system.StringFormat(defaultValue);
        }
        public virtual void AddPowerUp(Stats stats, int value)
        {
            if (Stats != stats)
                return;

            Value = value;

             if (Stats.Equals(Stats.JumpCount))
                amount.text = $"{defaultValue}";   
        }  
        public virtual void Initialize()
        {
            if (Enum.TryParse(Stats.ToString(), out GemType gemType))
            {
                int value = csm.GetGemCount(gemType);         
                SyncAmount(value);
            }   
            else if (Enum.TryParse(Stats.ToString(), out Stones stones))
            {
                int value = csm.GetStoneCount(stones);
                SyncAmount(value);
            }
            else if (Stats.Equals(Stats.JumpCount))
            {
                int value = csm.GetCurrentCharacterData().JumpCount;
                SyncAmount(value);
            }
            else if (Stats.Equals(Stats.Key))
            {
                int value = type != StatsType.Useable ? 0 : csm.PlayerDatas.GetAllKeyCount();
                SyncAmount(value);
            }
            else if (Stats.Equals(Stats.Time))
            {
                int value = csm.GetCurrentDefaultLevel().GetLevelTime();
                SyncAmount(value);
            }
        }
     
        public virtual void OnDestroy()
        {
            system.updateStats -= SyncStateValue;
           // system.updatePowerUp -= AddPowerUp;
            system.updateGameDisplay -= UpdateGameDisplay;
        }

        public void StatsColor(Color color,float duration)
        {
            amount.DOColor(color,duration);
            statsIcon.DOColor(color, duration);
            background.DOColor(color, duration);
            frame.DOColor(color, duration);

        }
    }

    public enum Stats
    {
        Gold,
        Diamond,
        Key,
        HP,
        Mana,
        Time,
        JumpCount,
        Speed,
        Philosophy,
        LifeCrystal,
        PowerCrystal,
        Moonstone
    }
    public enum StatsType
    {
        Useable,
        Unuseable
    }

}

