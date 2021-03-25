using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//no target? Try to find a target based on parentCreator criteria.
public class AcquireTargetBehaviour : BabyBrainsBehaviour
{
	public float acquireTargetDistance;

	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetVitals.trans == null && sensoryInfo.potentialTargetVitals != null)
			return true;
		else return false;
	}

	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		base.OnTaskStart (sensoryInfo);
		Debug.Log ("executing acquire target.");
		VitalsEntity closestEnemy = null;
		float closestEnemydistance = acquireTargetDistance;
		foreach (VitalsEntity vitalsEntity in sensoryInfo.potentialTargetVitals) {
			float enemyDistance = IsometricCoordinateUtils.IsoDistanceBetweenPoints(sensoryInfo.trans, vitalsEntity.creatureObject.transform);
			if (enemyDistance < closestEnemydistance) {
				closestEnemy = vitalsEntity;
				closestEnemydistance = enemyDistance;
			}
		}
		if (closestEnemy != null) {
			Debug.Log ("target successfully found");
			sensoryInfo.targetVitals = closestEnemy;
		}

	}
}
