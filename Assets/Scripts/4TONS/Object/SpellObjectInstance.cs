using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellObjectInstance : ObjectInstance {
	private SpellObject spellObject;
	public SpellObjectInstance (GameObject obj, Transform parentTransform) : base (obj, parentTransform) {

		spellObject = gameObject.GetComponent<SpellObject> ();

		
	}
	public void Reuse (Vector3 position, Quaternion rotation, VitalsEntity casterVitals) {
		base.Reuse (position, rotation);
		if (spellObject != null) {
			spellObject.ReuseSpellObject (casterVitals);
		}
	}
}
