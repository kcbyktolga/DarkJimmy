using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class Product : MonoBehaviour
    {
        [Header("Product Property")]
        [SerializeField]
        private TMP_Text productName;
        [SerializeField]
        private TMP_Text productPrice;
        [SerializeField]
        private TMP_Text productTitle;
        [SerializeField]
        private Image productIcon;
        [Header("Product Customize")]
        [SerializeField]
        private Image productFrame;
        [SerializeField]
        private Color paidColor;
        [SerializeField]
        private Color freeColor;

        public void SetProduct(ProductStruct productStruct)
        {
            productName.text = productStruct.productName;
            productPrice.text = productStruct.productPrice;
            productTitle.text = productStruct.productTitle;
            productIcon.sprite = productStruct.productIcon;
            productFrame.color = productStruct.payType.Equals(PayType.Free) ? freeColor : paidColor;
        }
    }

}
