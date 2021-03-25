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
		idleWanderTransform = new GameObject ("Idle Wander Target").transform;
		idleWanderTransform.parent = this.transform;
		idleWanderTransform.localPosition = Vector3.zero;
	}	

	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetPositionTransform == null && aStarUnit.canFollow) {
			return true;
		} else return false;
	}

	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		base.OnTaskStart (sensoryInfo);
		Debug.Log ("Executing idle wander");
		idleWanderTransform.parent = null;
		idleWanderTransform.position = RandomNearbyLocation (sensoryInfo.trans.position);
		aStarUnit.GoToPosition (idleWanderTransform);
	}

	public override void UpdateBehaviour (SensoryInfo sensoryInfo, float interval) {
		executionTimer -= interval;
		if (executionTimer <= 0) {
			aStarUnit.StopPathfinding ();
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
	private Vector3 RandomNearbyLocation(Vector3 position) {

		bool vacantLocation = false;

		Vector3 randomPosition = Vector3.zero;
		while (!vacantLocation) {
			float randomX = position.x + Random.Range (-wanderDistance, wanderDistance);
			float randomY = position.y + Random.Range (-wanderDistance, wanderDistance);
			randomPosition = new Vector3 (randomX, randomY, 0);
			PathfindingNode node = LevelManager.instance.NodeFromWorldPoint(randomPosition);
			if (node != null && node.walkable) {
				vacantLocation = true;
				randomPosition = node.worldPosition;
			}
		}

		return randomPosition;


	}

}
