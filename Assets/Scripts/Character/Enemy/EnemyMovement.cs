using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DarkJimmy.Characters
{
    public class EnemyMovement : CharacterMovement
    {      
        public override float Speed { get { return Data.speed;} }
        public EnemyData Data;
        public EnemyAnimation enemyAnimation;
        public float defaultGravityScale;
        public Vector2 defaultPosition;

        public override void Start()
        {
            base.Start();
            Data = (EnemyData)data;
        }
        public virtual void FixedUpdate()
        {
   
            PhysicsCheck();
            GroundMovement();
        }
        public override void FlipCharacterDirection()
        {
            base.FlipCharacterDirection();
        }
        public override void GroundMovement()
        {
            base.GroundMovement();

            float velocity = giveDamageTime > Time.time ? 0 : xVelocity;

            rigidBody.velocity = new Vector2(velocity, rigidBody.velocity.y);
        }
        public override void CharacterConfig()
        {
            base.CharacterConfig();

            defaultGravityScale = rigidBody.gravityScale;
            defaultPosition = transform.position;
        }
        public override void MidAirMovement()
        {
           
        }
        public override void PhysicsCheck()
        {

            base.PhysicsCheck();

            if (!gsm.CanPlay || !isAlive)
                return;

            Vector2 grabDir = new Vector2(direction, 0f);

            RaycastHit2D wallCheck = Raycast(new Vector2(Data.footOffset * direction, Data.eyeHeight), grabDir, Data.grabDistance, Data.groundLayer, Color.magenta);
            RaycastHit2D gabCheck= Raycast(new Vector2((Data.grabDistance + Data.footOffset)* direction, 0), Vector2.down, Data.grabDistance, Data.groundLayer, Color.green);

            if (wallCheck || !gabCheck)
                horizontal *= -1;

            RaycastHit2D forwardEnemyCheck = BoxCast(new Vector2(0.95f, 0.60f), grabDir, Data.grabDistance, Data.enemyLayer);
            RaycastHit2D backEnemyCheck = BoxCast(new Vector2(0.95f, 0.60f), -grabDir, Data.grabDistance, Data.enemyLayer);

            if (forwardEnemyCheck || backEnemyCheck)
            {
                RaycastHit2D hit = forwardEnemyCheck ? forwardEnemyCheck : backEnemyCheck;
                GiveDamage(hit,Data.damageForce,Data.damageUnit);
            }
        }
        public override void TakeDamage(Vector2 hitPosition, Vector2 damageDir, int damage)
        {
            rigidBody.gravityScale = 0;
            bodyCollider.enabled = false;
            isAlive = false;
            Invoke(nameof(DisableEnemy), 2);
        }

        public virtual void DisableEnemy()
        {
            gameObject.SetActive(false);
            transform.position = defaultPosition;
            rigidBody.gravityScale = defaultGravityScale;
            bodyCollider.enabled = true;
            isAlive = true;
            system.addGameElement(gameObject);
        }
    }
}

