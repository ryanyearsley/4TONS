using System.Linq;
using UnityEngine;

public class ProjectileObject : SpellObject {

	[SerializeField]
	protected float moveSpeed;

	protected float currentMoveSpeed;

	[SerializeField]
	private AnimationCurve speedCurve;

	[SerializeField]
	protected GameObject debrisObject;

	protected Damage damageEffect;
	public override void SetupObject () {
		base.SetupObject ();
		if (debrisObject != null)
			PoolManager.instance.CreateObjectPool (debrisObject, 2);
		currentMoveSpeed = CalculateMoveSpeed ();

		damageEffect = GetComponentInChildren<Damage> ();
	}
	public virtual float CalculateMoveSpeed() {
		return speedCurve.Evaluate (lifeTimer / lifeTime) * moveSpeed;
	}


	public virtual void FixedUpdate () {

		if (isAlive) {
			currentMoveSpeed = CalculateMoveSpeed ();
			transform.Translate (Vector2.right * currentMoveSpeed * Time.fixedDeltaTime);
		}
	}

	public override void OnEnemyHit (VitalsEntity enemyVitals) {
		base.OnEnemyHit (enemyVitals);
		DestroyProjectile ();
	}
	public override void OnWallHit () {
		DestroyProjectile ();
	}

	public override void OnBarrierHit (BarrierObject barrierObject) {
		damageEffect.OnBarrierHit (barrierObject);
		DestroyProjectile ();
	}
	protected virtual void DestroyProjectile () {
		//LevelManager.instance.DestroyEnvironment (floorPosition, 2);
		if (debrisObject != null) {
			PoolManager.instance.ReuseObject (debrisObject, trans.position + (Vector3.down * 0.325f), Quaternion.identity);
		}

		Destroy ();
	}

}
