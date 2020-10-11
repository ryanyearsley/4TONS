using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace PlayerManagement {
	[RequireComponent (typeof (Player), typeof (SpellController))]
	public class PlayerInput : MonoBehaviour {

        [SerializeField]
        private int playerIndex;

		public GameObject cursorPrefab;
		public float joystickCursorDistance = 4f;

		private Player player;
		private SpellController spellController;
		private PlayerStateController stateController;
		private Rewired.Player rewiredController;
		private new Camera camera;
		private Transform cursor;
		private Transform staffPivot;//staff Transform center point;
		private Transform staffTransform;//spawnpoint for spellObject(i.e. projectiles)
        private float cursorSmoothTime = 0.1f;
		private Vector3 cursorSmoothVelocity;
		private float cursorDeadzone = 0.1f;

        private Transform cursorTransform;

		private bool usingMouseControls;

		public Vector2 CursorDirection {
			get {
				if (cursor != null)
					return ((Vector2)cursor.position - (Vector2)transform.position).normalized;
				else
					return Vector2.zero;
			}
		}
		public float CursorDistance {
			get {
				if (cursor != null)
					return ((Vector2)cursor.position - (Vector2)transform.position).magnitude;
				else
					return 0;
			}
		}
		private void Awake () {
			spellController = GetComponent<SpellController> ();
			stateController = GetComponent<PlayerStateController> ();
			player = GetComponent<Player> ();
			camera = Camera.main;
			InitializePlayer (0);
		}
		private void Start() {
			rewiredController = ReInput.players.GetPlayer (playerIndex);
		}

        private void InitializePlayer(int playerIndex) {
            this.playerIndex = playerIndex;

            if (cursorPrefab != null)
            {
                CreateCursor();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

			CreateStaffPosition ();
			stateController.SetPlayerPositions (cursor, transform, staffTransform);
		}

		private void Update () {
			Vector2 directionalInput = new Vector2 (rewiredController.GetAxisRaw ("MoveHorizontal"), rewiredController.GetAxisRaw ("MoveVertical"));
			player.SetDirectionalInput (directionalInput, CursorDirection);

			if (rewiredController.GetButtonDown ("Dash")) {
				player.OnDashInputDown ();
			}

			UpdateCursorPosition ();
			SpellInput ();
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

		private void UpdateCursorPosition () {
			Vector2 joystickInput = new Vector2 (rewiredController.GetAxisRaw ("AimHorizontal"), rewiredController.GetAxisRaw ("AimVertical"));
			Vector2 mouseDelta = new Vector2 (rewiredController.GetAxis ("MouseX"), rewiredController.GetAxis ("MouseY"));

			if (usingMouseControls) {
				if (joystickInput != Vector2.zero) {
					usingMouseControls = false;
					cursor.parent = cursorTransform;
				}
			} else {
				if (mouseDelta != Vector2.zero) {
					usingMouseControls = true;
					cursor.parent = camera.transform;
				}
			}

			if (usingMouseControls)
				MouseCursorMovement (mouseDelta);
			else
				JoystickCursorMovement (joystickInput);

		}

		private void MouseCursorMovement (Vector2 mouseDelta) {
			Bounds screenBounds = camera.OrthographicBounds ();
			float x = Mathf.Clamp (cursor.position.x + mouseDelta.x * 0.0125f, screenBounds.min.x, screenBounds.max.x);
			float y = Mathf.Clamp (cursor.position.y + mouseDelta.y * 0.0125f, screenBounds.min.y, screenBounds.max.y);
			cursor.position = new Vector3 (x, y, -9f);
			float dist = Vector2.Distance(staffPivot.position, cursor.position);
			if (dist > cursorDeadzone) {
				Vector3 dir = cursor.position - staffPivot.position;
				float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				staffPivot.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
			} else {
				if (stateController.faceDirection >= 0) {
					staffPivot.rotation = new Quaternion (0, 0, 180, 0);
				} else {
					staffPivot.rotation = new Quaternion (0, 0, 0, 0);
				}
			}

		}

		private void JoystickCursorMovement (Vector2 input) {
			if (input.sqrMagnitude > 1f)
				input = input.normalized;

			Vector3 targetPosition = input * joystickCursorDistance;
			targetPosition.z = -9f;
			cursor.localPosition = Vector3.SmoothDamp (cursor.localPosition, targetPosition, ref cursorSmoothVelocity, cursorSmoothTime);
		
		
		}

		private void CreateCursor () {
			cursor = Instantiate (cursorPrefab, transform.position + Vector3.back * 9f, Quaternion.identity, transform).transform;
			CursorController cursorController = cursor.GetComponent<CursorController> ();
			if (cursorController != null) {
                GameObject go = new GameObject("CursorPivot");
                cursorTransform = go.transform;
                cursorTransform.parent = transform;
                cursorTransform.localPosition = new Vector3(0, 0.5f, 0);
				cursorController.SetCursorCenter (cursorTransform);
			}
		}
		private void CreateStaffPosition () {
			GameObject staffPivotObj = new GameObject ("staffPivot");
			staffPivot = staffPivotObj.transform;
			staffPivot.parent = transform;
			staffPivot.localPosition = Vector3.up * 0.5f;


			GameObject staffTip = new GameObject ("staffTip");
			staffTransform = staffTip.transform;
			staffTransform.parent = staffPivot;
			staffTransform.localPosition = Vector3.up * 0.25f;
		}

	}
}