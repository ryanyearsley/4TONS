using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract class for every create/player that's going to participate in combat.
//Manages player events (Actions) 
//Tracks player state
public class AbstractStateController : MonoBehaviour {

	[SerializeField]
	public float Health { get; private set; }

	public CreaturePositions creaturePositions { get; private set; }
	public void SetCreaturePositions (Transform targetTransform, Transform feetTransform, Transform staffTransform) {
		creaturePositions = new CreaturePositions (targetTransform, feetTransform, staffTransform);
	}

	public bool isDead { get; private set; }
	public event Action OnDeathEvent;
	public event Action<Vector3> OnRespawnEvent;

	public bool canWalk { get; private set; }
	public event Action<bool> OnSetCanWalkEvent;

	public bool canAttack { get; private set; }
	public event Action<bool> OnSetCanAttackEvent;

	public int faceDirection { get; private set; }
	public event Action<int> OnSetFaceDirEvent;

	public Vector2 velocity { get; private set; }
	public event Action<Vector2> OnSetVelocityEvent;

	public event Action<OnHitInfo> OnHitEvent;

	public event Action<DebuffInfo> OnAddDebuffEvent;

	public virtual void OnDeath () {
		if (isDead)
			return;

		isDead = true;
		OnDeathEvent?.Invoke ();
	}

	public virtual void OnRespawn (Vector3 spawnPosition) {
		isDead = false;
		OnRespawnEvent?.Invoke (spawnPosition);
	}

	public virtual void SetCanWalk (bool canWalk) {
		this.canWalk = canWalk;
		OnSetCanWalkEvent?.Invoke (canWalk);
	}

	public virtual void SetCanAttack (bool canAttack) {
		this.canAttack = canAttack;
		OnSetCanAttackEvent?.Invoke (canAttack);
	}

	public virtual void SetFaceDirection (int faceDirection) {
		this.faceDirection = faceDirection;
		OnSetFaceDirEvent?.Invoke (faceDirection);
	}

	public virtual void SetVelocity (Vector2 velocity) {
		Debug.Log ("SetVelocity: " + velocity.ToString ());
		this.velocity = velocity;
		OnSetVelocityEvent?.Invoke (velocity);
	}

	public virtual void OnHit (OnHitInfo onHitInfo) {
		Health = onHitInfo.healthRemaining;
		OnHitEvent?.Invoke (onHitInfo);
	}

	public virtual void AddDebuff (DebuffInfo debuffInfo) {
		OnAddDebuffEvent?.Invoke (debuffInfo);
	}

}