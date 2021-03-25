using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolManager : MonoBehaviour {

	private const int defaultPoolSize = 5;

	// Dictionary Queue to hold the Unique InstanceID of an object and the Queue of the object type
	private Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>> ();
	private Dictionary<int, Queue<CreatureObjectInstance>> creatureDictionary = new Dictionary<int, Queue<CreatureObjectInstance>> ();
	private Dictionary<int, Queue<SpellObjectInstance>> spellObjectDictionary = new Dictionary<int, Queue<SpellObjectInstance>> ();

	private Queue<PlayerObjectInstance> playerObjectPool = new Queue<PlayerObjectInstance> ();
	private Dictionary<int, Transform> poolHolders = new Dictionary<int, Transform> ();

	// Singleton Pattern to access this script with ease
	public static PoolManager instance;

	private void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}

	// Create a pool of gameobjects and add them to a dictionary
	public void CreateObjectPool (GameObject prefab, int poolSize) {
		int poolKey = prefab.GetInstanceID (); // GetInstanceID is a unique ID for every game object
		Transform poolParent;
		if (!poolDictionary.ContainsKey (poolKey)) {

			// Creates an empty game object to help keep track of your pool objects and parents it to the PoolManager
			poolParent = new GameObject (prefab.name + " pool").transform;
			poolParent.parent = transform;
			poolHolders.Add (poolKey, poolParent);

			poolDictionary.Add (poolKey, new Queue<ObjectInstance> ());
		} else {
			poolParent = poolHolders [poolKey];
		}

		// Create prefabs to add to pool queue.
		for (int i = 0; i < poolSize; i++) {
			ObjectInstance newObject = new ObjectInstance (Instantiate (prefab) as GameObject, poolParent); // Defaults to Vector3.zero for position and Quaternion.Identity for rotation
			poolDictionary [poolKey].Enqueue (newObject);
		}
	}
	public void CreateSpellObjectPool (GameObject prefab, int poolSize) {
		int poolKey = prefab.GetInstanceID (); // GetInstanceID is a unique ID for every game object
		Transform poolParent;
		if (!spellObjectDictionary.ContainsKey (poolKey)) {

			// Creates an empty game object to help keep track of your pool objects and parents it to the PoolManager
			poolParent = new GameObject (prefab.name + " pool").transform;
			poolParent.parent = transform;
			poolHolders.Add (poolKey, poolParent);

			spellObjectDictionary.Add (poolKey, new Queue<SpellObjectInstance> ());
		} else {
			poolParent = poolHolders [poolKey];
		}

		// Create prefabs to add to pool queue.
		for (int i = 0; i < poolSize; i++) {
			SpellObjectInstance newObject = new SpellObjectInstance (Instantiate (prefab) as GameObject, poolParent); // Defaults to Vector3.zero for position and Quaternion.Identity for rotation
			spellObjectDictionary [poolKey].Enqueue (newObject);
			newObject.SetParent (poolParent);
		}
	}
	public void CreateCreaturePool (GameObject prefab, int poolSize) {
		int poolKey = prefab.GetInstanceID (); // GetInstanceID is a unique ID for every game object
		Transform poolParent;
		if (!creatureDictionary.ContainsKey (poolKey)) {

			// Creates an empty game object to help keep track of your pool objects and parents it to the PoolManager
			poolParent = new GameObject (prefab.name + " pool").transform;
			poolParent.parent = transform;
			poolHolders.Add (poolKey, poolParent);

			creatureDictionary.Add (poolKey, new Queue<CreatureObjectInstance> ());
		} else {
			poolParent = poolHolders [poolKey];
		}

		// Create prefabs to add to pool queue.
		for (int i = 0; i < poolSize; i++) {
			CreatureObjectInstance newObject = new CreatureObjectInstance (Instantiate (prefab) as GameObject, poolParent); // Defaults to Vector3.zero for position and Quaternion.Identity for rotation
			creatureDictionary [poolKey].Enqueue (newObject);
			newObject.SetParent (poolParent);
		}
	}
	public void CreatePlayerPool (GameObject prefab, int poolSize) {
		Debug.Log ("Creating player pool");
		Transform poolParent = new GameObject ("Player pool").transform;
		for (int i = 0; i < poolSize; i++) {
			PlayerObjectInstance newObject = new PlayerObjectInstance (Instantiate (prefab) as GameObject, poolParent); // Defaults to Vector3.zero for position and Quaternion.Identity for rotation
			playerObjectPool.Enqueue (newObject);
			newObject.SetParent (poolParent);
		}
	}

	// Reuse a gameobject and places it in the desired position
	public GameObject ReuseObject (GameObject prefab, Vector3 position, Quaternion rotation) {
		int poolKey = prefab.GetInstanceID ();

		if (poolDictionary.ContainsKey (poolKey)) {

			// Dequeue then requeue the object then call the Objects Reuse function
			ObjectInstance objectToReuse = poolDictionary [poolKey].Dequeue ();
			poolDictionary [poolKey].Enqueue (objectToReuse);

			objectToReuse.Reuse (position, rotation);
			return objectToReuse.gameObject;
		} else {
			CreateObjectPool (prefab, defaultPoolSize);
			return ReuseObject (prefab, position, rotation);
		}
	}
	public GameObject ReuseCreatureObject (GameObject prefab, Vector3 position) {
		int poolKey = prefab.GetInstanceID ();

		if (creatureDictionary.ContainsKey (poolKey)) {

			// Dequeue then requeue the object then call the Objects Reuse function
			CreatureObjectInstance objectToReuse = creatureDictionary [poolKey].Dequeue ();
			creatureDictionary [poolKey].Enqueue (objectToReuse);
			objectToReuse.Reuse (position);
			return objectToReuse.gameObject;
		} else {
			CreateCreaturePool (prefab, defaultPoolSize);
			return ReuseCreatureObject (prefab, position);
		}
	}
	public GameObject ReusePlayerObject (Vector3 position, Player player) {
			// Dequeue then requeue the object then call the Objects Reuse function
			PlayerObjectInstance objectToReuse = playerObjectPool.Dequeue ();
			playerObjectPool.Enqueue (objectToReuse);

			objectToReuse.Reuse (position, player);
			return objectToReuse.gameObject;
	}
	public GameObject ReuseSpellObject (GameObject prefab, Vector3 position, Quaternion rotation, VitalsEntity vitalsEntity) {
		int poolKey = prefab.GetInstanceID ();

		if (spellObjectDictionary.ContainsKey (poolKey)) {

			// Dequeue then requeue the object then call the Objects Reuse function
			SpellObjectInstance objectToReuse = spellObjectDictionary [poolKey].Dequeue ();
			spellObjectDictionary [poolKey].Enqueue (objectToReuse);

			objectToReuse.Reuse (position, rotation, vitalsEntity);
			return objectToReuse.gameObject;
		} else {
			CreateSpellObjectPool (prefab, defaultPoolSize);
			return ReuseSpellObject (prefab, position, rotation, vitalsEntity);
		}
	}

}