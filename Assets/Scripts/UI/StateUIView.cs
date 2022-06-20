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

        public float defaultValue;
 
        CloudSaveManager csm;
        SystemManager system;
        private void Start()
        {
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;

            if (type.Equals(ViewType.UI))
            {
                SetStatsValue();
                system.updateStats += UpdateState;
                system.setStats += SetStatsPowerUp;
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
        public void SetStatsPowerUp(Stats stats, float value)
        {
            if (this.stats != stats)
                return;

            if (statsSlider !=null && amount !=null)
                if (stats.Equals(Stats.Mana) || stats.Equals(Stats.Energy))
                    SetSliderValues(value, value);
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
                amount.text = $"{(int)percent}/{maxValue}";
                yield return null;
            }
        }
        public void SetStatName(string name)
        {
            statsName.text = LanguageManager.GetText(name);
        }
        public Stats GetStatsType()
        {
            return stats;
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

