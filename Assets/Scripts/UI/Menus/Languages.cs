using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DarkJimmy;

namespace DarkJimmy.UI
{
    public class Languages : Menu
    {
        public RectTransform container;
        public LanguageButton languageButton;

        private int nextIndex = 0;
        private int previousIndex = 0;

        List<LanguageButton> buttons = new List<LanguageButton>();

        public override void Start()
        {
            base.Start();
            GenerateTabButton();
            ActivateBase();
        }

        private void GenerateTabButton()
        {
            int count = Enum.GetNames(typeof(Language)).Length;

            for (int i = 0; i < count; i++)
            {
                LanguageButton tabButton = Instantiate(languageButton,container);
                buttons.Add(tabButton);
                tabButton.OnClick(i,Selected);
                tabButton.SetTabButtonName(LanguageManager.GetLanguageName(((Language)i).ToString()));
            }

            Selected((int)LanguageManager.GetLanguage());
        }

        private void Selected(int index)
        {
            previousIndex = nextIndex;
            nextIndex = index;

            LanguageButton previous = GetLanguageButton(previousIndex);
            LanguageButton next = GetLanguageButton(nextIndex);

            previous.SetLanguageButton(false);
            next.SetLanguageButton(true);

            LanguageManager.SetLanguage((Language)nextIndex);
            LanguageManager.onChangedLanguage();
        }

        private LanguageButton GetLanguageButton(int index)
        {
            return buttons[index];
        }
    }
}

