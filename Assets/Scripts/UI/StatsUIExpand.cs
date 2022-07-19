using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DarkJimmy.UI
{
    public class StatsUIExpand : ExpandPanel<StatsUI>
    {
        [SerializeField]
        private int pageIndex = 2;
        public override void Awake()
        {
            base.Awake();
            system.onChangedPage += OpenPanel;
        }

        public override void ColorChange(StatsUI element, Color endColor, float duration)
        {           
            element.StatsColor(endColor,duration);
        }

        private void OpenPanel(int index)
        {
            Expand(index == pageIndex);
        }

        private void OnDestroy()
        {
            system.onChangedPage -= OpenPanel;
        }

    }
}

