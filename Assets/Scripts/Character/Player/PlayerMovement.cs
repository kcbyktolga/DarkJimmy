using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkJimmy.Characters.Inputs;
using Cinemachine;

namespace DarkJimmy.Characters
{
	public class PlayerMovement : CharacterMovement<PlayerData>
	{
		[SerializeField]
		private Transform dustTransform;
		[SerializeField]
		private GameObject dust;
		public PlayerAnimation anim;

		[Header("Status Flag")]
		public bool isAlive;
		public bool isJumping;
		public bool isHeadBlocked;
		public bool isWallSliding;

		private float wallJumpTime;                   
		private float jumpTime;
		private float coyoteTime;
		private float blockCheckTime;
		private float playerHeight;
		private float originalXScale;
		private int direction = 1;                     
		private int horizontal = 1;
		private int currentJumpAmount;
		private bool isPause;
		private int CurrentJumpAmount
		{
			get { return currentJumpAmount; }
			set
            {
				currentJumpAmount = value;
				system.updateStats(Stats.JumpCount, currentJumpAmount);
			}
		}
		private float Speed
        {
            get { return gsm.GetMultiple(Stats.Speed) * (0.1f * csm.GetCurrentCharacterData().Speed + data.speed); }
    //        set
    //        {
				//speed = gsm.GetMultiple(Stats.Speed) * (0.1f*csm.GetCurrentCharacterData().Speed + data.speed);
    //        }
        }

		private PlayerInput input;
		private GameSaveManager gsm;
		private SystemManager system;
		private CloudSaveManager csm;

		private void Start()
        {
			PlayerConfig();
        }

        private void Update()
        {
			if (!isAlive || !gsm.CanPlay)
				return;

			CheckInput();
        }
        private void FixedUpdate()
        {
			//Check the environment to determine status
			PhysicsCheck();

			//Process ground and air movements

			if (gsm.IsWon)
				return;

			GroundMovement();
			MidAirMovement();
		}

        public override void PlayerConfig()
        {
			system = SystemManager.Instance;
			csm = CloudSaveManager.Instance;
			gsm = GameSaveManager.Instance;
			//Get a reference to the required components
			input = GetComponent<PlayerInput>();
			rigidBody = GetComponent<Rigidbody2D>();
			bodyCollider = GetComponent<BoxCollider2D>();

			//Record the original x scale of the player
			originalXScale = transform.localScale.x;

			//Record the player's height from the collider
			playerHeight = bodyCollider.size.y*transform.localScale.y;

			//Speed = gsm.GetMultiple(Stats.Speed);
			CurrentJumpAmount = gsm.JumpCount;
			isAlive = true;
			isPause = true;

			//rigidBody.bodyType = !isPause ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;

			gsm.pause += OnPause;
			gsm.timeOut += OnDie;
		}
		public override void PhysicsCheck()
		{
			//Start by assuming the player isn't on the ground and the head isn't blocked
			isOnGround = false;
			isHeadBlocked = false;

			//Cast rays for the left and right foot
			//RaycastHit2D leftCheck = Raycast(new Vector2(-data.footOffset, 0f), Vector2.down, data.groundDistance,data.groundLayer,Color.blue);
			//RaycastHit2D rightCheck = Raycast(new Vector2(data.footOffset, 0f), Vector2.down, data.groundDistance, data.groundLayer, Color.blue);

			isOnGround = BoxCast(0.95f*Vector2.one,Vector2.down,data.groundDistance,data.groundLayer);

			////If either ray hit the ground, the player is on the ground
			//isOnGround = leftCheck || rightCheck;

			//If on the ground, the player can jump 
			if (isOnGround && rigidBody.velocity.y <= 0)
            {
				isJumping = false;
				CurrentJumpAmount = gsm.JumpCount;
			}
				
			//Cast the ray to check above the player's head
			isHeadBlocked = Raycast(new Vector2(0f, playerHeight), Vector2.up, data.headClearance, data.groundLayer, Color.yellow);

			//If that ray hits, the player's head is blocked
			//if (headCheck)
			//	data.isHeadBlocked = true;

			//Determine the direction of the wall grab attempt
			Vector2 grabDir = new Vector2(direction, 0f);

			//float forwardGrabDistance = jumpTime < Time.time ? data.grabDistance : data.grabDistance * data.backCheckMultiple;
			
			//Check if there is a wall in front of the player with the rays coming out of the top and bottom corners.
			//RaycastHit2D forwardTop = Raycast(new Vector2(data.footOffset * direction, playerHeight*0.95f), grabDir, data.grabDistance, data.groundLayer, Color.cyan);
			//RaycastHit2D forwardBottom = Raycast(new Vector2(data.footOffset * direction, 0), grabDir, data.grabDistance, data.groundLayer, Color.cyan);

			//If there is a wall reverse the direction of the player 
			//if (forwardBottom || forwardTop)
			//	horizontal *= -1;

			

			//Control obstacles within the player's jump distance
			RaycastHit2D blockedCheck = Raycast(new Vector2(data.footOffset * direction, 0), grabDir, data.blockCheckDistance, data.groundLayer, Color.magenta);

			//If there is an obstacle, increase the distance of the player's back controlling rays for t time.
			if (blockedCheck && blockCheckTime < Time.time)
				blockCheckTime = data.blockedCheckDuration + Time.time;

            float backCheckDistance = (blockCheckTime > Time.time || jumpTime > Time.time) && (!isWallSliding || !isOnGround) ? data.grabDistance * data.backCheckMultiple : data.grabDistance;

			//// iswallSliding check
			//RaycastHit2D backTop = Raycast(new Vector2(-data.footOffset * direction, playerHeight), -direction*Vector2.right, backCheckDistance, data.groundLayer, Color.black);
			//RaycastHit2D backBottom = Raycast(new Vector2(-data.footOffset * direction, 0), -direction*Vector2.right, backCheckDistance, data.groundLayer, Color.blue);

            RaycastHit2D backCheck = BoxCast(new Vector2(1f,0.975f),-grabDir, backCheckDistance, data.groundLayer);
			
			isWallSliding = !isOnGround && backCheck; //(backTop || backBottom);

			RaycastHit2D forward = BoxCast(new Vector2(0.95f,0.95f),grabDir, data.grabDistance, data.groundLayer);

			if (forward)
				horizontal *= -1;
		}
		public override void GroundMovement()
		{

			float xVelocity = gsm.IsLose || !gsm.IsStartGame ? 0 : Speed * horizontal; // !isAlive || 

			//If the sign of the velocity and direction don't match, flip the character
			if (xVelocity * direction < 0f)
				FlipCharacterDirection();

			//Apply the desired velocity 
			if (isWallSliding && wallJumpTime <= Time.time)
			{
				if (input.jumpPressed)
				{
					CurrentJumpAmount = gsm.JumpCount;

					wallJumpTime = data.wallJumpDuration + Time.time;

					//...add the jump force to the rigidbody...
					Vector2 force = new Vector2(direction * 5, data.jumpForce);
					Jump(force, true);
					Dust();
					AudioManager.Instance.PlayMusic("Wall Jump");
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
		private void OnPause()
        {
			isPause = !isPause;

			rigidBody.bodyType = !isPause ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
		}
		private void OnDie()
        {
			isAlive = false;		
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
			if(GameSaveManager.Instance.Mana > 0)
				if (CurrentJumpAmount > 0)
					return true;

			return false;
        }
		private void CheckInput()
        {
			//If the jump key is pressed and the player isn't already jumping and either the player is on the ground
			if (input.jumpPressed)
			{
				if (!isJumping && isOnGround)
				{
					//...The player is no longer on the groud and is jumping...
					isOnGround = false;
					//...add the jump force to the rigidbody...
					Vector2 force = new Vector2(0, data.jumpForce);
					Jump(force,true);
					Dust();
					AudioManager.Instance.PlaySound("Jump");
				}
				else if (isJumping && CanDoubleJump() && !isWallSliding)
				{
					CurrentJumpAmount--;										
					system.updateGMStats(Stats.Mana,-1);
					Vector2 force = new Vector2(0, data.jumpForce * data.jumpForceMultiple);
					Jump(force,CanDoubleJump());
					AudioManager.Instance.PlaySound("Air Jump");
				}
			}
		}
		private void Jump(Vector2 force, bool canJump)
        {
			jumpTime = data.jumpDuration + Time.time;
			isJumping = canJump;
			rigidBody.velocity = Vector2.zero;
			//...add the jump force to the rigidbody...
			rigidBody.AddForce(force, ForceMode2D.Impulse);
		}
		private void Dust()
        {
			dust.transform.localScale = new Vector2(-direction * dust.transform.localScale.x, dust.transform.localScale.y);
			dust.transform.position = dustTransform.position;
			dust.SetActive(true);
        }

        private void OnDestroy()
        {
			gsm.pause -= OnPause;
        }

    }
}

