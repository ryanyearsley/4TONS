using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestFleshCast : Spell {
	[SerializeField]
	private float totalHeal = 35;

	[SerializeField]
	private float healTimeDuration = 5f;

	[SerializeField]
	private float healInterval =  0.5f;

	[SerializeField]
	private GameObject corpseObject;

	[SerializeField]
	private ContactFilter2D contactFilter;

	public override void SetUpSpell () {
		if (spellData.spellObject != null) {
			PoolManager.instance.CreateObjectPool (spellData.spellObject, spellData.poolSize);
		}
		if (corpseObject != null)
			PoolManager.instance.CreateObjectPool (corpseObject, 2);
	}
	public override void CastSpell () {
		List<Collider2D> overlapResults = new List<Collider2D>();
		Physics2D.OverlapCircle (spellCastTransform.position, 1f, contactFilter, overlapResults);
		foreach (Collider2D collider in overlapResults) {
			Debug.Log ("BloodSacrificeSpell: Looking for vitals on object: " + collider.name);
			if (collider != null) {
				VitalsEntity potentialCorpsevitals = VitalsManager.Instance.GetVitalsEntityFromCorpse(collider);
				if (potentialCorpsevitals != null && potentialCorpsevitals.creatureObject.isDead) {
					//Found a dead one! let's sacrifice it...
					base.CastSpell ();
					StartCoroutine (HealOverTimeRoutine ());
					PoolManager.instance.ReuseObject (spellData.spellObject, potentialCorpsevitals.creatureObject.transform.position, Quaternion.identity);
					PoolManager.instance.ReuseObject (corpseObject, potentialCorpsevitals.creatureObject.transform.position, Quaternion.identity);
					potentialCorpsevitals.creatureObject.Destroy ();
					return;
				}
			}
		}
	}

	public IEnumerator HealOverTimeRoutine() {
		float tickCount = healTimeDuration / healInterval;
		float healPerTick = totalHeal / tickCount;

		for (int i = 0; i < tickCount; i++) {
			playerObject.vitalsEntity.health.Heal (healPerTick);
			yield return new WaitForSeconds (healInterval);
		}
	}
}
