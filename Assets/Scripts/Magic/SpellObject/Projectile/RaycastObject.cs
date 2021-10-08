using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastObject : SpellObject
{

	[SerializeField]
	private LineRenderer lineRenderer;

	[SerializeField]
	private GameObject debrisObject;

	public LayerMask layerMask;

	public override void SetupObject () {
		base.SetupObject ();
		PoolManager.instance.CreateObjectPool (debrisObject, 2);
	}

	public override void ReuseSpellObject (VitalsEntity vitalsEntity) {
		base.ReuseSpellObject (vitalsEntity);
		RaycastHit2D rayHit = Physics2D.Raycast(trans.position, trans.right, 50, layerMask);
		if (rayHit.collider != null) {
			OnHitEnemy (rayHit.point, VitalsManager.Instance.GetVitalsEntityFromHitBox (rayHit.collider));
		}
		else {
			OnHitWall (rayHit.point);
		}
	}

	public void OnHitEnemy (Vector2 position, VitalsEntity enemyVitals) {
		lineRenderer.SetPosition (0, trans.position);
		lineRenderer.SetPosition (1, position);
		OnEnemyHit (enemyVitals);
		PlaceDebrisObject (position);
		Debug.Log ("hit enemy");
	}
	private void OnHitWall(Vector2 position) {

		Vector2 endpoint = position + (Vector2.up * 0.325f);
		lineRenderer.SetPosition (0, trans.position);
		lineRenderer.SetPosition (1, endpoint);
		PlaceDebrisObject (endpoint);
		Debug.Log ("hit wall");
	}

	public void PlaceDebrisObject (Vector3 contactPoint) {
		if (debrisObject != null) {
			//debris objects always used at simulated floor position (below projectile hitbox)
			PoolManager.instance.ReuseObject (debrisObject, contactPoint + (Vector3.down * 0.325f), Quaternion.identity);
		}
	}
}
