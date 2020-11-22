using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEObject : AbstractSpellObject {
	List<VitalsEntity> inAreaTargets = new List<VitalsEntity>();

	public override void ReuseObject () {
		base.ReuseObject ();
		StartCoroutine (AoETickRoutine ());
	}
	IEnumerator AoETickRoutine () {
		for (; ; ) {
			foreach(VitalsEntity vitalsEntity in inAreaTargets) {
				vitalsEntity.iDamageable.ApplyDamage (spellObjectData.damage);
			}
			yield return new WaitForSeconds (1f);
		}
	}

	public override void TerminateObjectFunctions () {
		StopCoroutine (AoETickRoutine ());
		inAreaTargets.Clear ();
	}
	private void OnTriggerEnter2D (Collider2D other) {
		VitalsEntity vitals = VitalsManager.Instance.GetObjectByID (other.gameObject.GetInstanceID ());
		if (vitals != null) {
			inAreaTargets.Add (vitals);
		}

	}
	private void OnTriggerExit2D (Collider2D other) {
		VitalsEntity vitals = VitalsManager.Instance.GetObjectByID (other.gameObject.GetInstanceID ());
		if (vitals != null) {
			inAreaTargets.Remove (vitals);
		}
	}
}
