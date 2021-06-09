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

	protected Vector2 velocity;
	private Rigidbody2D rb;
	protected float speedEffectMultiplier = 1f;
	protected float creatureStateSpeedModifier = 1f;
	protected float passiveAbilityModifier = 1f;
	public int moveDirection { get; private set; }

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
			velocity = CalculateVelocity (direction);
			rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
		}
	}
	protected virtual Vector2 CalculateVelocity (Vector2 direction) {

		speedEffectMultiplier = CalculateSpeedMultipliers ();
		Vector2 normalizedInput = (direction.sqrMagnitude > 1f)
				? direction.normalized
				: direction;

		Vector2 targetVelocity = normalizedInput * CalculateActualSpeed() * isometricScaling;
		Vector2 smoothedVelocity = Vector2.SmoothDamp (velocity, targetVelocity, ref velocitySmoothing, acceleration);

		creatureObject.SetVelocity (
			(smoothedVelocity * speedEffectMultiplier));
		return smoothedVelocity;
	}
	private float CalculateActualSpeed() {
		 return currentMovementSpeed = movementSpeed * creatureStateSpeedModifier * passiveAbilityModifier * speedEffectMultiplier;
	}

	protected float CalculateSpeedMultipliers () {
		bool canWalk = true;
		bool canAttack = true;
		speedEffectMultiplier = 1f;
		for (int i = 0; i < speedEffects.Count; i++) {
			if (speedEffects [i].speedMultiplier == 0)
				canWalk = false;
			if (!speedEffects [i].canAttack)
				canAttack = false;

			speedEffectMultiplier += speedEffects [i].speedMultiplier - 1f;
			speedEffectMultiplier = Mathf.Clamp (speedEffectMultiplier, 0f, float.MaxValue);
			speedEffects [i].timeRemaining -= Time.deltaTime;
		}
		creatureObject.SetCanWalk (canWalk);
		creatureObject.SetCanAttack (canAttack);
		speedEffects.RemoveAll (debuff => debuff.timeRemaining <= 0f);
		return speedEffectMultiplier;
	}

}