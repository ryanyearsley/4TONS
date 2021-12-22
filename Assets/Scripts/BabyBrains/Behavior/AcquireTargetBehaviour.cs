﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//no target? Try to find a target based on parentCreator criteria.
public class AcquireTargetBehaviour : BabyBrainsBehaviour
{
	public float acquireTargetDistance;

	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetVitals == null && sensoryInfo.potentialTargetVitals != null)
			return true;
		else return false;
	}

	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		base.OnTaskStart (sensoryInfo);
		Debug.Log ("executing acquire target.");
		VitalsEntity closestEnemy = null;
		float closestEnemyDistance = acquireTargetDistance;
		foreach (VitalsEntity vitalsEntity in sensoryInfo.potentialTargetVitals) {
			float enemyDistance = IsometricCoordinateUtilites.IsoDistanceBetweenPoints(sensoryInfo.trans.position, vitalsEntity.creatureObject.transform.position);
			if (enemyDistance < closestEnemyDistance) {
				closestEnemy = vitalsEntity;
				closestEnemyDistance = enemyDistance;
			}
		}
		if (closestEnemy != null) {
			Debug.Log ("target successfully found");
			sensoryInfo.targetVitals = closestEnemy;
		}

	}
}
