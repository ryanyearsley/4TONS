
using UnityEngine;

public class ObjectInstance {
	public GameObject gameObject;
	private Transform transform;
	private Transform poolParentTransform;

	private bool hasPoolObjectComponent;
	private IPoolable poolObjectScript;
	private bool hasSpellObjectComponent;
	private ISpellObject spellObjectScript;

	public ObjectInstance (GameObject objectInstance) {
		gameObject = objectInstance;
		transform = gameObject.transform;
		gameObject.SetActive (false);

		// Keep track if any of the objects scripts inherit the PoolObject script
		if (gameObject.GetComponent<IPoolable> () != null) {
			hasPoolObjectComponent = true;
			poolObjectScript = gameObject.GetComponent<IPoolable> ();
			poolObjectScript.SetupObject (poolParentTransform);
		}

		if (gameObject.GetComponent<ISpellObject> () != null) {
			hasSpellObjectComponent = true;
			spellObjectScript = gameObject.GetComponent<ISpellObject> ();
		}
	}

	// Method called when an ObjectInstance is being reused
	public void Reuse (Vector3 position, Quaternion rotation) {

		// Reset the object as specified within it's own class and the PoolObject class

		// Move to desired position then set it active
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		gameObject.transform.parent = null;
		gameObject.SetActive (true);

		if (hasPoolObjectComponent) {
			poolObjectScript.ReuseObject ();
		}
	}

	public void ReuseSpellObject (Vector3 position, Quaternion rotation, VitalsEntity casterVitals) {

		// Reset the object as specified within it's own class and the PoolObject class

		// Move to desired position then set it active
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		gameObject.transform.parent = null;
		gameObject.SetActive (true);
		gameObject.tag = casterVitals.tag;
		if (hasSpellObjectComponent) {
			spellObjectScript.ReuseSpellObject (casterVitals);
		}
	}

	// Set the parent of the Object to help group objects properly
	public void SetParent (Transform parent) {
		transform.parent = parent;
		poolParentTransform = parent;
	}
}
