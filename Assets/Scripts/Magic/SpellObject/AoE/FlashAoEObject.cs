using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashAoEObject : SpellObject {


	[SerializeField]
	private Collider2D flashAoECollider;

	[SerializeField]
	private float animationCollisionTime = 0.25f;

	private GameObject debrisObjectPrefab;

	[SerializeField]
	private ContactFilter2D contactFilter;


	public override void SetupObject () {
		base.SetupObject ();
			//PoolManager.instance.CreateObjectPool (debrisObject, 2);
	}
	public override void ReuseSpellObject (VitalsEntity vitalsEntity) {
		base.ReuseSpellObject (vitalsEntity);
		StartCoroutine (FlashAoERoutine ());
	}

	public IEnumerator FlashAoERoutine () {
		yield return new WaitForSeconds (animationCollisionTime);
		List<Collider2D> overlapResults = new List<Collider2D>();
		Physics2D.OverlapCollider (flashAoECollider, contactFilter, overlapResults);
		foreach (Collider2D collider in overlapResults) {
			Debug.Log ("BloodSacrificeSpell: Looking for vitals on object: " + collider.name);
			VitalsEntity hitTargetVitals = VitalsManager.Instance.GetVitalsEntityFromFeet(collider);
			if (hitTargetVitals != null && hitTargetVitals.tag != casterVitalsEntity.tag) {
				OnEnemyHit (hitTargetVitals);
				//hitTargetVitals.creatureObject.Destroy ();
				//PoolManager.instance.ReuseObject (debrisObject, trans.position, Quaternion.identity);
			}
		}
	}
}
