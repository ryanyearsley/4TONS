
using UnityEngine;

public class ObjectInstance {
	public GameObject gameObject;
	private Transform trans;
	private Transform poolParentTransform;
	private PoolObject poolObject;
	public ObjectInstance (GameObject objectInstance, Transform poolParent) {
		gameObject = objectInstance;
		SetParent (poolParent);
		poolParentTransform = poolParent;
		trans = gameObject.transform;
		trans.parent = poolParentTransform;

		gameObject.SetActive (true);
		// Keep track if any of the objects scripts inherit the PoolObject script
		if (gameObject.GetComponent<PoolObject> () != null) {
			poolObject = objectInstance.GetComponent<PoolObject> ();
			poolObject.SetupObject ();
		}
		gameObject.SetActive (false);
	}


	// Set the parent of the Object to help group objects properly
	public void SetParent (Transform parent) {
		poolParentTransform = parent;
	}

	// Method called when an ObjectInstance is being reused
	public virtual void Reuse (Vector3 position, Quaternion rotation) {

		// Reset the object as specified within it's own class and the PoolObject class

		// Move to desired position then set it active
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		gameObject.SetActive (true);

		if (poolObject != null) {
			poolObject.ReuseObject ();
		}
	}

}
