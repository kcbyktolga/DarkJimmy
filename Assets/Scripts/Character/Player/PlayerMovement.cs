using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkJimmy.Characters.Inputs;

namespace DarkJimmy.Characters
{	
	public class PlayerMovement : CharacterMovement<PlayerData>
    {
		[SerializeField]
		private Transform dustTransform;
		[SerializeField]
		private GameObject dust;

		PlayerInput input;

		float wallJumpTime;                     //Variable to hold wall jump duration
		float jumpTime;                         //Variable to hold jump duration
		float blockCheckTime;            
		float playerHeight;                     //Height of the player


		float originalXScale;                   //Original scale on X axis
		int direction = 1;                      //Direction player is facing

		const float smallAmount = .05f;         //A small amount used for hanging position
		public PlayerAnimation anim;
		public int horizontal = 1;

		int currentJumpAmount;
		public int jumpEnergyCount = 10;
		public int energyCount = 10;
		bool isStartGame;

		private void Start()
        {
			Initialize();         
        }

        private void Update()
        {
			if (Input.GetKeyDown(KeyCode.Space))
				data.isAlive = !data.isAlive;

			if (Input.GetKeyDown(KeyCode.A))
				isStartGame = true;


			if (!data.isAlive || !isStartGame)
				return;

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
			data.isOnGround = leftCheck || rightCheck;

			//If on the ground, the player can jump 
			if (data.isOnGround && rigidBody.velocity.y <= 0)
            {
				data.isJumping = false;
				currentJumpAmount = data.jumpAmount;
			}
				
			//Cast the ray to check above the player's head
			data.isHeadBlocked = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, data.headClearance, data.groundLayer, Color.yellow);

			//If that ray hits, the player's head is blocked
			//if (headCheck)
			//	data.isHeadBlocked = true;

			//Determine the direction of the wall grab attempt
			Vector2 grabDir = new Vector2(direction, 0f);

			//float forwardGrabDistance = jumpTime < Time.time ? data.grabDistance : data.grabDistance * data.backCheckMultiple;
			
			//Check if there is a wall in front of the player with the rays coming out of the top and bottom corners.
			RaycastHit2D forwardTop = Raycast(new Vector2(data.footOffset * direction, bodyCollider.size.y), grabDir, data.grabDistance, data.groundLayer, Color.cyan);
			RaycastHit2D forwardBottom = Raycast(new Vector2(data.footOffset * direction, 0), grabDir, data.grabDistance, data.groundLayer, Color.cyan);

			//If there is a wall reverse the direction of the player 
			if (forwardBottom || forwardTop)
				horizontal *= -1;

			//Control obstacles within the player's jump distance
			RaycastHit2D blockedCheck = Raycast(new Vector2(data.footOffset * direction, 0), grabDir, data.blockCheckDistance, data.groundLayer, Color.magenta);

			//If there is an obstacle, increase the distance of the player's back controlling rays for t time.
			if (blockedCheck && blockCheckTime < Time.time)
				blockCheckTime = data.blockedCheckDuration + Time.time;

			float backCheckDistance = blockCheckTime > Time.time || jumpTime > Time.time ? data.grabDistance * data.backCheckMultiple : data.grabDistance;

			// iswallSliding check
			RaycastHit2D backTop = Raycast(new Vector2(-data.footOffset * direction, playerHeight), -direction*Vector2.right, backCheckDistance, data.groundLayer, Color.black);
			RaycastHit2D backBottom = Raycast(new Vector2(-data.footOffset * direction, 0), -direction*Vector2.right, backCheckDistance, data.groundLayer, Color.blue);

			data.isWallSliding = !data.isOnGround && (backTop || backBottom);

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

			currentJumpAmount = data.jumpAmount;
			//UIManager.Instance.updateState(Stats.Gold, LocalSaveManager.GetData(Stats.Gold));
			//UIManager.Instance.updateState(Stats.Energy, LocalSaveManager.GetData(Stats.Energy));
			//UIManager.Instance.updateState(Stats.Mana, LocalSaveManager.GetData(Stats.Mana));
			data.isAlive = true;

		}
        public override void GroundMovement()
        {
			float xVelocity = !data.isAlive || !isStartGame ? 0: data.speed * horizontal;
	
			//If the sign of the velocity and direction don't match, flip the character
			if (xVelocity * direction < 0f)
				FlipCharacterDirection();

			//Apply the desired velocity 
			if (data.isWallSliding && wallJumpTime < Time.time)
            {
                if (input.jumpPressed )
                {				
					currentJumpAmount = data.jumpAmount;
					wallJumpTime = data.coyoteDuration + Time.time;

					//...add the jump force to the rigidbody...
					Vector2 force = new Vector2(direction*5, data.jumpForce);
					Jump(force,true);
					Dust();

				}
                else
					rigidBody.velocity = new Vector2(-direction * data.wallSlidingSpeed.x, data.wallSlidingSpeed.y);

			}				
			else
				rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);		
		}
		public override void MidAirMovement()
		{
			//If player is falling to fast, reduce the Y velocity to the max
			if (rigidBody.velocity.y < data.maxFallSpeed)
				rigidBody.velocity = new Vector2(rigidBody.velocity.x, data.maxFallSpeed);
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
			if(LocalSaveManager.Instance.mana > 0)
            {
				if (currentJumpAmount > 0)
					return true;
				return false;		
            }
			return false;
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
					//...add the jump force to the rigidbody...

					Vector2 force = new Vector2(0, data.jumpForce);
					Jump(force,true);
					Dust();

					//...and tell the Audio Manager to play the jump audio
					//AudioManager.PlayJumpAudio();
				}
				else if (data.isJumping && CanDoubleJump() && !data.isWallSliding)
				{
					currentJumpAmount--;
					LocalSaveManager.Instance.mana--;
					UIManager.Instance.updateState(Stats.Mana, LocalSaveManager.Instance.mana);
					Vector2 force = new Vector2(0, data.jumpForce * data.jumpForceMultiple);
					Jump(force,false);
				}
			}
		}
		private void Jump(Vector2 force, bool canJump)
        {
			jumpTime = data.jumpDuration + Time.time;
			data.isJumping = canJump;
			rigidBody.velocity = Vector2.zero;
			//...add the jump force to the rigidbody...
			rigidBody.AddForce(force, ForceMode2D.Impulse);

			//...and tell the Audio Manager to play the jump audio
			//AudioManager.PlayJumpAudio();
		}
		private void Dust()
        {
			dust.transform.localScale = new Vector2(-direction * dust.transform.localScale.x, dust.transform.localScale.y);
			dust.transform.position = dustTransform.position;
			dust.SetActive(true);
        }

    }
}

