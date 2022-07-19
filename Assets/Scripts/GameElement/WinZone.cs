using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DarkJimmy.Objects
{
    public class WinZone : Interactable
    {
        [SerializeField]
        private GameObject flag;
        [SerializeField]
        private AnimationCurve curve;
        [SerializeField]
        private float targetY = 5.2f;
        [SerializeField]
        private float _duration = 1.5f;
        private GameSaveManager gsm;

        private void Start()
        {
            gsm = GameSaveManager.Instance;           
            activate += Win;
        }
        private void Win()
        {
            if (gsm.CountDown <= 0)
                return;
            gsm.IsVictory = true;

            flag.transform.DOLocalMoveY(targetY,_duration).SetEase(curve);
            gsm.endGame(UI.Menu.Menus.Victory);
        }
    }
}

