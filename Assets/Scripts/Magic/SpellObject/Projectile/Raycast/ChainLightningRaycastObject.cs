using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningRaycastObject : RaycastObject {

	[SerializeField]
	private int hitCountMax = 3;
	private int hitCount;

	[SerializeField]
	private Collider2D chainLightningSensorColl;

	[SerializeField]
	private ContactFilter2D contactFilter;

	private VitalsEntity closestEnemy;


	public override void ReuseSpellObject (VitalsEntity vitalsEntity) {
		base.ReuseSpellObject (vitalsEntity);
		hitCount = 0;
	}

	protected override void OnHitEnemy (Vector2 position, VitalsEntity enemyVitals) {
		base.OnHitEnemy (position, enemyVitals);
		hitCount++;
		if (hitCount <= hitCountMax) {
			StartCoroutine (ChainLightningRoutine (enemyVitals));
		}
	}
	public IEnumerator ChainLightningRoutine (VitalsEntity enemyVitals) {

		yield return new WaitForSeconds (0.1f);
		chainLightningSensorColl.transform.position = enemyVitals.trans.position;
		chainLightningSensorColl.transform.rotation = Quaternion.identity;

		VitalsEntity nearestEntity = FindNearbyEnemy (enemyVitals);
		if (nearestEntity != null) {
			OnHitEnemy (nearestEntity.trans.position + (Vector3.up * 0.325f), nearestEntity);
		}
	}
	public VitalsEntity FindNearbyEnemy (VitalsEntity hitEnemyVitals) {
		List<Collider2D> overlapResults = new List<Collider2D>();
		closestEnemy = null;
		Physics2D.OverlapCollider (chainLightningSensorColl, contactFilter, overlapResults);
		foreach (Collider2D collider in overlapResults) {
			if (collider != null) {
				VitalsEntity potentialNearbyEnemy = VitalsManager.Instance.GetVitalsEntityFromCorpse(collider);
				if (potentialNearbyEnemy != null
					&& potentialNearbyEnemy != hitEnemyVitals
					&& !potentialNearbyEnemy.creatureObject.isDead
					&& potentialNearbyEnemy.tag == hitEnemyVitals.creatureObject.tag) {
					closestEnemy = potentialNearbyEnemy;
				}
			}
		}
		return closestEnemy;
	}

}
