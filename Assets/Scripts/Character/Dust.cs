using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Characters
{
    public class Dust :MonoBehaviour,IAnimationEvent
    {
        private Vector2 originalScale;

        private void Start()
        {
            originalScale = transform.localScale;
            originalScale.x = Mathf.Abs(originalScale.x);
        }

        public void AnimationEvent()
        {
            gameObject.SetActive(false);
            transform.localScale = originalScale;
        }
    }
}

