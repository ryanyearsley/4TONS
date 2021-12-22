using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowObject : AoEObject {
	[SerializeField]
	private float moveSpeed = 5f;
	private float currentMoveSpeed;

	[SerializeField]
	private AnimationCurve speedCurve;

	private bool hasHitTarget;

	[SerializeField]
	private float baseHitboxRadius = 0.15f;
	[SerializeField]
	private AnimationCurve hitboxSizeCurve;

	private Animator animator;

	[SerializeField]
	private CircleCollider2D hitboxColl;

	public override void SetupObject () {
		base.SetupObject ();
		currentMoveSpeed = moveSpeed;
	}
	public override void ReuseObject () {
		base.ReuseObject ();
		currentMoveSpeed = moveSpeed;
		animator = GetComponentInChildren<Animator> ();
		animator.speed = 1 / lifeTime;
		hasHitTarget = false;
	}
	public virtual void FixedUpdate () {

		if (isAlive) {
			if (!hasHitTarget)
				currentMoveSpeed = moveSpeed * speedCurve.Evaluate (lifeTimer / lifeTime);
			hitboxColl.radius = baseHitboxRadius * hitboxSizeCurve.Evaluate (lifeTimer / lifeTime);
			transform.Translate (Vector2.right * currentMoveSpeed * Time.fixedDeltaTime);
		}
	}

	public override void OnEnemyEnter (VitalsEntity enemyVitals) {
		base.OnEnemyEnter (enemyVitals);
		base.OnEnemyHit (enemyVitals);
		Debug.Log ("Flamethrow: hit enemy. setting MoveSpeed to 0");
		hasHitTarget = true;
		currentMoveSpeed = 0;
	}
	public override void OnWallHit (Collider2D other) {
		Debug.Log ("Flamethrow: hit wall. setting MoveSpeed to 0");
		hasHitTarget = true;
		currentMoveSpeed = 0;
	}
}
