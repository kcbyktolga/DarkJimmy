using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

        [SerializeField]
        private bool init = true;

        private readonly float duration = 0.5f;
        private int isOnParamId;
        private Vector2 originalPos;
        private SystemManager system;
        public void Awake()
        {
            system = SystemManager.Instance;
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
            if (init)
            {
                init = false;
                LocalSaveManager.Save(Id, isOn);
                system.addGameElement(gameObject);
            }
            isOn = !isOn;
            //AudioManager.Instance.PlaySound("Lever");
            animator.SetBool(isOnParamId,isOn);
            Vector2 _targetPos = !isOn ? originalPos : targetPos;
            objectT.DOLocalMove(_targetPos, duration);
        }

        public void GetActivate()
        {
            isOn = LocalSaveManager.GetBoolValue(Id);
            objectT.localPosition = isOn ? targetPos : originalPos;
            animator.SetBool(isOnParamId, isOn);
            init = true;
        }
    }
}

