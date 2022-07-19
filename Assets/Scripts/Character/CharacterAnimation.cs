using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Characters
{
    public class CharacterAnimation<T> : MonoBehaviour where T: CharacterMovement
    {
        public T movement;       
        public Rigidbody2D rigidBody;                                 
        public Animator animator;

              
        public int groundParamID;                
        public int speedParamID;                                  
        public int dieParamID;

        public Transform parent;

        [SerializeField]
        private Vector2 blinkTimeRange;
        [SerializeField]
        private Vector2 blinkDuration;
        [SerializeField]
        private int blinkRate = 3;

        public GameObject headDie;
        public float blinkTime;
        public virtual void Start()
        {
            groundParamID = Animator.StringToHash("isOnGround");
            speedParamID = Animator.StringToHash("speed");
            dieParamID = Animator.StringToHash("isAlive");

            parent = transform.parent;

            //Get references to the needed components
            movement = parent.GetComponent<T>();
            rigidBody = parent.GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        public virtual void Update()
        {
            animator.SetBool(groundParamID, movement.isOnGround);
            animator.SetBool(dieParamID, movement.isAlive);
            animator.SetFloat(speedParamID, Mathf.Abs(rigidBody.velocity.x));

            BlinkCheck();
        }
        private void BlinkCheck()
        {
            if (headDie == null)
                return;

            if (!movement.isAlive && !headDie.activeSelf)
                headDie.SetActive(true);


            if (blinkTime > Time.time || !movement.isAlive)
                return;

           PlayBlink(movement.isAlive);
        }
        public void PlayBlink(bool isAlive)
        {
            StartCoroutine(Blink(isAlive));
        }

        IEnumerator Blink(bool isAlive)
        {
            blinkTime = Random.Range(blinkTimeRange.x, blinkTimeRange.y) + Time.time;

            int currentBlink = 0;
            int targetBlink = Random.Range(0, blinkRate);

            while (currentBlink <= targetBlink)
            {
                float time = 0;
                float duration = Random.Range(blinkDuration.x, blinkDuration.y);

                while (time <= duration)
                {
                    time += Time.deltaTime;
                    headDie.SetActive(true);

                    if (!isAlive)
                        yield break;

                    yield return null;
                }

                if (!isAlive)
                    yield break;

                headDie.SetActive(false);
                currentBlink++;
                yield return new WaitForSeconds(0.1f);

            }

        }
    }
}

