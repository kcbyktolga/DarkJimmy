using UnityEngine;
using System.Collections.Generic;
using DarkJimmy.Characters.Inputs;

namespace DarkJimmy.Characters
{
    public class CharacterAnimationController<T>: MonoBehaviour  where T: CharaterData
    {       
        CharacterMovement<T> movement;    //Reference to the PlayerMovement script component
        Rigidbody2D rigidBody;            //Reference to the Rigidbody2D component
        public PlayerInput input;                //Reference to the PlayerInput script component
        public Animator anim;                    //Reference to the Animator component

        int slideParamID;                 //ID of the isAttackking parameter
        int groundParamID;                //ID of the isOnGround parameter
        int rollParamID;                  //ID of the isRolling parameter
        int speedParamID;                 //ID of the speed parameter
        int fallParamID;                  //ID of the verticalVelocity parameter

        [HideInInspector] public
        int attackIndex;                  //ID of the attackIndex parameter                  


        public virtual void Start()
        {

            //Get the integer hashes of the parameters. This is much more efficient
            //than passing strings into the animator
            slideParamID = Animator.StringToHash("isSliding");
            groundParamID = Animator.StringToHash("isOnGround");
            rollParamID = Animator.StringToHash("isRolling");
            speedParamID = Animator.StringToHash("speed");
            fallParamID = Animator.StringToHash("verticalVelocity");
            attackIndex = Animator.StringToHash("attackIndex");

            //Grab a reference to this object's parent transform
            Transform parent = transform.parent;

            //Get references to the needed components
            movement = parent.GetComponent<CharacterMovement<T>>();
            rigidBody = parent.GetComponent<Rigidbody2D>();
            input = parent.GetComponent<PlayerInput>();
            anim = GetComponent<Animator>();

            //If any of the needed components don't exist...
            if (movement == null || rigidBody == null || input == null || anim == null)
            {
                //...log an error and then remove this component
                Debug.LogError("A needed component is missing from the player");
                Destroy(this);
            }
        }

        public virtual void Update()
        {
            //Update the Animator with the appropriate values
            anim.SetBool(groundParamID, movement.data.isOnGround);
            anim.SetFloat(fallParamID, rigidBody.velocity.y);

            //Use the absolute value of speed so that we only pass in positive numbers
            anim.SetFloat(speedParamID, Mathf.Abs(rigidBody.velocity.x));
        }

        

        //This method is called from events in the animation itself. This keeps the footstep
        //sounds in sync with the visuals
        public void StepAudio()
        {
            //Tell the Audio Manager to play a footstep sound
           // AudioManager.PlayFootstepAudio();
        }

        //This method is called from events in the animation itself. This keeps the footstep
        //sounds in sync with the visuals
        public void CrouchStepAudio()
        {
            //Tell the Audio Manager to play a crouching footstep sound
           // AudioManager.PlayCrouchFootstepAudio();
        }

        public virtual void Attack()
        {
            anim.SetTrigger(slideParamID);
        }
        public virtual void Roll()
        {
            anim.SetTrigger(rollParamID);
        }

        public virtual void SetIntAnim(int index)
        {         
            anim.SetInteger(attackIndex,index);
        }

        public int GetAttackIndex(PlayerType type)
        {
            switch (type)
            {
                case PlayerType.PlayerBow:
                    return 0;
                case PlayerType.PlayerSpear:
                    return Random.Range(0, 2);
                case PlayerType.PlayerSword:
                   return rigidBody.velocity.y != 0 && !movement.data.isOnGround ? 2 : Random.Range(0, 2);
                default:
                    return -1;

            }
        }
    }

}
