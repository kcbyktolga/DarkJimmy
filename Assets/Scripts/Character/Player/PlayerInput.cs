// This script handles inputs for the player. It serves two main purposes: 1) wrap up
// inputs so swapping between mobile and standalone is simpler and 2) keeping inputs
// from Update() in sync with FixedUpdate()

using UnityEngine;
using DarkJimmy.Manager;

//We first ensure this script runs before all other player scripts to prevent laggy
//inputs
namespace DarkJimmy.Characters.Inputs
{
	[DefaultExecutionOrder(-100)]
	public class PlayerInput : MonoBehaviour
    {
		public bool testTouchControlsInEditor = false;  //Should touch controls be tested?
		public float verticalDPadThreshold = .5f;       //Threshold touch pad inputs
		public FloatingJoystick joystick;               //Reference to Joystick
		//public TouchButton jumpButton;                  //Reference to jump TouchButton
		//public TouchButton rollButon;                   //Reference to roll TouchButton
		//public TouchButton switchButton;                //Reference to switch TouchButton
		//public TouchButton attackButton;                //Reference to attack TouchButton
		

		[HideInInspector] public float horizontal=1;    //Float that stores horizontal input
		[HideInInspector] public bool jumpHeld;         //Bool that stores jump pressed
		[HideInInspector] public bool jumpPressed;      //Bool that stores jump held
		[HideInInspector] public bool rollPressed;      //Bool that stores roll pressed
		[HideInInspector] public bool switchPressed;    //Bool that stores switch pressed
		[HideInInspector] public bool attackPressed;    //Bool that stores switch pressed
		

		bool dPadCrouchPrev;                            //Previous values of touch Joystick
		bool readyToClear;                              //Bool used to keep input in sync


        void Update()
        {
            //Clear out existing input values
            ClearInput();

			//If the Game Manager says the game is over, exit
			//if (!GameManager.IsGameOver())
			//{
			//    //Process keyboard, mouse, gamepad (etc) inputs
			//    ProcessInputs();
			//    //Process mobile (touch) inputs
			//    ProcessTouchInputs();

			//    //Clamp the horizontal input to be between -1 and 1
			//    horizontal = Mathf.Clamp(horizontal, -1f, 1f);
			//}

			//Process keyboard, mouse, gamepad (etc) inputs
			ProcessInputs();
			//Process mobile (touch) inputs
			//ProcessTouchInputs();

			//Clamp the horizontal input to be between -1 and 1
			//horizontal = Mathf.Clamp(horizontal, -1f, 1f);
			
		}

        void FixedUpdate()
		{
			//In FixedUpdate() we set a flag that lets inputs to be cleared out during the 
			//next Update(). This ensures that all code gets to use the current inputs
			readyToClear = true;

		}

		void ClearInput()
		{
			//If we're not ready to clear input, exit
			if (!readyToClear)
				return;

			//Reset all inputs
			//horizontal = 0f;
			jumpPressed = false;
			jumpHeld = false;
			switchPressed = false;
			rollPressed = false;
			attackPressed = false;

			readyToClear = false;
		}

		void ProcessInputs()
		{ 
			//If this isn't a mobile platform AND we aren't testing in editor, exit
			if (testTouchControlsInEditor)
				return;

			//Accumulate horizontal axis input
			//horizontal = Input.GetAxis("Horizontal");
	
			//Accumulate button inputs
			jumpPressed = jumpPressed || Input.GetKeyDown(KeyCode.Mouse0);
			//jumpHeld = jumpHeld || Input.GetButton("Jump");

			//rollPressed = rollPressed || Input.GetKeyDown(KeyCode.Mouse1);
			//attackPressed = attackPressed || Input.GetButtonDown("Fire1");

			//crouchPressed = crouchPressed || Input.GetButtonDown("Crouch");
			//crouchHeld = crouchHeld || Input.GetButton("Crouch");
		}

		void ProcessTouchInputs()
		{
            //If this isn't a mobile platform AND we aren't testing in editor, exit
            if (!Application.isMobilePlatform && !testTouchControlsInEditor)
                return;

            //Record inputs from screen thumbstick
            Vector2 joystickInput = joystick.GetDirection();

			//Accumulate horizontal input
			//horizontal += joystickInput.x;
			horizontal = joystickInput.x > 0 ? 1 : joystickInput.x < 0 ? -1 : 0;


			//Accumulate jump button input
			//jumpPressed = jumpPressed || jumpButton.GetButtonDown();
			//jumpHeld = jumpHeld || jumpButton.GetButton();

			//rollPressed = rollPressed || rollButon.GetButtonDown();
			//switchPressed = switchPressed || switchButton.GetButtonDown();
			//attackPressed = attackPressed || attackButton.GetButtonDown();


			//Using thumbstick, accumulate crouch input
			//bool dPadCrouch = joystickInput.y <= -verticalDPadThreshold;
			//switchPressed = switchPressed || (dPadCrouch && !dPadCrouchPrev);
			//rollPressed = rollPressed || dPadCrouch;

			//Record whether or not playing is crouching this frame (used for determining
			//if button is pressed for first time or held
			//dPadCrouchPrev = dPadCrouch;
		}
	}
}

