using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

namespace DarkJimmy.UI
{
    public class BaseButton : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
    {
        public Button button;
        public TMP_Text buttonName;
        [SerializeField]
        private RectTransform baseTransform;

        private const float clickDuration = 0.05f;
        private const float scaleMultiple = 0.95f;
        private bool isClick = false;
        private Vector2 originalScale = Vector2.one;


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
            if(buttonName!=null)
                buttonName.text = LanguageManager.GetText(name);

        }
        public virtual void OpenPage() { }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (baseTransform == null)
                return;

            //originalScale = baseTransform.localScale;
            isClick = true;
            // OnDrag(eventData);
            // clickTime = clickDuration + Time.time; 

            StartCoroutine(Scale(originalScale * scaleMultiple));
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (baseTransform == null)
                return;

            if (isClick)
            {
                isClick = false;
                StartCoroutine(Scale(originalScale*scaleMultiple));
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {       
            if (baseTransform == null)
                return;
            StartCoroutine(Scale(originalScale));
        }
        private IEnumerator Scale(Vector2 endScale)
        {
            float time = 0;

            while (time<=1)
            {
                time += Time.fixedDeltaTime / clickDuration;
                baseTransform.localScale = Vector2.Lerp(baseTransform.localScale,endScale,time);
                yield return null;
            }
        }
        private void OnDisable()
        {
            if(baseTransform !=null)
                baseTransform.localScale = originalScale;
        }

    }
}

