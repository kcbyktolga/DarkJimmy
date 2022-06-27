using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Objects
{
    public class WinZone : Interactable
    {
       // private Animator animator;
        private GameSaveManager gsm;

        private int winParamId;
        private void Start()
        {
            gsm = GameSaveManager.Instance;
            //animator = GetComponent<Animator>();
            //winParamId = Animator.StringToHash("Win");

            activate += Win;
        }
        private void Win()
        {
            if (gsm.CountDown <= 0)
                return;
            gsm.IsWon = true;

            AudioManager.Instance.PlaySound("Logo Intro");
            AudioManager.Instance.StopSource(SoundGroupType.Music);
           // animator.SetTrigger(winParamId);
            Invoke(nameof(OpenVictoryDisplay), 1.5f);
        }
    
        private void OpenVictoryDisplay()
        {
            UIManager.Instance.OpenMenu(UI.Menu.Menus.Victory);
        }

     
    }
}

