﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueTargetBehaviour : BabyBrainsBehaviour
{
	[SerializeField]
	private float aggroDistance;
	[SerializeField]
	private float breakAggroDistance;

	private PathfindingComponent aStarUnit;

	void Awake () {
		aStarUnit = GetComponentInParent<PathfindingComponent> ();
	}
	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetVitals.trans != null && sensoryInfo.distanceToTarget < aggroDistance && aStarUnit.canFollow) {
			return true;
		} else return false;
	}

	public override void OnTaskStart(SensoryInfo sensoryInfo) {
		Debug.Log ("Executing pursue target");
		_finished = false;
		cdTimer = Cooldown;
		aStarUnit.AutoFollowTarget (sensoryInfo.targetVitals.trans);
	}
	public override void UpdateBehaviour (SensoryInfo sensoryInfo, float interval) {
		Debug.Log ("Updating pursue target");
		if (sensoryInfo.distanceToTarget > breakAggroDistance || !aStarUnit.followingPath) {
			_finished = true;
		}
	}
	public override void OnTaskEnd () {
		base.OnTaskEnd ();
		aStarUnit.StopPathfinding ();
	}
}
