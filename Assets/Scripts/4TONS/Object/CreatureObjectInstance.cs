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
		go.transform.position = position;
		go.transform.rotation = Quaternion.identity;
		go.transform.parent = null;
		go.SetActive (true);

		if (creatureObj != null) {
			creatureObj.ReuseObject ();
		}
	}
}
