using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DarkJimmy;

namespace DarkJimmy.UI
{
    public class Language : Menu
    {
        public RectTransform container;
        public LanguageButton languageButton;

        public override void Start()
        {
            base.Start();

            GenerateTabButton();
        }

        private void GenerateTabButton()
        {
            int count = Enum.GetNames(typeof(Languages)).Length;

            for (int i = 0; i < count; i++)
            {
                LanguageButton tabButton = Instantiate(languageButton,container);
                tabButton.OnClick(i,Selected);
                tabButton.SetTabButtonName(LanguageManager.GetLanguageName(((Languages)i).ToString()));
            }          
        }

        private void Selected(int index)
        {
            
        }
    }
}

