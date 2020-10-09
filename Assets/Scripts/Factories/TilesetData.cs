using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Tileset", menuName = "ScriptableObjects/Tileset Data")]
public class TilesetData : ScriptableObject, IEnumerable<GameObject> {

	public List<GameObject> tilePrefabs;
	public List<GameObject> setPiecePrefabs;

	public GameObject this [int index] {
		get {
			if (index < tilePrefabs.Count)
				return tilePrefabs [index];
			else
				return setPiecePrefabs [index - tilePrefabs.Count];
		}
	}

	public int Count {
		get {
			return tilePrefabs.Count + setPiecePrefabs.Count;
		}
	}

	public IEnumerator<GameObject> GetEnumerator () {
		return tilePrefabs.GetEnumerator ();
	}

	IEnumerator IEnumerable.GetEnumerator () {
		return GetEnumerator ();
	}
}