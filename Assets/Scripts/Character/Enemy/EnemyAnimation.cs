using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Characters
{
    public class EnemyAnimation : CharacterAnimation<EnemyMovement>
    {
        public int isAttack;
      
        public override void Start()
        {
            isAttack = Animator.StringToHash("isAttackking");
            base.Start();
        }

        public void Attack(bool isOn)
        {
            animator.SetBool(isAttack,isOn);
        }

        
    }

}

