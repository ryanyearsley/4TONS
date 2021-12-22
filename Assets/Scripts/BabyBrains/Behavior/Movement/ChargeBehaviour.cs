using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBehaviour : BabyBrainsBehaviour {
	[SerializeField]
	private float chargeSpeedMultiplier = 2.5f;
	[SerializeField]
	private float minimumChargeDistance = 2f;
	[SerializeField]
	private float maxChargeDistance = 4f;

	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetVitals != null 
			&& sensoryInfo.isoDistanceToTarget < maxChargeDistance 
			&& sensoryInfo.isoDistanceToTarget > minimumChargeDistance)
			return true;
		else return false;
	}
	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		SpeedAlteringEffect sae = new SpeedAlteringEffect(chargeSpeedMultiplier, ExecutionTime, true);
		sensoryInfo.vitalsEntity.creatureObject.AddSpeedEffect (sae);
	}
}
