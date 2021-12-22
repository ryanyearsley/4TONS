using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannibalizeBehaviour : BabyBrainsBehaviour {

	VitalsEntity closestDeadEnemyVitals = null;
	[SerializeField]
	private CreatureData cannibalizableCreatureData;
	[SerializeField]
	private Collider2D cannibalizeProximityCollider;

	[SerializeField]
	private ContactFilter2D contactFilter;


	[SerializeField]
	private GameObject corpsePrefab;

	public override void SetUpBehaviour (SensoryInfo sensoryInfo) {
		if (corpsePrefab != null) {
			PoolManager.instance.CreateObjectPool (corpsePrefab, 2);
		}
	}


	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.potentialTargetVitals != null
			&& sensoryInfo.targetVitals == null
			&& FindClosestDeadEnemy (sensoryInfo) != null)
			return true;
		else return false;
	}

	public VitalsEntity FindClosestDeadEnemy (SensoryInfo sensoryInfo) {
		List<Collider2D> overlapResults = new List<Collider2D>();
		closestDeadEnemyVitals = null;
		Physics2D.OverlapCollider (cannibalizeProximityCollider, contactFilter, overlapResults);
		foreach (Collider2D collider in overlapResults) {
			if (collider != null) {
				VitalsEntity potentialCorpsevitals = VitalsManager.Instance.GetVitalsEntityFromCorpse(collider);
				if (potentialCorpsevitals != null
					&& potentialCorpsevitals.tag != sensoryInfo.vitalsEntity.creatureObject.tag
					&& potentialCorpsevitals.creatureObject.isDead) {
					closestDeadEnemyVitals = potentialCorpsevitals;
				}
			}
		}
		return closestDeadEnemyVitals;
	}

	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		base.OnTaskStart (sensoryInfo);
		Debug.Log ("executing cannibalize enemy");
		if (closestDeadEnemyVitals != null) {
			Debug.Log ("Resurrect Caster ID: " + sensoryInfo.vitalsEntity.creatureObject.GetInstanceID ());
			Vector3 spawnPosition = closestDeadEnemyVitals.creatureObject.transform.position;
			GameObject enemyPrefab = closestDeadEnemyVitals.creatureObject.creatureData.spawnObjectPrefab;
			PoolManager.instance.ReuseObject (corpsePrefab, closestDeadEnemyVitals.creatureObject.transform.position, Quaternion.identity);
			closestDeadEnemyVitals = null;
			SpeedAlteringEffect sae = new SpeedAlteringEffect(0, ExecutionTime, true);
			sensoryInfo.vitalsEntity.creatureObject.AddSpeedEffect (sae);
			sensoryInfo.vitalsEntity.creatureObject.OnAttackSpecial (new AttackInfo (ExecutionTime, 0));
		}
	}
}
