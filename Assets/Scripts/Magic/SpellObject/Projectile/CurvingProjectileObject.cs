using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvingProjectileObject : ProjectileObject
{

	[SerializeField]
	private float offsetDegrees = 10;

	[SerializeField]
	private float curveSpeed;
	[SerializeField]
	private int curveDirectionModifier = 1;//default curves to the right.

	[SerializeField]
	private AnimationCurve curveTrajectory;


	public override void ReuseObject () {
		base.ReuseObject ();

		//offset in opposite direction of curve  for AI aiming. 
		float offset = offsetDegrees * (-curveDirectionModifier);
		trans.Rotate (0, 0.0f, trans.rotation.z + offset, Space.World);
	}
	public override void Update () {
		base.Update ();

		float rotationInput = curveTrajectory.Evaluate (lifeTimer / lifeTime) * curveSpeed * curveDirectionModifier * Time.deltaTime;
		trans.Rotate (0, 0.0f, trans.rotation.z + rotationInput, Space.World);
	}

}
