using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AimingMode {
	RADIAL, CURSOR
}

public class PlayerAimingComponent : PlayerComponent {


	[SerializeField]
	private float mouseSensitivity = 0.0125f;
	[SerializeField]
	private float combatJoystickSensitivity = 0.15f;
	[SerializeField]
	private float puzzleJoystickSensitivity = 0.08f;

	private float currentJoystickSensitivity = 0.15f;

	[SerializeField]
	private GameObject cursorPrefab;
	[SerializeField]
	private GameObject staffPrefab;

	private CursorController cursorController;
	private StaffAimObject staffAimObject;
	private Transform aimingPivotTransform;
	private Transform cursorPivotTransform;
	private Transform cursorTransform;


	private float cursorSmoothTime = 0.1f;
	private Vector3 cursorSmoothVelocity;
	private float cursorDeadzone = 0.1f;
	public float joystickCursorDistance = 4f;
	private Camera mainCamera;

	#region Initialization

	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject); 
		mainCamera = Camera.main;
		CreateCursor ();
		CreateStaffAimObject ();
		playerObject.SetCreaturePositions (cursorTransform, transform, aimingPivotTransform, staffAimObject.staffTipTransform);
	}
	//TODO: Destroy cursor/staff after death.
	public override void ReusePlayerComponent (Player player) {
		base.ReusePlayerComponent (player);
		cursorController.InitializeComponent (player);
		if (player.wizardSaveData.primaryStaffSaveData.puzzleData != null) {
			staffAimObject.InitializeStaffAimObject (player.wizardSaveData.primaryStaffSaveData.puzzleData.puzzleSprite);
		}
		CameraController2D.instance.SetCameraDynamic (creatureObject.creaturePositions.centerTransform);
	}
	public override void OnSpawn (Vector3 spawnPosition) {
		playerObject.SetSmartCursor (false);
		currentJoystickSensitivity = combatJoystickSensitivity;
		playerObject.SetAimingMode (AimingMode.CURSOR);
	}

	private void CreateCursor () {
		GameObject aimingPivotGo = new GameObject("AimingPivot");
		aimingPivotTransform = aimingPivotGo.transform;
		aimingPivotTransform.parent = transform.GetComponentInChildren<AnimationComponent>().transform.parent;
		aimingPivotTransform.localPosition = new Vector3 (0, 0.325f, 0);


		GameObject cursorGo = new GameObject("CursorPivot");
		cursorPivotTransform = cursorGo.transform;
		cursorPivotTransform.parent = transform;
		cursorPivotTransform.localPosition = aimingPivotTransform.localPosition;

		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
		cursorTransform = Instantiate (cursorPrefab, transform.position + Vector3.back * 9f, Quaternion.identity, transform).transform;

		cursorController = cursorTransform.GetComponent<CursorController> ();
		cursorController.SetCursorCenter (cursorPivotTransform);
		cursorTransform.parent = mainCamera.transform;
		cursorTransform.localPosition = Vector3.zero;
		//if using mouse, parent is always main cam.
		//if using controller, camera hops between maincamera and player as parent.
	}

	private Transform CreateStaffAimObject () {
		GameObject staffObject = Instantiate(staffPrefab);
		staffObject.transform.parent = aimingPivotTransform;
		staffAimObject = staffObject.GetComponent<StaffAimObject> ();
		staffObject.transform.localPosition = Vector3.zero;
		return staffAimObject.staffTipTransform;
	}
	#endregion
	#region Public Accessors
	public override void OnSetAimingMode (AimingMode aimingMode) {
		if (!playerObject.usingMouseControls) {
			switch (aimingMode) {
				case (AimingMode.CURSOR): {
						cursorTransform.parent = mainCamera.transform;
						break;
					}
				case (AimingMode.RADIAL): {
						cursorTransform.parent = cursorPivotTransform;
						break;
					}
			}
		}
	}
	public void ToggleSmartCursorModeButtonDown () {
		if (!playerObject.usingMouseControls) {
			playerObject.SetSmartCursor (!playerObject.smartCursor);
			if (!playerObject.smartCursor) {
				playerObject.SetAimingMode (AimingMode.CURSOR);
			} else if (playerObject.currentPlayerState == PlayerState.COMBAT) {
				playerObject.SetAimingMode (AimingMode.RADIAL);
			}

		}
	}
	public Vector2 CursorDirection {
		get {
			if (cursorTransform != null)
				return ((Vector2)cursorTransform.position - (Vector2)aimingPivotTransform.position).normalized;
			else
				return Vector2.zero;
		}
	}
	public float CursorDistance {
		get {
			if (cursorTransform != null)
				return ((Vector2)cursorTransform.position - (Vector2)aimingPivotTransform.position).magnitude;
			else
				return 0;
		}
	}

	#endregion
	#region Player Events


	public override void OnChangePlayerState (PlayerState playerState) {
		cursorController.OnChangeState (playerState);
		switch (playerState) {
			case (PlayerState.COMBAT): {
					CameraController2D.instance.SetCameraDynamic (creatureObject.creaturePositions.centerTransform);
					staffAimObject.gameObject.SetActive (true);
					if (!playerObject.usingMouseControls) {
						currentJoystickSensitivity = combatJoystickSensitivity;
						if (playerObject.smartCursor) {
							playerObject.SetAimingMode (AimingMode.RADIAL);
						} else {
							playerObject.SetAimingMode (AimingMode.CURSOR);
						}
					}
					break;
				}
			case (PlayerState.PUZZLE_BROWSING):
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					CameraController2D.instance.SetCameraRigidFollow (playerObject.creaturePositions.centerTransform);
					staffAimObject.gameObject.SetActive (false);
					if (!playerObject.usingMouseControls) {
						currentJoystickSensitivity = puzzleJoystickSensitivity;
						playerObject.SetAimingMode (AimingMode.CURSOR);
					}
					break;
				}
		}
	}

	public override void OnEquipStaff (PuzzleKey region, PuzzleGameData puzzleGameData, StaffEquipType equipType) {
		staffAimObject.OnEquipStaff (puzzleGameData);
	}
	public override void OnDropStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		staffAimObject.OnDropStaff ();
	}

	#endregion

	public void MouseAimingUpdate (Vector2 mouseDelta) {
		Bounds screenBounds = mainCamera.OrthographicBounds ();
		float x = Mathf.Clamp (cursorTransform.position.x + mouseDelta.x * mouseSensitivity * Time.deltaTime, screenBounds.min.x, screenBounds.max.x);
		float y = Mathf.Clamp (cursorTransform.position.y + mouseDelta.y * mouseSensitivity * Time.deltaTime, screenBounds.min.y, screenBounds.max.y);
		cursorTransform.position = new Vector3 (x, y, -9f);
		float angle = Mathf.Atan2 (CursorDirection.y, CursorDirection.x) * Mathf.Rad2Deg;


		aimingPivotTransform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		cursorTransform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);
	}


	public void JoystickAimingUpdate (Vector2 joystickInput) {
		if (playerObject.currentAimingMode == AimingMode.CURSOR) {
			Bounds screenBounds = mainCamera.OrthographicBounds ();
			float x = Mathf.Clamp (cursorTransform.position.x + joystickInput.x * currentJoystickSensitivity, screenBounds.min.x, screenBounds.max.x);
			float y = Mathf.Clamp (cursorTransform.position.y + joystickInput.y * currentJoystickSensitivity, screenBounds.min.y, screenBounds.max.y);
			cursorTransform.position = new Vector3 (x, y, -9f);

			float angle = Mathf.Atan2 (CursorDirection.y, CursorDirection.x) * Mathf.Rad2Deg;
			aimingPivotTransform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
			cursorTransform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);

		} else if (playerObject.currentAimingMode == AimingMode.RADIAL) {
			if (joystickInput.sqrMagnitude > 1f)
				joystickInput = joystickInput.normalized;
			Vector3 targetPosition = joystickInput * joystickCursorDistance * new Vector3(1, 0.65f, -9f);
			targetPosition.z = -9f;
			float angle = Mathf.Atan2(CursorDirection.y, CursorDirection.x) * Mathf.Rad2Deg;
			cursorTransform.localPosition = Vector3.SmoothDamp (cursorTransform.localPosition, targetPosition, ref cursorSmoothVelocity, cursorSmoothTime);
			cursorTransform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);

			if (joystickInput != Vector2.zero) {
				aimingPivotTransform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
			}
		}
	}
	private void AimStaffWithDeadzone () {
		float dist = CursorDistance;
		if (dist > cursorDeadzone) {
			float angle = Mathf.Atan2(CursorDirection.y, CursorDirection.x) * Mathf.Rad2Deg;
			aimingPivotTransform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		} else {
			if (playerObject.faceDirection >= 0) {
				aimingPivotTransform.rotation = new Quaternion (0, 0, 180, 0);
			} else {
				aimingPivotTransform.rotation = new Quaternion (0, 0, 0, 0);
			}
		}
	}
}
