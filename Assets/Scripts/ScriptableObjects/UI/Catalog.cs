using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DarkJimmy.UI;

namespace DarkJimmy
{
    [CreateAssetMenu(menuName = "Data/Catalog", fileName = "Catalog")]
    public class Catalog : ScriptableObject
    {
        public TabButton tabButton;
        public List<PageStruct> pages;
        public List<CharacterData> characterDatas;

        private Dictionary<PageType, string> PagePaths = new Dictionary<PageType, string>
        {
             {PageType.Grid, "Pages/GridPage"},
             {PageType.Horizontal, "Pages/HorizontalPage"},
             {PageType.Single, "Pages/SinglePage"},
  
        };
        private Dictionary<ProductType, string> ProductPaths = new Dictionary<ProductType, string>
        {
             {ProductType.HorizontalSingle, "Products/HorizontalProductSingle"},
             {ProductType.HorizontalDuo, "Products/HorizontalProductDuo"},
             {ProductType.GridSingle, "Products/GridProductSingle"},
             {ProductType.GridDuo, "Products/GridProductDuo"},

        };
        public Page GetPage(PageType type)
        {
            if (PagePaths.TryGetValue(type, out string path))
                return Resources.Load<Page>(path);
           
            return null;
        }
        public Product GetProduct(ProductType type)
        {
            if (ProductPaths.TryGetValue(type, out string path))
                return Resources.Load<Product>(path);

            return null;
        }

    }

    [Serializable]
    public struct PageStruct
    {
        public string pageName;
        public PageType pageType;
        public List<ProductStruct> products;
    }
    [Serializable]
    public struct ProductStruct
    {
        public string productId;
        public string productName;
        public string productTitle;
        public string productPrice;
        public PayType payType;
        public ProductType productType;
        public List<Sprite> productIcon;
    }

    public enum PayType
    {
        Free,
        Paid
    }
    public enum PageType
    {
        Grid,
        Horizontal,
        Single
    }

    public enum ProductType
    {
        GridSingle,
        GridDuo,
        HorizontalSingle,
        HorizontalDuo
    }
}

