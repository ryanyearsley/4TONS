
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAppropriateAttackBehaviour : BabyBrainsBehaviour {
	[SerializeField]
	protected float resourceCost;
	[SerializeField]
	protected float castSpeedPenaltyMultiplier;
	[SerializeField]
	protected float minAttackDistance;
	[SerializeField]
	protected float maxAttackDistance;
	[SerializeField]
	protected GameObject[] rangedProjectilePrefabs;

	public override void SetUpBehaviour (SensoryInfo sensoryInfo) {
		foreach (GameObject gameObject in rangedProjectilePrefabs) {
			PoolManager.instance.CreateSpellObjectPool (gameObject, 2);
		}
	}

	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetVitals.trans != null
			&& sensoryInfo.currentResource > resourceCost
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
		GameObject rangeAppropriatePrefab = ChooseRangeAppropriatePrefab(sensoryInfo);
		PoolManager.instance.ReuseSpellObject (rangeAppropriatePrefab, sensoryInfo.lookTransform.position, sensoryInfo.lookTransform.rotation, sensoryInfo.vitalsEntity);
	}

	private GameObject ChooseRangeAppropriatePrefab(SensoryInfo sensoryInfo) {
		float enemyDistance = sensoryInfo.isoDistanceToTarget;
		float percentageToMaxDistance = (enemyDistance/maxAttackDistance) * 100;
		int prefabIndex = Mathf.RoundToInt ((rangedProjectilePrefabs.Length - 1) * percentageToMaxDistance / 100);
		return rangedProjectilePrefabs [prefabIndex];
	}
}
