using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class Starter : Menu
    {
        [SerializeField]
        private BaseButton starterButton;


        public override void Start()
        {
            base.Start();
            starterButton.OnClick(StartGame);
        }

        private void StartGame()
        {
            GameSaveManager.Instance.IsStartGame = true;
            GameSaveManager.Instance.StartCountDownTimer();

            UIManager.Instance.GoBack();
        }
    }
}

