using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportNearTargetBehaviour : BabyBrainsBehaviour
{
	[SerializeField]
	private float minRange = 5f;
	[SerializeField]
	private float maxRange = 10f;
	[SerializeField]
	private int tilesFromPlayerRadius;


	readonly AnimationHashID dashAnimID = new AnimationHashID("Dash");
	private AnimationComponent animationComponent;

	public override void SetUpBehaviour (SensoryInfo sensoryInfo) {
		base.SetUpBehaviour (sensoryInfo);
		animationComponent = transform.parent.GetComponentInChildren<AnimationComponent> ();
	}

	public override bool Valid (SensoryInfo sensoryInfo) {
		if (sensoryInfo.targetVitals != null
			&& sensoryInfo.targetVitals.trans != null
			&& sensoryInfo.isoDistanceToTarget > minRange
			&& sensoryInfo.isoDistanceToTarget < maxRange) {
			return true;
		} else return false;
	}
	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		base.OnTaskStart (sensoryInfo);
		Debug.Log ("TeleportNearTargetBehaviour: OnTaskStart");
		animationComponent.PlayTimedAnimation (dashAnimID, 0.5f);
		Vector3 coordPosition = LevelManager.instance.RandomNearbyCoordinatePosition (sensoryInfo.targetVitals.trans.position, tilesFromPlayerRadius);
		sensoryInfo.vitalsEntity.trans.position = new Vector3 (coordPosition.x, coordPosition.y, 0);
	}
}
