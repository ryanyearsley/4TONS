using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureObjectInstance : ObjectInstance
{
	private CreatureObject creatureObj;
	public CreatureObjectInstance (GameObject obj, Transform parentTransform) : base (obj, parentTransform) {
		creatureObj = obj.GetComponent<CreatureObject> ();
	}
	public void Reuse (Vector3 position) {

		// Reset the object as specified within it's own class and the PoolObject class

		// Move to desired position then set it active
		gameObject.transform.position = position;
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.transform.parent = null;
		gameObject.SetActive (true);

		if (creatureObj != null) {
			creatureObj.ReuseObject ();
		}
	}
}
