using UnityEngine;

public class Damage : SpellEffect {

	[SerializeField]
	private float damage;
	public override void OnEnemyHit (VitalsEntity caster, VitalsEntity enemyVitals) {
		if (enemyVitals.health != null)
			enemyVitals.health.ApplyDamage (damage);
	}

	public void OnBarrierHit (BarrierObject barrierObject) {
		barrierObject.DamageBarrier (damage);
	}

}
