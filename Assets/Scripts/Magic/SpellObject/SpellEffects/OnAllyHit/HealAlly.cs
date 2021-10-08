using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAlly : SpellEffect
{
	[SerializeField]
	private float healAmount;
	public override void OnAllyHit (VitalsEntity caster, VitalsEntity ally) {
		if (ally.health != null) {
			ally.health.Heal (healAmount);
		}
	}
}
