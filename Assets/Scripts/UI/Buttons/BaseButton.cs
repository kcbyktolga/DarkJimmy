using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

namespace DarkJimmy.UI
{
    public class BaseButton : MonoBehaviour
    {
        public Button button;
        public TMP_Text buttonName;

  
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
        public virtual void SetTabButtonName(string name)
        {
            buttonName.text = name; //LanguageManager.GetText(name);
        }
        public virtual void OpenPage() { }

    }
}

