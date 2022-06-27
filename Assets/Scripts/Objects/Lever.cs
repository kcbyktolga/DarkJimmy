using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Objects
{
    public class Lever : Interactable
    {
        [Header("Lever")]
        public Transform objectT;
        [SerializeField]
        private Vector2 targetPos;

        [SerializeField]
        private bool isOn = false;

        private readonly float duration = 0.5f;
        private int isOnParamId;
        private Vector2 originalPos;

        private void Awake()
        {
            isOnParamId = Animator.StringToHash("isOn");
            animator.SetBool(isOnParamId,isOn);
            originalPos = objectT.localPosition;
            objectT.localPosition = isOn ? targetPos : originalPos;

        }
        private void Start()
        {           
            activate += Slide;
        }

        private void Slide()
        {
            isOn = !isOn;
            //AudioManager.Instance.PlaySound("Lever");
            animator.SetBool(isOnParamId,isOn);
            Vector2 _targetPos = !isOn ? originalPos : targetPos;

            StartCoroutine(SlideObject(_targetPos));
        }

        private IEnumerator SlideObject( Vector2 _targetPos)
        {
            //AudioManager.Instance.PlaySound("Brick Slide");
            float time = 0;

            while (time<=1)
            {
                time += Time.deltaTime / duration;
                objectT.localPosition = Vector2.Lerp(objectT.localPosition, _targetPos,time);
                yield return null;
            }
        }
    }
}

