using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleWanderBehaviour : BabyBrainsBehaviour
{
	
	private Transform idleWanderTransform;

	[SerializeField]
	private int wanderDistance;

	private PathfindingComponent aStarUnit;

	void Awake () {
		aStarUnit = GetComponentInParent<PathfindingComponent> ();
		idleWanderTransform = new GameObject (transform.parent.name + " Idle Wander Target").transform;
		idleWanderTransform.parent = this.transform;
		idleWanderTransform.localPosition = Vector3.zero;
	}	

	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetPositionTransform == null) {
			return true;
		} else return false;
	}

	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		base.OnTaskStart (sensoryInfo);
		Debug.Log ("Executing idle wander");

		idleWanderTransform.parent = WaypointTransformHolder.instance.trans;
		idleWanderTransform.position = LevelManager.instance.RandomNearbyCoordinatePosition (sensoryInfo.trans.position, wanderDistance);
		aStarUnit.GoToPosition (idleWanderTransform);
	}

	public override void UpdateBehaviour (SensoryInfo sensoryInfo, float interval) {
		executionTimer -= interval;
		if (executionTimer <= 0) {
			_finished = true;
		} else if (!aStarUnit.followingPath) {
			_finished = true;
		}
	}
	public override void OnTaskEnd () {
		base.OnTaskEnd ();
		idleWanderTransform.parent = this.transform;
		idleWanderTransform.localPosition = Vector3.zero;
		aStarUnit.StopPathfinding ();
	}

}
