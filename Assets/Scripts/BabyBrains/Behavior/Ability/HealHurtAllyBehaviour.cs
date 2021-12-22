using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealHurtAllyBehaviour : InterruptableCastBehaviour {
	[SerializeField]
	private float healAmount = 20f;
	[SerializeField]
	private float healAllyHealthThreshold = 0.5f;

	VitalsEntity closestHurtAllyVitals = null;
	[SerializeField]
	private CreatureData healableCreatureData;
	[SerializeField]
	private Collider2D healProximityCollider;

	[SerializeField]
	private ContactFilter2D contactFilter;

	[SerializeField]
	private float resurrectRange = 8;

	[SerializeField]
	private int resourceCost = 5;
	[SerializeField]
	private float castSpeedPenaltyMultiplier = 0.7f;

	[SerializeField]
	private GameObject healAnimationPrefab;

	public override void SetUpBehaviour (SensoryInfo sensoryInfo) {
		if (healAnimationPrefab != null) {
			PoolManager.instance.CreateObjectPool (healAnimationPrefab, 2);
		}
	}


	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.currentResource > resourceCost
			&& sensoryInfo.potentialAllyVitals != null
			&& FindClosestHurtAlly (sensoryInfo) != null)
			return true;
		else return false;
	}

	public VitalsEntity FindClosestHurtAlly (SensoryInfo sensoryInfo) {
		List<Collider2D> overlapResults = new List<Collider2D>();
		closestHurtAllyVitals = null;
		Physics2D.OverlapCollider (healProximityCollider, contactFilter, overlapResults);
		foreach (Collider2D collider in overlapResults) {
			if (collider != null) {
				VitalsEntity potentialHurtAlly = VitalsManager.Instance.GetVitalsEntityFromFeet(collider);

				if (potentialHurtAlly != null
						&& potentialHurtAlly.creatureData == healableCreatureData
						&& potentialHurtAlly.tag == sensoryInfo.vitalsEntity.creatureObject.tag) {
					float currentHealth = potentialHurtAlly.health.GetValue ();
					float maxHealth = potentialHurtAlly.creatureData.maxHealth;
					if (currentHealth / maxHealth < healAllyHealthThreshold){
						closestHurtAllyVitals = potentialHurtAlly;
				}
				}
			}
		}
		return closestHurtAllyVitals;
	}

	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		base.OnTaskStart (sensoryInfo);
		SpeedAlteringEffect sae = new SpeedAlteringEffect(castSpeedPenaltyMultiplier, ExecutionTime, true);
		sensoryInfo.vitalsEntity.creatureObject.AddSpeedEffect (sae);
		sensoryInfo.vitalsEntity.creatureObject.OnAttackSpecial (new AttackInfo (ExecutionTime, castSpeedPenaltyMultiplier));
	}

	public override void ExecuteInterruptableTask (SensoryInfo sensoryInfo) {
		if (closestHurtAllyVitals != null) {
			Vector3 spawnPosition = closestHurtAllyVitals.creatureObject.transform.position;
			closestHurtAllyVitals.health.Heal (healAmount);
			PoolManager.instance.ReuseObject (healAnimationPrefab, closestHurtAllyVitals.creatureObject.transform.position, Quaternion.identity);

			if (sensoryInfo.vitalsEntity.resource != null) {
				sensoryInfo.vitalsEntity.resource.SubtractResourceCost (resourceCost);
			}
			SpeedAlteringEffect sae = new SpeedAlteringEffect(castSpeedPenaltyMultiplier, ExecutionTime, true);
			sensoryInfo.vitalsEntity.creatureObject.AddSpeedEffect (sae);
			sensoryInfo.vitalsEntity.creatureObject.OnAttack (new AttackInfo (ExecutionTime, castSpeedPenaltyMultiplier));
		}
	}
}
