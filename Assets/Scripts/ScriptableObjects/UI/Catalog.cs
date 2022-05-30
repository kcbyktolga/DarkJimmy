using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DarkJimmy.UI
{
    [CreateAssetMenu(menuName = "Data/Catalog", fileName = "Catalog")]
    public class Catalog : ScriptableObject
    {
        public TabButton tabButton;
        public List<PageStruct> pages;


        private Dictionary<PageType, string> PagePaths = new Dictionary<PageType, string>
        {
             {PageType.Grid, "Pages/Grid"},
             {PageType.Horizontal, "Pages/Horizontal"},
             {PageType.Single, "Pages/Single"},
  
        };
        private Dictionary<PageType, string> ProductPaths = new Dictionary<PageType, string>
        {
             {PageType.Grid, "Products/Grid"},
             {PageType.Horizontal, "Products/Horizontal"},
             {PageType.Single, "Products/Single"},

        };
        public Page GetPage(PageType type)
        {
            if (PagePaths.TryGetValue(type, out string path))
                return Resources.Load<Page>(path);
           
            return null;
        }
        public Product GetProduct(PageType type)
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
        public Sprite productIcon;
        public PayType payType;
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
}

