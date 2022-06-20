using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DarkJimmy
{
    public class GameSaveManager : Singleton<GameSaveManager>
    {
        [SerializeField]
        private GameObject GameElement;
     
        public int goldCount;
        public int keyCount;
        public float mana;
        public float energy;
        public int timer;
        public int diamond;
        public int jumpCount;

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

        private SystemManager system;
        

        private void Start()
        {
            system = SystemManager.Instance;
            goldCount = CloudSaveManager.Instance.PlayerDatas.Gold;
            keyCount = CloudSaveManager.Instance.PlayerDatas.Key;
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
                case Stats.Diamond:
                    diamond = value;
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
                Stats.Diamond => diamond,
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
            SystemManager.Instance.updateStats(stats,value);
        }
        public void UpdateCapacity(Stats stats, int value)
        {
            if (stats.Equals(Stats.Energy))
            {
                energy += value;
                maxEnergy = energy;
                system.setStats(stats, maxEnergy);
            }
            else if (stats.Equals(Stats.Mana))
            {
                mana += value;
                maxMana =mana;
               system.setStats(stats, maxMana);
            }
        }
        public void GenerateGameElement()
        {
            GameElement.SetActive(true);
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
