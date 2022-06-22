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

        private IStoreController controller;
        private StandardPurchasingModule module;
        private ConfigurationBuilder builder;
        private CloudSaveManager saveManager;
        private readonly Dictionary<string, ProductStruct> GetProductStruct = new Dictionary<string, ProductStruct>();

        private void Start()
        {
            saveManager = CloudSaveManager.Instance;
            CreateCatalog();
        }
        private void CreateCatalog()
        {
            module = StandardPurchasingModule.Instance(AppStore.GooglePlay);
            builder = ConfigurationBuilder.Instance(module);

            for (int i = 0; i < productCatalog.Pages.Count; i++)
            {
                PageStruct page = productCatalog.Pages[i];

                for (int j = 0; j < page.products.Count; j++)
                {                  
                    ProductStruct ps = page.products[j];


                    if (ps.payType.Equals(PayType.Free))
                        continue;

                    builder.AddProduct(ps.productId,ps.productType);
                    GetProductStruct.Add(ps.productId,ps);
                }
            }

            UnityPurchasing.Initialize(this, builder);

        }
        public void OnPurchase(Product product)
        {
            if (product.availableToPurchase && product != null)
                controller.InitiatePurchase(product);

            // UIManager.Instance.OpenMenu(UI.Menu.Menus.AppUpdate);
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
            UIManager.Instance.OpenMenu(UI.Menu.Menus.PurchaseProcess);
        }
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            string id = purchaseEvent.purchasedProduct.definition.id;

            if (GetProduct(id).availableToPurchase)
            {
                ProductStruct ps = GetProductStruct[id];

                switch (ps.typeOfProduct)
                {
                    case TypeofProduct.Gold:
                        saveManager.AddGem(GemType.Gold, ps.amount);
                        break;
                    case TypeofProduct.Diamond:
                        saveManager.AddGem(GemType.Diamond, ps.amount);
                        break;
                    case TypeofProduct.Premium:
                        saveManager.PlayerDatas.HasPremium = true;
                        Debug.Log("Hadi iyisin premýum oldun aq :D");
                        break;
                    case TypeofProduct.Costume:
                       // saveManager.Index = ps.amount;

                        break;
                    case TypeofProduct.Offers:
                        break;
                    default:
                        break;
                }

                return PurchaseProcessingResult.Complete;
            }
            else
            {
                Debug.Log("Burda olmamam lazým");
                return PurchaseProcessingResult.Pending;
            }
        }
    }
}

