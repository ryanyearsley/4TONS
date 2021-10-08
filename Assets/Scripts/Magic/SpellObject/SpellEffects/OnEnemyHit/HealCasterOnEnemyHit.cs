using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealCasterOnEnemyHit : SpellEffect
{
	[SerializeField]
	private float casterHealAmount;

	public override void OnEnemyHit (VitalsEntity caster, VitalsEntity enemy) {
		if (caster.health != null)
			caster.health.Heal (casterHealAmount);
	}
}
