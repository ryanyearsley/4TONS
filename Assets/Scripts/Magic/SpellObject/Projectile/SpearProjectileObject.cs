using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearProjectileObject : ProjectileObject
{
	public override void OnEnemyHit (VitalsEntity enemyVitals) {
		foreach (SpellEffect spellEffect in spellEffects) {
			spellEffect.OnEnemyHit (casterVitalsEntity, enemyVitals);
		}
	}
}
