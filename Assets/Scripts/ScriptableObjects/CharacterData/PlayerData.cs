using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Characters
{
    [CreateAssetMenu(menuName ="Data/Character Data/Player Data", fileName ="Player Data")]
    public class PlayerData : CharaterData
    {
        [Header("Player Data")]
        public bool isWallSliding;
        public bool blockedCheck;

        public int jumpAmount;
        public float jumpForceMultiple = 0.9f;
    }
}

