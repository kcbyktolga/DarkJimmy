using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class ShopSlidePage : TabGenerator<TabButton,Catalog>
    {
        [Header("Shop Page Property")]    
        [SerializeField]
        private RectTransform pageContent;
        [SerializeField]
        private ScrollRect scrollRect;

        private HorizontalLayoutGroup horizontalLayoutGroup;
        private List<RectTransform> pagesRectTransform = new List<RectTransform>();
        private List<float> scrollPos = new List<float>();
        private List<float> GetPosition = new List<float>();
        private float tabTime = 0;
       
        private List<Product> productList = new List<Product>();

        void Start()
        {
            horizontalLayoutGroup = pageContent.GetComponent<HorizontalLayoutGroup>();

            pageContent.anchoredPosition = new Vector2(0, pageContent.anchoredPosition.y);
            NextIndex = UIManager.Instance.PageIndex;

            Generate();
            OnSelect(false,NextIndex);
        }
        public override void Generate()
        {
            for (int i = 0; i < globalData.Pages.Count; i++)
            {
                PageStruct pageStruct = globalData.Pages[i];

                TabButton tab = Instantiate(prefab, container);
                tab.SetTabButtonName(pageStruct.pageName);
                tab.OnClick(false, i, OnSelect);
                tabs.Add(tab);

                ShopProductPage page = Instantiate(globalData.GetPage(pageStruct.pageType), pageContent);
                page.SetPage(pageStruct.pageName);

                GridLayoutGroup grid = page.container.GetComponent<GridLayoutGroup>();

                if(grid != null && pageStruct.pageType.Equals(PageType.Grid))
                    grid.constraintCount = pageStruct.products.Count / 2;

                pagesRectTransform.Add(page.GetComponent<RectTransform>());

                for (int j = 0; j < pageStruct.products.Count; j++)
                {
                    ProductStruct productStruct = pageStruct.products[j];
                    Product product = Instantiate(globalData.GetProduct(productStruct.productShape), page.container);
                    product.SetProduct(productStruct);
                    UpdateCanvas();
                }
            }

            UpdateCanvas();

            horizontalLayoutGroup.padding.left = (int)((UIManager.Instance.GetReferenceResolotion().x - pagesRectTransform[0].rect.width) * 0.5f);
            horizontalLayoutGroup.padding.right = (int)((UIManager.Instance.GetReferenceResolotion().x - pagesRectTransform[pagesRectTransform.Count - 1].rect.width) * 0.5f);

            UpdateCanvas();

            CalculatePagePosition();

            CalculateScrollBar();

        }

        private void LateUpdate()
        {
            if (tabTime < Time.time)
                OnSelect(true, GetCurrentIndex(pageContent.anchoredPosition.x));
        }
        public override void OnSelect(bool isOn, int index)
        {
            base.OnSelect(isOn, index);

            TabButton previous = GetTab(PreviousIndex);
            TabButton next = GetTab(NextIndex);

            previous.SetTabButton(false);
            next.SetTabButton(true);

            if (!isOn)
                StartCoroutine(Slide(pageContent,GetPosition[NextIndex]));

        }
        private void UpdateCanvas()
        {
            horizontalLayoutGroup.enabled = false;
            horizontalLayoutGroup.enabled = true;
            Canvas.ForceUpdateCanvases();
        }
        private void CalculatePagePosition()
        {
            float posX = 0;

            for (int i = 0; i < globalData.Pages.Count; i++)
            {
                //  GetPosX.Add(i, posX);
                GetPosition.Add(posX);

                if (i >= globalData.Pages.Count - 1)
                    continue;

                posX -= horizontalLayoutGroup.spacing + (pagesRectTransform[i].rect.width + pagesRectTransform[i + 1].rect.width) * 0.5f;
            }
        }
        private void CalculateScrollBar()
        {
            float posX = 0;
            for (int i = 0; i < pagesRectTransform.Count; i++)
            {
                if (i == 0)
                    posX -= (pagesRectTransform[i].rect.width + horizontalLayoutGroup.spacing) * 0.5f;
                else
                    posX -= pagesRectTransform[i].rect.width + horizontalLayoutGroup.spacing;

                scrollPos.Add(posX);
            }
        }
        private int GetCurrentIndex(float value)
        {
            if (value >= scrollPos[0])
                return 0;
            else if (value < scrollPos[0] && value >= scrollPos[1])
                return 1;
            else if (value < scrollPos[1] && value >= scrollPos[2])
                return 2;
            else if (value < scrollPos[2] && value >= scrollPos[3])
                return 3;
            else
                return 4;
        }
        public override IEnumerator Slide(RectTransform content, float endPos)
        {
            tabTime = Time.time + duration;
            scrollRect.StopMovement();   
            return base.Slide(content, endPos);
        }
        private void OnDestroy()
        {
            UIManager.Instance.PageIndex = 0;
        }

    }

}

