using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DarkJimmy.UI
{
    public class LevelStatistic : MonoBehaviour
    {
        [Header("Level Popup Property")]
        [SerializeField]
        private TMP_Text headerName;
        [SerializeField]
        private TMP_Text keyCount;
        [SerializeField]
        private TMP_Text goldCount;
        [SerializeField]
        private TMP_Text currentScore;
        [SerializeField]
        private TMP_Text maxScore;
        [SerializeField]
        private TMP_Text keyCountText;
        [SerializeField]
        private TMP_Text goldCountText;
        [SerializeField]
        private TMP_Text currentScoreText;
        [SerializeField]
        private TMP_Text maxScoreText;


        private void Start()
        {
            Level level = CloudSaveManager.Instance.GetLevel();
            SetPageProperty(level);
        }

        private void SetPageProperty(Level level)
        {
            keyCount.text = $"{level.keyCount}";
            goldCount.text = $"{level.goldCount}";
            currentScore.text = $"{level.currentScore}";
            maxScore.text = $"{level.maxScore}";

            headerName.text= LanguageManager.GetText("Statýstýcs");
            keyCountText.text = LanguageManager.GetText("Key");
            goldCountText.text = LanguageManager.GetText("Gold");
            currentScoreText.text = LanguageManager.GetText("Current Score");
            maxScoreText.text = LanguageManager.GetText("Max Score");
        }
    }

}
