using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class BaseButton : MonoBehaviour
    {
        public Button button;
        public virtual void OnClick(Action action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action.Invoke());
        }
        public virtual void OnClick<T>(T type, Action<T> action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action.Invoke(type));
        }
        public virtual void OnClick<T, A>(T type0, A type1, Action<T, A> action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action.Invoke(type0, type1));
        }
    }
}

