using System.Linq;
using UnityEngine;

public class ProjectileObject : SpellObject {

	[SerializeField]
	protected float moveSpeed;

	protected float currentMoveSpeed;

	[SerializeField]
	private AnimationCurve speedCurve;

	protected Damage damageEffect;
	public override void SetupObject () {
		base.SetupObject ();
		currentMoveSpeed = CalculateMoveSpeed ();
		damageEffect = GetComponentInChildren<Damage> ();
	}
	public virtual float CalculateMoveSpeed() {
		return speedCurve.Evaluate (lifeTimer / lifeTime) * moveSpeed;
	}


	public override void Update () {
		base.Update ();
		if (isAlive) {
			currentMoveSpeed = CalculateMoveSpeed ();
			transform.Translate (Vector2.right * currentMoveSpeed * Time.deltaTime);
		}
	}

	public override void OnEnemyHit (VitalsEntity enemyVitals) {
		base.OnEnemyHit (enemyVitals);
		Destroy ();
	}
	public override void OnWallHit (Collider2D otherCollider) {
		Destroy ();
	}

	public override void OnBarrierHit (BarrierObject barrierObject) {
		damageEffect.OnBarrierHit (barrierObject);
		Destroy ();
	}

	public override void TerminateObjectFunctions () {
		if (debrisObject != null) {
			PoolManager.instance.ReuseObject (debrisObject, trans.position, Quaternion.identity);
		}
		base.TerminateObjectFunctions ();
	}

}
