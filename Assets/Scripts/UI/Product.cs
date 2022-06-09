using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Purchasing;

namespace DarkJimmy.UI
{
    public class Product : MonoBehaviour
    {
        [Header("Product Property")]
        public PurchaseButton purchaseButton;
        [SerializeField]
        private TMP_Text productName;
        [SerializeField]
        private TMP_Text productPrice;
        [SerializeField]
        private TMP_Text productTitle;
        [SerializeField]
        private List<Image> productIcon;
        [Header("Product Customize")]
        [SerializeField]
        private Image productFrame;

        public void SetProduct(ProductStruct productStruct)
        {
            productName.text = productStruct.productName;
            productTitle.text = productStruct.productTitle;
            productFrame.sprite = CloudSaveManager.Instance.GetGridProductSprite(productStruct.payType);


            for (int i = 0; i < productStruct.productIcon.Count; i++)
                productIcon[i].sprite = productStruct.productIcon[i];

            if (productStruct.payType.Equals(PayType.Free))
            {
                productPrice.text = LanguageManager.GetText(productStruct.payType.ToString());
            }
            else
            {
                string id = string.IsNullOrEmpty(productStruct.productId) ? "com.rhombeusgaming.premium" : productStruct.productId;

                UnityEngine.Purchasing.Product product = IAPManager.Instance.GetProduct(id);
                purchaseButton.OnClick(product,IAPManager.Instance.OnPurchase);

                productPrice.text = product.metadata.localizedPriceString;

            }
        }
    }

}
