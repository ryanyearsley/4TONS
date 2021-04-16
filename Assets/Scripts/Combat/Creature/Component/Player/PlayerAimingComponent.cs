using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingComponent : PlayerComponent {

	[SerializeField]
	private GameObject cursorPrefab;
	[SerializeField]
	private GameObject staffPrefab;

	private CursorController cursorController;
	private StaffAimObject staffAimObject;
	private Transform staffTipTransform;
	private Transform aimingPivotTransform;
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
	}
	//TODO: Destroy cursor/staff after death.
	public override void ReusePlayerComponent (Player player) {
		base.ReusePlayerComponent (player);
		Transform targetTransform = CreateCursor(player);
		Transform staffAimTransform = CreateStaffAimObject(player.wizardSaveData.primaryStaffSaveData.puzzleData);
		Transform staffPivot = staffAimTransform.parent;
		playerObject.SetCreaturePositions (targetTransform, transform, staffAimTransform);
	}
	private Transform CreateCursor (Player player) {
		GameObject go = new GameObject("AimingPivot");
		aimingPivotTransform = go.transform;
		aimingPivotTransform.parent = transform;
		aimingPivotTransform.localPosition = new Vector3 (0, 0.325f, 0);

		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
		cursorTransform = Instantiate (cursorPrefab, transform.position + Vector3.back * 9f, Quaternion.identity, transform).transform;

		cursorController = cursorTransform.GetComponent<CursorController> ();
		cursorController.SetCursorCenter (aimingPivotTransform);
		cursorController.InitializeComponent (player);
		return cursorTransform;
	}

	private Transform CreateStaffAimObject (PuzzleData puzzleData) {
		GameObject staffObject = Instantiate(staffPrefab);
		staffObject.transform.parent = aimingPivotTransform;
		staffAimObject = staffObject.GetComponent<StaffAimObject> ();
		staffAimObject.InitializeStaffAimObject (puzzleData.puzzleSprite);
		staffObject.transform.localPosition = Vector3.zero;
		return staffAimObject.staffTipTransform;
	}
	#endregion
	#region Public Accessors
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
		switch (playerState) {
			case (PlayerState.COMBAT): {
					staffAimObject.gameObject.SetActive (true);
					break;
				}
			case (PlayerState.PUZZLE_BROWSING):
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					staffAimObject.gameObject.SetActive (false);
					break;
				}
		}
		cursorController.OnChangeState (playerState);
	}

	public override void OnEquipStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		staffAimObject.OnEquipStaff (puzzleGameData);
	}
	public override void OnDropStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		staffAimObject.OnDropStaff ();
	}

	#endregion

	public void MouseAimingUpdate (Vector2 mouseDelta) {
		cursorTransform.parent = mainCamera.transform;
		Bounds screenBounds = mainCamera.OrthographicBounds ();
		float x = Mathf.Clamp (cursorTransform.position.x + mouseDelta.x * 0.0125f, screenBounds.min.x, screenBounds.max.x);
		float y = Mathf.Clamp (cursorTransform.position.y + mouseDelta.y * 0.0125f, screenBounds.min.y, screenBounds.max.y);
		cursorTransform.position = new Vector3 (x, y, -9f);
		float angle = Mathf.Atan2 (CursorDirection.y, CursorDirection.x) * Mathf.Rad2Deg;
		AimStaffRaw ();
	}
	public void JoystickAimingUpdate (Vector2 input) {
		cursorTransform.parent = aimingPivotTransform;
		if (input.sqrMagnitude > 1f)
			input = input.normalized;
		Vector3 targetPosition = input * joystickCursorDistance;
		targetPosition.z = -9f;
		cursorTransform.localPosition = Vector3.SmoothDamp (cursorTransform.localPosition, targetPosition, ref cursorSmoothVelocity, cursorSmoothTime);
		AimStaffWithDeadzone ();
	}

	private void AimStaffRaw() {

		float angle = Mathf.Atan2(CursorDirection.y, CursorDirection.x) * Mathf.Rad2Deg;
		cursorTransform.rotation = Quaternion.AngleAxis (angle - 90, Vector3.forward);
		aimingPivotTransform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
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
