using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class Product : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text productName;
        [SerializeField]
        private TMP_Text productPrice;
        [SerializeField]
        private TMP_Text productTitle;
        [SerializeField]
        private Image productIcon;

        public void SetProduct(ProductStruct productStruct)
        {
            productName.text = productStruct.productName;
            productPrice.text = productStruct.productPrice;
            productTitle.text = productStruct.productTitle;
            productIcon.sprite = productStruct.productIcon;
        }
    }

}
