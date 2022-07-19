using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;

namespace DarkJimmy.UI
{
    public class BaseButton : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
    {
        public Button button;
        public TMP_Text buttonName;
        [SerializeField]
        private RectTransform baseTransform;

        public const float clickDuration = 0.05f;
        private const float scaleMultiple = 0.95f;
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
            ClickDownSound();
            // StartCoroutine(Scale(originalScale * scaleMultiple));
            Scale(originalScale * scaleMultiple,1,Ease.Linear);
        }  
        public void OnPointerUp(PointerEventData eventData)
        {       
            if (baseTransform == null)
                return;
            ClickUpSound();
            //StartCoroutine(Scale(originalScale));
            Scale(originalScale,10,Ease.OutElastic);

        }     
        private void Scale(Vector2 endScale,int mulitple, Ease ease)
        {
            baseTransform.DOScale(endScale, clickDuration * mulitple).SetEase(ease)
                 .OnUpdate(DoKill);
        }

        private void DoKill()
        {
            if (baseTransform == null)
                baseTransform.DOKill();
        }
        private void OnDisable()
        {
            if(baseTransform !=null)
                baseTransform.localScale = originalScale;
        }

        public virtual void ClickDownSound()
        {
            AudioManager.Instance.PlaySound("Click Down");
        }
        public virtual void ClickUpSound()
        {
            AudioManager.Instance.PlaySound("Click Up");
        }

    }
}

