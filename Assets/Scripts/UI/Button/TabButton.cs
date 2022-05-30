using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkJimmy.UI
{
    public class TabButton : BaseButton
    {
        [Header("Tab Button Property")]
        [SerializeField]
        private Image focus;
        [SerializeField]
        private Color idleColor;
        [SerializeField]
        private Color focusColor;
        [SerializeField]
        private TMP_Text tabName;

        public void SetTabButton(bool isOn)
        {
            tabName.color = isOn ? focusColor : idleColor;
            focus.enabled = isOn;
        }

        public void SetTabButtonName(string name)
        {
            tabName.text = name;
        }
    }

}