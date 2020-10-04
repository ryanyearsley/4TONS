using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManagement {
	public class CameraController : MonoBehaviour {

		public MovementController target;
		public float lookAheadDistance;
		public float lookSmoothTime;
		public Vector2 focusAreaSize;

		private PlayerInput playerInput;
		private FocusArea focusArea;

		private Vector2 currentLookAhead;
		private Vector2 smoothLookVelocity;

		private void Start () {
			focusArea = new FocusArea (target.collider.bounds, focusAreaSize);
			if (target != null)
				playerInput = target.GetComponent<PlayerInput> ();
		}

		private void LateUpdate () {
			focusArea.Update (target.collider.bounds);

			Vector2 focusPosition = focusArea.center;
			CalculateCursorLookAhead ();

			focusPosition += currentLookAhead;
			transform.position = (Vector3)focusPosition + Vector3.forward * -10f;
		}

		private void CalculateCursorLookAhead () {
			if (playerInput == null)
				return;

			Vector2 direction = playerInput.CursorDirection;
			float cursorDistance = Mathf.Clamp(playerInput.CursorDistance, 0f, playerInput.joystickCursorDistance);
			cursorDistance = cursorDistance.MapValue (0f, playerInput.joystickCursorDistance, 0f, 1f);

			Vector2 targetLookAhead = direction * lookAheadDistance * cursorDistance;
			currentLookAhead = Vector2.SmoothDamp (currentLookAhead, targetLookAhead, ref smoothLookVelocity, lookSmoothTime);
		}

		private void OnDrawGizmos () {
			Gizmos.color = new Color (1f, 0f, 0f, 0.5f);
			Gizmos.DrawCube (focusArea.center, focusAreaSize);
		}

		private struct FocusArea {
			public Vector2 center;
			public Vector2 velocity;
			private float left, right;
			private float top, bottom;

			public FocusArea (Bounds targetBounds, Vector2 size) {
				left = targetBounds.center.x - size.x / 2;
				right = targetBounds.center.x + size.x / 2;
				bottom = targetBounds.center.y - size.y / 2;
				top = targetBounds.center.y + size.y / 2;

				velocity = Vector2.zero;
				center = new Vector2 ((left + right) / 2, (top + bottom) / 2);
			}

			public void Update (Bounds targetBounds) {
				float shiftX = 0;
				if (targetBounds.min.x < left) {
					shiftX = targetBounds.min.x - left;
				} else if (targetBounds.max.x > right) {
					shiftX = targetBounds.max.x - right;
				}
				left += shiftX;
				right += shiftX;

				float shiftY = 0;
				if (targetBounds.min.y < bottom) {
					shiftY = targetBounds.min.y - bottom;
				} else if (targetBounds.max.y > top) {
					shiftY = targetBounds.max.y - top;
				}
				bottom += shiftY;
				top += shiftY;
				center = new Vector2 ((left + right) / 2, (top + bottom) / 2);
				velocity = new Vector2 (shiftX, shiftY);
			}
		}
	}
}