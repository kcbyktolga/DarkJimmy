using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class UpgradeMenu : Menu
    {

        public override void OnEnable()
        {
            base.OnEnable();
            SetVolume(false);
        }

        public override void OnDestroy()
        {
            SetVolume(true);
        }

    }
}

