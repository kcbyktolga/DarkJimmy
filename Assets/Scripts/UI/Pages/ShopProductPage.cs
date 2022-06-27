using UnityEngine;
using TMPro;
using System.Collections;


namespace DarkJimmy.UI
{
    public class ShopProductPage : MonoBehaviour
    {
        public RectTransform container;
        [SerializeField]

        private TMP_Text pageName;
        public void SetPage(string name)
        {
            pageName.text = LanguageManager.GetText(name);
        }
        public void ProductEnable()
        {
            StartCoroutine(ProductsActive());
        }
        public void ProductDisable()
        {
            for (int i = 0; i < container.childCount; i++)
                container.GetChild(i).gameObject.SetActive(false);
        }

        private IEnumerator ProductsActive()
        {
            yield return new WaitForEndOfFrame();

            for (int i = 0; i < container.childCount; i++)
            {
                container.GetChild(i).gameObject.SetActive(true);
                AudioManager.Instance.PlaySound("Card Flip");

                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}

