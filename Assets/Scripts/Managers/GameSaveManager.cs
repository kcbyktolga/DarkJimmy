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

        public delegate void JumpCountUpdate(int count);
        public JumpCountUpdate jumpCountUpdate;

        public delegate void StartGame();
        public StartGame starter;

        public int Gold;
        public int Key;
        public int Mana;
        public int Energy;
        public int Timer;
        public int Diamond;
        public int JumpCount;
        public int Speed;

        private int maxMana;
        private int maxEnergy;
        private int maxJumpCount;
        private int maxSpeed;
        private int maxTime;

        private int countDown;
        public int CountDown
        {
            get { return countDown; }
            set
            {
                countDown = value;
            }
        }
   
        private SystemManager system;
        private CloudSaveManager csm;

        public bool IsStartGame { get; set; } = false;
        public bool IsEndGame { get; set; } = false;
        public bool CanPlay { get { return IsStartGame && !IsEndGame; } }
        private void Start()
        {
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;

            Gold =csm.PlayerDatas.Gold;
            
            maxMana= Mana = csm.GetCurrentCharacterData().Mana;
            maxEnergy= Energy = csm.GetCurrentCharacterData().Energy;
            maxJumpCount=JumpCount = csm.GetCurrentCharacterData().JumpCount;
            CountDown = csm.GetCurrentDefaultLevel().GetLevelTime();
        }

        private ref int  GetValue(Stats stats)
        {
            switch (stats)
            {
                default:
                case Stats.Gold:
                    return ref Gold;
                case Stats.Diamond:
                    return ref Diamond;
                case Stats.Key:
                    return ref Key;
                case Stats.Energy:
                    return ref Energy;
                case Stats.Mana:
                    return ref Mana;
                case Stats.Timer:
                    return ref Timer;
                case Stats.JumpCount:
                    return ref JumpCount;
                case Stats.Speed:
                    return ref Speed;
            }
        }
        private void SetValue(Stats stats, int value)
        {
            if (stats.Equals(Stats.Energy))
                ClampCapacity(stats, value, ref maxEnergy);
            else if (stats.Equals(Stats.Mana))
                ClampCapacity(stats, value, ref maxMana);
            else
                GetValue(stats) = value;
        }
        private void SetCapacity(Stats stats,int value, ref int maxCapacity)
        {
            GetValue(stats) += value;
            maxCapacity = GetValue(stats);
            system.updateStatsCapacity(stats, maxCapacity);
        }
        private void ClampCapacity(Stats stats, int value, ref int maxCapacity)
        {
            if (value > maxCapacity)
                GetValue(stats) = maxCapacity;
            else
                GetValue(stats) += value;
        }
        public void UpdateStatsValue(Stats stats, int amount)
        {
            int value = GetValue(stats) + amount;
            SetValue(stats,value);
            system.updateStats(stats,value);
        }
        public void UpdateCapacity(Stats stats, int value)
        {
            if (stats.Equals(Stats.Energy))
                SetCapacity(stats, value, ref maxEnergy);
            else if (stats.Equals(Stats.Mana))
                SetCapacity(stats, value, ref maxMana);
            else if (stats.Equals(Stats.JumpCount))
                SetCapacity(stats, value, ref maxJumpCount);
            else if (stats.Equals(Stats.Speed))
                SetCapacity(stats, value, ref maxSpeed);
            else if (stats.Equals(Stats.Speed))
                SetCapacity(stats, value, ref maxTime);

        }
        public void GenerateGameElement()
        {
            GameElement.SetActive(true);
        }
        public float GetMultiple(Stats stats)
        {
            if (stats.Equals(Stats.Speed))
                return 1 + GetValue(Stats.Speed) * 0.01f;
            else
                return GetValue(stats);
        }
        public void StartCountDownTimer()
        {
            StartCoroutine(CountDownTimer());
        }
        IEnumerator CountDownTimer()
        {
            while (CountDown>0 && IsStartGame)
            {
                CountDown--;
                system.updateStats(Stats.Timer,CountDown);
                yield return new WaitForSeconds(1);
            }

            if (CountDown <= 0)
                IsEndGame = true;
            else
                IsStartGame = false;
        }

        private void OnDestroy()
        {
            csm.SetLevelKey(GetValue(Stats.Key));
            SaveData(Stats.Gold);

        }
        private void SaveData(Stats stats)
        {
            if (Enum.TryParse(stats.ToString(), out GemType gemType))
                CloudSaveManager.Instance.SetGem(gemType,GetValue(stats));
        }
    }


}
