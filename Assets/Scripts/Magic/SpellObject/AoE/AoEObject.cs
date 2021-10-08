using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEObject : SpellObject {

	//List<AoEModifier> modifiers = new List<AoEModifier>();
	List<VitalsEntity> inAreaTargets = new List<VitalsEntity>();
	List<VitalsEntity> inAreaAllies = new List<VitalsEntity>();

	//Default: 10x a second
	private float tickInterval = 0.25f;

	public override void SetupObject () {
		base.SetupObject ();
	}

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

	private void EnemyTick (VitalsEntity vitalsEntity) {
		if (vitalsEntity.creatureObject.isDead) {
			inAreaTargets.Remove (vitalsEntity);
		} else {
			OnEnemyHit (vitalsEntity);
		}
	}
	private void AlliedTick (VitalsEntity vitalsEntity) {
		if (vitalsEntity.creatureObject.isDead) {
			inAreaAllies.Remove (vitalsEntity);
		} else {
			OnAllyHit (vitalsEntity);
		}
	}

	public virtual void OnEnemyEnter (VitalsEntity enemyVitals) {
		inAreaTargets.Add (enemyVitals);
	}
	public virtual void OnEnemyExit (VitalsEntity enemyVitals) {
		inAreaTargets.Remove (enemyVitals);
	}

	public virtual void OnAllyEnter (VitalsEntity allyVitals) {
		inAreaAllies.Add (allyVitals);
	}
	public virtual void OnAllyExit (VitalsEntity allyVitals) {
		inAreaAllies.Remove (allyVitals);
	}

	public override void TerminateObjectFunctions () {
		StopCoroutine (AoETickRoutine ());
		inAreaTargets.Clear ();
		inAreaAllies.Clear ();
	}

}
