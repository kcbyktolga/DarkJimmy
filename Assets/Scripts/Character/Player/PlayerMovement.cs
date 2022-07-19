using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkJimmy.Characters.Inputs;
using Cinemachine;
using DG.Tweening;

namespace DarkJimmy.Characters
{
	public class PlayerMovement : CharacterMovement
	{
		[Header("Player Properties")]
		[SerializeField]
		private Material playerMaterial;
        [SerializeField]
        private Transform dustTransform;
        [SerializeField]
		private Transform impactTransform;
		[SerializeField]
		private GameObject dust;
		public PlayerAnimation anim;

		[Header("Status Flag")]
		public bool isJumping;
		public bool isWallSliding;

		private float wallJumpTime;                   
		private float jumpTime;

		private float blockCheckTime;

		private int currentJumpAmount;	
		private int CurrentJumpAmount
		{
			get { return currentJumpAmount; }
			set
            {
				currentJumpAmount = value;
				system.updateStats(Stats.JumpCount, currentJumpAmount);
			}
		}
		public override float Speed
        {
            get { return gsm.GetMultiple(Stats.Speed) * (0.1f * csm.GetCurrentCharacterData().GetCurrentCharacterProperty(CharacterProperty.Speed) + Data.speed); }
        }

		private PlayerInput input;
		private CloudSaveManager csm;
		private PlayerData Data;
		
		public override void Start()
        {
			base.Start();
            Data = (PlayerData)data;
			gsm.Player = this;
        }
        private void Update()
        {
			if (!isAlive || !gsm.CanPlay)
				return;

			CheckInput();
        }
        private void  FixedUpdate()
        {
			//Check the environment to determine status
			PhysicsCheck();

			//Process ground and air movements
			GroundMovement();
			MidAirMovement();
		}
        public override void CharacterConfig()
        {
			base.CharacterConfig();	

			csm = CloudSaveManager.Instance;
			input = GetComponent<PlayerInput>();
			CurrentJumpAmount = gsm.JumpCount;

			gsm.timeOut += OnDie;
		}
		public override void PhysicsCheck()
		{

			base.PhysicsCheck();

			if (!gsm.CanPlay)
				return;

			//Start by assuming the player isn't on the ground and the head isn't blocked
			//If on the ground, the player can jump 
			if (isOnGround && rigidBody.velocity.y <= 0)
            {
				isJumping = false;
				CurrentJumpAmount = gsm.JumpCount;
			}

            //Cast rays for the left and right foot
            RaycastHit2D chekcObstacle = BoxCast(0.95f * Vector2.one, Vector2.down, Data.groundDistance, Data.obstacleLayer);

            if (chekcObstacle && takeDamageTime <= Time.time)
            {
				Vector2 force = new Vector2(5, 20);
				TakeDamage(chekcObstacle.point,force,-10);

				if (gsm.HP <= 0)
					return;

				isJumping = true;
				takeDamageTime = Time.time;
				isAlive = true;

              
			}
                

            //Determine the direction of the wall grab attempt
            Vector2 grabDir = new Vector2(direction, 0f);

            //Control obstacles within the player's jump distance
            RaycastHit2D blockedCheck = Raycast(new Vector2(Data.footOffset * direction, 0), grabDir, Data.blockCheckDistance, Data.groundLayer, Color.magenta);

			//If there is an obstacle, increase the distance of the player's back controlling rays for t time.
			if (blockedCheck && blockCheckTime < Time.time)
                blockCheckTime = Data.blockedCheckDuration + Time.time;

            float backCheckDistance = (blockCheckTime > Time.time || jumpTime > Time.time) && (!isWallSliding || !isOnGround) ? Data.grabDistance * Data.backCheckMultiple : Data.grabDistance;

            RaycastHit2D backCheck = BoxCast(new Vector2(1f,0.975f),-grabDir, backCheckDistance, Data.groundLayer);
			
			isWallSliding = !isOnGround && backCheck; //(backTop || backBottom);

            RaycastHit2D forward = BoxCast(new Vector2(0.95f,0.95f), grabDir, Data.grabDistance, Data.groundLayer);

			if (forward)
				horizontal *= -1;

            RaycastHit2D crushCheck = BoxCast(0.95f * Vector2.one, Vector2.down, Data.groundDistance, Data.enemyLayer);

            if (crushCheck)
				GiveDamage(crushCheck, Data.damageForce,Data.damageUnit);

		}
		public override void GroundMovement()
		{
				base.GroundMovement();

				//Apply the desired velocity 
				if (isWallSliding && wallJumpTime <= Time.time && gsm.CanPlay)
				{
					if (input.jumpPressed)
					{
						CurrentJumpAmount = gsm.JumpCount;

						wallJumpTime = Data.wallJumpDuration + Time.time;

						//...add the jump force to the rigidbody...
						Vector2 force = new Vector2(direction * 5, Data.jumpForce);
						Jump(force, true);
						//AudioManager.Instance.PlayMusic("Wall Jump");
					}
					else
						rigidBody.velocity = new Vector2(-direction * Data.wallSlidingSpeed.x, Data.wallSlidingSpeed.y);
				}
				else
					if (takeDamageTime < Time.time)
					rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);
		}
		public override void MidAirMovement()
		{
			//If player is falling to fast, reduce the Y velocity to the max
			if (rigidBody.velocity.y < Data.maxFallSpeed)
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, Data.maxFallSpeed);
		}
		private void OnDie()
        {
			isAlive = false;
			gsm.IsDefeat = true;
			//gsm.EndGameMenus = UI.Menu.Menus.Defeat;
			//AdManager.Instance.ShowInterstitial();
			gsm.endGame(UI.Menu.Menus.Defeat);
		}
  
		public override void FlipCharacterDirection()
        {
			base.FlipCharacterDirection();
		}
        public override void TakeDamage(Vector2 hitPosition, Vector2 damageForce, int damageUnit)
        {
			Vector2 newDamageForce = isOnGround ? new Vector2(damageForce.x, 0) : damageForce;
			base.TakeDamage(hitPosition, newDamageForce, damageUnit);
			Dust(hitPosition);
			system.updateGMStats(Stats.HP, damageUnit);
			system.cameraShake();
			ChangeColor(Color.red);

            if (gsm.HP <= 0)
            {
				OnDie();
				return;
            }

			//	takeDamageTime = data.takeDamageduration + Time.time;
			isAlive = false;
			Invoke(nameof(Alive), Data.takeDamageDuration);
			//AudioManager.Instance.PlaySound("Take Damage");
		}

		public void ChangeColor(Color color)
		{
			playerMaterial.shader = Shader.Find("GUI/Text Shader");
			playerMaterial.DOColor(color, 0.1f).SetLoops(5, LoopType.Yoyo).OnComplete(() => playerMaterial.DOColor(Color.white, 0.05f).OnComplete(() => playerMaterial.shader = Shader.Find("Sprites/Default")));
		}

		public override void GiveDamage(RaycastHit2D hit, Vector2 damageDir, int damage)
        {
			base.GiveDamage(hit, damageDir, damage);
			Vector2 force = new Vector2(0, Data.jumpForce * 0.5f);
			Jump(force, true);
		}
        private void Alive()
        {
			if (gsm.IsDefeat)
				return;

			isAlive = true;
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
                    Vector2 force = new Vector2(0, Data.jumpForce);
					Jump(force,true);		
					
				}
				else if (isJumping && CanDoubleJump() && !isWallSliding)
				{
					CurrentJumpAmount--;										
					system.updateGMStats(Stats.Mana,-1);
                    Vector2 force = new Vector2(0, Data.jumpForce * Data.jumpForceMultiple);
					Jump(force,CanDoubleJump());
					//AudioManager.Instance.PlaySound("Air Jump");
				}
			}
		}
		private void Jump(Vector2 force, bool canJump)
        {
			if(canJump)
				Dust(dustTransform.position);

			AudioManager.Instance.PlaySound("Jump");
			jumpTime = Data.jumpDuration + Time.time;
			isJumping = canJump;
			rigidBody.velocity = Vector2.zero;
			//...add the jump force to the rigidbody...
			rigidBody.AddForce(force, ForceMode2D.Impulse);
		}
		private void Dust(Vector2 dustPos)
        {
			dust.transform.localScale = new Vector2(-direction * dust.transform.localScale.x, dust.transform.localScale.y);
			dust.transform.position = dustPos;
			dust.SetActive(true);
        }


        public override void OnDestroy()
        {
			base.OnDestroy();

			gsm.timeOut -= OnDie;
		}
    }
}

