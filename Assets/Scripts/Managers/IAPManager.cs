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
        private readonly Dictionary<string, ProductBase> GetProductStruct = new Dictionary<string, ProductBase>();

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
               ProductPageBase page =productCatalog.Pages[i];

                for (int j = 0; j < page.products.Count; j++)
                {                  
                    ProductBase pb = page.products[j];              

                    if (!pb.payType.Equals(ProductPayType.Paid))
                        continue;

                    builder.AddProduct(pb.productId, pb.productType);
                    GetProductStruct.Add(pb.productId, pb);
                }
            }

            UnityPurchasing.Initialize(this, builder);

        }
        public void OnPurchase(Product product)
        {
            if (product.availableToPurchase && product != null)
                controller.InitiatePurchase(product);
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
                ProductBase pb = GetProductStruct[id];
                ProductOperation(pb.typeOfProduct,pb.amount);
                if (pb.hasDependProduct)
                    ProductOperation(pb.dependProduct.typeOfProduct, pb.dependProduct.amount);



                return PurchaseProcessingResult.Complete;
            }
            else
            {                
                return PurchaseProcessingResult.Pending;
            }
        }

        public void ProductOperation( TypeofProduct typeOfProduct, int amount)
        {
            switch (typeOfProduct)
            {
                case TypeofProduct.Gold:
                    saveManager.AddGem(GemType.Gold, amount);
                    break;
                case TypeofProduct.Diamond:
                    saveManager.AddGem(GemType.Diamond, amount);
                    break;
                case TypeofProduct.RemoveAds:
                    saveManager.PlayerDatas.HasRemoveAds = true;
                    break;
                case TypeofProduct.Stones:
                    

                    break;
                case TypeofProduct.Offers:
                    break;
                default:
                    break;
            }
        }
    }
}

