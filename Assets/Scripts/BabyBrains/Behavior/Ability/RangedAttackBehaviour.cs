using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackBehaviour : BabyBrainsBehaviour
{
	[SerializeField]
	protected float resourceCost;
	[SerializeField]
	protected float castSpeedPenaltyMultiplier;
	[SerializeField]
	protected float minAttackDistance;
	[SerializeField]
	protected float maxAttackDistance;
	[SerializeField]
	protected int poolSize = 5;
	[SerializeField]
	protected GameObject rangedProjectilePrefab;

	public override void SetUpBehaviour (SensoryInfo sensoryInfo) {
		PoolManager.instance.CreateSpellObjectPool (rangedProjectilePrefab, poolSize);
	}

	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetVitals != null 
			&& sensoryInfo.currentResource >= resourceCost
			&& sensoryInfo.targetWithinLoS 
			&& sensoryInfo.isoDistanceToTarget > minAttackDistance 
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
		PoolManager.instance.ReuseSpellObject (rangedProjectilePrefab, sensoryInfo.projectileTransform.position, sensoryInfo.lookTransform.rotation, sensoryInfo.vitalsEntity);
	}
}
