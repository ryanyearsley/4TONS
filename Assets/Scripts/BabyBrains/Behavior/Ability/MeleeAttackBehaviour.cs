using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehaviour : BabyBrainsBehaviour {

	[SerializeField]
	private float attackDistance;
	[SerializeField]
	private float attackDamage;

	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetVitals != null
			&& !sensoryInfo.targetVitals.creatureObject.isDead
			&& sensoryInfo.isoDistanceToTarget < attackDistance)
			return true;
		else return false;
	}
	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		StartCoroutine (MeleeAttackRoutine (sensoryInfo));
	}
	public IEnumerator MeleeAttackRoutine(SensoryInfo sensoryInfo) {

		SpeedAlteringEffect sae = new SpeedAlteringEffect(4, ExecutionTime, true);
		sensoryInfo.vitalsEntity.creatureObject.AddSpeedEffect (sae);
		sensoryInfo.vitalsEntity.creatureObject.OnAttack (new AttackInfo (ExecutionTime, 1.5f));
		yield return new WaitForSeconds (ExecutionTime);
		sensoryInfo.targetVitals.health.ApplyDamage (attackDamage);
	}
	


}
