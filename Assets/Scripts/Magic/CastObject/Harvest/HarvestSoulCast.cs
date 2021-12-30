using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestSoulCast : Spell {
	[SerializeField]
	private float totalManaRegen = 35;

	[SerializeField]
	private float manaRegenTimeDuration = 5f;

	[SerializeField]
	private float manaRegenInterval =  0.5f;
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
			Debug.Log ("HarvestSoulSpell: Looking for vitals on object: " + collider.name);
			if (collider != null) {
				VitalsEntity potentialCorpsevitals = VitalsManager.Instance.GetVitalsEntityFromCorpse(collider);
				if (potentialCorpsevitals != null && potentialCorpsevitals.creatureObject.isDead) {
					//Found a dead one! let's sacrifice it...
					base.CastSpell ();
					StartCoroutine (ManaOverTimeRoutine ());
					PoolManager.instance.ReuseObject (spellData.spellObject, potentialCorpsevitals.creatureObject.transform.position, Quaternion.identity);
					PoolManager.instance.ReuseObject (corpseObject, potentialCorpsevitals.creatureObject.transform.position, Quaternion.identity);
					potentialCorpsevitals.creatureObject.Destroy ();
					return;
				}
			}
		}
	}

	public IEnumerator ManaOverTimeRoutine () {
		float tickCount = manaRegenTimeDuration / manaRegenInterval;
		float manaRegenPerTick = totalManaRegen / tickCount;

		for (int i = 0; i < tickCount; i++) {
			if (!playerObject.isDead) {
				playerObject.vitalsEntity.resource.RegenerateMana (manaRegenPerTick);
			} else {
				StopCoroutine (ManaOverTimeRoutine ());
			}
			yield return new WaitForSeconds (manaRegenInterval);
		}
	}

}
