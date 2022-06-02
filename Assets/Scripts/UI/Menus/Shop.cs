using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkJimmy.UI
{
    public class Shop : Menu
    {
        [Header("Shop Property")]
        [SerializeField]
        private Catalog catalog;
        [SerializeField]
        private RectTransform pageContent;
        [SerializeField]
        private RectTransform tabContent;
        [SerializeField]
        private ScrollRect scrollRect;
        [SerializeField]
        private float duration = 0.5f;

        private Dictionary<int, float> GetPosition = new Dictionary<int, float>();
        private HorizontalLayoutGroup horizontalLayoutGroup;
        private List<RectTransform> pagesRectTransform = new List<RectTransform>();
        private List<float> scrollPos = new List<float>();

        private float tabTime = 0;
        private int previousIndex = 0;
        private int nextIndex = 0;

        public override void Start()
        {
            base.Start();
            pageContent.anchoredPosition = new Vector2(0,pageContent.anchoredPosition.y);             
            nextIndex = UIManager.Instance.PageIndex;
            GeneratePageAndTabs();
        }
        public void GeneratePageAndTabs()
        {
            horizontalLayoutGroup = pageContent.GetComponent<HorizontalLayoutGroup>();

            for (int i = 0; i < catalog.pages.Count; i++)
            {
                PageStruct pageStruct = catalog.pages[i];

                Page _page = Instantiate(catalog.GetPage(pageStruct.pageType), pageContent);
                _page.SetPage(pageStruct.pageName);

                pagesRectTransform.Add(_page.GetComponent<RectTransform>());

                for (int j = 0; j < pageStruct.products.Count; j++)
                {
                    Product product = Instantiate(catalog.GetProduct(pageStruct.products[j].productType),_page.container);
                    product.SetProduct(pageStruct.products[j]);

                    UpdateCanvas();
                }

                TabButton _tabButton = Instantiate(catalog.tabButton, tabContent);
                _tabButton.SetTabButtonName(pageStruct.pageName);
                _tabButton.OnClick(false,i,Selected);               
            }

            UpdateCanvas();

            horizontalLayoutGroup.padding.left = (int)((UIManager.Instance.GetReferenceResolotion().x - pagesRectTransform[0].rect.width) * 0.5f);
            horizontalLayoutGroup.padding.right = (int)((UIManager.Instance.GetReferenceResolotion().x - pagesRectTransform[pagesRectTransform.Count - 1].rect.width) * 0.5f);

            UpdateCanvas();

            CalculatePagePosition();

            CalculateScrollBar();

            Selected(false,nextIndex);
        }
        private void Selected(bool onDrag, int index)
        {
            previousIndex = nextIndex;
            nextIndex = index;

            TabButton previous = GetTabButton(previousIndex);
            TabButton next = GetTabButton(nextIndex);

            previous.SetTabButton(false);
            next.SetTabButton(true);

            if (!onDrag)
                StartCoroutine(nameof(PageSwipe));
        }
        private IEnumerator PageSwipe()
        {       
            float time = 0;
            tabTime = Time.time + duration;
            scrollRect.StopMovement();
            float currentPos = pageContent.anchoredPosition.x;
            float endPos = GetPosition[nextIndex];

            while (time<=1)
            {
                time += Time.deltaTime / duration;
                float posX = Mathf.Lerp(currentPos,endPos,time);
                pageContent.anchoredPosition = new Vector2(posX,pageContent.anchoredPosition.y);

                yield return null;
            }
        }
        private TabButton GetTabButton(int index)
        {
            return tabContent.GetChild(index).GetComponent<TabButton>();
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

            for (int i = 0; i < catalog.pages.Count; i++)
            {
                GetPosition.Add(i, posX);

                if (i >= catalog.pages.Count - 1)
                    continue;

                posX -= horizontalLayoutGroup.spacing + (pagesRectTransform[i].rect.width + pagesRectTransform[i + 1].rect.width) * 0.5f;
            }
        }

        private void LateUpdate()
        {
            if(tabTime<Time.time)
                Selected(true,GetCurrentIndex(pageContent.anchoredPosition.x));
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
        private void OnDestroy()
        {
            UIManager.Instance.PageIndex = 0;
        }
    }

}

