using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellObjectInstance : ObjectInstance {
	private SpellObject spellObject;
	public SpellObjectInstance (GameObject obj, Transform parentTransform) : base (obj, parentTransform) {

		spellObject = go.GetComponent<SpellObject> ();

		
	}
	public void Reuse (Vector3 position, Quaternion rotation, VitalsEntity casterVitals) {
		
		base.Reuse (position, rotation);
		if (spellObject != null) {
			int id = go.GetInstanceID();
			spellObject.SetSpellObjectTag (casterVitals.tag);
			spellObject.ReuseSpellObject (casterVitals);
		}

	}
}
