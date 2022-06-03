using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Characters
{
    public class Impact :MonoBehaviour,IAnimationEvent
    {
        private Vector2 originalScale;
        private void Start()
        {
            originalScale = transform.localScale;
        }
        public void AnimationEvent()
        {
            gameObject.SetActive(false);
            transform.localScale = originalScale;
        }
    }
}

