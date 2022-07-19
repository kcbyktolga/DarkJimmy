using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DarkJimmy.Characters
{
   
    public abstract class CharacterMovement : MonoBehaviour 
    {
        #region Fields
        [Header("Data")]
        public CharaterData data;
        [Header("Components")]
        [HideInInspector]
        public BoxCollider2D bodyCollider;             
        [HideInInspector]
        public Rigidbody2D rigidBody;

        [Header("References")]
        public GameSaveManager gsm;
        public SystemManager system;

        [Header("Status Flag")]
        public bool isOnGround;
        public bool isHeadBlocked;
        public bool isAlive;
        public bool isPause;
        public bool canTakeDamage = true;
        

        [Header("Properties")]
        public int direction = 1;
        public int horizontal = 1;

        public Vector2 damageForce;

        public float playerHeight;
        public float originalXScale;
        public float xVelocity;

        public virtual float Speed { get; set; }

        [Header("Times")]
        public float takeDamageTime;
        public float giveDamageTime;
        #endregion

        public virtual void Start()
        {
            CharacterConfig();
        }

     
        public virtual void CharacterConfig() 
        {
            system = SystemManager.Instance;
            gsm = GameSaveManager.Instance;

            rigidBody = GetComponent<Rigidbody2D>();
            bodyCollider = GetComponent<BoxCollider2D>();

            //Record the original x scale of the player
            originalXScale = transform.localScale.x;

            //Record the player's height from the collider
            playerHeight = bodyCollider.size.y * transform.localScale.y;

            isAlive = true;
            isPause = true;
            
            gsm.pause += OnPause;

        }
        public virtual void OnPause()
        {
            isPause = !isPause;
            rigidBody.bodyType = !isPause ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
        }
        public virtual void PhysicsCheck()
        {
            //Start by assuming the player isn't on the ground and the head isn't blocked
            isOnGround = false;

            //Cast rays for the left and right foot
            isOnGround = BoxCast(0.95f * Vector2.one, Vector2.down, data.groundDistance, data.groundLayer);
            //Cast the ray to check above the player's head
            isHeadBlocked = Raycast(new Vector2(0f, playerHeight), Vector2.up, data.headClearance, data.groundLayer, Color.yellow);
        }
        public virtual void GroundMovement()
        {
              xVelocity = (gsm.IsDefeat || !gsm.IsStartGame || !isAlive)? 0 : Speed * horizontal; // !isAlive || 
           // xVelocity = (!gsm.CanPlay || !isAlive) ? 0 : Speed * horizontal;

            //If the sign of the velocity and direction don't match, flip the character
            if (xVelocity * direction < 0f)
                FlipCharacterDirection();
        }
        public abstract void MidAirMovement();
        public virtual void FlipCharacterDirection() 
        {
            //Turn the character by flipping the direction
            direction *= -1;

            //Record the current scale
            Vector3 scale = transform.localScale;

            //Set the X scale to be the original times the direction
            scale.x = originalXScale * direction;

            //Apply the new scale
            transform.localScale = scale;
        }
        public virtual void TakeDamage(Vector2 hitPosition,Vector2 damageForce,int damageUnit) 
        {
            Vector2 newDamageForce = new Vector2(-direction * damageForce.x, damageForce.y);            
            rigidBody.velocity = Vector2.zero;
            rigidBody.AddForce(newDamageForce, ForceMode2D.Impulse);
        }
        public virtual void GiveDamage(RaycastHit2D hit,Vector2 damageForce, int damageUnit)
        {
            if (giveDamageTime > Time.time)
                return;

            
            var character = hit.collider.GetComponent<CharacterMovement>();

            if (character != null && character.canTakeDamage)
            {
                giveDamageTime = data.giveDamageDuration + Time.time;
                character.takeDamageTime = character.data.takeDamageDuration + Time.time;
                character.TakeDamage(hit.point, damageForce, -damageUnit);
            }
        }
        public virtual void Move(float horizontal)
        {
            //Calculate the desired velocity based on inputs
            float xVelocity = data.speed *horizontal;

            //Apply the desired velocity 
            rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);
        }
        public RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask, Color color)
        {
            //Record the player's position
            Vector2 pos = transform.position;

            //Send out the desired raycasr and record the result
            RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

            //If we want to show debug raycasts in the scene...
            if (data.drawDebugRaycasts)
            {
                //...determine the color based on if the raycast hit...
                Color _color = hit ? Color.green : color;
                //...and draw the ray in the scene view
                Debug.DrawRay(pos + offset, rayDirection * length, _color);
            }

            //Return the results of the raycast
            return hit;
        }
        public RaycastHit2D BoxCast(Vector2 multiple,Vector2 rayDirection,float length,LayerMask mask)
        {
            Vector2 center = bodyCollider.bounds.center;
            Vector2 size = new Vector2(bodyCollider.bounds.size.x*multiple.x, bodyCollider.bounds.size.y*multiple.y);

            //Record the player's position

            //Send out the desired raycasr and record the result

            RaycastHit2D hit = Physics2D.BoxCast(center, size, 0, rayDirection, length, mask);
            //Return the results of the raycast

            return hit;
        }


        public virtual void OnDestroy()
        {
            gsm.pause -= OnPause;
        }

    }
}

