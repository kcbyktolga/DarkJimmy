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
        private Stats stats;
        [SerializeField]
        private TMP_Text amount;
        [SerializeField]
        private Slider statsSlider;

        private void Start()
        {
            SetStatsValue();
            UIManager.Instance.updateState += UpdateState;
        }

        public void UpdateState(Stats state,int amount)
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

        private void SetSlider(int value)
        {
            statsSlider.maxValue = value;
            statsSlider.value = value;
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

}

