using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkJimmy.Manager;

namespace DarkJimmy.UI
{
    public class TabButton : BaseButton
    {
        [Header("Tab Button Property")]
        public Image focus;
        [SerializeField]
        private Color idleColor;
        [SerializeField]
        private Color focusColor;

        public virtual void SetTabButton(bool isOn)
        {
            buttonName.color = isOn ? focusColor : idleColor;
            focus.enabled = isOn;
            button.interactable = !isOn;
        }
    }

}
