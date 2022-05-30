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
        private float duration = 0.5f;


        private Dictionary<int, float> GetPosition = new Dictionary<int, float>();
        VerticalLayoutGroup verticalLayoutGroup;

        int previousIndex = 0;
        int nextIndex = 0;


        private void Start()
        {
            verticalLayoutGroup = pageContent.GetComponent<VerticalLayoutGroup>();

        }

        public void GeneratePageAndTabs()
        {
            for (int i = 0; i < catalog.pages.Count; i++)
            {
                PageStruct pageStruct = catalog.pages[i];

                Page _page = Instantiate(catalog.GetPage(pageStruct.pageType), pageContent);
                _page.SetPage(pageStruct.pageName);


                for (int j = 0; j < pageStruct.products.Count; j++)
                {
                    Product product = Instantiate(catalog.GetProduct(pageStruct.pageType),_page.container);
                    product.SetProduct(pageStruct.products[j]);
                }

                TabButton _tabButton = Instantiate(catalog.tabButton, tabContent);
                _tabButton.SetTabButtonName(pageStruct.pageName);
                _tabButton.OnClick(false,i,Selected);               
            }
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
            float currentPos = pageContent.anchoredPosition.x;
            float endPos = GetPosition[nextIndex];

            while (time<1)
            {
                time += Time.deltaTime / duration;

                float posX = Mathf.Lerp(currentPos,endPos,duration);
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

