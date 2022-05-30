using UnityEngine;
using TMPro;


namespace DarkJimmy.UI
{
    public class Page : MonoBehaviour
    {
        public RectTransform container;
        [SerializeField]
        private TMP_Text pageName;
        public void SetPage(string name)
        {
            pageName.text = name;
        }
    }
}

