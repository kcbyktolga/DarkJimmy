using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace DarkJimmy.UI
{
    public class LevelTab : BaseButton
    {
        [Header("Level Tab Property")]
        [SerializeField]
        private GameObject focus;
        [SerializeField]
        private TMP_Text levelText;
        [SerializeField]
        private List<Image> stars;
        [SerializeField]
        private float duration = 0.5f;    
        [SerializeField]
        private Image mask;
        [SerializeField]
        private Image levelIcon;
        [SerializeField]
        private Image frame;

        private SystemManager system;
 
        private void Awake()
        {
            system = SystemManager.Instance;
            
        }
        public override void OpenPage()
        {
            UIManager.Instance.OpenMenu(Menu.Menus.LevelPrevious);
        }
        public void SetLevelTab(int index, Level level, bool isLocked)
        {
            if (level.levelStatus.Equals(LevelStatus.Passed))
            {
                for (int i = 0; i < level.rankCount; i++)
                    stars[i].DOColor(system.GetWhiteAlfaColor(true), duration);
            }

            frame.color = system.GetLevelColor(level.levelStatus);
            mask.enabled = level.levelStatus.Equals(LevelStatus.Passive);
            focus.SetActive(level.levelStatus.Equals(LevelStatus.Active) && !isLocked);
        }

        public void SetLevelImage(Level defaultLevel)
        {
            if (defaultLevel.GetLevelIcon() != null)
                levelIcon.sprite = defaultLevel.GetLevelIcon();
        }

        public void SetLevelName(int stageIndex,int levelIndex)
        {
            levelText.text = $"{LanguageManager.GetText("Level")} {stageIndex+1}-{levelIndex+1}";
        }

    }
}

