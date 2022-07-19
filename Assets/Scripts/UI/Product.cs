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
        private GameObject dependsProductPanel;
        [SerializeField]
        private List<Image> productIcon;
        [Header("Product Customize")]
        [SerializeField]
        private Image productBackground;
        [HideInInspector]
        public ProductPayType payType;
        [SerializeField]
        private Image priceIcon;
        [SerializeField]
        private Image dependProductIcon;
        [SerializeField]
        private TMP_Text dependProductDesc;

        CloudSaveManager csm;
        private void Awake()
        {
            csm = CloudSaveManager.Instance;
        }

        private Coroutine Timer;
        private readonly Dictionary<RewardType, int> GetRewardAmount = new Dictionary<RewardType, int>();
        private RewardType rewardType;       
        public void SetProduct(ProductBase pb)
        {
            string _productName = string.Empty;
            payType = pb.payType;

            if (pb.typeOfProduct.Equals(TypeofProduct.Gold) || pb.typeOfProduct.Equals(TypeofProduct.Diamond))
            {
                _productName = $"{SystemManager.Instance.StringFormat(pb.amount)} {LanguageManager.GetText(pb.typeOfProduct.ToString())}";
            }
            else if (pb.typeOfProduct.Equals(TypeofProduct.Stones))
            {
                _productName= $"{SystemManager.Instance.StringFormat(pb.amount)} {LanguageManager.GetText(pb.stoneType.ToString())}";
            }
            else
                _productName = LanguageManager.GetText(pb.productName);

            productName.text = _productName;
            // productFrame.sprite = CloudSaveManager.Instance.GetGridProductSprite(ps.payType);

            if(!pb.payType.Equals(ProductPayType.Free))
                productBackground.sprite = SystemManager.Instance.GetProductBackground(pb.typeOfProduct);


            for (int i = 0; i < pb.productIcon.Count; i++)
                productIcon[i].sprite = pb.productIcon[i];

            if (pb.payType.Equals(ProductPayType.Free))
            {                       
                rewardType = GetRewardType(pb.typeOfProduct);

                if (IsTimeOut())
                {
                    productPrice.text = LanguageManager.GetText(pb.payType.ToString());

                    GetRewardAmount.Add(rewardType, pb.amount);
                    purchaseButton.OnClick(rewardType, ShowRewardedAd);
                }
                else
                    StartTimer();
            }
            else if(pb.payType.Equals(ProductPayType.Paid))
            {
                string id = string.IsNullOrEmpty(pb.productId) ? "com.rhombeusgaming.premium" : pb.productId;

                UnityEngine.Purchasing.Product product = IAPManager.Instance.GetProduct(id);
                purchaseButton.OnClick(product,IAPManager.Instance.OnPurchase);

                productPrice.text = product.metadata.localizedPriceString;

            }
            else if(pb.payType.Equals(ProductPayType.Gem))
            {
                priceIcon.sprite = SystemManager.Instance.GetPaySprite(pb.gemPayType);
                priceIcon.gameObject.SetActive(true);
                productPrice.text = pb.price.ToString();

                purchaseButton.OnClick(pb,PurchaseStone);
            }

            //depend products operations here..
            dependsProductPanel.SetActive(pb.hasDependProduct);

            DependProduct dp = pb.dependProduct;
            
            dependProductIcon.sprite = dp.productIcon;
            string desc = dp.typeOfProduct.Equals(TypeofProduct.Stones) ? $"+{dp.amount} {LanguageManager.GetText(dp.stoneType.ToString())} Bonus" : dp.typeOfProduct.Equals(TypeofProduct.RemoveAds) ? LanguageManager.GetText(TypeofProduct.RemoveAds.ToString()) : $"+{dp.amount} {LanguageManager.GetText(dp.typeOfProduct.ToString())} Bonus";
            dependProductDesc.text = desc;

            Canvas.ForceUpdateCanvases();
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
        private void PurchaseStone(ProductBase pb)
        {
            if (csm.CanSpendGem(pb.gemPayType,pb.price))
            {
                csm.SpendGem(pb.gemPayType,pb.price);
                csm.AddStones(pb.stoneType, pb.amount);
            }
            else
            {
                UIManager.Instance.PageIndex = (int)pb.gemPayType;
                UIManager.Instance.OpenMenu(Menu.Menus.ShopOrientation);
            }
        }
        private void ShowRewardedAd(RewardType rewardType)
        {
            AdManager.Instance.ShowRewardedAd(rewardType,GetReward);          
        }
        private void SetResetTime()
        {
            DateTime resetTime = DateTime.Now.AddMinutes(CloudSaveManager.Instance.RewardAdFrequency);
            LocalSaveManager.Save(rewardType.ToString(),resetTime); 
            
            StartTimer();
        }
        private void StartTimer()
        {
            purchaseButton.button.interactable = false;
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
            return DateTime.Now >= LocalSaveManager.GetResetTime(rewardType.ToString());
        }
        private IEnumerator TimeUpdate()
        {
            while (!IsTimeOut())
            {
                TimeSpan diffTime =LocalSaveManager.GetResetTime(rewardType.ToString()).Subtract(DateTime.Now);

                string timer = diffTime.Hours > 0 ? $"{diffTime.Hours}{LanguageManager.GetText("sa")} : {diffTime.Minutes}{LanguageManager.GetText("dk")}" : $"{diffTime.Minutes}{LanguageManager.GetText("dk")} : {diffTime.Seconds}{LanguageManager.GetText("sn")}";

                productPrice.text = $"{timer}"; //{LanguageManager.GetText("Remaining")}:

                Debug.Log($"{rewardType}: {timer} ");

                yield return new WaitForSeconds(1);
            }
      
            productPrice.text = LanguageManager.GetText(ProductPayType.Free.ToString());
            purchaseButton.button.interactable = true;
            purchaseButton.OnClick(rewardType, ShowRewardedAd);
        }
        private void OnDisable()
        {
            if (payType != ProductPayType.Free)
                return;

            StopTimer();
        }
        private void OnEnable()
        {
            if (payType != ProductPayType.Free)
                return;

            StartTimer();
        }
    }

}
