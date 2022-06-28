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
        private float powerValue;
        private bool isSelect;
        private GemType payType;
        private int price;

        SystemManager system;
        int k = 0;

        private void Awake()
        {
            system = SystemManager.Instance;
        }
     
        public override  void SetTabButton(bool isOn)
        {
            isSelect = isOn ? !isSelect : isOn;
 
            focus.gameObject.SetActive(isSelect);

            if (!isOn && k < 0)
                return;
           
            k = isSelect ? 1 : -1;

            if (Enum.TryParse(powerUpType.ToString(), out Stats stats))
                system.updatePowerUp(stats, (int)(k * powerValue));
        }

        public void SetPowerUpButton(PowerUpStruct ps)
        {
            powerUpType = ps.powerUpType;
            SetTabIcon(ps.powerUpIcon);
            buttonName.text = system.StringFormat(price = ps.powerUpPrice);
            priceIcon.sprite =system.GetPaySprite(payType = ps.powerUpPayType);
            description = ps.powerUpDescription;
            powerUpName = ps.powerUpName;
            powerValue = ps.multiple;
            
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

