using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DarkJimmy.UI;
using UnityEngine.Purchasing;


namespace DarkJimmy
{
    [CreateAssetMenu(menuName = "Data/Catalog", fileName = "Catalog")]
    public class Catalog : ScriptableObject
    {

        [SerializeField]
        private List<ProductPageBase> shopPages;
        [SerializeField]
        private List<PowerUpStruct> powerUps;
        [SerializeField]
        private List<RewardProduct> luckySpin;

        public List<PowerUpStruct> GetPowerUps
        {
            get { return powerUps; }
        }
        public List<RewardProduct> GetProductLuckySpin
        {
            get { return luckySpin; }
        }
        public List<ProductPageBase> Pages
        {
            get { return shopPages; }
        }


        [ContextMenu("Set Product ID")]
        private void SetProductId()
        {
            for (int i = 0; i < Pages.Count; i++)
            {
                ProductPageBase pb = Pages[i];

                for (int j = 0; j < pb.products.Count; j++)
                {
                    ProductBase product = pb.products[j];

                    if (!product.payType.Equals(ProductPayType.Paid))
                        continue;

                    product.productId = $"com.rhombeusgaming.{i}{j}";

                }
            }
        }

    }

    [Serializable]
    public class ProductBase
    {
        public string productName;
        public TypeofProduct typeOfProduct;
       
        public Stones stoneType;
        public ProductPayType payType;
        public ProductType productType;
        public int amount;
        public int price;
        public string productId;
        public string productTitle;
        public GemType gemPayType;
        public List<Sprite> productIcon;
        public bool hasDependProduct = false;
        public DependProduct dependProduct;
    }

    [Serializable]
    public class DependProduct
    {       
        public Sprite productIcon;
        public TypeofProduct typeOfProduct;
        public Stones stoneType;
        public int amount;
    }

    [Serializable]
    public class RewardProduct
    {
        public TypeofProduct typeOfProduct;
        public int luckyFactor;
        public int amount;
        public string productName;
        public Stones stoneType;
        public List<Sprite> productIcon;
    }

    [Serializable]
    public struct PowerUpStruct
    {
        public string powerUpName;
        public Sprite powerUpIcon;
        public PowerUp powerUpType;
        public GemType powerUpPayType;
        public int powerUpPrice;
        public float multiple;
        [TextArea(5, 1)]
        public string powerUpDescription;
    }

    public enum ProductPayType
    {
        Free,
        Paid,
        Gem
    } 
    public enum TypeofProduct
    {
        Gold,
        Diamond,
        RemoveAds,
        Stones,
        Offers
    }

}

