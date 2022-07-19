using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DarkJimmy.UI;
using DarkJimmy.Objects;
using DarkJimmy.Characters;

namespace DarkJimmy
{
    public class GameSaveManager : Singleton<GameSaveManager>
    {
        [SerializeField]
        private GameObject GameElement;
        [SerializeField]
        private ParticleSystem sparkle;

        #region Collections
        private readonly Stack<GameObject> elementStack = new Stack<GameObject>();
        private readonly Stack<CheckPoint> checkPoints = new Stack<CheckPoint>();
        private readonly Queue<ParticleSystem> sparkles = new Queue<ParticleSystem>();
        private int stackObjectCount;
        #endregion

        #region Delegates
        public delegate void JumpCountUpdate(int count);
        public JumpCountUpdate jumpCountUpdate;

        public delegate void EndGame(Menu.Menus menu);
        public EndGame endGame;

        public delegate void PauseGame();
        public PauseGame pause;
        public PauseGame timeOut;
        #endregion

        #region Fields
        #region Values
        [HideInInspector]
        public int Gold;
        [HideInInspector]
        public int Key;
        [HideInInspector]
        public int Mana;
        [HideInInspector]
        public int HP;
        [HideInInspector]
        public int Timer;
        [HideInInspector]
        public int Diamond;
        [HideInInspector]
        public int JumpCount;
        [HideInInspector]
        public int Speed;

        private int maxMana;
        private int maxEnergy;
        private int maxJumpCount;
        private int maxSpeed;
        private int maxTime;

        private int countDown;
        private int startTime;
        private int startGold;
        private int onCheckPointPlayerDir;

        private const int sparkleCount = 5;
        private int showAdDelay = 2;
        #endregion
        #region Properties
        public int CountDown
        {
            get { return countDown; }
            set
            {
                countDown = value;
            }
        }
        public bool IsStartGame { get; set; } = false;
        public bool IsVictory { get; set; } = false;
        public bool IsDefeat { get; set; } = false;
        public bool IsEndGame { get { return IsVictory || IsDefeat; } }
        public bool CanPlay { get { return IsStartGame && !IsEndGame; } }
        public PlayerMovement Player { get; set; }

        private Platform Platform { get; set; }
        private Menu.Menus EndGameMenus { get; set; }

        #endregion
        #endregion

        #region References
        private SystemManager system;
        private CloudSaveManager csm;
       
        #endregion

        private void Start()
        {
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;

            Initialize();
            GenerateLevel();

            system.updatePowerUp += UpdatePoweUp;
            system.updateGMStats += UpdateGMStats;
            system.addGameElement += AddGameElement;

            endGame += SetEndGameDisplay;
            system.particle += PlaySparkle;
        }
        private void Initialize()
        {
            startGold = Gold = csm.PlayerDatas.Gold;
            maxMana = Mana = csm.GetCurrentCharacterData().GetCurrentCharacterProperty(CharacterProperty.Mana);
            maxEnergy = HP = csm.GetCurrentCharacterData().GetCurrentCharacterProperty(CharacterProperty.HP);
            maxJumpCount = JumpCount = csm.GetCurrentCharacterData().JumpCount;
            startTime = CountDown = csm.GetCurrentDefaultLevel().GetLevelTime();

            //Create DefaultSparkle..

            for (int i = 0; i < sparkleCount; i++)
            {
                ParticleSystem _sparkle=Instantiate(sparkle, transform);
                sparkles.Enqueue(_sparkle);
            }
        }
        public void PlaySparkle(Material material, Vector3 position)
        {
            ParticleSystem _sparkle=sparkles.Dequeue();
            _sparkle.GetComponent<Renderer>().material = material;
            _sparkle.transform.position = position;
            _sparkle.Play();
            sparkles.Enqueue(_sparkle);

        }
        public void GenerateLevel()
        {
            string path = $"Levels/Level {csm.StageIndex + 1}-{csm.LevelIndex + 1}";
            system.SelectedLevel = csm.GetCurrentDefaultLevel();
            Platform = Resources.Load<Platform>(path);

            if (Platform == null)
                return;

            Instantiate(Platform,GameElement.transform);

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

            for (int i = 0; i < count-4; i++)
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
                case Stats.HP:
                    return ref HP;
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
                case Stats.HP:
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
            AdManager.Instance.onClosed += OpenEndGameDisplay;
            GameElement.SetActive(true);
            AudioManager.Instance.PlayMusic("Game Theme");

        }
        private void SetEndGameDisplay( Menu.Menus menu)
        {
            EndGameMenus = menu;
            csm.AddGem(GemType.Gold,GetValueResult(Result.Gold, out int value));
            AudioManager.Instance.PlaySound(menu.ToString());
            AudioManager.Instance.StopSource(SoundGroupType.Music);
            StartCoroutine(nameof(ShowAd));
        }
        IEnumerator ShowAd()
        {
            float time =0;
            while (time<=1)
            {
                time += Time.deltaTime / showAdDelay;

                while (!IsStartGame)
                    yield return null;

                yield return null;
            }

            AdManager.Instance.ShowInterstitial(); 
        }
        private void OpenEndGameDisplay()
        {
            UIManager.Instance.OpenMenu(EndGameMenus);
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
        private IEnumerator CountDownTimer()
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
                IsDefeat = true;
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

        public bool IsMaxValue(Stats stats)
        {
            return GetValue(stats) == GetMaxValue(stats);
        }
        public int ValueDiff(Stats stats)
        {
           return   GetMaxValue(stats)- GetValue(stats);
        } 

        #region Review for Game
        /// <summary>
        /// When the checkpoint is reached, that checkpoint is added to the stack.
        /// </summary>
        /// <param name="checkPoint"></param>
        public void RegisterCheckPoint(CheckPoint checkPoint)
        {
            checkPoints.Push(checkPoint);
        }
        /// <summary>
        /// Checks if any checkpoint has been reached.
        /// </summary>
        /// <returns></returns>
        public bool AnyReachedCheckPoint()
        {
            return checkPoints.Count != 0;
        }
        /// <summary>
        /// Adds interacted objects of type GameObject to the stack.
        /// </summary>
        /// <param name="obj"></param>
        private void AddGameElement(GameObject obj)
        {
            elementStack.Push(obj);
            stackObjectCount++;
        }
        /// <summary>
        /// saves the values.
        /// </summary>
        /// <param name="isOn"></param>
        public void SetElements(bool isOn)
        {
            if (isOn)
            {
                stackObjectCount = 0;
                onCheckPointPlayerDir = Player.horizontal = Player.direction;
            }
            else
                Player.horizontal = onCheckPointPlayerDir;

            int count = Enum.GetNames(typeof(Stats)).Length;

            for (int i = 0; i < count - 4; i++)
            {
                Stats stats = (Stats)i;

                if (stats == Stats.Gold || stats == Stats.Key)
                    continue;

                if (isOn)
                {
                    int amount = stats.Equals(Stats.Time) ? CountDown : GetValue(stats);
                    PlayerPrefs.SetInt(stats.ToString(), amount);
                }
                else
                {
                    if (stats.Equals(Stats.Time))
                    {
                        int time = PlayerPrefs.GetInt(stats.ToString());
                        CountDown = time <=15?time +15:time;
                        system.updateStats(stats, CountDown);
                    }
                    else
                    {
                        GetValue(stats) = PlayerPrefs.GetInt(stats.ToString());
                        system.updateGameDisplay(stats, GetValue(stats));
                    }
                }
            }
        }
        /// <summary>
        /// When the game rewinds, it reactivates the elements in the stack.
        /// </summary>
        /// <param name="isOn"></param>
        public void ToRewindElements()
        {
            for (int i = 0; i < stackObjectCount; i++)
            {
                // take out from stack next object
                GameObject element = elementStack.Pop();

                // type control..

                if (element.TryGetComponent(out Lever lever))
                    lever.GetActivate();
                else
                {
                    element.SetActive(true);
                }
            }

           // restore current values

            SetElements(false);

            IsStartGame = false;
            IsDefeat = false;
            pause();
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            player.isAlive = true;
            CheckPoint cp = checkPoints.Pop();
            cp.RewindCheckPoint();
            player.transform.position = cp.gameObject.transform.position;
            AudioManager.Instance.PlayMusic("Game Theme");


        }
        #endregion

        private void OnDestroy()
        {
            system.updatePowerUp -= UpdatePoweUp;
            system.updateGMStats -= UpdateGMStats;
            system.addGameElement -= AddGameElement;
            AdManager.Instance.onClosed -= OpenEndGameDisplay;
            system.particle -= PlaySparkle;
        }

    }

}
