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
                Debug.Log("sat�n al�nmad�");
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
            string id = purchaseEvent.purchasedProduct.definition.id;

            if (GetProduct(id).availableToPurchase)
            {
                Debug.Log(id);
 
                return PurchaseProcessingResult.Complete;
            }
            else
            {
                Debug.Log("Burda olmamam laz�m");
                return PurchaseProcessingResult.Pending;
            }
        }

    }
}
