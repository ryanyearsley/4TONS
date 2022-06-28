using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public enum CameraState {
	STATIC, DYNAMIC, RIGID_FOLLOW
}
public enum CameraZoom {
	CLOSE, MID, FAR
}
public class CameraController2D : MonoBehaviour {
	#region Singleton
	public static CameraController2D instance;
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
	public Transform targetTransform;
	private PlayerAimingComponent playerAiming;
	public float lookAheadDistance;
	public float lookSmoothTime;

	private Vector2 currentLookAhead;
	private Vector2 smoothLookVelocity;
	public Vector3 rigidFollowTargetOffset = new Vector3(0, 0f, -10);
	public float rigidFollowSmoothTime;
	private Vector3 rigidFollowLookVelocity;

	public float moveSmoothTime;
	public Vector2 smoothMoveVelocity;
	public Vector3 minValues, maxValues;

	private void Awake () {
		SingletonInitialization ();
		trans = GetComponent<Transform> ();
		pixelPerfectCamera = GetComponent<PixelPerfectCamera> ();
	}
	private void Start () {
		SetCameraZoom (cameraZoom);
	}

	void LateUpdate () {
		if (cameraState.Equals (CameraState.DYNAMIC)) {
			Vector2 focusPosition = targetTransform.position;
			CalculateCursorLookAhead ();

			focusPosition += currentLookAhead;

			trans.position = focusPosition.ToVector3WithZ (-10);

		} else if (cameraState.Equals (CameraState.RIGID_FOLLOW)) {
			trans.position = Vector3.SmoothDamp (trans.position, targetTransform.position + rigidFollowTargetOffset, ref rigidFollowLookVelocity, rigidFollowSmoothTime);
			//trans.position = targetTransform.position + Vector3.back * 10f + Vector3.up * 1f;
		}
	}


	public void SetCameraStatic (Vector3 targetPosition) {
		cameraState = CameraState.STATIC;
		if (targetTransform != null) {
			targetTransform = null;
			playerAiming = null;
		}
	}

	public void SetCameraRigidFollow (Transform targetTransform) {
		cameraState = CameraState.RIGID_FOLLOW;
		if (targetTransform != null) {
			this.targetTransform = targetTransform;
			playerAiming = null;
		}
		SetCameraZoom (CameraZoom.MID);
	}


	public void SetCameraDynamic (Transform targetTransform) {
		cameraState = CameraState.DYNAMIC;
		if (targetTransform != null) {
			currentLookAhead = Vector2.zero;
			this.targetTransform = targetTransform;
			CalculateCursorLookAhead ();
			playerAiming = targetTransform.parent.GetComponent<PlayerAimingComponent> ();
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

	private void CalculateCursorLookAhead () {
		if (playerAiming == null) {
			currentLookAhead = Vector2.zero;
			return;
		}

		Vector2 direction = playerAiming.CursorDirection;
		float cursorDistance = Mathf.Clamp(playerAiming.CursorDistance, 0f, playerAiming.joystickCursorDistance);
		cursorDistance = cursorDistance.MapValue (0f, playerAiming.joystickCursorDistance, 0f, 1f);

		Vector2 targetLookAhead = direction * lookAheadDistance * cursorDistance;
		currentLookAhead = Vector2.SmoothDamp (currentLookAhead, targetLookAhead, ref smoothLookVelocity, lookSmoothTime);
	}

}