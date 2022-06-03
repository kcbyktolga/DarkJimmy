using UnityEngine;
using DarkJimmy.Characters.Inputs;
using System.Collections;

namespace DarkJimmy.Characters
{
 
    public class PlayerAnimation : MonoBehaviour
    {
        [Header("Property")]
        [SerializeField]
        private Vector2 blinkTimeRange;
        [SerializeField]
        private Vector2 blinkDuration;
        [SerializeField]
        private int blinkRate=3;
        [SerializeField]
        private GameObject headDie;

        PlayerMovement movement;    //Reference to the PlayerMovement script component
        Rigidbody2D rigidBody;            //Reference to the Rigidbody2D component
        PlayerInput input;                //Reference to the PlayerInput script component
        Animator anim;                    //Reference to the Animator component
        [SerializeField]
        private Dust impact;

        int slideParamID;                 //ID of the isAttackking parameter
        int groundParamID;                //ID of the isOnGround parameter
        int speedParamID;                 //ID of the speed parameter
        int fallParamID;                  //ID of the verticalVelocity parameter
        int dieParamID;

        float blinkTime;

        public virtual void Start()
        {

            //Get the integer hashes of the parameters. This is much more efficient
            //than passing strings into the animator
            slideParamID = Animator.StringToHash("isSliding");
            groundParamID = Animator.StringToHash("isOnGround");
            speedParamID = Animator.StringToHash("speed");
            fallParamID = Animator.StringToHash("verticalVelocity");
            dieParamID = Animator.StringToHash("isAlive");

            //Grab a reference to this object's parent transform
            Transform parent = transform.parent;

            //Get references to the needed components
            movement = parent.GetComponent<PlayerMovement>();
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
            anim.SetBool(slideParamID,movement.data.isWallSliding);
            anim.SetFloat(fallParamID, rigidBody.velocity.y);
            anim.SetBool(dieParamID, movement.data.isAlive);

            //Use the absolute value of speed so that we only pass in positive numbers
            anim.SetFloat(speedParamID, Mathf.Abs(rigidBody.velocity.x));

            if (!movement.data.isAlive && !headDie.activeSelf)
                headDie.SetActive(true);

            BlinkCheck();
       
        }

        private void BlinkCheck()
        {
            if (blinkTime > Time.time || !movement.data.isAlive)
                return;

            StartCoroutine(nameof(Blink));

        }

        IEnumerator Blink()
        {
            blinkTime = Random.Range(blinkTimeRange.x, blinkTimeRange.y)+ Time.time;

            int currentBlink = 0;
            int targetBlink = Random.Range(0 , blinkRate);

            while (currentBlink <= targetBlink)
            {
                float time = 0;
                float duration = Random.Range(blinkDuration.x, blinkDuration.y);

                while (time <= duration)
                {
                    time += Time.deltaTime;
                    headDie.SetActive(true);

                    if (!movement.data.isAlive)
                        yield break;

                    yield return null;
                }

                if (!movement.data.isAlive)
                    yield break;

                headDie.SetActive(false);
                currentBlink++;
                yield return new WaitForSeconds(0.1f);

            }
          
        } 
    }



   
}

