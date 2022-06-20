using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DarkJimmy.Manager;

namespace DarkJimmy.UI
{
    public class TabButton : BaseButton
    {
        [Header("Tab Button Property")]
        public Image focus;
        [SerializeField]
        private GameObject on;
        [SerializeField]
        private GameObject off;

        [SerializeField]
        private List<Image> tabIcons;

        private HorizontalLayoutGroup layout;
        private void Awake()
        {
            layout = GetComponent<HorizontalLayoutGroup>();
        }

        public virtual void SetTabButton(bool isOn)
        {
            off.SetActive(!isOn);
            on.SetActive(isOn);
            button.interactable = !isOn;

            if (layout != null)
            {
                layout.enabled = false;
                layout.enabled = true;
                Canvas.ForceUpdateCanvases();
            }
        }

        public virtual void SetTabIcon(Sprite sprite)
        {
            for (int i = 0; i < tabIcons.Count; i++)
                tabIcons[i].sprite = sprite;
        }
     
    }

}
