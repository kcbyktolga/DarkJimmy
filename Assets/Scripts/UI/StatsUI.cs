using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace DarkJimmy
{
    public class StatsUI : MonoBehaviour
    {
        [SerializeField]
        private  StatsType type;
        [SerializeField]
        private  Stats stats;    
        public TMP_Text amount;  
        public Stats Stats
        {
            get { return stats; }
        }

        [HideInInspector]
        public CloudSaveManager csm;
        [HideInInspector]
        public SystemManager system;

        public virtual void Start()
        {
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;

            SetStatsValue();
            system.updateStats += UpdateStats;
            system.updateStatsCapacity += SetStatsPowerUp;
        }
        public virtual void UpdateStats(Stats stats,int value)
        {
            if (Stats != stats)
                return;

            if (amount != null)
                SetAmount(value);
            
        }
        public virtual void SetAmount(int value)
        {
            amount.text = system.StringFormat(value);
        }
        public virtual void SetStatsPowerUp(Stats stats, int value)
        {
            if (Stats != stats)
                return;

             if (Stats.Equals(Stats.JumpCount))
                amount.text = $"{value}";   
        }  
        public virtual void SetStatsValue()
        {
            if (Enum.TryParse(stats.ToString(), out GemType gemType))
                amount.text = system.StringFormat(csm.GetGemCount(gemType));
            else if (stats.Equals(Stats.JumpCount))
                amount.text = $"{csm.GetCurrentCharacterData().JumpCount}";
            else if (Stats.Equals(Stats.Key) && type.Equals(StatsType.Selectable))
                amount.text=$"{csm.PlayerDatas.GetAllKeyCount()}";      
        }
    }

    public enum Stats
    {
        Gold,
        Diamond,
        Key,
        Energy,
        Mana,
        Timer,
        JumpCount,
        Speed
    }
    public enum StatsType
    {
        Selectable,
        UnSelectable
    }

}

