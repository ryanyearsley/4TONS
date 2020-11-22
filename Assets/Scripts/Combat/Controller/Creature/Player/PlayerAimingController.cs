using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingController : MonoBehaviour {

	private Player player;

	private PlayerStateController playerStateController;
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

	private void OnEnable () {
		playerStateController = transform.GetComponent<PlayerStateController> ();
		playerStateController.OnChangeStateEvent += OnChangeState;
	}

	private void OnDisable () {
		playerStateController.OnChangeStateEvent -= OnChangeState;
	}

	public void OnChangeState (PlayerState playerState) {
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
	public void InitializeComponent (Player player) {
		this.player = player;
		Transform targetTransform = CreateCursor();
		Transform staffAimTransform = CreateStaffAimObject(player.currentWizard.primaryStaffSaveData.staffData);
		Transform staffPivot = staffAimTransform.parent;
		playerStateController = GetComponent<PlayerStateController> ();
		playerStateController.SetCreaturePositions (targetTransform, transform, staffAimTransform);
		mainCamera = Camera.main;
	}

	private Transform CreateCursor () {
		GameObject go = new GameObject("AimingPivot");
		aimingPivotTransform = go.transform;
		aimingPivotTransform.parent = transform;
		aimingPivotTransform.localPosition = new Vector3 (0, 0.5f, 0);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		cursorTransform = Instantiate (ConstantsManager.instance.cursorTemplatePrefab, transform.position + Vector3.back * 9f, Quaternion.identity, transform).transform;

		cursorController = cursorTransform.GetComponent<CursorController> ();
		cursorController.SetCursorCenter (aimingPivotTransform);
		cursorController.InitializeComponent (player);
		return cursorTransform;
	}

	private Transform CreateStaffAimObject (StaffData staffData) {
		GameObject staffObject = Instantiate(ConstantsManager.instance.staffTemplatePrefab);
		staffObject.transform.parent = aimingPivotTransform;
		staffAimObject = staffObject.GetComponent<StaffAimObject>();
		staffAimObject.InitializeStaffAimObject (staffData.staffSprite);
		staffObject.transform.localPosition = Vector3.zero;
		return staffAimObject.staffTipTransform;
	}
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
			if (playerStateController.faceDirection >= 0) {
				aimingPivotTransform.rotation = new Quaternion (0, 0, 180, 0);
			} else {
				aimingPivotTransform.rotation = new Quaternion (0, 0, 0, 0);
			}
		}
	}
}
