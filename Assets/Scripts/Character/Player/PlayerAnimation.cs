using UnityEngine;
using DarkJimmy.Characters.Inputs;
using System.Collections;

namespace DarkJimmy.Characters
{
 
    public class PlayerAnimation : CharacterAnimation<PlayerMovement>
    {

        PlayerInput input;                //Reference to the PlayerInput script component
        BlinkAnimation blink;
       
        int slideParamID;                 //ID of the isAttackking parameter
        int fallParamID;                  //ID of the verticalVelocity parameter

        public override void Start()
        {
            base.Start();

            slideParamID = Animator.StringToHash("isSliding");
            fallParamID = Animator.StringToHash("verticalVelocity");

            input = parent.GetComponent<PlayerInput>();          
            blink = GetComponent<BlinkAnimation>();

        }

        public override void Update()
        {
            base.Update();

            animator.SetBool(slideParamID,movement.isWallSliding);
            animator.SetFloat(fallParamID, rigidBody.velocity.y);

            //if (!movement.isAlive && !blink.headDie.activeSelf)
            //    blink.headDie.SetActive(true);

           // BlinkCheck();
        }

        private void BlinkCheck()
        {
            if (!movement.isAlive && !blink.headDie.activeSelf)
                blink.headDie.SetActive(true);


            if (blink.blinkTime > Time.time || !movement.isAlive)
                return;

            blink.PlayBlink(movement.isAlive);
        }

    }



   
}

