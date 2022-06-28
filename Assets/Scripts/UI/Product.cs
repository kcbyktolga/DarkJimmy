using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

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
        private Image productBackground;
        [HideInInspector]
        public PayType payType; 


        private Coroutine Timer;
        private readonly Dictionary<RewardType, int> GetRewardAmount = new Dictionary<RewardType, int>();
        private RewardType rewardType;       
        public void SetProduct(ProductStruct ps)
        {
            string _productName = string.Empty;

            if (ps.typeOfProduct.Equals(TypeofProduct.Gold) || ps.typeOfProduct.Equals(TypeofProduct.Diamond))
            {
                _productName = $"{SystemManager.Instance.StringFormat(ps.amount)} {LanguageManager.GetText(ps.typeOfProduct.ToString())}";
            }
            else
                _productName = LanguageManager.GetText(ps.productName);

             productName.text = _productName;
            // productFrame.sprite = CloudSaveManager.Instance.GetGridProductSprite(ps.payType);

            if(ps.payType.Equals(PayType.Paid))
                productBackground.sprite = SystemManager.Instance.GetProductBackground(ps.typeOfProduct);


            for (int i = 0; i < ps.productIcon.Count; i++)
                productIcon[i].sprite = ps.productIcon[i];

            if (ps.payType.Equals(PayType.Free))
            {                       
                rewardType = GetRewardType(ps.typeOfProduct);

                if (IsTimeOut())
                {
                    productPrice.text = LanguageManager.GetText(ps.payType.ToString());

                    GetRewardAmount.Add(rewardType, ps.amount);
                    purchaseButton.OnClick(rewardType, ShowRewardedAd);
                }
                else
                {
                    StartTimer();
                    purchaseButton.button.interactable = false;
                }

            }
            else
            {
                string id = string.IsNullOrEmpty(ps.productId) ? "com.rhombeusgaming.premium" : ps.productId;

                UnityEngine.Purchasing.Product product = IAPManager.Instance.GetProduct(id);
                purchaseButton.OnClick(product,IAPManager.Instance.OnPurchase);

                productPrice.text = product.metadata.localizedPriceString;
            }
        }
        RewardType GetRewardType(TypeofProduct type)
        {
            if (Enum.TryParse(type.ToString(), out RewardType rewardType))
                return rewardType;
            else
                return 0;
        }
        private void GetReward(RewardType rewardType)
        {
            int amount = GetRewardAmount[rewardType];

            if (Enum.TryParse(rewardType.ToString(), out GemType gemType))
            {
                CloudSaveManager.Instance.AddGem(gemType, amount);
                SetResetTime();
            }
        }
        private void ShowRewardedAd(RewardType rewardType)
        {
            AdManager.Instance.ShowRewardedAd(rewardType,GetReward);          
        }
        private void SetResetTime()
        {
            DateTime resetTime = DateTime.Now.AddMinutes(1);
            CloudSaveManager.Instance.SetResetTime(resetTime,rewardType);
            
            StartTimer();
        }
        private void StartTimer()
        {
            if (Timer == null)
                Timer = StartCoroutine(nameof(TimeUpdate));
        }
        private void StopTimer()
        {
            StopCoroutine(nameof(TimeUpdate));
            Timer = null;
        }
        private bool IsTimeOut()
        {
            return DateTime.Now >= CloudSaveManager.Instance.GetResetTime(rewardType);
        }
        private IEnumerator TimeUpdate()
        {
            while (!IsTimeOut())
            {
                TimeSpan diffTime = CloudSaveManager.Instance.GetResetTime(rewardType).Subtract(DateTime.Now);

                string timer = diffTime.Hours > 0 ? $"{diffTime.Hours}{LanguageManager.GetText("sa")} : {diffTime.Minutes}{LanguageManager.GetText("d")}" : $"{diffTime.Minutes}{LanguageManager.GetText("dk")} : {diffTime.Seconds}{LanguageManager.GetText("sn")}";

                productPrice.text = $"{timer}"; //{LanguageManager.GetText("Remaining")}:

                Debug.Log($"{rewardType}: {timer} ");

                yield return new WaitForSeconds(1);
            }
      
            productPrice.text = LanguageManager.GetText(PayType.Free.ToString());
            purchaseButton.button.interactable = true;
            purchaseButton.OnClick(rewardType, ShowRewardedAd);
        }
        private void OnDisable()
        {
            if (payType != PayType.Free)
                return;

            StopTimer();
        }
        private void OnEnable()
        {
            if (payType != PayType.Free)
                return;

            StartTimer();
        }
    }

}
