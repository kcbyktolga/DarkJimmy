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
        [SerializeField]
        private GameObject lockPanel;
        [SerializeField]
        private PurchaseButton purchaseButton;
        [SerializeField]
        private TMP_Text description;
      

        public override void Generate()
        {
            pageName.text = LanguageManager.GetText(globalData.stageName);

            lockPanel.SetActive(globalData.stageIsLocked);

            if (globalData.stageIsLocked)
            {
                purchaseButton.buttonName.text = $"{localData.GetStagePrice()}";
                purchaseButton.priceIcon.sprite = CloudSaveManager.Instance.GetPaySprite(localData.GetPayType());
                purchaseButton.OnClick(Purchase);
            }

            bool priority = false;

            for (int i = 0; i < globalData.levels.Count; i++)
            {
                LevelTab levelTab = Instantiate(prefab, container);


                if (globalData.levels[i].levelStatus.Equals(LevelStatus.Passive) && !priority && PasedCheck(i))
                {
                    globalData.levels[i].levelStatus = LevelStatus.Active;
                    priority = true;
                }

                levelTab.SetLevelTab(i, globalData.levels[i], globalData.stageIsLocked);
                tabs.Add(levelTab);
                levelTab.OnClick(i, OnSelect);
            }
        }

        private void UpdateLevelTab()
        {
            bool priority = false;

            for (int i = 0; i < globalData.levels.Count; i++)
            {
                if (globalData.levels[i].levelStatus.Equals(LevelStatus.Passive) && !priority && PasedCheck(i))
                {
                    globalData.levels[i].levelStatus = LevelStatus.Active;
                    priority = true;
                }

                tabs[i].SetLevelTab(i, globalData.levels[i], globalData.stageIsLocked);
            }
        }
        private void Purchase()
        {
            if (CloudSaveManager.Instance.CanSpendGem(localData.GetPayType(),localData.GetStagePrice()))
            {
                CloudSaveManager.Instance.SpendGem(localData.GetPayType(),localData.GetStagePrice());

                lockPanel.SetActive(false);
                globalData.stageIsLocked = false;

                CloudSaveManager.Instance.updateStage();
                UpdateLevelTab();
            }

            Debug.Log("Yetersiz Bakiye");
        }


        public override void OnSelect(int index)
        {
           // base.OnSelect(index);
           // LevelTab previous = GetTab(PreviousIndex);

            LevelTab next = GetTab(NextIndex);

            CloudSaveManager.Instance.LevelIndex = index;

            next.OpenPage();

        }
        private bool PasedCheck(int index)
        {
            if (index == 0)
                return false;

           return globalData.levels[index - 1].levelStatus.Equals(LevelStatus.Passed);
        }

    }
}

