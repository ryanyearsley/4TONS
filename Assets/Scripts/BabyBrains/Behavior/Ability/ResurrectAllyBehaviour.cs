using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectAllyBehaviour : InterruptableCastBehaviour {

	VitalsEntity closestDeadAllyVitals = null;
	[SerializeField]
	private CreatureData resurrectableCreatureData;
	[SerializeField]
	private Collider2D resurrectProximityCollider;

	[SerializeField]
	private ContactFilter2D contactFilter;

	[SerializeField]
	private float resurrectRange = 6;

	[SerializeField]
	private int resourceCost = 10;
	[SerializeField]
	private float castSpeedPenaltyMultiplier = 0.3f;

	[SerializeField]
	private GameObject resurrectHandPrefab;

	public override void SetUpBehaviour (SensoryInfo sensoryInfo) {
		if (resurrectHandPrefab != null) {
			PoolManager.instance.CreateObjectPool (resurrectHandPrefab, 2);
		}
	}


	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.currentResource > resourceCost
			&& sensoryInfo.potentialAllyVitals != null
			&& FindClosestDeadZombie (sensoryInfo) != null)
			return true;
		else return false;
	}

	public VitalsEntity FindClosestDeadZombie (SensoryInfo sensoryInfo) {
		List<Collider2D> overlapResults = new List<Collider2D>();
		closestDeadAllyVitals = null;
		Physics2D.OverlapCollider (resurrectProximityCollider, contactFilter, overlapResults);
		foreach (Collider2D collider in overlapResults) {
			if (collider != null) {
				VitalsEntity potentialCorpsevitals = VitalsManager.Instance.GetVitalsEntityFromCorpse(collider);
				if (potentialCorpsevitals != null
					&& potentialCorpsevitals.creatureObject.isDead
					&& potentialCorpsevitals.tag == sensoryInfo.vitalsEntity.creatureObject.tag
					&& potentialCorpsevitals.creatureData == resurrectableCreatureData) {
					closestDeadAllyVitals = potentialCorpsevitals;
				}
			}
		}
		return closestDeadAllyVitals;
	}

	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		base.OnTaskStart (sensoryInfo);
		SpeedAlteringEffect sae = new SpeedAlteringEffect(castSpeedPenaltyMultiplier, ExecutionTime, true);
		sensoryInfo.vitalsEntity.creatureObject.AddSpeedEffect (sae);
		sensoryInfo.vitalsEntity.creatureObject.OnAttackSpecial (new AttackInfo (ExecutionTime, castSpeedPenaltyMultiplier));
	}

	public override void ExecuteInterruptableTask (SensoryInfo sensoryInfo) {
		Debug.Log ("executing resurrect ally.");
		if (closestDeadAllyVitals != null) {
			Debug.Log ("Resurrect Caster ID: " + sensoryInfo.vitalsEntity.creatureObject.GetInstanceID ());
			Vector3 spawnPosition = closestDeadAllyVitals.creatureObject.transform.position;
			GameObject enemyPrefab = closestDeadAllyVitals.creatureObject.creatureData.spawnObjectPrefab;
			closestDeadAllyVitals.creatureObject.Destroy ();
			GameObject enemyObject = PoolManager.instance.ReuseCreatureObject(enemyPrefab, spawnPosition);
			Debug.Log ("Resurrect enemy object ID: " + enemyObject.GetInstanceID ());
			PoolManager.instance.ReuseObject (resurrectHandPrefab, enemyObject.transform.position, Quaternion.identity);
			sensoryInfo.vitalsEntity.creatureObject.OnAttack (new AttackInfo (0.3f, castSpeedPenaltyMultiplier));
			if (sensoryInfo.vitalsEntity.resource != null) {
				sensoryInfo.vitalsEntity.resource.SubtractResourceCost (resourceCost);
			}


		}
	}
}
