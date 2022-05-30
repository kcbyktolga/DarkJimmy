using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DarkJimmy.Characters
{
    public enum PlayerType
    {
        Player,
        PlayerBow,
        PlayerSpear,
        PlayerSword
    }

    public class PlayerAnimation : CharacterAnimationController<PlayerData>
    {
 
        public delegate void ChangePlayer(PlayerType newValue);
        public ChangePlayer changePlayer;

        public PlayerType playerType;
        int playerIndex = 3;
 
  
        public override void Start()
        {
            base.Start();

            SetAnimation((int)playerType);
            changePlayer += OnChangedPlayer;
        }

        public override void Update()
        {
            base.Update();

            if (input.switchPressed)
            {
                playerIndex++;

                if (playerIndex > 3)
                    playerIndex = 0;
              
            }

        }
        private void OnChangedPlayer(PlayerType playerType)
        {
            SetAnimation((int)playerType);
        }

        public override void Attack()
        {
            SetIntAnim(GetAttackIndex(playerType));    
            base.Attack();
        }

    }



   
}

