using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEHitboxColliderComponent : MonoBehaviour {
	private AoEObject aoeObject;
	private Collider2D coll;
	private Collider2D[] results = new Collider2D[16];
	[SerializeField]
	private ContactFilter2D contactFilter;
	private void Awake () {
		aoeObject = GetComponentInParent<AoEObject> ();
		coll = GetComponent<Collider2D> ();
	}/*
	private void OnEnable () {
		int colliderCount = Physics2D.OverlapCollider (coll, contactFilter, results);

		if (colliderCount > 0) {
			int overlappingColliderCount = 0;
			foreach (Collider2D overlappingCollider in results) {
				overlappingColliderCount += 1;
				Debug.Log ("overlappingColliderCount = " + overlappingColliderCount);
				VitalsEntity vitals = VitalsManager.Instance.GetVitalsEntityFromHitBox (overlappingCollider);
				if (vitals != null) {
					if (vitals != aoeObject.casterVitalsEntity) {
						aoeObject.OnEnemyEnter (vitals);
					} else {
						aoeObject.OnAllyEnter (vitals);
					}
				} else if (overlappingCollider.tag == "Environment") {
					aoeObject.OnWallHit ();
				}
			}
		}
	}*/

	protected virtual void OnTriggerEnter2D (Collider2D other) {
		VitalsEntity vitals = VitalsManager.Instance.GetVitalsEntityFromHitBox (other);

		if (vitals != null) {
			if (vitals != aoeObject.casterVitalsEntity) {
				aoeObject.OnEnemyEnter (vitals);
			} else if (vitals.tag == aoeObject.casterVitalsEntity.tag) {
				aoeObject.OnAllyEnter (vitals);
			} 
		} else if (other.tag == "Environment") {
			aoeObject.OnWallHit (other);
		}
	}

	protected virtual void OnTriggerExit2D (Collider2D other) {
		VitalsEntity vitals = VitalsManager.Instance.GetVitalsEntityFromHitBox (other);
		
		if (vitals != null && !vitals.creatureObject.isDead) {
			if (vitals != aoeObject.casterVitalsEntity) {
				aoeObject.OnEnemyExit (vitals);
			}  else if (vitals.tag == aoeObject.casterVitalsEntity.tag) {
				aoeObject.OnAllyExit (vitals);
			}
		}
	}
}
