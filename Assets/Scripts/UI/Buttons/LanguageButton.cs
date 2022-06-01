using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class LanguageButton : BaseButton
    {
        [SerializeField]
        private Image focus;
        [SerializeField]
        private Sprite offFocus;
        [SerializeField]
        private Sprite onFocus;

        public void SetLanguageButton(bool isOn)
        {
            focus.sprite = isOn ? onFocus : offFocus;
            button.interactable = !isOn;
        }

    }

}
