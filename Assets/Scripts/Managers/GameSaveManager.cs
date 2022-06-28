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

            Initialize();
            GenerateLevel();

            system.updatePowerUp += UpdatePoweUp;
            system.updateGMStats += UpdateGMStats;
        }

        private void Initialize()
        {
            startGold = Gold = csm.PlayerDatas.Gold;
            maxMana = Mana = csm.GetCurrentCharacterData().Mana;
            maxEnergy = Energy = csm.GetCurrentCharacterData().Energy;
            maxJumpCount = JumpCount = csm.GetCurrentCharacterData().JumpCount;
            startTime = CountDown = csm.GetCurrentDefaultLevel().GetLevelTime();
        }

        public void GenerateLevel()
        {
            Platform _platform = csm.GetCurrentDefaultLevel().GetPlatform();

            if (_platform == null)
                return;
          
            platform = Instantiate(_platform,GameElement.transform);
        }
        private void UpdatePoweUp(Stats stats, int value)
        {
            GetValue(stats) = value < 0 ? GetMaxValue(stats) : GetMaxValue(stats) + value;
            system.updateStats(stats,GetValue(stats));
        }
        private void UpdateGMStats(Stats stats, int value)
        {
            if (stats.Equals(Stats.Key) || stats.Equals(Stats.Gold))
                GetValue(stats) += value;
            else if (stats.Equals(Stats.Time))
            {
                CountDown += value + GetValue(stats);
                system.updateStats(stats, CountDown);
                return;
            }
                
            else
                GetValue(stats) = GetValue(stats) + value > GetMaxValue(stats) ? GetMaxValue(stats) : GetValue(stats) + value;

            system.updateGameDisplay(stats, GetValue(stats));
        }

        private void SetCapacity()
        {
            int count = Enum.GetNames(typeof(Stats)).Length; ;

            for (int i = 0; i < count; i++)
            {
                Stats stats = (Stats)i;

                if (stats.Equals(Stats.Key) || stats.Equals(Stats.Gold) || stats.Equals(Stats.Time))
                    continue;

                GetMaxValue(stats) = GetValue(stats);                   
            }
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
                case Stats.Time:
                    return ref Timer;
                case Stats.JumpCount:
                    return ref JumpCount;
                case Stats.Speed:
                    return ref Speed;
            }
        }
        private ref int GetMaxValue(Stats stats)
        {
            switch (stats)
            {
                default:
                case Stats.Energy:
                    return ref maxEnergy;
                case Stats.Mana:
                    return ref maxMana;
                case Stats.Time:
                    return ref maxTime;
                case Stats.JumpCount:
                    return ref maxJumpCount;
                case Stats.Speed:
                    return ref maxSpeed;
            }
        }
        public void ActivateGameElement()
        {
            SetCapacity();
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
                    system.updateStats(Stats.Time,CountDown);

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
    
    }

}
