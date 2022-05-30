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
        private Scrollbar scrollBar;
        [SerializeField]
        private float duration = 0.5f;

        private float tabTime = 0;
        private Dictionary<int, float> GetPosition = new Dictionary<int, float>();
        HorizontalLayoutGroup horizontalLayoutGroup;
        List<RectTransform> pagesRextTransform = new List<RectTransform>();

        int previousIndex = 0;
        int nextIndex = 0;


        private void Start()
        {           
            pageContent.anchoredPosition = new Vector2(0,pageContent.anchoredPosition.y);
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

                pagesRextTransform.Add(_page.GetComponent<RectTransform>());

                for (int j = 0; j < pageStruct.products.Count; j++)
                {
                    Product product = Instantiate(catalog.GetProduct(pageStruct.pageType),_page.container);
                    product.SetProduct(pageStruct.products[j]);

                    horizontalLayoutGroup.enabled = false;
                    horizontalLayoutGroup.enabled = true;
                    Canvas.ForceUpdateCanvases();
                }

                TabButton _tabButton = Instantiate(catalog.tabButton, tabContent);
                _tabButton.SetTabButtonName(pageStruct.pageName);
                _tabButton.OnClick(false,i,Selected);               
            }
           
            horizontalLayoutGroup.enabled = false;
            horizontalLayoutGroup.enabled = true;
            Canvas.ForceUpdateCanvases();

            horizontalLayoutGroup.padding.left = (int)((UIManager.Instance.GetReferenceResolotion().x - pagesRextTransform[0].rect.width) * 0.5f);
            horizontalLayoutGroup.padding.right = (int)((UIManager.Instance.GetReferenceResolotion().x - pagesRextTransform[pagesRextTransform.Count - 1].rect.width) * 0.5f);

            horizontalLayoutGroup.enabled = false;
            horizontalLayoutGroup.enabled = true;
            Canvas.ForceUpdateCanvases();

            float posX = 0;

            for (int i = 0; i < catalog.pages.Count; i++)
            {
                GetPosition.Add(i,posX);
                Debug.Log(posX);

                if (i >= catalog.pages.Count - 1)
                    continue;

                posX -= horizontalLayoutGroup.spacing + (pagesRextTransform[i].rect.width + pagesRextTransform[i + 1].rect.width) * 0.5f;
            }

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
            tabTime = Time.time + duration;
            float time = 0;
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
    }

}

