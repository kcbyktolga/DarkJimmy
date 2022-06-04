using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DarkJimmy
{
    public class LocalSaveManager : Singleton<LocalSaveManager>
    {
        public int goldCount;
        public int keyCount;
        public int mana;
        public int energy;
        public int timer;
        public int token;

        private void Start()
        {
           
        }

        private void SetValue(Stats stats, int value)
        {
            switch (stats)
            {
                case Stats.Gold:
                    goldCount = value;
                    break;
                case Stats.Token:
                    token = value;
                    break;
                case Stats.Key:
                    keyCount = value;
                    break;
                case Stats.Energy:
                    energy = value;
                    break;
                case Stats.Mana:
                    mana = value;
                    break;
                case Stats.Timer:
                    timer = value;
                    break;
 
            }
        }
        private int GetValue(Stats stats)
        {
            return stats switch
            {
                Stats.Token => token,
                Stats.Key => keyCount,
                Stats.Energy => energy,
                Stats.Mana => mana,
                Stats.Timer => timer,
                _ => goldCount,
            };
        }
        public void AddCollectable(Stats stats, int amount)
        {
            int value = GetValue(stats) + amount;
            SetValue(stats,value);
            UIManager.Instance.updateState(stats,value);
        }

        private void OnDestroy()
        {
            SaveData(Stats.Gold);
            SaveData(Stats.Key);
        }

        void SaveData(Stats stats)
        {
            if (Enum.TryParse(stats.ToString(), out GemType gemType))
                CloudSaveManager.Instance.AddGem(gemType,GetValue(stats));
        }
    }


}
