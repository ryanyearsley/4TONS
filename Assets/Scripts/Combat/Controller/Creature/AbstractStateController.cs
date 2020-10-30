using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract class for every create/player that's going to participate in combat.
//Manages player events (Actions) 
//Tracks player state
public class AbstractStateController : MonoBehaviour {

	public bool isDead { get; private set; }
	public event Action OnDeathEvent;
	public event Action OnRespawnEvent;

	public bool canWalk { get; private set; }
	public event Action<bool> OnSetCanWalkEvent;

	public bool canAttack { get; private set; }
	public event Action<bool> OnSetCanAttackEvent;

	public int faceDirection { get; private set; }
	public event Action<int> OnSetFaceDirEvent;

	public Vector2 velocity { get; private set; }
	public event Action<Vector2> OnSetVelocityEvent;

	public event Action<Vector2> OnHitEvent;

	public event Action<DebuffInfo> OnAddDebuffEvent;

	public virtual void OnDeath () {
		if (isDead)
			return;

		isDead = true;
		OnDeathEvent?.Invoke ();
	}

	public virtual void OnRespawn () {
		isDead = false;
		OnRespawnEvent?.Invoke ();
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
		this.velocity = velocity;
		OnSetVelocityEvent?.Invoke (velocity);
	}

	public virtual void OnHit (Vector2 direction) {
		OnHitEvent?.Invoke (direction);
	}

	public virtual void AddDebuff (DebuffInfo debuffInfo) {
		OnAddDebuffEvent?.Invoke (debuffInfo);
	}

}