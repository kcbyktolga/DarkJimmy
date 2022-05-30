using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.Characters
{
    [CreateAssetMenu(menuName ="Data/Character Data/Player Data", fileName ="Player Data")]
    public class PlayerData : CharaterData
    {
        [Header("Player Data")]

        public int attackIndex;
        public int rollForce = 5;
        public bool isWallSliding;
    }
}

