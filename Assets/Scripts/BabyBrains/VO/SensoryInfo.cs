using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SensoryInfo {

	//header-level info
	public Transform trans;
	public ParentCreator parentCreator;
	public VitalsEntity vitalsEntity;

	//looking at enemy or point of interest.
	public Transform lookTransform;
	public Transform projectileTransform;
	public bool targetWithinLoS;
	public Transform targetPositionTransform;
	public VitalsEntity targetVitals = null;
	public List<VitalsEntity> potentialTargetVitals;
	public List<VitalsEntity> potentialAllyVitals;


	public BabyBrainsBehaviour currentMovementTask;
	public BabyBrainsBehaviour currentAbilityTask;
	public float currentAbilityTimer;

	//monitored
	public float currentHealth;
	public float currentResource;

	//calculated
	public float isoDistanceToTarget;
	public float rawDistanceToTarget;

}
