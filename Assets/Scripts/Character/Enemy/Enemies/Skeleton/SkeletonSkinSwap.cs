using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace DarkJimmy.Characters
{
    public class SkeletonSkinSwap : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField]
        private SpriteResolver Head;
        [SerializeField]
        private SpriteResolver HeadDie;
        [SerializeField]
        private SpriteResolver LArm;
        [SerializeField]
        SpriteRenderer Quiver;

        public void SetSkin(SkeletonType type)
        {
            Head.SetCategoryAndLabel("Head", $"Head-{GetHeadIndex(type)}");
            LArm.SetCategoryAndLabel("L-Arn", $"L-Arm-{(int)type}");
            HeadDie.SetCategoryAndLabel("HeadDie", $"HeadDie-{GetHeadIndex(type)}");
            Quiver.enabled = type.Equals(SkeletonType.Archer);
        }   
        private int GetHeadIndex(SkeletonType type)
        {
            return type switch
            {
                SkeletonType.Archer => 1,
                SkeletonType.Wizard => 2,
                _ => 0,
            };
        }

	}
}


