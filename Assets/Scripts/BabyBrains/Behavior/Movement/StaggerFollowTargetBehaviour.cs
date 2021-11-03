using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaggerFollowTargetBehaviour : MovementBehaviour {

	private Transform staggerFollowTransform;

	[SerializeField]
	private float aggroDistance = 8f;
	[SerializeField]
	private float breakAggroDistance = 10f;
	[SerializeField]
	private int staggerVariance;

	private PathfindingComponent aStarUnit;

	void Awake () {
		aStarUnit = GetComponentInParent<PathfindingComponent> ();
		staggerFollowTransform = new GameObject (gameObject.name + "_StaggerFollowTarget").transform;
		staggerFollowTransform.parent = this.transform;
		staggerFollowTransform.localPosition = Vector3.zero;
	}

	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetVitals != null
			&& sensoryInfo.targetVitals.trans != null
			&& !sensoryInfo.targetVitals.creatureObject.isDead
			&& sensoryInfo.isoDistanceToTarget < aggroDistance) {
			return true;
		} else return false;
	}

	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		base.OnTaskStart (sensoryInfo);
		Debug.Log ("Executing staggerFollow");
		staggerFollowTransform.parent = WaypointTransformHolder.instance.trans;
		staggerFollowTransform.position = LevelManager.instance.RandomNearbyCoordinatePosition (sensoryInfo.targetVitals.trans.position, staggerVariance);
		aStarUnit.GoToPosition (staggerFollowTransform);
	}

	public override void UpdateBehaviour (SensoryInfo sensoryInfo, float interval) {
		base.UpdateBehaviour (sensoryInfo, interval);
		if (sensoryInfo.isoDistanceToTarget > breakAggroDistance
			|| !aStarUnit.followingPath
			|| sensoryInfo.targetVitals.creatureObject.isDead) {
			_finished = true;
		}
	}
	public override void OnTaskEnd () {
		base.OnTaskEnd ();
		staggerFollowTransform.parent = this.transform;
		staggerFollowTransform.localPosition = Vector3.zero;
		aStarUnit.StopPathfinding ();
	}

}
