using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace PlayerManagement {
	public class PlayerInputComponent : PlayerComponent {

		[SerializeField]
		private int controllerIndex;
		private Rewired.Player rewiredController;

		[SerializeField]
		private Vector2 joystickInput;
		[SerializeField]
		private Vector2 mouseDelta;

		private PlayerMovementComponent playerMovementComponent;
		private PlayerAimingComponent playerAimingController;
		private PlayerSpellComponent playerSpellController;
		private PlayerPuzzleComponent playerPuzzleController;
		private PlayerInteractComponent playerInteractComponent;

		private bool canReadActionInput;
		public override void SetUpComponent (GameObject rootObject) {
			base.SetUpComponent (rootObject);
			playerAimingController = GetComponent<PlayerAimingComponent> ();
			playerObject = GetComponent<PlayerObject> ();
			playerMovementComponent = GetComponent<PlayerMovementComponent> ();
			playerSpellController = GetComponentInChildren<PlayerSpellComponent> ();
			playerPuzzleController = GetComponentInChildren<PlayerPuzzleComponent> ();
			playerInteractComponent = GetComponent<PlayerInteractComponent> ();
		
		}
		public override void ReusePlayerComponent (Player player) {
			base.ReusePlayerComponent (player);
			this.controllerIndex = player.controllerIndex;
			rewiredController = ReInput.players.GetPlayer (controllerIndex);
		}

		private void Update () {
			if (!playerObject.isDead && !GameManager.instance.isPaused) {
				AimingInput ();
				PuzzleInput ();
				SpellInput ();
				InteractInput ();
			}
			PauseInput ();
			if (rewiredController.GetButtonDown ("Dash")) {
				playerMovementComponent.OnDashInputDown ();
			}
		}

		private void FixedUpdate () {
			Vector2 directionalInput = new Vector2 (rewiredController.GetAxisRaw ("MoveHorizontal"), rewiredController.GetAxisRaw ("MoveVertical"));
			playerMovementComponent.UpdateMovementInput (directionalInput, playerAimingController.CursorDirection);
		}
		private void AimingInput () {
			
			joystickInput = new Vector2 (rewiredController.GetAxisRaw ("AimHorizontal"), rewiredController.GetAxisRaw ("AimVertical"));
			mouseDelta = new Vector2 (rewiredController.GetAxis ("AimHorizontalMouse"), rewiredController.GetAxis ("AimVerticalMouse"));
			if (playerObject.usingMouseControls) {
				if (joystickInput != Vector2.zero) {
					playerObject.SetUsingMouseControls (false);
					playerObject.playerUI.UpdateBindings (ControllerType.Joystick);
				}
			} else {
				if (mouseDelta != Vector2.zero) {
					playerObject.SetUsingMouseControls (true);
					playerObject.playerUI.UpdateBindings (ControllerType.Mouse);
				}
			}

			if (rewiredController.GetButtonDown ("ToggleAimingMode")) {
				playerAimingController.ToggleSmartCursorModeButtonDown ();
			}
			if (playerObject.usingMouseControls)
				playerAimingController.MouseAimingUpdate (mouseDelta);
			else
				playerAimingController.JoystickAimingUpdate (joystickInput);

		}
		private void SpellInput () {
			for (int spellIndex = 0; spellIndex <= 3; spellIndex++) {
				string buttonName = "Spell" + spellIndex;
				if (rewiredController.GetButton (buttonName))
					playerSpellController.OnSpellButton (spellIndex);
				if (rewiredController.GetButtonDown (buttonName)) {
					playerSpellController.OnSpellButtonDown (spellIndex);
				}
				if (rewiredController.GetButtonUp (buttonName)) {
					playerSpellController.OnSpellButtonUp (spellIndex);
				}
			}
		}
		private void PuzzleInput () {
			playerPuzzleController.PuzzleUpdate (playerObject.currentPlayerState);

			if (rewiredController.GetButtonDown ("AutoBindItem")) {
				playerPuzzleController.OnBindButtonDown (playerObject.currentPlayerState);
			}

			if (playerObject.currentPlayerState == PlayerState.PUZZLE_BROWSING || playerObject.currentPlayerState == PlayerState.PUZZLE_MOVING_SPELLGEM) {
				for (int spellIndex = 0; spellIndex <= 3; spellIndex++) {
					string buttonName = "Spell" + spellIndex;
					if (rewiredController.GetButtonDown (buttonName)) {
						playerPuzzleController.OnSpellBindingButtonDown (playerObject.currentPlayerState, spellIndex);
					}
				}
			}
			if (rewiredController.GetButtonDown ("TogglePuzzle")) {
				Debug.Log ("toggle puzzle button down.");
				playerPuzzleController.OnTogglePuzzleMenuButtonDown (playerObject.currentPlayerState);
			}
			if (rewiredController.GetButtonShortPressDown ("DropItem")) {
				playerPuzzleController.OnDropButtonDown (playerObject.currentPlayerState);
			}
			if (rewiredController.GetButtonDown ("GrabItem")) {
				playerPuzzleController.OnGrabButtonDown (playerObject.currentPlayerState);
			}
			if (rewiredController.GetButtonDown ("RotateItemCCW")) {
				playerPuzzleController.OnRotateSpellGemCCWButtonDown (playerObject.currentPlayerState);
			}
			if (rewiredController.GetButtonDown ("RotateItemCW")) {
				Debug.Log ("PlayerInputComponent: Rotating gem clockwise button pressed");
				playerPuzzleController.OnRotateSpellGemCWButtonDown (playerObject.currentPlayerState);
			}
			if (rewiredController.GetButtonDown ("SwitchToPrimaryStaff")) {
				playerPuzzleController.OnSwitchToPrimaryStaffButtonDown ();
			}
			if (rewiredController.GetButtonDown ("SwitchToSecondaryStaff")) {
				playerPuzzleController.OnSwitchToSecondaryStaffButtonDown ();
			}
			if (rewiredController.GetButtonDown ("SwitchToAlternateStaff")) {
				playerPuzzleController.OnSwitchToSecondaryStaffButtonDown ();
			}
			ReInput.mapping.GetControllerMap (0);
		}

		private void InteractInput () {

			if (rewiredController.GetButtonDown ("GrabItem")) {
				playerInteractComponent.OnGrabButtonDown ();
			}
		}

		private void PauseInput () {
			if (rewiredController.GetButtonDown ("Pause")) {
				GameManager.instance.OnPause ();
			}
		}
	}
}