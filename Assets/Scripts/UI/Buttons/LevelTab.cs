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
        private float duration = 0.01f;    
        [SerializeField]
        private Image background;
        [SerializeField]
        private Sprite active;
        [SerializeField]
        private Sprite passed;

        SystemManager system;
        private void Start()
        {
            system = SystemManager.Instance;
            levelText.text = LanguageManager.GetText("Level");
        }
        public override void OpenPage()
        {
            UIManager.Instance.OpenMenu(Menu.Menus.LevelPrevious);
        }
        public void SetLevelTab(int index, Level level, bool isLocked)
        {
            levelIndex.text = $"{index+1}";

            if (level.levelStatus.Equals(LevelStatus.Passed))
                StartCoroutine(ChangeStartColor(level.rankCount));

            background.sprite = level.levelStatus.Equals(LevelStatus.Passed) ? passed : active;

   
            focus.SetActive(level.levelStatus.Equals(LevelStatus.Active) && !isLocked);

        }
        private IEnumerator ChangeStartColor(int count)
        {
            int index = 0;

            while (index < count)
            {
                float time = 0;
  
                while (time <= 1)
                {
                    time += Time.deltaTime/duration;
                    Color color = Color.Lerp(system.GetWhiteAlfaColor(false), system.GetWhiteAlfaColor(true), time);
                    stars[index].color = color;
                    yield return null;
                }

                index++;
                yield return null;
            }
        }  

    }
}

