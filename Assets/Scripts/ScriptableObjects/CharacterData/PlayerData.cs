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
        [Header("Forces")]
        public Vector2 wallSlidingSpeed;

        [Header("Distances")]
        public float blockCheckDistance = 1f;

        [Header("Multiples")]
        public float jumpForceMultiple = 0.9f;
        public float backCheckMultiple = 1.5f;

        [Header("Durations")]
        public float jumpDuration = 1f;
        public float wallJumpDuration = 0.1f;
        [Header("Layer Masks")]
        public LayerMask obstacleLayer;
      
    }
}

