using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenCasterManaOnEnemyHit : SpellEffect
{

	[SerializeField]
	private float casterManaRegenAmount;

	public override void OnEnemyHit (VitalsEntity caster, VitalsEntity enemy) {
		if (caster.resource != null)
			caster.resource.RegenerateMana (casterManaRegenAmount);
	}
}
