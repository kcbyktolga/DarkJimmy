using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DarkJimmy.Characters
{
    public class CharaterData : ScriptableObject
    {
		public bool drawDebugRaycasts = true;   //Should the environment checks be visualized

		[Header("Movement Properties")]
		public float speed = 8f;                //character speed
		public float crouchSpeedDivisor = 3f;   //Speed reduction when crouching
		public float coyoteDuration = .05f;     //How long the character can jump after falling
		public float maxFallSpeed = -25f;       //Max speed character can fall
		public float attackRate = 0.5f;         //How long the character can attack after moving

		[Header("Jump Properties")]
		public float jumpForce = 6.3f;          //Initial force of jump
		public float crouchJumpBoost = 2.5f;    //Jump boost when crouching
		public float hangingJumpForce = 15f;    //Force of wall hanging jumo
		public float jumpHoldForce = 1.9f;      //Incremental force when jump is held
		public float jumpHoldDuration = .1f;    //How long the jump key can be held

		[Header("Environment Check Properties")]
		public float footOffset = .4f;          //X Offset of feet raycast
		public float eyeHeight = 1.5f;          //Height of wall checks
		public float reachOffset = .7f;         //X offset for wall grabbing
		public float headClearance = .5f;       //Space needed above the character's head
		public float groundDistance = .2f;      //Distance character is considered to be on the ground
		public float grabDistance = .4f;        //The reach distance for wall grabs
		public LayerMask groundLayer;           //Layer of the ground

		[Header("Status Flags")]
		public bool isOnGround;                 //Is the character on the ground?
		public bool isJumping;                  //Is character jumping?
		public bool isHeadBlocked;              //Is character blocked?
		public bool wallCheck;                  //Is character hitting wall?;
	}
}

