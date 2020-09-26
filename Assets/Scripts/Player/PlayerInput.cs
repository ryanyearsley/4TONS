using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace PlayerManagement {
	[RequireComponent (typeof (Player))]
	public class PlayerInput : MonoBehaviour {

		public GameObject cursorPrefab;
		public float joystickCursorDistance = 4f;

		private Player player;
		private Rewired.Player controller;
		private new Camera camera;
		private Transform cursor;
        private float cursorSmoothTime = 0.1f;
		private Vector3 cursorSmoothVelocity;

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

		private void Start () {
			player = GetComponent<Player> ();

			controller = ReInput.players.GetPlayer (0);
			camera = Camera.main;

			if (cursorPrefab != null) {
				CreateCursor ();
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		private void Update () {
			Vector2 directionalInput = new Vector2 (controller.GetAxisRaw ("MoveHorizontal"), controller.GetAxisRaw ("MoveVertical"));
			player.SetDirectionalInput (directionalInput, CursorDirection);

			if (controller.GetButtonDown ("Dash")) {
				player.OnDashInputDown ();
			}

			UpdateCursorPosition ();

#if UNITY_EDITOR
			EditorImpulseForceTest ();
#endif
		}

		private void UpdateCursorPosition () {
			Vector2 joystickInput = new Vector2 (controller.GetAxisRaw ("AimHorizontal"), controller.GetAxisRaw ("AimVertical"));
			Vector2 mouseDelta = new Vector2 (controller.GetAxis ("MouseX"), controller.GetAxis ("MouseY"));

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

		private void EditorImpulseForceTest () {
			if (controller.GetButtonDown ("LeftClick")) {
				Vector3 mousePosition = camera.ScreenToWorldPoint (Input.mousePosition);
				Vector2 direction = (transform.position - mousePosition).normalized;
				player.AddImpulseForce (direction, 15f);
			} else if (controller.GetButtonDown ("RightClick")) {
				Vector3 mousePosition = camera.ScreenToWorldPoint (Input.mousePosition);
				Vector2 direction = (transform.position - mousePosition).normalized;
				player.AddImpulseForce (direction, 60f);
			}
		}
	}
}