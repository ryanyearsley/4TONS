using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAllyEffect : SpellEffect
{
	[SerializeField]
	private float healAmount;

	[SerializeField]
	private GameObject healAnimationObject;

	private void Awake () {
		if (healAnimationObject != null) {
			PoolManager.instance.CreateObjectPool (healAnimationObject, 8);
		}
	}
	public override void OnAllyHit (VitalsEntity caster, VitalsEntity ally) {
		if (ally.health != null) {
			ally.health.Heal (healAmount);
			if (healAnimationObject != null) {
				PoolManager.instance.ReuseObject (healAnimationObject, ally.creatureObject.transform.position, Quaternion.identity);
			}
		}

	}
}
