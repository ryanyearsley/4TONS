using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ( menuName = "Scriptable Object/Tileset")]
public class TilesetData : ScriptableObject, IEnumerable<GameObject> {

	[SerializeField]
	public List<GameObject> tilePrefabs;

	public GameObject this [int index] {
		get {
			return tilePrefabs [index];
		}
	}

	public int Count {
		get {
			return tilePrefabs.Count;
		}
	}

	public IEnumerator<GameObject> GetEnumerator () {
		return tilePrefabs.GetEnumerator ();
	}

	IEnumerator IEnumerable.GetEnumerator () {
		return GetEnumerator ();
	}
}