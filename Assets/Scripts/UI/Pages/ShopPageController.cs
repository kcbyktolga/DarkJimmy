using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class ShopPageController : TabGenerator<TabButton, Catalog>
    {
        [Header("Shop Property")]
        [SerializeField]
        private RectTransform pageContent;
        [SerializeField]
        private ShopProductPage pagePrefab;
        [SerializeField]
        private Product productPrefab;

        private List<ShopProductPage> pages = new List<ShopProductPage>();
  
        private HorizontalLayoutGroup layout;
        private SystemManager system;

        private void Start()
        {
            system = SystemManager.Instance;
            globalData = CloudSaveManager.Instance.GetDefaultData().Catalog;           
            NextIndex = UIManager.Instance.PageIndex;
            layout = container.GetComponent<HorizontalLayoutGroup>();
            Generate();
            system.toPage += OnSelect;
        }

        public override void Generate()
        {
            for (int i = 0; i < globalData.Pages.Count; i++)
            {
   
                ProductPageBase pageBase = globalData.Pages[i];

                TabButton tab = Instantiate(prefab, container);
                tab.SetTabButtonName(pageBase.pageName);
                tab.SetTabIcon(pageBase.pageIcon);
                tab.OnClick(i, OnSelect);
                tabs.Add(tab);

                ShopProductPage page = Instantiate(pagePrefab, pageContent);
                pages.Add(page);
                
                for (int j = 0; j < pageBase.products.Count; j++)
                {
                    ProductBase productBase = pageBase.products[j];
                    Product product = Instantiate(productPrefab, page.container);
                    product.payType = productBase.payType;
                    product.SetProduct(productBase);
                    product.gameObject.SetActive(false);
                }
                page.gameObject.SetActive(false);
            }
            OnSelect(NextIndex);
        }
        public override void OnSelect(int index)
        {
            base.OnSelect(index);

            TabButton prevTab = GetTab(PreviousIndex);
            TabButton nextTab = GetTab(NextIndex);

            prevTab.SetTabButton(false);
            nextTab.SetTabButton(true);

            pages[PreviousIndex].gameObject.SetActive(false);
            pages[NextIndex].gameObject.SetActive(true);

            pages[PreviousIndex].ProductDisable();
            pages[NextIndex].ProductEnable();

            layout.enabled = false;
            layout.enabled = true;
            Canvas.ForceUpdateCanvases();

            system.onChangedPage(NextIndex);
           // AudioManager.Instance.PlaySound("Turn Page");

        }

        private void OnDestroy()
        {
            UIManager.Instance.PageIndex = 0;
            system.toPage -= OnSelect;
        }
    }
}


