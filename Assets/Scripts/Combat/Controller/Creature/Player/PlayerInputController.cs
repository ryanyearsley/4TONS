using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace PlayerManagement {
	[RequireComponent (typeof (PlayerMovementController), typeof (PlayerAiming),
		typeof (SpellController))]
	public class PlayerInputController : MonoBehaviour {

        [SerializeField]
        private int controllerIndex;
		private Rewired.Player rewiredController;

		[SerializeField]
		private Vector2 joystickInput;
		[SerializeField]
		private Vector2 mouseDelta;

		private PlayerMovementController playerMovementController;
		private PlayerAiming playerAiming;
		private SpellController spellController;
		private PlayerStateController stateController;


		private bool usingMouseControls;

		private void Awake () {
			playerAiming = GetComponent<PlayerAiming> ();
			spellController = GetComponent<SpellController> ();
			stateController = GetComponent<PlayerStateController> ();
			playerMovementController = GetComponent<PlayerMovementController> ();
		}
		private void Start () {

			InitializePlayerComponent (0);
		}

		public void InitializePlayerComponent (int controllerIndex) {
			this.controllerIndex = controllerIndex;
			rewiredController = ReInput.players.GetPlayer (controllerIndex);
		}
		public void InitializePlayerComponent(Player player) {
            this.controllerIndex = player.controllerIndex;
			rewiredController = ReInput.players.GetPlayer (controllerIndex);
		}
		private void Update () {
			
			MovementInput ();
			AimingInput ();
			SpellInput ();
		}

		private void MovementInput () {
			Vector2 directionalInput = new Vector2 (rewiredController.GetAxisRaw ("MoveHorizontal"), rewiredController.GetAxisRaw ("MoveVertical"));
			
			if (rewiredController.GetButtonDown ("Dash")) {
				playerMovementController.OnDashInputDown ();
			}
			playerMovementController.UpdateMovementInput (directionalInput, playerAiming.CursorDirection);

		}
		private void AimingInput () {
			joystickInput = new Vector2 (rewiredController.GetAxisRaw ("AimHorizontal"), rewiredController.GetAxisRaw ("AimVertical"));
			mouseDelta = new Vector2 (rewiredController.GetAxis ("MouseX"), rewiredController.GetAxis ("MouseY"));
			if (usingMouseControls) {
				if (joystickInput != Vector2.zero) {
					usingMouseControls = false;
				}
			} else {
				if (mouseDelta != Vector2.zero) {
					usingMouseControls = true;
				}
			}
			if (usingMouseControls)
				playerAiming.MouseAimingUpdate (mouseDelta);
			else
				playerAiming.JoystickAimingUpdate (joystickInput);

		}
		private void SpellInput () {
			for (int spellIndex = 0; spellIndex < 4; spellIndex++) {
				string buttonName = "Spell" + spellIndex;
				if (rewiredController.GetButton (buttonName))
					spellController.OnSpellButton (spellIndex);

				if (rewiredController.GetButtonDown (buttonName)) {
					Debug.Log ($"Get button down {spellIndex}");
					spellController.OnSpellButtonDown (spellIndex);
				}
				if (rewiredController.GetButtonUp (buttonName))
					spellController.OnSpellButtonUp (spellIndex);
			}
		}
		private void PuzzleInput() {
			if (rewiredController.GetButtonDown("TogglePuzzle")) {

			}
			Vector2 spellMovementDir = new Vector2();
			if (rewiredController.GetButtonDown("UIUp")) {
				spellMovementDir += Vector2.up;
			}
			if (rewiredController.GetButtonDown ("UIDown")) {
				spellMovementDir += Vector2.down;
			}
			if (rewiredController.GetButtonDown ("UILeft")) {
				spellMovementDir += Vector2.left;
			}
			if (rewiredController.GetButtonDown ("UIRight")) {
				spellMovementDir += Vector2.right;
			}
			
		}
	}
}