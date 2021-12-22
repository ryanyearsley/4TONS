using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingProjectileObject : ProjectileObject {

	private Vector3 lastFramePosition;
	private Vector3 lastFrameVelocity;

	[SerializeField]
	private int bounceCountMax = 1;
	private int currentBounceCount = 0;

	public override void ReuseObject () {
		base.SetupObject ();
		currentBounceCount = 0;
	}

	public override void Update () {
		base.Update ();
		lastFrameVelocity = trans.position - lastFramePosition;
		lastFramePosition = trans.position;
	}


	public override void OnWallHit (Collider2D otherCollider) {
		if (currentBounceCount < bounceCountMax) {
			Vector2 closestWallPosition = otherCollider.ClosestPoint(this.trans.position);
			Vector2 wallNormal = (trans.position.XY() - closestWallPosition);
			wallNormal.y /= 2;
			wallNormal = wallNormal.normalized;
			Vector2 bounceDirection = Vector2.Reflect (lastFrameVelocity, wallNormal);
			float bounceAngle = Mathf.Atan2 (bounceDirection.y, bounceDirection.x) * Mathf.Rad2Deg;

			trans.rotation = Quaternion.AngleAxis (bounceAngle, Vector3.forward);
			currentBounceCount++;
		} else {
			Destroy ();
		}
	}

}
