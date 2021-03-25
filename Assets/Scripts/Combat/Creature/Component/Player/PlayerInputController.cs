using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace PlayerManagement {
	public class PlayerInputController : PlayerComponent {

		[SerializeField]
		private int controllerIndex;
		private Rewired.Player rewiredController;

		[SerializeField]
		private Vector2 joystickInput;
		[SerializeField]
		private Vector2 mouseDelta;

		private PlayerMovementComponent playerMovementController;
		private PlayerAimingComponent playerAimingController;
		private PlayerSpellComponent playerSpellController;
		private PlayerPuzzleComponent playerPuzzleController;
		private bool usingMouseControls;

		private bool canReadActionInput;
		public override void SetUpComponent (GameObject rootObject) {
			base.SetUpComponent (rootObject);
			playerAimingController = GetComponent<PlayerAimingComponent> ();
			playerObject = GetComponent<PlayerObject> ();
			playerMovementController = GetComponent<PlayerMovementComponent> ();
			playerSpellController = GetComponentInChildren<PlayerSpellComponent> ();
			playerPuzzleController = GetComponentInChildren<PlayerPuzzleComponent> ();
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
			}
			PauseInput ();
		}

		private void FixedUpdate () {
			MovementInput ();
		}

		private void MovementInput () {
			Vector2 directionalInput = new Vector2 (rewiredController.GetAxisRaw ("MoveHorizontal"), rewiredController.GetAxisRaw ("MoveVertical"));

			if (rewiredController.GetButtonDown ("Dash")) {
				playerMovementController.OnDashInputDown ();
			}
			playerMovementController.UpdateMovementInput (directionalInput, playerAimingController.CursorDirection);

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
				playerAimingController.MouseAimingUpdate (mouseDelta);
			else
				playerAimingController.JoystickAimingUpdate (joystickInput);

		}
		private void SpellInput () {
			for (int spellIndex = 0; spellIndex < 4; spellIndex++) {
				string buttonName = "Spell" + spellIndex;
				if (rewiredController.GetButton (buttonName))
					playerSpellController.OnSpellButton (spellIndex);

				if (rewiredController.GetButtonDown (buttonName)) {
					playerSpellController.OnSpellButtonDown (spellIndex);
					playerPuzzleController.OnSpellBindingButtonDown (playerObject.currentPlayerState, spellIndex);
				}
				if (rewiredController.GetButtonUp (buttonName))
					playerSpellController.OnSpellButtonUp (spellIndex);
			}

		}
		private void PuzzleInput () {
			playerPuzzleController.PuzzleUpdate (playerObject.currentPlayerState);

			if (rewiredController.GetButtonDown ("TogglePuzzle")) {
				Debug.Log ("toggle puzzle button down.");
				playerPuzzleController.OnTogglePuzzleMenuButtonDown (playerObject.currentPlayerState);
			}
			if (rewiredController.GetButtonDown ("DropSpellGem")) {
				playerPuzzleController.OnDropSpellGemButtonDown (playerObject.currentPlayerState);
			}
			if (rewiredController.GetButtonDown ("GrabSpellGem")) {
				playerPuzzleController.OnGrabSpellGemButtonDown (playerObject.currentPlayerState);
			}
			if (rewiredController.GetButtonDown ("RotateSpellGem")) {
				playerPuzzleController.OnRotateSpellGemButtonDown (playerObject.currentPlayerState);
			}
		}
		private void PauseInput () {
			if (rewiredController.GetButtonDown ("Pause")) {
				GameManager.instance.OnPause ();
			}
		}
	}
}