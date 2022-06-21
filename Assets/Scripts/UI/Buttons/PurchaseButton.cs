using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using System;

namespace DarkJimmy.UI
{
    public class PurchaseButton : BaseButton
    {      
        public Image priceIcon;
        public string productId;

        public override void ClickDownSound()
        {
            //base.ClickDownSound();
            AudioManager.Instance.PlaySound("Card Flip");
        }
    }

}
