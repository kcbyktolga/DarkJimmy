using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class LevelTab : BaseButton
    {
        [Header("Level Tab Property")]
        [SerializeField]
        private GameObject focus;
        [SerializeField]
        private TMP_Text levelIndex;
        [SerializeField]
        private TMP_Text levelText;
        [SerializeField]
        private List<Image> stars;
        [SerializeField]
        private float duration = 0.25f;
        [SerializeField]
        private Color onColor;
        [SerializeField]
        private Color offColor;

        private void Start()
        {
            levelText.text = LanguageManager.GetText("Level");
        }

        public void SetLevelTab(int index, Level level)
        {
            levelIndex.text = $"{index}";

            if (level.levelStatus.Equals(LevelStatus.Passed))
                StartCoroutine(ChangeColor(level.rankCount));

            focus.SetActive(level.levelStatus.Equals(LevelStatus.Active));

        }
        IEnumerator ChangeColor(int count)
        {
            int index = 0;

            while (index < count)
            {
                float time = 0;
  
                while (time <= duration)
                {
                    time += Time.deltaTime/duration;
                    stars[index].color = Color.Lerp(offColor,onColor,time);
                    yield return null;
                }

                index++;
                yield return null;
            }
        }  
    }
}

