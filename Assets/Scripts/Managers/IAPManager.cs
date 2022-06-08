using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace DarkJimmy
{
    public class IAPManager : Singleton<IAPManager>,IStoreListener
    {
        [SerializeField]
        private Catalog productCatalog;
        public IStoreController controller;
        StandardPurchasingModule module;
        ConfigurationBuilder builder;


        public Dictionary<string, ProductStruct> Products = new Dictionary<string, ProductStruct>();
        Dictionary<string, UI.PurchaseButton> ShopButtons = new Dictionary<string, UI.PurchaseButton>();

        private void Start()
        {
            CreateCatalog();
        }
        private void CreateCatalog()
        {
            module = StandardPurchasingModule.Instance(AppStore.GooglePlay);
            builder = ConfigurationBuilder.Instance(module);

            for (int i = 0; i < productCatalog.pages.Count; i++)
            {
                PageStruct page = productCatalog.pages[i];

                for (int j = 0; j < page.products.Count; j++)
                {                  
                    ProductStruct ps = page.products[j];

                    if (ps.payType.Equals(PayType.Free))
                        continue;

                    Products.Add(ps.productId, ps);
                    builder.AddProduct(ps.productId,ps.productType);                    
                }
            }

            UnityPurchasing.Initialize(this, builder);
        }
        public void OnPurchase(Product product)
        {
            if (product.availableToPurchase && product != null)
                controller.InitiatePurchase(product);
            else
                Debug.Log("satýn alýnmadý");
        }
        public Product GetProduct(string id)
        {
            return controller.products.WithID(id);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.controller = controller;
        }
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            
        }
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            
        }
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            throw new System.NotImplementedException();
        }

    }
}

