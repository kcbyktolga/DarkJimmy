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


        private void Start()
        {
            globalData = CloudSaveManager.Instance.GetDefaultData().Catalog;           
            NextIndex = UIManager.Instance.PageIndex;
            layout = container.GetComponent<HorizontalLayoutGroup>();
            Generate();
        }

        public override void Generate()
        {
            for (int i = 0; i < globalData.Pages.Count; i++)
            {
                PageStruct pageStruct = globalData.Pages[i];

                TabButton tab = Instantiate(prefab, container);
                tab.SetTabButtonName(pageStruct.pageName);
                tab.SetTabIcon(pageStruct.pageIcon);
                tab.OnClick(i, OnSelect);
                tabs.Add(tab);

                ShopProductPage page = Instantiate(pagePrefab, pageContent);
                pages.Add(page);
                // page.SetPage(pageStruct.pageName);

                for (int j = 0; j < pageStruct.products.Count; j++)
                {
                    ProductStruct productStruct = pageStruct.products[j];
                    Product product = Instantiate(productPrefab, page.container);
                    product.payType = productStruct.payType;
                    product.SetProduct(productStruct);
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

           // AudioManager.Instance.PlaySound("Turn Page");
         
        }

        private void OnDestroy()
        {
            UIManager.Instance.PageIndex = 0;
        }
    }
}


