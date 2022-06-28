using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class Starter : Menu
    {
        [SerializeField]
        private BaseButton starterButton;
        private GameSaveManager gsm;
        private void Awake()
        {
            gsm = GameSaveManager.Instance;
        }
        public override void Start()
        {
            base.Start();
            starterButton.OnClick(StartGame);
        }
        private void StartGame()
        {
            gsm.IsStartGame = true;
            gsm.StartCountDownTimer();
            gsm.pause();
            UIManager.Instance.GoBack();
        }
    }
}

