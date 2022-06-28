using UnityEngine;
using TMPro;
using System;

namespace DarkJimmy
{
    public class StatsUI : MonoBehaviour
    {
        [SerializeField]
        private StatsType type;
        [SerializeField]
        private  Stats stats;    
        public TMP_Text amount;  
        public Stats Stats
        {
            get { return stats; }
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
        public virtual void Start()
        {
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;

            Initialize();
            system.updateStats += SyncStateValue;
           // system.updatePowerUp += AddPowerUp;
            system.updateGameDisplay += UpdateGameDisplay;
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
    }

    public enum Stats
    {
        Gold,
        Diamond,
        Key,
        Energy,
        Mana,
        Time,
        JumpCount,
        Speed
    }
    public enum StatsType
    {
        Useable,
        Unuseable
    }

}

