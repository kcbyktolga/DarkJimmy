using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class Stages : TabGenerator<StageTab,LevelData>
    {
        [Header("Stage Property")]
        [SerializeField]
        private LevelData levelData;
        [SerializeField]
        private RectTransform stageContent;
        [SerializeField]
        private LevelPage pagePrefab;
        [SerializeField]
        private BaseButton next;
        [SerializeField]
        private BaseButton previous;
        [SerializeField]
        private float duration = 0.25f;

        private List<float> GetPosition = new List<float>();

        private void Start()
        {
            data = levelData;

            next.OnClick(true,1, Move);
            previous.OnClick(true,-1, Move);
            SetMoveButton();
            Generate();
            OnSelect(Index);
        }
        public override void Generate()
        {
            float _posX = 0;
            for (int i = 0; i < data.stages.Count; i++)
            {

                StageTab stageTab = Instantiate(prefab, container);
                stageTab.SetStage(data.stages[i]);
                stageTab.OnClick(i,OnSelect);
                tabs.Add(stageTab);

                LevelPage page = Instantiate(pagePrefab,stageContent);
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
        private void Move(bool onClick,int amount)
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

            StartCoroutine(nameof(Slide));
        }
        private void SetStageTab()
        {
      
            StageTab previousTab = GetTab(PreviousIndex);
            StageTab nextTab = GetTab(NextIndex);

            previousTab.SetTabButton(false);
            nextTab.SetTabButton(true);
        }
        private void SetMoveButton()
        {
            previous.gameObject.SetActive(Index != 0);
            next.gameObject.SetActive(Index != data.stages.Count - 1);
        }
        private IEnumerator Slide()
        {
            float time = 0;
            //tabTime = Time.time + duration;
       
            float currentPos = stageContent.anchoredPosition.x;
            float endPos = GetPosition[Index];

            while (time <= 1)
            {
                time += Time.deltaTime / duration;
                float posX = Mathf.Lerp(currentPos, endPos, time);
                stageContent.anchoredPosition = new Vector2(posX, stageContent.anchoredPosition.y);

                yield return null;
            }
        }
    }

}
