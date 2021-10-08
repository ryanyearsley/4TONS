using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitboxColliderComponent : MonoBehaviour {
	private ProjectileObject projectileObject;
	private void Awake () {
		projectileObject = GetComponentInParent<ProjectileObject> ();
	}
	protected virtual void OnTriggerEnter2D (Collider2D other) {
		if (other.tag != this.tag) {
			VitalsEntity hitTargetEntity = VitalsManager.Instance.GetVitalsEntityFromHitBox(other);
			if (hitTargetEntity != null) {
				Debug.Log ("projectile hit valid target. hitTargetEntity:" + hitTargetEntity.creatureObject.name);
				//apply spell effects
				projectileObject.OnEnemyHit (hitTargetEntity);
			} else if (other.gameObject.layer == 13) {//environment
				projectileObject.OnWallHit ();
			} else if (other.gameObject.layer == 17) {//barrier 
				projectileObject.OnBarrierHit (other.transform.parent.GetComponent<BarrierObject>());
			}
		}
	}
}
