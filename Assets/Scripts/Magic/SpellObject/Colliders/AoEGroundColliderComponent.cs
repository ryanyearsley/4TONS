using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEGroundColliderComponent : MonoBehaviour {
	private AoEObject aoeObject;
	private Collider2D coll;
	Collider2D[] results = new Collider2D[16];
	private ContactFilter2D contactFilter;
	private void Awake () {
		aoeObject = GetComponentInParent<AoEObject> ();
		coll = GetComponent<Collider2D> ();
	}
/*
	private void OnEnable () {
		int colliderCount = Physics2D.OverlapCollider (coll, contactFilter, results);

		if (colliderCount > 0) {
			foreach (Collider2D overlappingCollider in results) {
				VitalsEntity vitals = VitalsManager.Instance.GetVitalsEntityFromFeet (overlappingCollider);
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
		VitalsEntity vitals = VitalsManager.Instance.GetVitalsEntityFromFeet (other);

		if (vitals != null) {
			if (vitals != aoeObject.casterVitalsEntity) {
				aoeObject.OnEnemyEnter (vitals);
			} else {
				aoeObject.OnAllyEnter (vitals);
			}
		} else if (other.tag == "Environment") {
			aoeObject.OnWallHit ();
		}
	}

	protected virtual void OnTriggerExit2D (Collider2D other) {
		VitalsEntity vitals = VitalsManager.Instance.GetVitalsEntityFromFeet (other);

		if (vitals != null) {
			if (vitals.factionTag != aoeObject.casterVitalsEntity.factionTag) {
				aoeObject.OnEnemyExit (vitals);
			} else {
				aoeObject.OnAllyExit (vitals);
			}
		}
	}
}
