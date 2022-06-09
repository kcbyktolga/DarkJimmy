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

        public override void SetTabButton(bool isOn)
        {
            focus.enabled = isOn;
            button.interactable = !isOn;
        }
        public void SetStage(Stage stage,Sprite sprite )
        {
            stageIcon.sprite = sprite;
            stageLocked.gameObject.SetActive(stage.stageIsLocked);
        }
        private void UpdateStageTab()
        {
            stageLocked.gameObject.SetActive(false);
        }
    }

}
