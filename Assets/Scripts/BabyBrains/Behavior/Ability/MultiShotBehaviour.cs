using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiShotBehaviour : RangedAttackBehaviour {

	[SerializeField]
	private float spread;
	[SerializeField]
	private int shotCount;
	[SerializeField]
	private float shotInterval;

	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		if (sensoryInfo.vitalsEntity.resource != null) {
			sensoryInfo.vitalsEntity.resource.SubtractResourceCost (resourceCost);
		}
		StartCoroutine (MultiShotRoutine (sensoryInfo));

	}

	public IEnumerator MultiShotRoutine (SensoryInfo sensoryInfo) {
		for (int i = 0; i < shotCount; i++) {
			SpeedAlteringEffect sae = new SpeedAlteringEffect(castSpeedPenaltyMultiplier, ExecutionTime, true);
			sensoryInfo.vitalsEntity.creatureObject.AddSpeedEffect (sae);
			sensoryInfo.vitalsEntity.creatureObject.OnAttack (new AttackInfo (ExecutionTime, castSpeedPenaltyMultiplier));
			float randomRotation = Random.Range(-spread, spread);
			PoolManager.instance.ReuseSpellObject (rangedProjectilePrefab, sensoryInfo.lookTransform.position, sensoryInfo.lookTransform.rotation, sensoryInfo.vitalsEntity);
			yield return new WaitForSeconds (shotInterval);
		}
	}
}
