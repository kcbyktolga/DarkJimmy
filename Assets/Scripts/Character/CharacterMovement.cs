using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Characters
{
   
    public abstract class CharacterMovement<T> : MonoBehaviour where T: CharaterData
    {
        #region Fields
        [Header("Data")]
        public T data;
        [Header("Components")]
        [HideInInspector]
        public BoxCollider2D bodyCollider;             //The collider component
        [HideInInspector]
        public Rigidbody2D rigidBody;                  //The rigidbody component

        #endregion

        public abstract void Initialize();
        public abstract void PhysicsCheck();
        public abstract void GroundMovement();
        public abstract void MidAirMovement();
        public abstract void FlipCharacterDirection();

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

        public RaycastHit2D[] Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask, Color color,int segment)
        {
            //Record the player's position
            Vector2 pos = transform.position;

            //Send out the desired raycasr and record the result
            RaycastHit2D[] hit = Physics2D.RaycastAll(pos + offset, rayDirection, length, mask);

            return hit;
        }
    }
}

