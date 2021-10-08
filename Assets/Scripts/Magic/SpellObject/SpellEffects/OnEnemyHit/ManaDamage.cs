using UnityEngine;

public class ManaDamage : SpellEffect {

	[SerializeField]
	private int manaDamage;
	public override void OnEnemyHit (VitalsEntity caster, VitalsEntity enemyVitals) {
		if (enemyVitals.resource != null)
			enemyVitals.resource.ApplyResourceDamage (manaDamage);
	}
}
