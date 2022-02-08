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
		lineRenderer.positionCount = 0;
		RaycastProcedure ();
	}
	public void RaycastProcedure () {
		AddLineRendererPoint (trans.position);
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
	protected virtual void OnHitEnemy (Vector2 position, VitalsEntity enemyVitals) {
		AddLineRendererPoint (position);
		OnEnemyHit (enemyVitals);
		PlaceDebrisObject (position);
		Debug.Log ("hit enemy");
	}
	protected void OnHitWall(Vector2 position) {
		AddLineRendererPoint (position);
		PlaceDebrisObject (position);
		Debug.Log ("hit wall");
	}

	protected void AddLineRendererPoint(Vector2 position) {
		Debug.Log ("RaycastObject: Placing Line Renderer point at " + position);
		lineRenderer.positionCount = lineRenderer.positionCount + 1;
		lineRenderer.SetPosition (lineRenderer.positionCount - 1, position);
	}
	public void PlaceDebrisObject (Vector3 contactPoint) {
		if (debrisObject != null) {
			//debris objects always used at simulated floor position (below projectile hitbox)
			PoolManager.instance.ReuseObject (debrisObject, contactPoint, Quaternion.identity);
		}
	}
}
