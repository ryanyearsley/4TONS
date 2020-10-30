using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace PlayerManagement {

	public enum CameraState {
		STATIC, DYNAMIC
	}
	public enum CameraZoom {
		CLOSE, MID, FAR
	}
	public class CameraController : MonoBehaviour {

		[SerializeField]
		private CameraState cameraState;
		[SerializeField]
		private CameraZoom cameraZoom;

		private PixelPerfectCamera pixelPerfectCamera;

		//static variables
		public Vector3 staticCameraOriginPosition;
		//dynamic variables
		public MovementController target;
		private PlayerInputController playerInput;
		private FocusArea focusArea;
		public float lookAheadDistance;
		public float lookSmoothTime;
		public Vector2 focusAreaSize;

		private Vector2 currentLookAhead;
		private Vector2 smoothLookVelocity;

		private void Start () {
			pixelPerfectCamera = GetComponent<PixelPerfectCamera> ();
			switch (cameraState) {
				case CameraState.STATIC:
					SetCameraStatic (staticCameraOriginPosition);
					break;
				case CameraState.DYNAMIC:
					if (target != null) {
						SetCameraDynamic (target);
					}
					break;
			}

			SetCameraZoom (cameraZoom);
		}

		private void LateUpdate () {
			if (cameraState.Equals(CameraState.DYNAMIC)) {
				focusArea.Update (target.collider.bounds);

				Vector2 focusPosition = focusArea.center;
				CalculateCursorLookAhead ();

				focusPosition += currentLookAhead;
				transform.position = (Vector3)focusPosition + Vector3.forward * -10f;
			}
		}
		public void SetCameraStatic (Vector3 targetPosition) {
			cameraState = CameraState.STATIC;
			if (target != null) {
				target = null;
				playerInput = null;
			}
		}


		public void SetCameraDynamic(MovementController focusTarget) {
			cameraState = CameraState.DYNAMIC;
			if (focusTarget != null) {
				target = focusTarget;
				focusArea = new FocusArea (target.collider.bounds, focusAreaSize);
				playerInput = target.GetComponent<PlayerInputController> ();
			}
		}

		public void SetCameraZoom(CameraZoom zoom) {
			cameraZoom = zoom;
			switch (cameraZoom) {
				case CameraZoom.CLOSE:
					pixelPerfectCamera.refResolutionX = ConstantsManager.instance.pixelPerfectReferenceResolutionClose.x;
					pixelPerfectCamera.refResolutionY = ConstantsManager.instance.pixelPerfectReferenceResolutionClose.y;
					break;
				case CameraZoom.MID:
					pixelPerfectCamera.refResolutionX = ConstantsManager.instance.pixelPerfectReferenceResolutionMid.x;
					pixelPerfectCamera.refResolutionY = ConstantsManager.instance.pixelPerfectReferenceResolutionMid.y;

					break;
				case CameraZoom.FAR:
					pixelPerfectCamera.refResolutionX = ConstantsManager.instance.pixelPerfectReferenceResolutionFar.x;
					pixelPerfectCamera.refResolutionY = ConstantsManager.instance.pixelPerfectReferenceResolutionFar.y;
					break;
			}
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