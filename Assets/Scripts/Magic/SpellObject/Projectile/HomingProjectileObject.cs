using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectileObject : ProjectileObject {
	[SerializeField]
	private float homingAngleRange;
	[SerializeField]
	private LayerMask layerMask;

	private VitalsEntity targetVitals;
	private Transform homingTargetTransform;

	[SerializeField]
	private float rotationSpeed = 1f;
	Quaternion rotateToTarget;
	Vector3 dir;




	public override void ReuseSpellObject (VitalsEntity vitalsEntity) {
		base.ReuseSpellObject (vitalsEntity);
		StartCoroutine (FindTargetRoutine ());
	}

	public override void FixedUpdate () {
		base.FixedUpdate ();

		if (targetVitals != null) {
			HomeTowardsTarget ();
		}
	}

	private IEnumerator FindTargetRoutine () {
		while (isAlive) {
			targetVitals = FindHomingTarget ();
			if (targetVitals != null) {
				break;
			}
			yield return new WaitForSeconds (0.25f);
		}
	}
	public VitalsEntity FindHomingTarget () {

		VitalsEntity hitVitals = null;
		Debug.Log ("HomingProjectileObject: Attempting to find target...");
		RaycastHit2D rayHit = Physics2D.Raycast(trans.position, trans.right, 50, layerMask);
		if (rayHit.collider != null) {
			hitVitals = VitalsManager.Instance.GetVitalsEntityFromHitBox (rayHit.collider);
			if (hitVitals != null) {
				Debug.Log ("HomingProjectileObject: Target acquired!");
				return hitVitals;
			}
		}
		return hitVitals;
	}

	private void HomeTowardsTarget () {
		dir = (targetVitals.trans.position - transform.position).normalized;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		rotateToTarget = Quaternion.AngleAxis (angle, Vector3.forward);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotateToTarget, Time.fixedDeltaTime * rotationSpeed);
	}
}
