using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeUnderSelfBehaviour : BabyBrainsBehaviour
{
	[SerializeField]
	protected float resourceCost = 10;
	[SerializeField]
	protected float maxAttackDistance = 3;
	[SerializeField]
	protected float castSpeedPenaltyMultiplier = 0.5f;
	[SerializeField]
	protected GameObject aoePrefab;

	public override void SetUpBehaviour (SensoryInfo sensoryInfo) {
		PoolManager.instance.CreateSpellObjectPool (aoePrefab, 3);
	}

	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetVitals.trans != null
			&& sensoryInfo.currentResource > resourceCost
			&& sensoryInfo.targetWithinLoS
			&& sensoryInfo.isoDistanceToTarget < maxAttackDistance)
			return true;
		else return false;
	}

	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		SpeedAlteringEffect sae = new SpeedAlteringEffect(castSpeedPenaltyMultiplier, ExecutionTime, true);
		if (sensoryInfo.vitalsEntity.resource != null) {
			sensoryInfo.vitalsEntity.resource.SubtractResourceCost (resourceCost);
		}
		sensoryInfo.vitalsEntity.creatureObject.AddSpeedEffect (sae);
		sensoryInfo.vitalsEntity.creatureObject.OnAttack (new AttackInfo (ExecutionTime, castSpeedPenaltyMultiplier));
		PoolManager.instance.ReuseSpellObject (aoePrefab, sensoryInfo.trans.position, Quaternion.identity, sensoryInfo.vitalsEntity);
	}
}
