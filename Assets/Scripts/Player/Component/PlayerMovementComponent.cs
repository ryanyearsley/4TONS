using PlayerManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementComponent : PlayerComponent
{

	private MovementComponent movement;
	private AnimationComponent animationComponent;
	private DashInfo dashInfo;

	[Tooltip("In seconds.")]
	[Range(0.01f, 1f)] public float dashInvulnerableTime = 0.2f;
	[Range(0.01f, 1f)] public float dashAnimationTime = 0.4f;
	[Range(0.01f, 3f)] public float dashSpeedMultiplier = 2.0f;
	[Range(0.01f, 1f)] public float dashCooldown = 1.5f;

	private bool dashing = false;
	private bool canDash = true;

	private float UITimer;

	public override void SetUpComponent(GameObject rootObject)
	{
		base.SetUpComponent(rootObject);
		movement = creatureObject.GetComponent<MovementComponent>();
		animationComponent = creatureObject.GetComponentInChildren<AnimationComponent>();
	}

	public void UpdateMovementInput(Vector2 movementInput, Vector2 cursorDirection)
	{
		movement.MovementFixedUpdate(movementInput);
		if (!dashing)
			creatureObject.SetFaceDirection((int)Mathf.Sign(cursorDirection.x));
	}
	public void OnDashInputDown()
	{
		if (canDash && playerObject.currentPlayerState == PlayerState.COMBAT)
		{
			dashInfo = new DashInfo(dashInvulnerableTime, dashAnimationTime, dashSpeedMultiplier, dashCooldown);
			playerObject.OnDash(dashInfo);
		}
	}
	public override void OnChangePlayerState(PlayerState playerState)
	{
		switch (playerState)
		{
			case (PlayerState.COMBAT):
				{
					movement.SetCreatureStateModifier(1);
					break;
				}
			case (PlayerState.PUZZLE_BROWSING):
			case (PlayerState.PUZZLE_MOVING_SPELLGEM):
				{
					movement.SetCreatureStateModifier(0.2f);
					break;
				}
		}
	}
	public override void OnDash(DashInfo dashInfo)
	{
		StartCoroutine(DashMovementRoutine(dashInfo));
	}
	private void Update()
	{
		if (!canDash)
		{
			UITimer += Time.deltaTime;
			playerObject.playerUI.UpdateDodgeUICooldown(UITimer, dashInfo.cooldown);
		}
	}

	private IEnumerator DashMovementRoutine(DashInfo dashInfo)
	{
		dashing = true;
		canDash = false;
		creatureObject.SetFaceDirection((int)Mathf.Sign(movement.moveDirection));
		SpeedAlteringEffect sae = new SpeedAlteringEffect(dashSpeedMultiplier, dashInfo.invulnerableTime, false);
		creatureObject.AddSpeedEffect(sae);
		dashing = false;
		UITimer = 0;
		yield return new WaitForSeconds(dashInfo.cooldown);
		playerObject.playerUI.ClearDodgeUI();
		canDash = true;
	}

}

