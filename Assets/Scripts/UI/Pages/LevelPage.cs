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
      
        public int PageIndex { get; set; }

        public override void Generate()
        {
            pageName.text = LanguageManager.GetText(localData.stageName);

            lockPanel.SetActive(globalData.stageIsLocked);

            if (globalData.stageIsLocked)
            {
                purchaseButton.buttonName.text = $"{localData.GetStagePrice()}";
                purchaseButton.priceIcon.sprite = SystemManager.Instance.GetPaySprite(localData.GetPayType());
                purchaseButton.OnClick(Purchase);
            }


            for (int i = 0; i < globalData.levels.Count; i++)
            {
                LevelTab levelTab = Instantiate(prefab, container);

                if (globalData.levels[i].levelStatus.Equals(LevelStatus.Passive)&& PasedCheck(i) && !HasActiveLevel())
                    globalData.levels[i].levelStatus = LevelStatus.Active;

                levelTab.SetLevelTab(i, globalData.levels[i], globalData.stageIsLocked);
                levelTab.SetLevelName(globalData.stageIndex,i);
                levelTab.SetLevelImage(localData.levels[i]);
                tabs.Add(levelTab);
                levelTab.OnClick(i, OnSelect);
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
        public void UpdateLevelTab()
        {
            lockPanel.SetActive(false);
            CloudSaveManager.Instance.updateStage(globalData);

            for (int i = 0; i < globalData.levels.Count; i++)
            {
                if (globalData.levels[i].levelStatus.Equals(LevelStatus.Passive) && PasedCheck(i))
                    globalData.levels[i].levelStatus = LevelStatus.Active;

                tabs[i].SetLevelTab(i, globalData.levels[i], globalData.stageIsLocked);
            }
        }
        private void Purchase()
        {
            if (CloudSaveManager.Instance.CanUnlockStage(PageIndex))            
            {
                if (CloudSaveManager.Instance.CanSpendGem(localData.GetPayType(), localData.GetStagePrice()))
                {
                    globalData.stageIsLocked = false;
                    CloudSaveManager.Instance.SpendGem(localData.GetPayType(), localData.GetStagePrice());
                    
                    UpdateLevelTab();
                }
                else
                {
                    UIManager.Instance.PageIndex = (int)localData.GetPayType();
                    UIManager.Instance.OpenMenu(Menu.Menus.ShopOrientation);
                }
            }
            else
                UIManager.Instance.OpenMenu(Menu.Menus.StageLockOrientation);

        }
        private bool PasedCheck(int index)
        {
            if (index == 0)
                return false;

           return globalData.levels[index - 1].levelStatus.Equals(LevelStatus.Passed);
        }
        private bool HasActiveLevel()
        {
            foreach (var item in globalData.levels)
                if (item.levelStatus.Equals(LevelStatus.Active))
                    return true;
            return false;
        }
    }
}

