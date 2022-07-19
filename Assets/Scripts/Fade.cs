using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace DarkJimmy.UI
{
    public class Fade : Singleton<Fade>
    {
        [SerializeField]
        private Image fadeImage;
        [SerializeField]
        private float duration = 1f;
        private SystemManager system;
        private bool isOn;

        public delegate void FaderTransition();
        public FaderTransition OnTransitionAfter;
        public FaderTransition OnTransitionBefore;
  
        void Start()
        {
            system = SystemManager.Instance;
            FadeIn(null, null);
        }
 
        public void FadeOut(FaderTransition onTransitionAfter, FaderTransition onTransitionBefore)
        {
            OnTransitionBefore = onTransitionBefore;
            OnTransitionAfter = onTransitionAfter;
            PlayFade(true);
        }

        public void FadeIn(FaderTransition onTransitionAfter, FaderTransition onTransitionBefore)
        {
            OnTransitionBefore = onTransitionBefore;
            OnTransitionAfter = onTransitionAfter;
            PlayFade(false);
        }


        private void PlayFade(bool isOn)
        {           
            this.isOn = isOn;
            AudioManager.Instance.PlaySound("Fade");
            fadeImage.color= system.GetWhiteAlfaColor(isOn);
            Color endColor = system.GetWhiteAlfaColor(!isOn);
            fadeImage.DOColor(endColor, duration).OnStart(()=> OnTransitionBefore?.Invoke()).OnComplete(Set);
        }
        private void Set()
        {
            fadeImage.raycastTarget = isOn;
            OnTransitionAfter?.Invoke();

            //if (OnTransitionAfter != null)
            //{
            //    OnTransitionAfter.Invoke();
            //}
        }
    }

}
