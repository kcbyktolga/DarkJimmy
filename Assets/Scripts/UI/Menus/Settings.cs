using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class Settings : Menu
    {
        private void Start()
        {
            pageName.text = LanguageManager.GetText(menuType.ToString());
        }

    }

}

