using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class StageTab : TabButton
    {
        [Header("Stage Tab Property")]
        [SerializeField]
        private Image stageLocked;
        [SerializeField]
        private Image stageIcon;

        private Stage dependStage;

        private void Start()
        {
            CloudSaveManager.Instance.updateStage += UpdateStageTab;
        }
        public override void SetTabButton(bool isOn)
        {
            focus.enabled = isOn;
            button.interactable = !isOn;
        }
        public void SetStage(Stage stage,Sprite sprite )
        {
            dependStage = stage;
            stageIcon.sprite = sprite;
            stageLocked.gameObject.SetActive(stage.stageIsLocked);
        }
        private void UpdateStageTab(Stage stage)
        {
            if (!dependStage.Equals(stage))
                return;

            stageLocked.gameObject.SetActive(stage.stageIsLocked);
        }
    }

}
