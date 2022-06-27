using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DarkJimmy
{
    public class StatsUISlider : StatsUI
    {
        [Header("Stats Slider")]
        [SerializeField]
        private Slider statsSlider;
        [SerializeField]
        private TMP_Text statsName;

        private readonly float duration = 0.5f;
        public override void UpdateStats(Stats stats, int amount)
        {
            if (Stats != stats)
                return;

            SetSliderValues(amount, statsSlider.maxValue);
        }
        public override void SetStatsPowerUp(Stats stats, int value)
        {
            if(Stats.Equals(stats))
                SetSliderValues(value, value);
        }
        public override void SetStatsValue()
        {
            if (Stats.Equals(Stats.Energy))
                SetSliderMaxValue(csm.GetCurrentCharacterData().Energy);
            else if (Stats.Equals(Stats.Mana))
                SetSliderMaxValue(csm.GetCurrentCharacterData().Mana);
        }
        private void SetSliderMaxValue(float value)
        {
            statsSlider.maxValue = value;
            statsSlider.value = value;

            if (amount != null)
                SetValue();
        }
        public void SetSliderValues(float value, float maxValue)
        {
           // StartCoroutine(SetSliderValue(value, maxValue));

            statsSlider.maxValue = maxValue;
            float amount = statsSlider.value;
            statsSlider.DOValue(value, duration).onUpdate += SetValue;
         
        }
        private void SetValue()
        {
            amount.text = $"{(int)statsSlider.value}/{(int)statsSlider.maxValue}";
        }
        //private IEnumerator SetSliderValue(float value, float maxValue)
        //{
        //    statsSlider.maxValue = maxValue;
        //    float time = 0;

        //    while (time <= 1)
        //    {
        //        time += Time.deltaTime / duration;
        //        float percent = Mathf.Lerp(statsSlider.value, value, time);
        //        statsSlider.value = percent;
        //        SetAmount((int)percent);
        //        yield return null;
        //    }
        //}
        public void SetStatName(string name)
        {
            statsName.text = LanguageManager.GetText(name);
        }
    }


}
