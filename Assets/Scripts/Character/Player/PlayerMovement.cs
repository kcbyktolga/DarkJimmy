using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkJimmy.Characters.Inputs;

namespace DarkJimmy.Characters
{	
	public class PlayerMovement : CharacterMovement<PlayerData>
    {
        PlayerInput input;

		float jumpTime;                         //Variable to hold jump duration
		float coyoteTime;                       //Variable to hold coyote duration
		float blockCheckTime;
		float playerHeight;                     //Height of the player


		float originalXScale;                   //Original scale on X axis
		int direction = 1;                      //Direction player is facing

		const float smallAmount = .05f;         //A small amount used for hanging position
		public PlayerAnimation anim;
		public int horizontal = 1;

		int currentJumpAmount;
		public int jumpEnergyCount = 10;

		private void Start()
        {
			Initialize();         
        }

        private void Update()
        {
			CheckInput();
        }
        private void FixedUpdate()
        {
			//Check the environment to determine status
			PhysicsCheck();

			//Process ground and air movements
			GroundMovement();
			MidAirMovement();

		}

		public override void PhysicsCheck()
		{
			//Start by assuming the player isn't on the ground and the head isn't blocked
			data.isOnGround = false;
			data.isHeadBlocked = false;
	
			//Cast rays for the left and right foot
			RaycastHit2D leftCheck = Raycast(new Vector2(-data.footOffset, 0f), Vector2.down, data.groundDistance,data.groundLayer,Color.blue);
			RaycastHit2D rightCheck = Raycast(new Vector2(data.footOffset, 0f), Vector2.down, data.groundDistance, data.groundLayer, Color.blue);

			//If either ray hit the ground, the player is on the ground
			if (leftCheck || rightCheck)
            {
				data.isOnGround = true;
				data.isJumping = rigidBody.velocity.y < 0;
			}
				

			//Cast the ray to check above the player's head
			data.isHeadBlocked = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, data.headClearance, data.groundLayer, Color.yellow);

			//If that ray hits, the player's head is blocked
			//if (headCheck)
			//	data.isHeadBlocked = true;

			//Determine the direction of the wall grab attempt
			Vector2 grabDir = new Vector2(direction, 0f);

			//Cast three rays to look for a wall grab
			 RaycastHit2D blockedCheck= Raycast(new Vector2(data.footOffset * direction, 0), grabDir, data.blockCheckDistance, data.groundLayer, Color.magenta);

			if(blockedCheck && blockCheckTime < Time.time)
				blockCheckTime = data.blockedCheckDuration + Time.time;

			float backCheckDistance = blockCheckTime > Time.time ? data.grabDistance * data.backCheckMultiple : data.grabDistance;

			// iswallSliding check
			RaycastHit2D backTop = Raycast(new Vector2(-data.footOffset * direction, playerHeight), -direction*Vector2.right, backCheckDistance, data.groundLayer, Color.black);
			RaycastHit2D backBottom = Raycast(new Vector2(-data.footOffset * direction, 0), -direction*Vector2.right, backCheckDistance, data.groundLayer, Color.blue);

			data.isWallSliding = !data.isOnGround && (backTop || backBottom);

			//wall check
			RaycastHit2D forwardTop = Raycast(new Vector2(data.footOffset * direction, data.eyeHeight), grabDir, data.grabDistance,data.groundLayer, Color.cyan);
			RaycastHit2D forwardBottom = Raycast(new Vector2(data.footOffset * direction, data.eyeHeight), grabDir, data.grabDistance, data.groundLayer, Color.cyan);

			data.wallCheck = forwardBottom || forwardTop;

			if (data.wallCheck)
				horizontal *= -1;

		}
        public override void Initialize()
        {
			//Get a reference to the required components
			input = GetComponent<PlayerInput>();
			rigidBody = GetComponent<Rigidbody2D>();
			bodyCollider = GetComponent<BoxCollider2D>();

			//Record the original x scale of the player
			originalXScale = transform.localScale.x;

			//Record the player's height from the collider
			playerHeight = bodyCollider.size.y;
		}
        public override void GroundMovement()
        {
			
			float xVelocity = data.speed * horizontal;
	
			//If the sign of the velocity and direction don't match, flip the character
			if (xVelocity * direction < 0f)
				FlipCharacterDirection();

			//Apply the desired velocity 
			if (data.isWallSliding)
				rigidBody.velocity = new Vector2(-direction*1.5f, -2);
			else
				rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);
			

			//If the player is on the ground, extend the coyote time window
			if (data.isOnGround)
				coyoteTime = Time.time + data.coyoteDuration;
		}
		public override void MidAirMovement()
		{


			//If player is falling to fast, reduce the Y velocity to the max
			if (rigidBody.velocity.y < data.maxFallSpeed)
				rigidBody.velocity = new Vector2(rigidBody.velocity.x, data.maxFallSpeed);
		}
		private void CheckInput()
        {
			//If the jump key is pressed AND the player isn't already jumping AND EITHER
			//the player is on the ground or within the coyote time window...
			if (input.jumpPressed)
			{
				if (!data.isJumping && data.isOnGround)
				{
					//...The player is no longer on the groud and is jumping...
					data.isOnGround = false;
					data.isJumping = true;

					//...add the jump force to the rigidbody...
					rigidBody.AddForce(new Vector2(0f, data.jumpForce), ForceMode2D.Impulse);

					//...and tell the Audio Manager to play the jump audio
					//AudioManager.PlayJumpAudio();
				}
				else if (data.isJumping && CanDoubleJump())
				{
					data.isJumping = false;
					//...add the jump force to the rigidbody...
					rigidBody.AddForce(new Vector2(0f, data.jumpForce * data.jumpForceMultiple), ForceMode2D.Impulse);

					//...and tell the Audio Manager to play the jump audio
					//AudioManager.PlayJumpAudio();
				}


			}
		}

		public override void FlipCharacterDirection()
        {
			//Turn the character by flipping the direction
			direction *= -1;

			//Record the current scale
			Vector3 scale = transform.localScale;

			//Set the X scale to be the original times the direction
			scale.x = originalXScale * direction;

			//Apply the new scale
			transform.localScale = scale;
		}


		private bool CanDoubleJump()
        {
			if(jumpEnergyCount > 0)
            {
				if (currentJumpAmount < data.jumpAmount)
					return true;
				return false;		
            }
			return false;
        }

    }
}

