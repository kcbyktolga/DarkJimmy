using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Characters
{
    [CreateAssetMenu(menuName ="Data/Character Data/Player Data", fileName ="Player Data")]
    public class PlayerData : CharaterData
    {
        [Header("Player Data")]    
        public bool blockedCheck;

        public int jumpAmount;
        public float jumpForceMultiple = 0.9f;
        public float backCheckMultiple = 1.5f;
        public float blockCheckDistance = 1f;
        public float jumpDuration = 1f;
        public float wallJumpDuration = 0.1f;
        public Vector2 wallSlidingSpeed;


    }
}

