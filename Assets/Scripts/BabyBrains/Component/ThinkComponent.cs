﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PathfindingComponent), typeof (BabyBrainsObject))]
public class ThinkComponent : BabyBrainsComponent {

	[SerializeField]
	private BabyBrainsBehaviour[] behaviours = null;

	[SerializeField]
	private SensoryInfo sensoryInfo = new SensoryInfo();

	public bool Paused = true;

	[SerializeField]
	private float thinkInterval;

	[SerializeField]
	private bool isThinking;


	private AIPriorityQueue movementPq = new AIPriorityQueue(1);

	[SerializeField]
	private AIPriorityQueue abilityBehaviourPq = new AIPriorityQueue(1);
	[SerializeField]
	private AIPriorityQueue movementBehaviourPq = new AIPriorityQueue(1);

	private List<BabyBrainsBehaviour> invalidatedBehaviours = new List<BabyBrainsBehaviour>();
	private List<BabyBrainsBehaviour> onCooldownBehaviours = new List<BabyBrainsBehaviour>();

	public LayerMask layerMask;

	private Vector3 creatureBodyOffset = new Vector3 (0, 0.325f, 0);


	#region BabyBrainsComponent Callbacks
/*
	public override void OnBeginLevel (int levelIndex) {
		Paused = false;
		sensoryInfo.potentialTargetVitals = VitalsManager.Instance.AcquirePotentialTargets (babyBrainsObject.vitalsEntity);
		sensoryInfo.potentialAllyVitals = VitalsManager.Instance.AcquirePotentialAllies (babyBrainsObject.vitalsEntity);
		StartThinking ();
	}*/

	public override void OnSpawn (Vector3 spawnPosition) {
		Paused = false;
		sensoryInfo.potentialTargetVitals = VitalsManager.Instance.AcquirePotentialTargets (babyBrainsObject.vitalsEntity);
		sensoryInfo.potentialAllyVitals = VitalsManager.Instance.AcquirePotentialAllies (babyBrainsObject.vitalsEntity);
		StartThinking ();
	}
	public override void OnDeath () {
		base.OnDeath ();
		StopThinking ();
	}

	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject);
		SetUpSensoryInfo ();
		SetUpBehaviours ();
		Paused = true;
	}
	#endregion
	#region Public Methods

	private void StartThinking () {
		isThinking = true;
		StartCoroutine (ThinkRoutine ());
	}

	private void StopThinking () {
		isThinking = false;
		if (sensoryInfo.currentAbilityTask != null) {
			sensoryInfo.currentAbilityTask.InterruptBehaviour ();
		}
		StopCoroutine (ThinkRoutine ());
	}
	#endregion

	#region Private Initialization Methods
	private void SetUpSensoryInfo () {
		sensoryInfo.trans = this.transform;
		sensoryInfo.lookTransform = new GameObject ("LookTransform").transform;
		sensoryInfo.lookTransform.parent = sensoryInfo.trans;
		sensoryInfo.lookTransform.localPosition = creatureBodyOffset;

		sensoryInfo.projectileTransform = new GameObject ("ProjectileTransform").transform;
		sensoryInfo.projectileTransform.parent = sensoryInfo.lookTransform;
		sensoryInfo.projectileTransform.localPosition = new Vector3 (0.25f, 0, 0);
		sensoryInfo.vitalsEntity = babyBrainsObject.vitalsEntity;
		sensoryInfo.targetVitals = null;

	}
	private void SetUpBehaviours () {
		BabyBrainsBehaviour[] behaviours = GetComponentsInChildren<BabyBrainsBehaviour> ();
		for (int i = 0; i < behaviours.Length; i++) {
			RegisterBehaviour (behaviours [i]);
			behaviours [i].SetUpBehaviour (sensoryInfo);
		}
	}
	private void RegisterBehaviour (BabyBrainsBehaviour behaviour) {
		if (behaviour.behaviourData.behaviourType == BehaviourType.MOVEMENT) {
			movementBehaviourPq.Insert (behaviour, behaviour.behaviourData.Weight);
		} else {
			abilityBehaviourPq.Insert (behaviour, behaviour.behaviourData.Weight);
		}
	}
	#endregion

	#region Think Routines
	//THINK ROUTINE is responsible for validating behaviours on an interval.
	public IEnumerator ThinkRoutine () {
		//grace period for player
		yield return new WaitForSeconds (1.5f);
		while (isThinking) {
			Think ();
			yield return new WaitForSeconds (thinkInterval);
		}

	}

	private void Think () {
		GatherSensoryInfo ();
		ResetInvalidatedBehaviours ();
		UpdateCooldownTimers ();
		UpdateCurrentTaskStatus ();
	}
	#endregion

	#region Think Cycle Private Methods

	private void GatherSensoryInfo () {
		sensoryInfo.currentHealth = babyBrainsObject.vitalsEntity.health.GetValue ();
		if (sensoryInfo.vitalsEntity.resource != null) {
			sensoryInfo.currentResource = babyBrainsObject.vitalsEntity.resource.GetValue ();
		}
		if (sensoryInfo.targetVitals != null) {

			if (!sensoryInfo.targetVitals.creatureObject.isDead) {
				sensoryInfo.isoDistanceToTarget = IsometricCoordinateUtilites.IsoDistanceBetweenPoints (sensoryInfo.trans.position, sensoryInfo.targetVitals.trans.position);
				sensoryInfo.rawDistanceToTarget = IsometricCoordinateUtilites.RawDistanceBetweenPoints (sensoryInfo.trans, sensoryInfo.targetVitals.trans);
				sensoryInfo.lookTransform.right = (sensoryInfo.targetVitals.trans.position + creatureBodyOffset) - sensoryInfo.trans.position;
				sensoryInfo.targetWithinLoS = CalculateWithinLoS (sensoryInfo.lookTransform, sensoryInfo.targetVitals.trans);
			} else {
				sensoryInfo.targetVitals = null;
				sensoryInfo.isoDistanceToTarget = 100;
				sensoryInfo.rawDistanceToTarget = 100;
				sensoryInfo.targetWithinLoS = false;
				if (sensoryInfo.vitalsEntity.creatureObject.faceDirection >= 0) {
					sensoryInfo.lookTransform.right = Vector3.right;
				} else {
					sensoryInfo.lookTransform.right = Vector3.left;
				}
			}
		} else {
			sensoryInfo.isoDistanceToTarget = 100;
			sensoryInfo.rawDistanceToTarget = 100;
			sensoryInfo.lookTransform.right = Vector3.right;
			sensoryInfo.targetWithinLoS = false;
		}

	}
	private bool CalculateWithinLoS (Transform lookTransform, Transform targetTrans) {
		RaycastHit2D hit = Physics2D.Raycast(lookTransform.position, lookTransform.TransformDirection(Vector3.right), sensoryInfo.rawDistanceToTarget, layerMask);
		if (hit.collider != null && sensoryInfo.targetVitals.trans == hit.collider.transform.parent) {
			return true;
		} else return false;

	}
	private void ResetInvalidatedBehaviours () {
		for (int i = invalidatedBehaviours.Count - 1; i >= 0; i--) {
			BabyBrainsBehaviour behaviour = invalidatedBehaviours[i];
			RegisterBehaviour (behaviour);
			invalidatedBehaviours.RemoveAt (i);
		}
	}

	private void UpdateCooldownTimers () {
		for (int i = onCooldownBehaviours.Count - 1; i >= 0; i--) {
			BabyBrainsBehaviour behaviour = onCooldownBehaviours[i];
			behaviour.cdTimer -= thinkInterval;
			if (behaviour.cdTimer <= 0) {
				RegisterBehaviour (behaviour);
				onCooldownBehaviours.RemoveAt (i);
			}
		}
	}
	private void UpdateCurrentTaskStatus () {

		//Ability task logic. always sequential.
		if (sensoryInfo.currentAbilityTask != null) {
			sensoryInfo.currentAbilityTask.UpdateBehaviour (sensoryInfo, thinkInterval);
			if (sensoryInfo.currentAbilityTask.Finished ()) {
				EndTask (sensoryInfo.currentAbilityTask);
				sensoryInfo.currentAbilityTask = null;
				sensoryInfo.currentAbilityTask = AttemptBestTask (abilityBehaviourPq);
			}
		} else {
			sensoryInfo.currentAbilityTask = AttemptBestTask (abilityBehaviourPq);
		}

		//Movement task logic. looks for potential overrides, but otherwise updates the current movement task.

		if (sensoryInfo.currentMovementTask == null) {
			sensoryInfo.currentMovementTask = AttemptBestTask (movementBehaviourPq);
		} else {
			BabyBrainsBehaviour bestPotentialMovement = PreviewBestValid(movementBehaviourPq);
			if (bestPotentialMovement != null) {
				if (bestPotentialMovement.behaviourData.Weight < sensoryInfo.currentMovementTask.behaviourData.Weight) {
					EndTask (sensoryInfo.currentMovementTask);
					sensoryInfo.currentMovementTask = bestPotentialMovement;
					bestPotentialMovement.OnTaskStart (sensoryInfo);
				} else {
					invalidatedBehaviours.Add (bestPotentialMovement);
				}
			}
			sensoryInfo.currentMovementTask.UpdateBehaviour (sensoryInfo, thinkInterval);
			if (sensoryInfo.currentMovementTask.Finished ()) {
				EndTask (sensoryInfo.currentMovementTask);
				sensoryInfo.currentMovementTask = null;
				sensoryInfo.currentMovementTask = AttemptBestTask (movementBehaviourPq);
			}
		}
	}
	private BabyBrainsBehaviour AttemptBestTask (AIPriorityQueue pq) {
		while (!pq.isEmpty ()) {
			//choose a new behaviour to begin execution.
			BabyBrainsBehaviour topBehaviour = pq.Pop();
			if (topBehaviour.Valid (sensoryInfo)) {
				topBehaviour.OnTaskStart (sensoryInfo);
				return topBehaviour;
			} else {
				invalidatedBehaviours.Add (topBehaviour);
			}
		}
		return null;
	}

	private BabyBrainsBehaviour PreviewBestValid (AIPriorityQueue pq) {

		while (!pq.isEmpty ()) {
			//choose a new behaviour to begin execution.
			BabyBrainsBehaviour topBehaviour = pq.Pop();
			if (topBehaviour.Valid (sensoryInfo)) {
				Debug.Log ("PreviewBestValid: Valid behaviour found! Behaviour: " + topBehaviour.behaviourData.behaviourName);
				return topBehaviour;
			} else {
				invalidatedBehaviours.Add (topBehaviour);
			}
		}
		Debug.Log ("PreviewBestValid: No valid behaviours to preview. returning null.");
		return null;
	}

	//use for behaviours post-pop
	private void EndTask (BabyBrainsBehaviour behaviour) {
		behaviour.OnTaskEnd ();
		behaviour.cdTimer = behaviour.Cooldown;
		onCooldownBehaviours.Add (behaviour);
	}


	#endregion
}
