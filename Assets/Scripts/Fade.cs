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
        private float duration = 0.5f;
        private SystemManager system;
        private bool isOn;

        public delegate void FaderTransition();
        public FaderTransition transition;
  
        void Start()
        {
            system = SystemManager.Instance;
            FadeIn(null);
        }
 
        public void FadeOut(FaderTransition fadeTransition)
        {
            transition = fadeTransition;
            PlayFade(true);
        }

        public void FadeIn(FaderTransition fadeTransition)
        {
            transition = fadeTransition;
            PlayFade(false);
        }


        private void PlayFade(bool isOn)
        {         
            this.isOn = isOn;
            fadeImage.color= system.GetWhiteAlfaColor(isOn);
            Color endColor = system.GetWhiteAlfaColor(!isOn);
            fadeImage.DOColor(endColor, duration).OnComplete(Set);
        }
        private void Set()
        {
            fadeImage.raycastTarget = isOn;

            if (transition != null)
            {
                transition.Invoke();
            }
        }

        //IEnumerator Fade(bool isOn, FaderTransition transition)
        //{
        //    float time = 0;
        //    Color startColor = system.GetWhiteAlfaColor(isOn);
        //    Color endColor = system.GetWhiteAlfaColor(!isOn);

        //    while (time<=1)
        //    {
        //        time += Time.deltaTime / duration;
        //        fadeImage.color = Color.Lerp(startColor, endColor, time);
        //        yield return null;
        //    }

        //    fadeImage.raycastTarget = isOn;

        //    this.transition = transition;

        //    if (this.transition != null)
        //    {
        //        this.transition.Invoke();
        //        this.transition = null;
        //    }


        //}
    }

}
