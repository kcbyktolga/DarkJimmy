using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class Stages : Menu
    {
        public override void Start()
        {
            base.Start();

            AudioManager.Instance.PlayMusic("Stage Theme");
        }
    }
}
