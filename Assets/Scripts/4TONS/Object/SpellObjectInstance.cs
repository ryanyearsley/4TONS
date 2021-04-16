using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellObjectInstance : ObjectInstance {
	private SpellObject spellObject;
	public SpellObjectInstance (GameObject obj, Transform parentTransform) : base (obj, parentTransform) {

		spellObject = go.GetComponent<SpellObject> ();

		
	}
	public void Reuse (Vector3 position, Quaternion rotation, VitalsEntity casterVitals) {
		Debug.Log ("spellObjectInstance.reuse");
		if (spellObject != null) {
			int id = go.GetInstanceID();
			Debug.Log (" spell object [" + id + "] setting tags to " + casterVitals.tag + " before activation.");
			spellObject.SetSpellObjectTag (casterVitals);
		}
		base.Reuse (position, rotation);
		spellObject.ReuseSpellObject (casterVitals);

	}
}
