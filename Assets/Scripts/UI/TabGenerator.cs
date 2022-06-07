using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkJimmy.UI
{
    public abstract class TabGenerator<T,A> : MonoBehaviour where T: class where A : class
    {
        [Header("Components")]
        public T prefab;
        public A globalData;
        public A localData;
        public List<T> tabs;
        public RectTransform container;
        public float duration = 0.5f;

        public abstract void Generate();
        public virtual int NextIndex { get; set; } = 0;
        public virtual int PreviousIndex { get; set; } = 0;
        public virtual int Index { get; set; } = 0;
        public virtual void OnSelect(int index)
        {
            PreviousIndex = NextIndex;
            NextIndex = index;
        }
        public virtual void OnSelect(bool isOn , int index)
        {
            PreviousIndex = NextIndex;
            NextIndex = index;
        }
        public virtual T GetTab(int index)
        {
            return tabs[index];
        }
        public virtual IEnumerator Slide(RectTransform content, float endPos )
        {
            float time = 0;
            float currentPos = content.anchoredPosition.x;
           // float endPos = GetPosition[Index];

            while (time <= 1)
            {
                time += Time.deltaTime / duration;
                float posX = Mathf.Lerp(currentPos, endPos, time);
                content.anchoredPosition = new Vector2(posX, content.anchoredPosition.y);

                yield return null;
            }
        }
    }
}
