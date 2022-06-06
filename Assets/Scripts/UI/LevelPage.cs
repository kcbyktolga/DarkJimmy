using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DarkJimmy.UI
{
    public class LevelPage : TabGenerator<LevelTab,Stage>
    {
        [SerializeField]
        private TMP_Text pageName;
        public override void Generate()
        {
            pageName.text = LanguageManager.GetText(data.stageName);

            for (int i = 0; i < data.levels.Count; i++)
            {
                LevelTab levelTab = Instantiate(prefab, container);

                levelTab.SetLevelTab(i,data.levels[i], data.stageIsLocked);
                tabs.Add(levelTab);
                levelTab.OnClick(i,OnSelect);
            }
        }

        public override void OnSelect(int index)
        {
           // base.OnSelect(index);
           // LevelTab previous = GetTab(PreviousIndex);

            LevelTab next = GetTab(NextIndex);

            CloudSaveManager.Instance.LevelIndex = index;

            next.OpenPage();

        }

    }
}

