using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class StageLockOrientationPopup : Popup
    {

        private StagePage stagePage;

        public override void Start()
        {
            base.Start();

            if (popupButton != null)
                popupButton.SetName(LanguageManager.GetText("Go"));

            stagePage = FindObjectOfType<StagePage>();

            popupButton.OnClick(CloudSaveManager.Instance.GetLockedFirstStage()+1, Go);
        }

        private void Go(int index)
        {
            stagePage.OnSelect(index);

            GoBack();
        }
    }
}

