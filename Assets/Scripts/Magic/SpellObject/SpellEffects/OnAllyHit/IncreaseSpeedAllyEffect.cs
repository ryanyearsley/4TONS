using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseSpeedAllyEffect : SpellEffect
{
	[SerializeField]
	private float allySpeedMultiplier;

	public override void OnAllyHit (VitalsEntity caster, VitalsEntity ally) {
		if (ally.movement != null) {
			SpeedAlteringEffect sae = new SpeedAlteringEffect(allySpeedMultiplier, 0.25f, true);
			ally.creatureObject.AddSpeedEffect (sae);
		}

	}
}
