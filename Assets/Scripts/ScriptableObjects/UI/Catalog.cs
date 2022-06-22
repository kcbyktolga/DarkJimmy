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
        public int startIndex = 2;
        public TabButton tabButton;
        public List<PageStruct> Pages;
        public List<PowerUpStruct> GetPowerUps;
        public List<LuckyProduct> GetProductLuckySpin;
    
    
        [ContextMenu("Set Product ID")]
        private void SetProductId()
        {
            for (int i = startIndex; i < Pages.Count; i++)
            {
                PageStruct ps = Pages[i];

                for (int j = 0; j < ps.products.Count; j++)
                {
                    ProductStruct product = ps.products[j];

                    if (product.payType.Equals(PayType.Free))
                        continue;

                    ps.products[j].productId = $"com.rhombeusgaming.{i}{j}";

                }
            }
        }

    }

    [Serializable]
    public struct PageStruct
    {
        public string pageName;
        public Sprite pageIcon;
        public List<ProductStruct> products;
    }
    [Serializable]
    public class ProductStruct
    {
        public TypeofProduct typeOfProduct;
        public int amount;
        public string productId;
        public string productName;
        public string productTitle;
        public PayType payType;
        public ProductType productType;
        public List<Sprite> productIcon;
    }
    [Serializable]
    public class LuckyProduct
    {
        public TypeofProduct typeOfProduct;
        public int luckyFactor;
        public int amount;
        public string productName;
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

    public enum PayType
    {
        Free,
        Paid
    } 
    public enum TypeofProduct
    {
        Gold,
        Diamond,
        Premium,
        Costume,
        Offers
    }

}

