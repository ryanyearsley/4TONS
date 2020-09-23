using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManagement {
	public class CameraController : MonoBehaviour {

		public MovementController target;
		public float lookAheadDistance;
		public float lookSmoothTime;
		public Vector2 focusAreaSize;

		private FocusArea focusArea;

		private float currentLookAheadX;
		private float currentLookAheadY;
		private float targetLookAheadX;
		private float targetLookAheadY;
		private float lookAheadDirectionX;
		private float lookAheadDirectionY;
		private float smoothLookVelocityX;
		private float smoothLookVelocityY;

		private bool lookAheadXStopped;
		private bool lookAheadYStopped;

		private void Start () {
			focusArea = new FocusArea (target.collider.bounds, focusAreaSize);
		}

		private void LateUpdate () {
			focusArea.Update (target.collider.bounds);

			Vector2 focusPosition = focusArea.center;
			CalculateXLookAhead ();
			CalculateYLookAhead ();

			focusPosition += Vector2.right * currentLookAheadX;
			focusPosition += Vector2.up * currentLookAheadY;
			transform.position = (Vector3)focusPosition + Vector3.forward * -10f;
		}

		private void CalculateXLookAhead () {
			if (focusArea.velocity.x != 0) {
				lookAheadDirectionX = Mathf.Sign (focusArea.velocity.x);
				if (Mathf.Sign (target.playerInput.x) == Mathf.Sign (focusArea.velocity.x) && target.playerInput.x != 0) {
					lookAheadXStopped = false;
					targetLookAheadX = lookAheadDirectionX * lookAheadDistance;
				} else {
					if (!lookAheadXStopped) {
						lookAheadXStopped = true;
						targetLookAheadX = currentLookAheadX + (lookAheadDirectionX * lookAheadDistance - currentLookAheadX) / 4f;
					}
				}
			}

			targetLookAheadX = lookAheadDirectionX * lookAheadDistance;
			currentLookAheadX = Mathf.SmoothDamp (
				currentLookAheadX,
				targetLookAheadX,
				ref smoothLookVelocityX,
				lookSmoothTime);
		}

		private void CalculateYLookAhead () {
			if (focusArea.velocity.y != 0) {
				lookAheadDirectionY = Mathf.Sign (focusArea.velocity.y);
				if (Mathf.Sign (target.playerInput.y) == Mathf.Sign (focusArea.velocity.y) && target.playerInput.y != 0) {
					lookAheadYStopped = false;
					targetLookAheadY = lookAheadDirectionY * lookAheadDistance;
				} else {
					if (!lookAheadYStopped) {
						lookAheadYStopped = true;
						targetLookAheadY = currentLookAheadY + (lookAheadDirectionY * lookAheadDistance - currentLookAheadY) / 4f;
					}
				}
			}

			targetLookAheadY = lookAheadDirectionY * lookAheadDistance;
			currentLookAheadY = Mathf.SmoothDamp (
				currentLookAheadY,
				targetLookAheadY,
				ref smoothLookVelocityY,
				lookSmoothTime);
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