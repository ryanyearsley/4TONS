using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastObject : SpellObject {

	[SerializeField]
	protected float raycastDistance = 50f;

[SerializeField]
	protected LineRenderer lineRenderer;
	[SerializeField]
	protected LayerMask layerMask;

	[SerializeField]
	protected float accuracy = 0;
	public override void SetupObject () {
		base.SetupObject ();
		PoolManager.instance.CreateObjectPool (debrisObject, 2);
	}

	public override void ReuseSpellObject (VitalsEntity vitalsEntity) {
		base.ReuseSpellObject (vitalsEntity);
		float randomRotation = Random.Range(-accuracy, accuracy);
		trans.Rotate (0, 0.0f, trans.rotation.z + randomRotation, Space.World);
		RaycastProcedure ();
	}
	public void RaycastProcedure () {

		RaycastHit2D rayHit = Physics2D.Raycast(trans.position, trans.right, raycastDistance, layerMask);
		if (rayHit.collider != null) {
			VitalsEntity colliderVitals = VitalsManager.Instance.GetVitalsEntityFromHitBox (rayHit.collider);
			if (colliderVitals != null) {
				OnHitEnemy (rayHit.point, VitalsManager.Instance.GetVitalsEntityFromHitBox (rayHit.collider));
			} else {
				OnHitWall (rayHit.point);
			}
		} else {
			OnHitWall (rayHit.point);
		}
	}
	protected void OnHitEnemy (Vector2 position, VitalsEntity enemyVitals) {
		lineRenderer.SetPosition (0, trans.position);
		lineRenderer.SetPosition (1, position);
		OnEnemyHit (enemyVitals);
		PlaceDebrisObject (position);
		Debug.Log ("hit enemy");
	}
	protected void OnHitWall(Vector2 position) {

		Vector2 endpoint = position;
		lineRenderer.SetPosition (0, trans.position);
		lineRenderer.SetPosition (1, endpoint);
		PlaceDebrisObject (endpoint);
		Debug.Log ("hit wall");
	}

	public void PlaceDebrisObject (Vector3 contactPoint) {
		if (debrisObject != null) {
			//debris objects always used at simulated floor position (below projectile hitbox)
			PoolManager.instance.ReuseObject (debrisObject, contactPoint, Quaternion.identity);
		}
	}
}
