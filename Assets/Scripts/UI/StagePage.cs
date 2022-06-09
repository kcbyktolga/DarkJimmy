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
                stageTab.SetStage(globalData.Stages[i],CloudSaveManager.Instance.GetSystemData().Stages[i].GetStageIcon());
                stageTab.OnClick(i, OnSelect);
                tabs.Add(stageTab);

                LevelPage page = Instantiate(pagePrefab, stageContent);
                page.globalData = globalData.Stages[i];
                page.localData = CloudSaveManager.Instance.GetSystemData().Stages[i];
                page.PageIndex = i;
                
                GetPosition.Add(_posX);
                _posX -= UIManager.Instance.GetReferenceResolotion().x;

                page.Generate();
            }
        }
        public override void OnSelect(int index)
        {
            base.OnSelect(index);
            Index = NextIndex;
            Move(false, NextIndex);
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

            SetStageTab();
            SetMoveButton();
            StartCoroutine(Slide(stageContent,GetPosition[NextIndex]));
        }
        private void SetStageTab()
        {
            StageTab previousTab = GetTab(PreviousIndex);
            StageTab nextTab = GetTab(NextIndex);

            CloudSaveManager.Instance.WorldIndex = NextIndex;

            previousTab.SetTabButton(false);
            nextTab.SetTabButton(true);
        }
    }

}
