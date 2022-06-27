using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DarkJimmy.UI;
using DarkJimmy.Objects;

namespace DarkJimmy
{
    public class GameSaveManager : Singleton<GameSaveManager>
    {
        [SerializeField]
        private GameObject GameElement;

        public delegate void JumpCountUpdate(int count);
        public JumpCountUpdate jumpCountUpdate;

        public delegate void PauseGame();
        public PauseGame pause;
        public PauseGame timeOut;

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

        private int startTime;
        private int startGold;

        private SystemManager system;
        private CloudSaveManager csm;
        private Platform platform;
        public bool IsStartGame { get; set; } = false;
        public bool IsWon { get; set; } = false;
        public bool IsLose { get; set; } = false;
        public bool IsEndGame { get { return IsWon || IsLose; }}
        public bool CanPlay { get { return IsStartGame && !IsEndGame; } }
        private void Start()
        {
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;

            startGold=Gold =csm.PlayerDatas.Gold;
            
            maxMana= Mana = csm.GetCurrentCharacterData().Mana;
            maxEnergy= Energy = csm.GetCurrentCharacterData().Energy;
            maxJumpCount=JumpCount = csm.GetCurrentCharacterData().JumpCount;
            startTime = CountDown = csm.GetCurrentDefaultLevel().GetLevelTime();

            GenerateLevel();
        }

        public void GenerateLevel()
        {
            Platform _platform = csm.GetCurrentDefaultLevel().GetPlatform();

            if (_platform == null)
                return;
          
            platform = Instantiate(_platform,GameElement.transform);
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
            while (CountDown>0 && CanPlay)
            {
                CountDown--;
                if (CountDown < 10)
                    AudioManager.Instance.PlaySound("Tick Tock");
                system.updateStats(Stats.Timer,CountDown);
                yield return new WaitForSeconds(1);
            }

            if (CountDown <= 0)
            {
                IsLose = true;
                timeOut();
            }
                
        }

        public int GetValueResult(Result result, out int maxValue)
        {
            if (result.Equals(Result.Score))
            {
                int max = csm.GetCurrentLevel().maxScore;
                int score= CalculateScore();
                maxValue = max > score ? max : score;
                return score;

            }
            else if (result.Equals(Result.Gold))
            {
                maxValue = GetValue(Stats.Gold);
                return maxValue - startGold;
            }
            else if(result.Equals(Result.Time))
            {
                maxValue = startTime;
                return CountDown;
            }
            else
            {
                maxValue = csm.GetCurrentDefaultLevel().keyCount;
                return Key;
            }
        }
        private int CalculateScore()
        {
            return CountDown * 2 + (Gold-startGold) + Key * 10;
        }
    
        private void SaveData(Stats stats)
        {
            if (Enum.TryParse(stats.ToString(), out GemType gemType))
                CloudSaveManager.Instance.SetGem(gemType,GetValue(stats));
        }

    }

}
