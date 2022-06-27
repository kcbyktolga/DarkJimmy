using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace DarkJimmy.UI
{
    public class Victory : Menu
    {
        [Header("victory")]
        [SerializeField]
        private Image dimed;
        [SerializeField]
        private List<LevelResult> results;
        
        private GameSaveManager gsm;
        private SystemManager system;
        private CloudSaveManager csm;
        private readonly float duration = 0.5f;
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

            level.levelStatus = LevelStatus.Passed;

            for (int i = 0; i < results.Count; i++)
            {
                int resultValue = gsm.GetValueResult(results[i].result, out int maxValue);
                SaveValue(ref level, results[i].result, resultValue);
            }
            csm.SetLevel(level);
        
            StartCoroutine(ChangeColor());
        }
        IEnumerator ChangeColor()
        {
            Color startColor = system.GetWhiteAlfaColor(false);
            Color endColor = system.GetWhiteAlfaColor(true);
            
            float time = 0;

            while (time<=1)
            {
                time += Time.deltaTime / duration;
                Color color = Color.Lerp(startColor, endColor, time);
                dimed.color = pageName.color = color;           
                yield return null;
            }
  
            for (int i = 0; i < results.Count; i++)
            {
                AudioManager.Instance.PlaySound("Pop Up");
                int resultValue = gsm.GetValueResult(results[i].result,out int maxValue);
                results[i].SetResultValue(0,maxValue);

                time = 0;

                while (time <= 1)
                {
                    time += Time.deltaTime / duration;
                    Color color = Color.Lerp(startColor,endColor,time);
                    results[i].SetColor(color);
                    yield return null;
                }

                int value = 0;
                float _duration = resultValue > 50 ? 0.001f : 0.1f;

                while (value < resultValue)
                {                
                    value++; 
                    results[i].SetResultValue(value,maxValue);
                    AudioManager.Instance.PlaySound("Click Up");
                    yield return new WaitForSeconds(_duration);
                }
                yield return new WaitForSeconds(duration);
            }

            yield return new WaitForSeconds(5);

            Fade.Instance.FadeOut(LoadStage);
        }

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
        private void LoadStage()
        {
            SceneManager.LoadScene(Menus.Stages.ToString());
        }
    }
}

