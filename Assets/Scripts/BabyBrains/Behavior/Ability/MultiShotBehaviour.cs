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
		SpeedAlteringEffect sae = new SpeedAlteringEffect(castSpeedPenaltyMultiplier, ExecutionTime, true);
		sensoryInfo.vitalsEntity.creatureObject.AddSpeedEffect (sae);
		sensoryInfo.vitalsEntity.creatureObject.OnAttack (new AttackInfo (ExecutionTime, castSpeedPenaltyMultiplier));

		if (sensoryInfo.vitalsEntity.resource != null) {
			sensoryInfo.vitalsEntity.resource.SubtractResourceCost (resourceCost);
		}
		StartCoroutine (MultiShotRoutine (sensoryInfo));

	}
	public override void InterruptBehaviour () {
		base.InterruptBehaviour ();
		StopAllCoroutines ();
	}
	public IEnumerator MultiShotRoutine (SensoryInfo sensoryInfo) {
		for (int i = 0; i < shotCount; i++) {
			float randomRotation = Random.Range(-spread, spread);
			Quaternion shotRotation = Quaternion.Euler(0, 0, sensoryInfo.lookTransform.rotation.eulerAngles.z + randomRotation);
			PoolManager.instance.ReuseSpellObject (rangedProjectilePrefab, sensoryInfo.lookTransform.position, shotRotation, sensoryInfo.vitalsEntity);
			yield return new WaitForSeconds (shotInterval);
		}
	}
}
