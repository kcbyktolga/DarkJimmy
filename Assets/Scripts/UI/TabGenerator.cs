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
        public A data;
        public List<T> tabs;
       
        public RectTransform container;

        public virtual int NextIndex { get; set; } = 0;
        public virtual int PreviousIndex { get; set; } = 0;
        public virtual int Index { get; set; } = 0;

        public abstract void Generate();
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

    }

}
