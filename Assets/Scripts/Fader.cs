using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class Fader : Singleton<Fader>
    {
        [SerializeField]
        private Image fadeImage;
        [SerializeField]
        private float duration = 0.5f;
        private SystemManager system;
      
        void Start()
        {
            system = SystemManager.Instance;

            FadeIn();
        }

        public void FadeOut()
        {
            StartCoroutine(Fade(true));
        }

        public void FadeIn()
        {
            StartCoroutine(Fade(false));
        }

        IEnumerator Fade(bool isOn)
        {
            float time = 0;
            Color startColor = system.GetWhiteAlfaColor(isOn);
            Color endColor = system.GetWhiteAlfaColor(!isOn);

            while (time<=1)
            {
                time += Time.deltaTime / duration;
                fadeImage.color = Color.Lerp(startColor, endColor, time);
                yield return null;
            }

            fadeImage.raycastTarget = isOn;
        }

    }

}
