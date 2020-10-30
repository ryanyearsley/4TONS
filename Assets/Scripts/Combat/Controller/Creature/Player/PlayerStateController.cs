using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour {

	public PlayerPositions playerPositions { get; private set; }

	public bool isDead { get; private set; }
	public event Action OnDeathEvent;
	public event Action OnRespawnEvent;

	public bool canWalk { get; private set; }
	public event Action<bool> OnSetCanWalkEvent;

	public bool canCast { get; private set; }
	public event Action<bool> OnSetCanCastEvent;

	public event Action OnDashEvent;

	public int faceDirection { get; private set; }
	public event Action<int> OnSetFaceDirEvent;

	public Vector2 velocity { get; private set; }
	public event Action<Vector2> OnSetVelocityEvent;

	public event Action<Vector2> OnHitEvent;

	public event Action<DebuffInfo> OnAddDebuffEvent;

    public void OnDeath () {
		if (isDead)
			return;

		isDead = true;
		OnDeathEvent?.Invoke ();
	}

	public void OnRespawn () {
		isDead = false;
		OnRespawnEvent?.Invoke ();
	}

	public void SetCanWalk (bool canWalk) {
		this.canWalk = canWalk;
		OnSetCanWalkEvent?.Invoke (canWalk);
	}

	public void SetCanCast (bool canCast) {
		this.canCast = canCast;
		OnSetCanCastEvent?.Invoke (canCast);
	}

	public void OnDash () {
		OnDashEvent?.Invoke ();
	}

	public void SetFaceDirection (int faceDirection) {
		this.faceDirection = faceDirection;
		OnSetFaceDirEvent?.Invoke (faceDirection);
	}

	public void SetVelocity (Vector2 velocity) {
		this.velocity = velocity;
		OnSetVelocityEvent?.Invoke (velocity);
	}

	public void OnHit (Vector2 direction) {
		OnHitEvent?.Invoke (direction);
	}

	public void AddDebuff (DebuffInfo debuffInfo) {
		OnAddDebuffEvent?.Invoke (debuffInfo);
	}

	public void SetPlayerPositions (Transform cursorTransform, Transform feetTransform, Transform staffTransform) {
		playerPositions = new PlayerPositions (cursorTransform, feetTransform, staffTransform);
	}
}

public enum PlayerState {
	COMBAT, PUZZLE, DEAD
}