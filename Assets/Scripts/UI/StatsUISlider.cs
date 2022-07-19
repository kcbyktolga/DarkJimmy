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

        private string _statName;

        private readonly float duration = 0.5f;

        public override void Awake()
        {
            base.Awake();
            system.updateStatsMax += SetSliderValueAndMax;
        }
        public override void Initialize()
        {
            LanguageManager.onChangedLanguage += ChangeName;

            if (System.Enum.TryParse(Stats.ToString(), out CharacterProperty property))
                SyncStateValue(Stats, csm.GetCurrentCharacterData().GetCurrentCharacterProperty(property));


            if (property.Equals(CharacterProperty.HP))
            {
                Debug.Log(csm.GetCurrentCharacterData().GetCurrentCharacterProperty(property));
            }
           
        }
        private void SetSliderValueAndMax(Stats stats, int value, int maxValue)
        {
            if (Stats != stats)
                return;

            SetSliderValues(value,maxValue);

            SetStatName(stats.ToString());
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
            statsSlider.DOValue(value, duration).OnUpdate(SetValue);
        }
        private void SetValue()
        {
            amount.text = $"{(int)statsSlider.value}/{(int)statsSlider.maxValue}";
        }   
        public void SetStatName(string name)
        {
            _statName = name;
            statsName.text = LanguageManager.GetText(name);
        }

        private void ChangeName()
        {
            statsName.text = LanguageManager.GetText(_statName);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            system.updateStatsMax -= SetSliderValueAndMax;
            LanguageManager.onChangedLanguage -= ChangeName;

            statsSlider.DOKill();
        }
    }


}
