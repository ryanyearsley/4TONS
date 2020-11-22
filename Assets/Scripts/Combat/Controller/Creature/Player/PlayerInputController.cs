using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace PlayerManagement {
	[RequireComponent (typeof (PlayerMovementController), typeof (PlayerAimingController))]
	public class PlayerInputController : MonoBehaviour {

		[SerializeField]
		private int controllerIndex;
		private Rewired.Player rewiredController;

		[SerializeField]
		private Vector2 joystickInput;
		[SerializeField]
		private Vector2 mouseDelta;

		private PlayerMovementController playerMovementController;
		private PlayerAimingController playerAimingController;
		private PlayerStateController playerStateController;
		private PlayerSpellController playerSpellController;
		private PlayerPuzzleController playerPuzzleController;


		private bool usingMouseControls;

		private void Awake () {
			playerAimingController = GetComponent<PlayerAimingController> ();
			playerStateController = GetComponent<PlayerStateController> ();
			playerMovementController = GetComponent<PlayerMovementController> ();
			playerSpellController = GetComponentInChildren<PlayerSpellController> ();
			playerPuzzleController = GetComponentInChildren<PlayerPuzzleController> ();
		}
		private void Start () {

			InitializePlayerComponent (0);
		}

		public void InitializePlayerComponent (int controllerIndex) {
			this.controllerIndex = controllerIndex;
			rewiredController = ReInput.players.GetPlayer (controllerIndex);
		}
		public void InitializeComponent (Player player) {
			this.controllerIndex = player.controllerIndex;
			rewiredController = ReInput.players.GetPlayer (controllerIndex);
		}
		private void Update () {
			AimingInput ();
			MovementInput ();
			PuzzleInput ();
			SpellInput ();
		}

		private void MovementInput () {
			Vector2 directionalInput = new Vector2 (rewiredController.GetAxisRaw ("MoveHorizontal"), rewiredController.GetAxisRaw ("MoveVertical"));

			if (rewiredController.GetButtonDown ("Dash")) {
				Debug.Log ("dash input detected");
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
					Debug.Log ($"Get button down {spellIndex}");
					playerSpellController.OnSpellButtonDown (spellIndex);
					playerPuzzleController.OnSpellBindingButtonDown (playerStateController.currentPlayerState, spellIndex);
				}
				if (rewiredController.GetButtonUp (buttonName))
					playerSpellController.OnSpellButtonUp (spellIndex);
			}

		}
		private void PuzzleInput () {
			playerPuzzleController.PuzzleUpdate (playerStateController.currentPlayerState);

			if (rewiredController.GetButtonDown ("TogglePuzzle")) {
				playerPuzzleController.OnTogglePuzzleMenuButtonDown (playerStateController.currentPlayerState);
			}
			if (rewiredController.GetButtonDown ("PickUpSpellGem")) {
				playerPuzzleController.OnPickUpSpellGemButtonDown (playerStateController.currentPlayerState);
			}
			if (rewiredController.GetButtonDown ("DropSpellGem")) {
				playerPuzzleController.OnDropSpellGemButtonDown (playerStateController.currentPlayerState);
			}
			if (rewiredController.GetButtonDown ("GrabSpellGem")) {
				playerPuzzleController.OnGrabSpellGemButtonDown (playerStateController.currentPlayerState);
			}
			if (rewiredController.GetButtonDown ("RotateSpellGem")) {
				playerPuzzleController.OnRotateSpellGemButtonDown (playerStateController.currentPlayerState);
			}
		}
	}
}