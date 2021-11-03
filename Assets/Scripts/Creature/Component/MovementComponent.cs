using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (Rigidbody2D))]
public class MovementComponent : CreatureComponent {

	[SerializeField]
	protected float currentMovementSpeed;

	[SerializeField]
	protected float movementSpeed;
	[Tooltip ("Scaling amount used to adjust the speeds on different axis.")]
	public Vector2 isometricScaling = new Vector2 (1f, 0.5f);
	[Range (0.01f, 0.9f)]
	public float acceleration = 0.1f;
	private Vector2 velocitySmoothing;

	[HideInInspector]
	public Collider2D feetCollider;

	protected bool isVelocityOverride = false;
	protected Vector2 currentVelocityOverride;
	protected Vector2 currentVelocity;
	private Rigidbody2D rb;
	protected float speedEffectMultiplier = 1f;
	protected float creatureStateSpeedModifier = 1f;
	protected float passiveAbilityModifier = 1f;
	public int moveDirection { get; private set; }

	[SerializeField]
	private List<SpeedAlteringEffect> speedEffects = new List<SpeedAlteringEffect>();


	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject);
		rb = GetComponent<Rigidbody2D> ();
		feetCollider = GetComponent<Collider2D> ();
		movementSpeed = creatureObject.creatureData.movementSpeed;
	}
	#region PlayerComponent callbacks
	public override void OnSpawn (Vector3 spawnPosition) {
		EnableMovement ();
	}
	public override void OnDeath () {
		DisableMovement ();
	}
	public override void OnAddDebuff (SpeedAlteringEffect speedEffectInfo) {
		if (!creatureObject.isDead)
			speedEffects.Add (speedEffectInfo);
	}
	#endregion
	private void DisableMovement () {
		creatureObject.SetCanWalk (false);
		creatureObject.SetVelocity (Vector2.zero);
		rb.constraints = RigidbodyConstraints2D.FreezeAll;
	}

	private void EnableMovement () {
		creatureObject.SetCanWalk (true);
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	public void SetCreatureStateModifier (float modifier) {
		creatureStateSpeedModifier = modifier;
	}


	//ONLY CALL FROM FIXED UPDATE
	public void MovementFixedUpdate (Vector2 direction) {
		moveDirection = (int)Mathf.Sign (direction.x);
		if (creatureObject.canWalk) {
			CalculateVelocity (direction);
			rb.MovePosition (rb.position + currentVelocity * Time.fixedDeltaTime);
		} else {
			creatureObject.SetVelocity (Vector2.zero);
		}
	}
	protected virtual void CalculateVelocity (Vector2 direction) {

		speedEffectMultiplier = CalculateSpeedMultipliers ();
		//
		if (!isVelocityOverride) {
			Vector2 normalizedInput = (direction.sqrMagnitude > 1f)
				? direction.normalized
				: direction;

			Vector2 oldVelocity = currentVelocity;

			float actualSpeed = CalculateActualSpeed();
			Vector2 targetVelocity = normalizedInput * actualSpeed * isometricScaling;
			currentVelocity =  Vector2.SmoothDamp (oldVelocity, targetVelocity, ref velocitySmoothing, acceleration);
			creatureObject.SetVelocity (
					(targetVelocity));
			}
		else {
			currentVelocity = currentVelocityOverride;
			creatureObject.SetVelocity (
				(currentVelocityOverride));
		}
	}
	private float CalculateActualSpeed() {
		return currentMovementSpeed = movementSpeed * creatureStateSpeedModifier * passiveAbilityModifier * speedEffectMultiplier;
	}

	protected float CalculateSpeedMultipliers () {
		bool canWalk = true;
		bool canAttack = true;
		isVelocityOverride = false;
		currentVelocityOverride = Vector2.zero;
		speedEffectMultiplier = 1f;
		for (int i = 0; i < speedEffects.Count; i++) {
			if (speedEffects [i].speedMultiplier == 0)
				canWalk = false;
			if (!speedEffects [i].canAttack)
				canAttack = false;
			if (speedEffects [i].isVelocityOverride) {
				isVelocityOverride = true;
				Debug.Log ("MovementComponent: VELOCITY OVERRIDE FOUND");
				currentVelocityOverride = speedEffects [i].velocityOverride;
			}
			//speedEffectMultiplier -= speedEffects [i].speedMultiplier;
			speedEffectMultiplier += speedEffects [i].speedMultiplier - 1f;
			speedEffects [i].effectTimer += Time.fixedDeltaTime;
		}

		speedEffectMultiplier = Mathf.Clamp (speedEffectMultiplier, 0f, 5f);
		creatureObject.SetCanWalk (canWalk);
		creatureObject.SetCanAttack (canAttack);
		speedEffects.RemoveAll (debuff => debuff.effectTimer >= debuff.effectTime);
		Debug.Log ("MovementComponent: SpeedEffects length:" + speedEffects.Count);
		return speedEffectMultiplier;
	}

}