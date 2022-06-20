using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace DarkJimmy.UI
{
    public class PowerUpButton : TabButton
    {
        [Header("Power Up Button")]
        public PowerUp powerUpType;    
        [SerializeField]
        private Image priceIcon;
        [SerializeField]
        private Image background;

        private string description;
        private string powerUpName;
        private float multiple;
        private bool isSelect;
        private GemType payType;
        private int price;

        GameSaveManager gsm;
        int k = 0;
        private void Start()
        {
            gsm = GameSaveManager.Instance;
        }
        public override  void SetTabButton(bool isOn)
        {
            isSelect = isOn ? !isSelect : isOn;
 
            focus.gameObject.SetActive(isSelect);


            if (!isOn && k < 0)
                return;
           
            k = isSelect ? 1 : -1;

            if (Enum.TryParse(powerUpType.ToString(), out Stats stats))
                gsm.UpdateCapacity(stats, (int)(k * multiple));

        }

        public void SetPowerUpButton(PowerUpStruct ps)
        {
            powerUpType = ps.powerUpType;
            SetTabIcon(ps.powerUpIcon);
            buttonName.text = SystemManager.Instance.StringFormat(price = ps.powerUpPrice);
            priceIcon.sprite = SystemManager.Instance.GetPaySprite(payType = ps.powerUpPayType);
            description = ps.powerUpDescription;
            powerUpName = ps.powerUpName;
            multiple = ps.multiple;
            
        } 
        public string GetDescription(out string header, out bool isOn, out GemType payType, out int price)
        {
            header  = powerUpName;
            isOn    =   isSelect;
            payType = this.payType;
            price   = this.price;

            return description;
        }
    }
}

