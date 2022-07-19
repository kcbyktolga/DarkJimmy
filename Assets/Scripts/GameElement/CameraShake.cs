using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

namespace DarkJimmy
{
    public class CameraShake : MonoBehaviour
    {
        //[SerializeField]
        //private AnimationCurve curve;
        private Animator animator;
        private int shakeId;
       
        void Start()
        {
            animator = GetComponent<Animator>();
            shakeId = Animator.StringToHash("Shake");
            SystemManager.Instance.cameraShake += Shake;
        }

        private void Shake()
        {
            animator.SetTrigger(shakeId);
            //transform.DORotate(-2 * Vector3.forward, 0.2f).SetEase(curve).OnComplete(()=> transform.DORotate(2 * Vector3.forward, 0.2f).SetEase(curve).OnComplete(() => transform.DORotate(Vector3.zero, 0.1f).SetEase(curve)));
            //transform.DOShakeRotation(0.5f);
            //int a = Random.Range(-1,2);
            //int k = a <= 0 ? -1 : 1;
            //transform.eulerAngles = 2.5f*k*Vector3.forward;
            //transform.DORotate(Vector3.zero,0.25f).SetEase(curve);
        }
        private void OnDestroy()
        {
            SystemManager.Instance.cameraShake -= Shake;
        }
    }
}

