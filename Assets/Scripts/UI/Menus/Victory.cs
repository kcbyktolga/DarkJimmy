using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

namespace DarkJimmy.UI
{
    public class Victory : Menu
    {
        
        [Header("Victory")]
        [SerializeField]
        private Image dimed;
        [SerializeField]
        private TMP_Text levelCount;
        [SerializeField]
        private TMP_Text stageText;
        [SerializeField]
        private TMP_Text levelName;

        [SerializeField]
        private List<LevelResult> results;
        [SerializeField]
        private List<Image> stars;
        
        private GameSaveManager gsm;
        private SystemManager system;
        private CloudSaveManager csm;
        private readonly float duration = 0.5f;

        bool hasPassed = false;
     
        private void Awake()
        {
            gsm = GameSaveManager.Instance;
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;
        }
        public override void Start()
        {
            base.Start();
            Level level = csm.GetCurrentLevel();
            hasPassed = level.levelStatus.Equals(LevelStatus.Passed);

            system.Reward = system.GetReward();
            level.levelStatus = LevelStatus.Passed;



            for (int i = 0; i < results.Count; i++)
            {
                int resultValue = gsm.GetValueResult(results[i].result, out int maxValue);
                SaveValue(ref level, results[i].result, resultValue);
            }
            csm.SetLevel(level);

            //StartCoroutine(ChangeColor());
            Initialize();

        }

        private void Initialize()
        {
            levelCount.text = $"{csm.LevelIndex + 1}";
            stageText.text = $"{LanguageManager.GetText("Stage")}";
            levelName.text = $"{csm.GetDefaultStageName()}-{csm.GetDefaultLevelName()}";
            system.Reward = system.GetReward();

            for (int i = 0; i < results.Count; i++)
            {
                int resultValue = gsm.GetValueResult(results[i].result, out int maxValue);
                results[i].SetResultValue(resultValue, maxValue);
            }

            Invoke(nameof(OpenDimed), duration * 3);
        }

        private void OpenDimed()
        {
            dimed.DOColor(system.GetWhiteAlfaColor(true), duration).OnComplete(ActivateBase);
        }

        public override void ActivateBase()
        {
            base.ActivateBase();
            base.ScaleAnimation();
            dimed.DOKill();
            // Invoke(nameof(OpenStars), duration * 2);
            AudioManager.Instance.PlaySound("Pop Up");
        }

        public override void AfterScaleAnimation()
        {
            base.AfterScaleAnimation();
            StartCoroutine(nameof(OpenStars));       
            Invoke(nameof(OpenRewardPopup), duration * 6);
        }

        private void OpenRewardPopup()
        {
            if (hasPassed)
                Fade.Instance.FadeOut(()=>SceneManager.LoadScene(Menus.Stages.ToString()),null);           
            else
                UIManager.Instance.OpenMenu(Menus.RewardPopup);
        }
        private IEnumerator OpenStars()
        {
            for (int i = 0; i < gsm.GetValueResult(Result.Key, out int max); i++)
            {
                stars[i].DOColor(system.GetWhiteAlfaColor(true), duration).OnComplete(()=>stars[i].DOKill());
                AudioManager.Instance.PlaySound("Card Flip");
                yield return new WaitForSeconds(duration);
            }

        }
        //IEnumerator ChangeColor()
        //{
        //    Color startColor = system.GetWhiteAlfaColor(false);
        //    Color endColor = system.GetWhiteAlfaColor(true);
            
        //    float time = 0;

        //    while (time<=1)
        //    {
        //        time += Time.deltaTime / duration;
        //        Color color = Color.Lerp(startColor, endColor, time);
        //        dimed.color = pageName.color = color;           
        //        yield return null;
        //    }
  
        //    for (int i = 0; i < results.Count; i++)
        //    {
        //        AudioManager.Instance.PlaySound("Pop Up");
        //        int resultValue = gsm.GetValueResult(results[i].result,out int maxValue);
        //        results[i].SetResultValue(0,maxValue);

        //        time = 0;

        //        while (time <= 1)
        //        {
        //            time += Time.deltaTime / duration;
        //            Color color = Color.Lerp(startColor,endColor,time);
        //            results[i].SetColor(color);
        //            yield return null;
        //        }

        //        int value = 0;
        //        float _duration = resultValue > 50 ? 0.001f : 0.1f;

        //        while (value < resultValue)
        //        {                
        //            value++; 
        //            results[i].SetResultValue(value,maxValue);
        //            AudioManager.Instance.PlaySound("Click Up");
        //            yield return new WaitForSeconds(_duration);
        //        }
        //        yield return new WaitForSeconds(duration);
        //    }

        //    if (!LocalSaveManager.GetBoolValue("Rate", false) && DateTime.Now > LocalSaveManager.GetResetTime("ResetRate"))
        //        UIManager.Instance.OpenMenu(Menus.RateGame);


        //    yield return new WaitForSeconds(3);

        //    while (UIManager.Instance.GetCurrentMenu().GetType().Equals(typeof(RateGamePopup)))
        //        yield return null;

        //    Fade.Instance.FadeOut(()=> SceneManager.LoadScene(Menus.Stages.ToString()));
        //}

        private void SaveValue(ref Level level,Result result ,int value)
        {
            switch (result)
            {
                case Result.Time:
                    break;
                case Result.Gold:
                    level.goldCount += value;
                    break;
                case Result.Key:
                    int count = level.keyCount > value ? level.keyCount : value;
                    level.rankCount = level.keyCount = count;
                    break;
                case Result.Score:
                    int maxScore = level.maxScore > value ? level.maxScore : value;
                    level.maxScore = maxScore;
                    level.currentScore = value;

                    break;
                default:
                    break;
            }

        }

        public override void OnEnable()
        {
           // base.OnEnable();
        }

    }
}

