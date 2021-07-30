using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace PlayerManagement {
	public enum CameraState {
		STATIC, DYNAMIC, RIGID_FOLLOW
	}
	public enum CameraZoom {
		CLOSE, MID, FAR
	}
	public class CameraController : MonoBehaviour {
		#region Singleton
		public static CameraController instance;
		void SingletonInitialization () {
			if (instance == null) {
				instance = this;
			} else if (instance != this) {
				Destroy (gameObject);
			}
		}
		#endregion

		private Transform trans;
		//Camera details
		public Vector2Int pixelPerfectReferenceResolutionClose;
		public Vector2Int pixelPerfectReferenceResolutionMid;
		public Vector2Int pixelPerfectReferenceResolutionFar;

		[SerializeField]
		private CameraState cameraState;
		[SerializeField]
		private CameraZoom cameraZoom;

		private PixelPerfectCamera pixelPerfectCamera;

		//static variables
		public Vector3 staticCameraOriginPosition;
		//dynamic variables
		public MovementComponent target;
		public Transform targetTransform;
		private PlayerAimingComponent playerAiming;
		[SerializeField]
		private FocusArea focusArea;
		public float lookAheadDistance;
		public float lookSmoothTime;
		public Vector2 focusAreaSize;

		private Vector2 currentLookAhead;
		private Vector2 smoothLookVelocity;

		public float moveSmoothTime;
		public Vector2 smoothMoveVelocity;
		private Vector3 boundPosition;
		private float boundSmooth;
		public Vector3 minValues, maxValues;

		private void Awake () {
			SingletonInitialization ();
			trans = GetComponent<Transform> ();
			pixelPerfectCamera = GetComponent<PixelPerfectCamera> ();
		}
		private void Start () {
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
			if (cameraState.Equals (CameraState.DYNAMIC)) {
				focusArea.Update (target.feetCollider.bounds);

				Vector2 focusPosition = focusArea.center;
				currentLookAhead = CalculateCursorLookAhead ();

				focusPosition += currentLookAhead;

				trans.position = PixelPerfectSmoothDamp(focusPosition).ToVector3WithZ (-10);
				
			} else if (cameraState.Equals (CameraState.RIGID_FOLLOW)) {
				trans.position = target.transform.position + Vector3.back * 10f + Vector3.up * 1f;
			}
		}

		private Vector2 PixelPerfectSmoothDamp(Vector2 desiredPosition) {
			Vector2 smoothDampedPosition = Vector2.SmoothDamp (trans.position, desiredPosition, ref smoothMoveVelocity, moveSmoothTime);

			Vector2 vectorInPixels = new Vector2(
				Mathf.RoundToInt (smoothDampedPosition.x * 32),
				Mathf.RoundToInt(smoothDampedPosition.y * 32));
			return vectorInPixels / 32;
		}
		public void SetCameraStatic (Vector3 targetPosition) {
			cameraState = CameraState.STATIC;
			if (target != null) {
				target = null;
				targetTransform = null;
				playerAiming = null;
			}
		}
		public void SetCameraRigidFollow (MovementComponent focusTarget) {
			cameraState = CameraState.RIGID_FOLLOW;
			if (focusTarget != null) {
				target = focusTarget;
				targetTransform = focusTarget.GetComponent<Transform> ();
				focusArea = new FocusArea (target.feetCollider.bounds, focusAreaSize);
				playerAiming = target.GetComponent<PlayerAimingComponent> ();
			}

			SetCameraZoom (CameraZoom.MID);
		}


		public void SetCameraDynamic (MovementComponent focusTarget) {
			cameraState = CameraState.DYNAMIC;
			if (focusTarget != null) {
				target = focusTarget;
				targetTransform = focusTarget.GetComponent<Transform> ();
				focusArea = new FocusArea (target.feetCollider.bounds, focusAreaSize);
				playerAiming = target.GetComponent<PlayerAimingComponent> ();
			}
			SetCameraZoom (CameraZoom.MID);
		}

		public void SetCameraZoom (CameraZoom zoom) {
			cameraZoom = zoom;
			switch (cameraZoom) {
				case CameraZoom.CLOSE:
					pixelPerfectCamera.refResolutionX = pixelPerfectReferenceResolutionClose.x;
					pixelPerfectCamera.refResolutionY = pixelPerfectReferenceResolutionClose.y;
					break;
				case CameraZoom.MID:
					pixelPerfectCamera.refResolutionX = pixelPerfectReferenceResolutionMid.x;
					pixelPerfectCamera.refResolutionY = pixelPerfectReferenceResolutionMid.y;

					break;
				case CameraZoom.FAR:
					pixelPerfectCamera.refResolutionX = pixelPerfectReferenceResolutionFar.x;
					pixelPerfectCamera.refResolutionY = pixelPerfectReferenceResolutionFar.y;
					break;
			}
		}

		private Vector2 CalculateCursorLookAhead () {
			if (playerAiming == null)
				return Vector2.zero;

			Vector2 direction = playerAiming.CursorDirection;
			float cursorDistance = Mathf.Clamp(playerAiming.CursorDistance, 0f, playerAiming.joystickCursorDistance);
			cursorDistance = cursorDistance.MapValue (0f, playerAiming.joystickCursorDistance, 0f, 1f);

			Vector2 targetLookAhead = direction * lookAheadDistance * cursorDistance;
			return Vector2.SmoothDamp (currentLookAhead, targetLookAhead, ref smoothLookVelocity, lookSmoothTime);
		}

		private void OnDrawGizmosSelected () {
			Gizmos.color = new Color (1f, 0f, 0f, 0.5f);
			Gizmos.DrawCube (focusArea.center, focusAreaSize);
		}

		[Serializable]
		private struct FocusArea {
			public Vector2 center;
			//public Vector2 velocity;
			[SerializeField]
			private float left, right;
			[SerializeField]
			private float top, bottom;

			public FocusArea (Bounds targetBounds, Vector2 size) {
				left = targetBounds.center.x - size.x / 2;
				right = targetBounds.center.x + size.x / 2;
				bottom = targetBounds.center.y - size.y / 2;
				top = targetBounds.center.y + size.y / 2;

				//velocity = Vector2.zero;
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
				//velocity = new Vector2 (shiftX, shiftY);
			}
		}
	}
}