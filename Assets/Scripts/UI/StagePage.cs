using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class StagePage: SlidePage<StageTab,LevelData>
    {
        [Header("Stage Property")]
        [SerializeField]
        private LevelData levelData;
        [SerializeField]
        private RectTransform stageContent;
        [SerializeField]
        private LevelPage pagePrefab;
        //[SerializeField]
        //private BaseButton next;
        //[SerializeField]
        //private BaseButton previous;

       // public List<float> GetPosition = new List<float>();


        public override void Start()
        {
            data = levelData;
            Count = levelData.stages.Count;
            base.Start();

            //next.OnClick(true, 1, Move);
            //previous.OnClick(true, -1, Move);
            SetMoveButton();
            Generate();
            OnSelect(Index);
        }
        public override void Generate()
        {
            float _posX = 0;

            for (int i = 0; i < levelData.stages.Count; i++)
            {
                StageTab stageTab = Instantiate(prefab, container);
                stageTab.SetStage(data.stages[i]);
                stageTab.OnClick(i, OnSelect);
                tabs.Add(stageTab);

                LevelPage page = Instantiate(pagePrefab, stageContent);
                page.data = data.stages[i];

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
                Index = Mathf.Clamp(Index, 0, data.stages.Count - 1);

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
        //private void SetMoveButton()
        //{
        //    previous.gameObject.SetActive(Index != 0);
        //    next.gameObject.SetActive(Index != data.stages.Count - 1);
        //}

    }

}
