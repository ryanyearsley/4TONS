using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PentagramObject : AoEObject
{
	[SerializeField]
	private GameObject debrisObjectPrefab;
	[SerializeField]
	private int killCount;

	private List<Vector3> killLocations = new List<Vector3>();
	public float healPerKill;

	public override void ReuseSpellObject (VitalsEntity vitalsEntity) {
		base.ReuseSpellObject (vitalsEntity);
		killCount = 0;
		killLocations.Clear ();
	}


	protected override void EnemyTick (VitalsEntity vitalsEntity) {
		Debug.Log ("PentagramObject: Enemy tick");
		if (vitalsEntity.creatureObject.isDead) {
			Debug.Log ("PentagramObject: DEATH DETECTED! Removing from list.");
			MarkKill (vitalsEntity);
			inAreaTargets.Remove (vitalsEntity);
		} else {
			OnEnemyHit (vitalsEntity);
		}
	}

	private void MarkKill (VitalsEntity deadEnemyVitals) {
		Debug.Log ("PentagramObject: Kill Marked!");
		killCount++;
		killLocations.Add (deadEnemyVitals.creatureObject.transform.position);
	}

	public override void TerminateObjectFunctions () {
		for (int i = inAreaTargets.Count - 1; i >= 0; i--) {
			if (inAreaTargets [i].creatureObject.isDead) {
				MarkKill (inAreaTargets [i]);
			}
		}

		foreach (Vector3 killLocation in killLocations) {
			PoolManager.instance.ReuseObject (debrisObjectPrefab, killLocation, Quaternion.identity);
		}
		casterVitalsEntity.health.Heal (killCount * healPerKill);
		killLocations.Clear ();
		base.TerminateObjectFunctions ();
	}
}
