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

        public void SetProduct(ProductStruct ps)
        {
            string _productName = string.Empty;

            if (ps.typeOfProduct.Equals(TypeofProduct.Gold) || ps.typeOfProduct.Equals(TypeofProduct.Diamond))
            {
                _productName = $"{CloudSaveManager.Instance.StringFormat(ps.amount)} {LanguageManager.GetText(ps.typeOfProduct.ToString())}";
            }
            else
                _productName = LanguageManager.GetText(ps.productName);

            productTitle.text = productName.text = _productName;
            productFrame.sprite = CloudSaveManager.Instance.GetGridProductSprite(ps.payType);


            for (int i = 0; i < ps.productIcon.Count; i++)
                productIcon[i].sprite = ps.productIcon[i];

            if (ps.payType.Equals(PayType.Free))
            {
                productPrice.text = LanguageManager.GetText(ps.payType.ToString());
            }
            else
            {
                string id = string.IsNullOrEmpty(ps.productId) ? "com.rhombeusgaming.premium" : ps.productId;

                UnityEngine.Purchasing.Product product = IAPManager.Instance.GetProduct(id);
                purchaseButton.OnClick(product,IAPManager.Instance.OnPurchase);

                productPrice.text = product.metadata.localizedPriceString;

            }
        }
    }

}
