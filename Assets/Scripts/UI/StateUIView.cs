using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace DarkJimmy
{
    public class StateUIView : MonoBehaviour
    {
        [SerializeField]
        private ViewType type;
        [SerializeField]
        private Stats stats;
        [SerializeField]
        private TMP_Text amount;
        [SerializeField]
        private Slider statsSlider;
        [SerializeField]
        private float duration=0.5f;
        [SerializeField]
        private TMP_Text statsName;
        [SerializeField]
        private Image fill;       
        public Color color;

        CloudSaveManager csm;
        private void Start()
        {
            csm = CloudSaveManager.Instance;
            if (type.Equals(ViewType.UI))
            {
                SetStatsValue();
                UIManager.Instance.updateState += UpdateState;
            }
        }

        public void UpdateState(Stats state,float amount)
        {
            if (this.stats != state)
                return;

            if (this.amount != null)
                this.amount.text = csm.StringFormat((int)amount);

            if (statsSlider != null)
                statsSlider.value = amount;

        }
        void SetStatsValue()
        {         
            if (Enum.TryParse(stats.ToString(), out GemType gemType))
                amount.text = csm.StringFormat(csm.GetGemCount(gemType));
            else
            {
                if (stats.Equals(Stats.Energy))
                    SetSlider(csm.GetCurrentCharacterData().Energy);
                else if (stats.Equals(Stats.Mana))
                    SetSlider(csm.GetCurrentCharacterData().Mana);
                else if (stats.Equals(Stats.Timer))
                    amount.text = "00:59";
            }
        }
        private void SetSlider(float value)
        {
            statsSlider.maxValue = value;
            statsSlider.value = value;
        }
        public void SetInfoSlider(float value, float maxValue )
        {
            StartCoroutine(SetSliderValue(value,maxValue));
        }
        public void SetColor(Color color, string key)
        {
            statsName.text = LanguageManager.GetText(key);
            statsName.color = color;
            fill.color = color;
            amount.color = color;
        }
        IEnumerator SetSliderValue(float value, float maxValue)
        {
            statsSlider.maxValue = maxValue;
            float time = 0;

            while (time <= 1)
            {
                time += Time.deltaTime / duration;
                float percent = Mathf.Lerp(0, value, time);
                statsSlider.value = percent;
                amount.text = $"{(int)percent}%";
                yield return null;
            }
        }
    }

    public enum Stats
    {
        Gold,
        Diamond,
        Key,
        Energy,
        Mana,
        Timer
    }
    public enum ViewType
    {
        UI,
        Info
    }

}

