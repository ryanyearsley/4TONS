
using UnityEngine;

public class ObjectInstance {

	public GameObject go;
	private Transform trans;
	private Transform poolParentTransform;
	private PoolObject poolObject;
	public ObjectInstance (GameObject objectInstance, Transform poolParent) {
		go = objectInstance;
		SetParent (poolParent);
		poolParentTransform = poolParent;
		trans = go.transform;
		trans.parent = poolParentTransform;

		go.SetActive (true);
		// Keep track if any of the objects scripts inherit the PoolObject script
		if (go.GetComponent<PoolObject> () != null) {
			poolObject = objectInstance.GetComponent<PoolObject> ();
			poolObject.SetupObject ();
		}
		go.SetActive (false);
	}


	// Set the parent of the Object to help group objects properly
	public void SetParent (Transform parent) {
		poolParentTransform = parent;
	}

	// Method called when an ObjectInstance is being reused
	public virtual void Reuse (Vector3 position, Quaternion rotation) {
		Debug.Log ("base object.reuse.");
		if (go.activeInHierarchy) {
			if (poolObject != null) {
				Debug.Log ("RECYCLING ALREADY-ACTIVE GAME OBJECT. ");
				poolObject.TerminateObjectFunctions ();
				go.SetActive (false);
			}
		}
		// Move to desired position then set it active
		go.transform.position = position;
		go.transform.rotation = rotation;
		go.SetActive (true); 
		Debug.Log ("object set active.");


		if (poolObject != null) {
			poolObject.ReuseObject (); 
		}
	}

}
