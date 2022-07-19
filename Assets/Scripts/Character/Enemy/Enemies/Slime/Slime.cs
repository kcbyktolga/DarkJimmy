using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Characters
{
    public class Slime : EnemyMovement
    {
        [SerializeField]
        private SlimeType slimeType;

        private bool isAttack = false;
        public override float Speed { get { return slimeType == SlimeType.Dynamic ? Data.speed : 0; } }

        public override void Start()
        {
           base.Start();

            if(slimeType==SlimeType.Static)
                InvokeRepeating(nameof(AttackMode), 0, 4);
        }

        private void AttackMode()
        {
            if (!isAlive)
                return;

            isAttack = !isAttack;
            canTakeDamage = !isAttack;
            
            enemyAnimation.Attack(isAttack);
        }

        public override void PhysicsCheck()
        {
            base.PhysicsCheck();

            if (isAttack && giveDamageTime < Time.time)
            {
                RaycastHit2D topEnemyCheck = BoxCast(0.95f * Vector2.one, Vector2.up, Data.groundDistance*1.5f, Data.enemyLayer);

                if (topEnemyCheck)
                    GiveDamage(topEnemyCheck, Data.damageForce, Data.damageUnit * 10);
            }          
        }

        //public override void TakeDamage(Transform hitTransform, Vector2 damageDir, int damage)
        //{
        //  //  bodyCollider.enabled = false;
        //  //  isAlive = false;

        //  //Invoke(nameof(DisableEnemy), 2);
        //}

        public enum SlimeType
        {
            Static,
            Dynamic
        }

    }

}
