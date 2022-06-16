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
        public List<ProductStruct> GetProductShop;
        public List<ProductStruct> GetProductLuckySpin;
    
        private Dictionary<PageType, string> PagePaths = new Dictionary<PageType, string>
        {
             {PageType.Grid, "Pages/GridPage"},
             {PageType.Horizontal, "Pages/HorizontalPage"},
             {PageType.Single, "Pages/SinglePage"},
  
        };
        private Dictionary<ProductShape, string> ProductPaths = new Dictionary<ProductShape, string>
        {
             {ProductShape.HorizontalSingle, "Products/HorizontalProductSingle"},
             {ProductShape.HorizontalDuo, "Products/HorizontalProductDuo"},
             {ProductShape.GridSingle, "Products/GridProductSingle"},
             {ProductShape.GridDuo, "Products/GridProductDuo"},

        };
        public ShopProductPage GetPage(PageType type)
        {
            if (PagePaths.TryGetValue(type, out string path))
                return Resources.Load<ShopProductPage>(path);
           
            return null;
        }
        public UI.Product GetProduct(ProductShape type)
        {
            if (ProductPaths.TryGetValue(type, out string path))
                return Resources.Load<UI.Product>(path);

            return null;
        }

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
        public PageType pageType;
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
        public ProductShape productShape;
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

    public enum ProductShape
    {
        GridSingle,
        GridDuo,
        HorizontalSingle,
        HorizontalDuo
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

