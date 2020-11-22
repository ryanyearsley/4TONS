using PlayerManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MovementController
{
	
	private float currentDashSpeedMultipler = 1;
	private float currentPuzzleSpeedMultiplier = 1f;
	[Tooltip ("In seconds.")]
	[Range (0.01f, 1f)] public float dashInvulnerableTime = 0.2f;
	[Range (0.01f, 1f)] public float dashSpeedMultiplier = 2.0f;
	[Range (0.01f, 1f)] public float dashCooldown = 1.5f;

	private bool canDash = true;
	
	private PlayerStateController playerStateController;

	private void OnEnable () {
		playerStateController = transform.GetComponent<PlayerStateController> ();
		playerStateController.OnDashEvent += OnDash;
		playerStateController.OnCastSpellEvent += OnCastSpell;
		playerStateController.OnChangeStateEvent += OnChangeState;
	}

	private void OnDisable () {
		playerStateController.OnDashEvent -= OnDash;
		playerStateController.OnCastSpellEvent -= OnCastSpell;
		playerStateController.OnChangeStateEvent -= OnChangeState;
	}
	public void UpdateMovementInput (Vector2 movementInput, Vector2 cursorDirection) {
		velocity = CalculateVelocity (movementInput);
		if (currentDashSpeedMultipler == 1)
			stateController.SetFaceDirection ((int)Mathf.Sign (cursorDirection.x));
	}
	protected override Vector2 AggregateMovementModifiers () {
		return moveSpeed * debuffSpeedMultiplier * isometricScaling * currentDashSpeedMultipler * currentPuzzleSpeedMultiplier;
	}
	public void OnDashInputDown () {
		if (canDash && playerStateController.currentPlayerState == PlayerState.COMBAT) {
			playerStateController.OnDash (new DashInfo (dashInvulnerableTime, dashSpeedMultiplier, dashCooldown));
		}
	}
	public void OnChangeState (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.COMBAT): {
					currentPuzzleSpeedMultiplier = 1f;
					break;
				}
			case (PlayerState.PUZZLE_BROWSING):
			case (PlayerState.PUZZLE_MOVING_SPELLGEM):
			{
				currentPuzzleSpeedMultiplier = 0.2f;
				break;
			}
		}
	}
	public void OnDash(DashInfo dashInfo) {
		StartCoroutine (DashRoutine (dashInfo));
	}
	public void OnCastSpell (SpellData spellData) {
		AddDebuff (new DebuffInfo (spellData.castTime, spellData.castSpeedReduction, true));
	}

	private IEnumerator DashRoutine(DashInfo dashInfo) {
		canDash = false;
		currentDashSpeedMultipler = dashInfo.dashSpeedMultiplier;
		stateController.SetFaceDirection ((int)Mathf.Sign (faceDirection));
		yield return new WaitForSeconds (dashInfo.invulnerableTime);
		currentDashSpeedMultipler = 1;
		yield return new WaitForSeconds (dashInfo.cooldown - dashInfo.invulnerableTime);
		canDash = true;
	}
	private IEnumerator ResetIsDashing (float duration) {
		yield return new WaitForSeconds (duration);
		canDash = false;
		currentDashSpeedMultipler = 1f;
	}

}

