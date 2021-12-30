using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingProjectileObject : ProjectileObject {
	[SerializeField]
	private Transform rotatingTransform;
	[SerializeField]
	private float rotateSpeed;

	private int directionModifier;

	[SerializeField]
	private SpriteRenderer projectileSprite;

	[SerializeField]
	private AnimationCurve rotateCurve;

	public override void ReuseObject () {
		float zRotation = trans.eulerAngles.z;
		Debug.Log ("Axe throw object rotation: " + zRotation);
		if (zRotation > 90 && zRotation < 270) {
			directionModifier = 1;
			projectileSprite.flipX = true;
		} else {
			directionModifier = -1;
			projectileSprite.flipX = false;
		}
	}
	public override void Update () {
		base.Update ();
		if (isAlive) {
			float currentRotateSpeed = rotateCurve.Evaluate(lifeTimer/lifeTime) * rotateSpeed;
			rotatingTransform.Rotate (new Vector3 (0, 0, currentRotateSpeed * directionModifier * Time.deltaTime), Space.Self);

		}
	}
}
