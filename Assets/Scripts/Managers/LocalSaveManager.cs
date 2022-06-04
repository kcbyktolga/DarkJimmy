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
        public float mana;
        public float energy;
        public int timer;
        public int token;

        float maxMana;
        float maxEnergy;
        public float Mana 
        { 
            get 
            { 
                return mana; 
            }           
            set 
            {
                if (value > maxMana)
                    mana = maxMana;
                else
                    mana = value;
            } 
        }
        public float Energy
        {
            get
            {
                return energy;
            }
            set
            {
                if (value > maxEnergy)
                    energy = maxEnergy;
                else
                    energy = value;
            }
        }

        private void Start()
        {
            goldCount = CloudSaveManager.Instance.playerData.Gold;
            keyCount = CloudSaveManager.Instance.playerData.Key;
            maxMana= mana = CloudSaveManager.Instance.GetCurrentCharacterData().Mana;
            maxEnergy= energy = CloudSaveManager.Instance.GetCurrentCharacterData().Energy;
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
                    Energy = value;
                    break;
                case Stats.Mana:
                    Mana = value;
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
                Stats.Energy => (int)energy,
                Stats.Mana => (int)mana,
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
                CloudSaveManager.Instance.SetGem(gemType,GetValue(stats));
        }
    }


}
