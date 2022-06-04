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

        [SerializeField]
        private Color color;
        private void Start()
        {
            if (statsName != null)
            {
                statsName.text = LanguageManager.GetText(stats.ToString());
                //statsName.color = color;
                //fill.color = color;
            }
                

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

            if(this.amount !=null)
                this.amount.text = amount.ToString();

            if (statsSlider != null)
                statsSlider.value = amount;

        }
        void SetStatsValue()
        {         
            if (Enum.TryParse(stats.ToString(), out GemType gemType))
                amount.text = CloudSaveManager.Instance.GetGemCount(gemType).ToString();
            else
            {
                if (stats.Equals(Stats.Energy))
                    SetSlider(CloudSaveManager.Instance.GetCurrentCharacterData().Energy);
                else if (stats.Equals(Stats.Mana))
                    SetSlider(CloudSaveManager.Instance.GetCurrentCharacterData().Mana);
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
        IEnumerator SetSliderValue(float value, float maxValue)
        {
            statsSlider.maxValue = maxValue;
            float time = 0;

            while (time <= 1)
            {
                time += Time.deltaTime / duration;
                float percent = Mathf.Lerp(0, value, time);
                statsSlider.value = percent;
                yield return null;
            }
        }
    }

    public enum Stats
    {
        Gold,
        Token,
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

