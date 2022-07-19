using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class StagePage: SlidePage<StageTab,PlayerData>
    {
        [Header("Stage Property")]
        [SerializeField]
        private RectTransform stageContent;
        [SerializeField]
        private LevelPage pagePrefab;

        private bool IsOpenStage { get; set; } = false;
        private int _index;
        private readonly List<LevelPage> pages = new List<LevelPage>();
        
    
        public override void Start()
        {
            globalData = CloudSaveManager.Instance.PlayerDatas;         
            Count = globalData.Stages.Count;
            base.Start();

            Generate();
            OnSelect(Index);
        }
        public override void Generate()
        {
            float _posX = 0;

            for (int i = 0; i < globalData.Stages.Count; i++)
            {
                StageTab stageTab = Instantiate(prefab, container);
                stageTab.SetStage(globalData.Stages[i],CloudSaveManager.Instance.GetDefaultData().Stages[i].GetStageIcon());
                stageTab.OnClick(i, OnSelect);
                tabs.Add(stageTab);

                LevelPage page = Instantiate(pagePrefab, stageContent);
                page.globalData = globalData.Stages[i];
                page.localData = CloudSaveManager.Instance.GetDefaultData().Stages[i];
                page.PageIndex = i;
                pages.Add(page);
                
                GetPosition.Add(_posX);
                _posX -= UIManager.Instance.GetReferenceResolotion().x;

                page.Generate();

                if (globalData.Stages[i].stageIsLocked)
                {
                    if (CloudSaveManager.Instance.PlayerDatas.GetAllKeyCount() >= page.localData.KeyCount)
                    {
                        CloudSaveManager.Instance.UnlockStage(i, false);
                        IsOpenStage = true;
                        _index = i;

                        Invoke(nameof(StageMove),1);
                    }
                }
            }
        }

        private void StageMove()
        {
            OnSelect(_index);
        }
        private void OpenStage()
        {
            pages[NextIndex].UpdateLevelTab();
            IsOpenStage = false;
        }
        
        public override void OnSelect(int index)
        {
            base.OnSelect(index);
            Index = NextIndex;
            Move(false, NextIndex);

            if (IsOpenStage)
                OpenStage();

        }
        public override void Move(bool onClick, int amount)
        {
            if (onClick)
            {
                Index += amount;
                Index = Mathf.Clamp(Index, 0, globalData.Stages.Count - 1);

                PreviousIndex = NextIndex;
                NextIndex = Index;
            }

            SystemManager.Instance.onChangedBackground(CloudSaveManager.Instance.GetDefaultData().Stages[NextIndex].GetBackgroundType());
            SetStageTab();
            SetMoveButton();
            //StartCoroutine(Slide(stageContent,GetPosition[NextIndex]));
            Sliding(stageContent,GetPosition[NextIndex]);
            AudioManager.Instance.PlaySound("Turn Page");
        }
        private void SetStageTab()
        {
            StageTab previousTab = GetTab(PreviousIndex);
            StageTab nextTab = GetTab(NextIndex);

            CloudSaveManager.Instance.StageIndex = NextIndex;

            previousTab.SetTabButton(false);
            nextTab.SetTabButton(true);
        }
    }

}
