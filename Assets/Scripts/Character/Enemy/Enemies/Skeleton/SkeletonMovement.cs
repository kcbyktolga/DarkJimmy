using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Characters
{
    public class SkeletonMovement : EnemyMovement
    {
        [Header("Skeleton")]
        [SerializeField]
        private SkeletonType skeletonType;
        [SerializeField]
        private SkeletonSkinSwap skin;

        public override void CharacterConfig()
        {
            base.CharacterConfig();
            skin.SetSkin(skeletonType);
        }

        public override void TakeDamage(Vector2 hitPosition, Vector2 damageDir, int damage)
        {
             base.TakeDamage(hitPosition, damageDir, damage);
        }
    }

    public enum SkeletonType
    {
        Normal,
        Sword,
        Speard,
        Wizard,
        Archer
    }
}

