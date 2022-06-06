using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class LevelPopup : Menu
    {
        public override void Start()
        {
            string text = CloudSaveManager.Instance.GetLevel().levelName;
            pageName.text = LanguageManager.GetText(text);
        }
    }
}

