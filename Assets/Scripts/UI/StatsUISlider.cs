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
        public override void Initialize()
        {
            if (System.Enum.TryParse(Stats.ToString(), out CharacterProperty property))
                SyncStateValue(Stats, csm.GetCurrentCharacterData().GetCharacterProperty(property));
        }
        public override void SyncStateValue(Stats stats, int amount)
        {
            if (Stats != stats)
                return;

            minValue = defaultValue = amount;

            if (Type.Equals(StatsType.Unuseable))
                SetSliderValues(amount, statsSlider.maxValue);
            else
                SetSliderValues(amount, Value);

        }
        public override void AddPowerUp(Stats stats, int value)
        {
            if (Stats != stats)
                return;

            Value = value;

            SetSliderValues(Value, Value);
        }

        public override void UpdateGameDisplay(Stats stats, int value)
        {
            if (Stats != stats)
                return;

            SetSliderValues(value,Value);
        }
        public void SetSliderValues(float value, float maxValue)
        {
            statsSlider.maxValue = maxValue;
            statsSlider.DOValue(value, duration).onUpdate += SetValue;
           
        }
        private void SetValue()
        {
            amount.text = $"{(int)statsSlider.value}/{(int)statsSlider.maxValue}";
        }   
        public void SetStatName(string name)
        {
            statsName.text = LanguageManager.GetText(name);
        }
    }


}
