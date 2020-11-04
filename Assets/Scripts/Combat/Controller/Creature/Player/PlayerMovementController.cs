using PlayerManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MovementController
{
	
	public float dashSpeedMultiplier = 2f;
	[Tooltip ("In seconds.")]
	[Range (0.01f, 1f)] public float dashDuration = 0.2f;

	private bool isDashing;
	
	private PlayerStateController playerStateController;

	protected override void Awake() {
		base.Awake ();
		playerStateController = GetComponent<PlayerStateController> ();
	}
	public void UpdateMovementInput (Vector2 movementInput, Vector2 cursorDirection) {
		velocity = CalculateVelocity (movementInput * dashSpeedMultiplier);
		if (!isDashing)
			stateController.SetFaceDirection ((int)Mathf.Sign (cursorDirection.x));
	}
	public void OnDashInputDown () {
		if (!isDashing) {
			isDashing = true;
			dashSpeedMultiplier = 2f;
			StartCoroutine (ResetIsDashing (dashDuration));
			stateController.SetFaceDirection ((int)Mathf.Sign (faceDirection));
			playerStateController.OnDash ();
		}
	}
	private IEnumerator ResetIsDashing (float duration) {
		yield return new WaitForSeconds (duration);
		isDashing = false;
		dashSpeedMultiplier = 1f;
	}

}

