using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEObject : SpellObject {
	List<VitalsEntity> inAreaTargets = new List<VitalsEntity>();
	List<VitalsEntity> inAreaAllies = new List<VitalsEntity>();

	//Default: 10x a second
	private float tickInterval = 0.25f;

	public override void ReuseSpellObject (VitalsEntity vitalsEntity) {
		base.ReuseSpellObject (vitalsEntity);
		StartCoroutine (AoETickRoutine ());
	}
	IEnumerator AoETickRoutine () {
		for (; ; ) {
			for (int i = inAreaTargets.Count - 1; i >= 0; i--) {
				EnemyTick (inAreaTargets [i]);
			}

			for (int i = inAreaAllies.Count - 1; i >= 0; i--) {
				AlliedTick (inAreaAllies [i]);
			}
			yield return new WaitForSeconds (tickInterval);
		}
	}

	private void EnemyTick(VitalsEntity vitalsEntity) {
		if (vitalsEntity.creatureObject.isDead) {
			inAreaTargets.Remove (vitalsEntity);
		} else {
			if (vitalsEntity.health != null)
				vitalsEntity.health.ApplyDamage (spellObjectData.damage * tickInterval);
			if (vitalsEntity.resource != null)
				vitalsEntity.resource.ApplyResourceDamage (spellObjectData.manaDamage * tickInterval);
			if (vitalsEntity.movement != null)
				vitalsEntity.movement.OnAddDebuff (new SpeedAlteringEffect (spellObjectData.enemySpeedReduction, tickInterval, true));
		}
	}
	private void AlliedTick(VitalsEntity vitalsEntity) {
		if (vitalsEntity.creatureObject.isDead) {
			inAreaAllies.Remove (vitalsEntity);
		} else {
			if (vitalsEntity.health != null) {
				Debug.Log ("heal friendly tick");
				vitalsEntity.health.Heal (spellObjectData.casterHealthHealAmount * tickInterval);
			}
			if (vitalsEntity.resource != null)
				vitalsEntity.resource.RegenerateMana (spellObjectData.casterManaRegenAmount * tickInterval);
			if (vitalsEntity.movement != null)
				vitalsEntity.movement.OnAddDebuff (new SpeedAlteringEffect (spellObjectData.casterSpeedIncrease, tickInterval, true));
		}
	}
	public override void TerminateObjectFunctions () {
		StopCoroutine (AoETickRoutine ());
		inAreaTargets.Clear ();
		inAreaAllies.Clear ();
	}
	private void OnTriggerEnter2D (Collider2D other) {
		VitalsEntity vitals = VitalsManager.Instance.GetVitalsEntitybyID (other.transform.GetInstanceID ());
		if (vitals != null) {
			if (vitals != casterVitalsEntity) {
				inAreaTargets.Add (vitals);
				//EnemyTick (vitals);
			} else {
				inAreaAllies.Add (vitals);
				//AlliedTick (vitals);
			}
		}

	}
	private void OnTriggerExit2D (Collider2D other) {
		VitalsEntity vitals = VitalsManager.Instance.GetVitalsEntitybyID (other.transform.GetInstanceID ());
		Debug.Log (other.gameObject.name + " leaving AoE ");
		if (vitals != null) {
			if (vitals != casterVitalsEntity)
				inAreaTargets.Remove (vitals);
			else {
				inAreaAllies.Remove (vitals);
			}
		}
	}
}
