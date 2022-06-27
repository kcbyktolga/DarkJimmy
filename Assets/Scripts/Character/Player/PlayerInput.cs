// This script handles inputs for the player. It serves two main purposes: 1) wrap up
// inputs so swapping between mobile and standalone is simpler and 2) keeping inputs
// from Update() in sync with FixedUpdate()

using UnityEngine;
using DarkJimmy.Manager;
using UnityEngine.EventSystems;

//We first ensure this script runs before all other player scripts to prevent laggy
//inputs
namespace DarkJimmy.Characters.Inputs
{
	[DefaultExecutionOrder(-100)]
	public class PlayerInput : MonoBehaviour
    {
		public bool testTouchControlsInEditor = false;  //Should touch controls be tested?
		public float verticalDPadThreshold = .5f;       //Threshold touch pad inputs
	

		

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

			//Process keyboard, mouse, gamepad (etc) inputs
			ProcessInputs();
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
			jumpPressed = false;
			readyToClear = false;
		}

		void ProcessInputs()
		{ 
			//If this isn't a mobile platform AND we aren't testing in editor, exit
			if (testTouchControlsInEditor)
				return;

			//Accumulate button inputs
			jumpPressed = (jumpPressed || Input.GetKeyDown(KeyCode.Mouse0)) && !IsTouchUI();
			//jumpHeld = jumpHeld || Input.GetButton("Jump");

		}

		private bool IsTouchUI()
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
return Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject( Input.GetTouch( 0 ).fingerId );
#else
			return EventSystem.current.IsPointerOverGameObject();
#endif
		}

	}
}

