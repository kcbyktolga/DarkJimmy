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
 
        CloudSaveManager csm;
        SystemManager system;
        private void Start()
        {
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;

            if (type.Equals(ViewType.UI))
            {
                SetStatsValue();
                UIManager.Instance.updateState += UpdateState;
            }
        }
        public void UpdateState(Stats stats,float amount)
        {
            if (this.stats != stats)
                return;

            if (statsSlider != null)
                statsSlider.value = amount;

            if (this.amount != null)
            {
                if (stats.Equals(Stats.Gold) || stats.Equals(Stats.Key))
                    this.amount.text = system.StringFormat((int)amount);
                else if (stats.Equals(Stats.Mana) || stats.Equals(Stats.Energy))
                    this.amount.text = $"{statsSlider.value}/{statsSlider.maxValue}";
                //else if(stats.Equals(Stats.Timer))

            }          
        }
        void SetStatsValue()
        {         
            if (Enum.TryParse(stats.ToString(), out GemType gemType))
                amount.text = system.StringFormat(csm.GetGemCount(gemType));
            else
            {
                if (stats.Equals(Stats.Energy))
                    SetSliderMaxValue(csm.GetCurrentCharacterData().Energy);
                else if (stats.Equals(Stats.Mana))
                    SetSliderMaxValue(csm.GetCurrentCharacterData().Mana);
               // else if (stats.Equals(Stats.Timer))
                   // amount.text = 
            }
        }
        private void SetSliderMaxValue(float value)
        {
            statsSlider.maxValue = value;
            statsSlider.value = value;

            if (amount != null)
                amount.text = $"{statsSlider.value}/{statsSlider.maxValue}";
        }
        public void SetSliderValues(float value, float maxValue )
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
                amount.text = $"{(int)percent}%";
                yield return null;
            }
        }
        public void SetStatName(string name)
        {
            statsName.text = LanguageManager.GetText(name);
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

