using System.Linq;
using UnityEngine;

public class ProjectileObject : SpellObject {

	private Vector2 isoSpeedMuliplyVector = new Vector2 (1, 0.5f);

	[SerializeField]
	protected float moveSpeed;

	protected float currentMoveSpeed;

	[SerializeField]
	private AnimationCurve speedCurve;

	protected Damage damageEffect;

	protected AnimationCurve isoSpeedCurve;
	public override void SetupObject () {
		base.SetupObject ();
		currentMoveSpeed = CalculateRealMoveSpeed ();
		damageEffect = GetComponentInChildren<Damage> ();
		isoSpeedCurve = ConstantsManager.instance.isoSpeedCurve;
	}
	public virtual float CalculateRealMoveSpeed() {
		return speedCurve.Evaluate (lifeTimer / lifeTime) * moveSpeed;
	}
	public virtual float CalculateIsometricMoveSpeedMultiplier() {
		//if a projectile has a rotation dot value equal to vector2.right,
		float baseSpeed = CalculateRealMoveSpeed() / 2;
		float isoSpeedModifier = Vector2.Dot(Vector2.right, trans.TransformDirection(Vector2.right));
		if (isoSpeedModifier < 0) {
			isoSpeedModifier *= -1;
		}
		isoSpeedModifier = isoSpeedCurve.Evaluate (isoSpeedModifier);
		float isoSpeed = baseSpeed + (baseSpeed * isoSpeedModifier);
		//Debug.Log ("ProjectileObject: IsoSpeed: " + isoSpeed + ", speedModifier: " + isoSpeedModifier);
		return isoSpeed;
	}
	public override void Update () {
		base.Update ();
		if (isAlive) {
			currentMoveSpeed = CalculateIsometricMoveSpeedMultiplier ();
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
