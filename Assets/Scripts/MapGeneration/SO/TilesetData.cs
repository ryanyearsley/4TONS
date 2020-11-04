using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu (fileName = "New Tileset", menuName = "ScriptableObjects/Tileset Data")]
public class TilesetData : ScriptableObject, IEnumerable<Tile> {

	public List<Tile> tilePrefabs;
	public List<Tile> setPiecePrefabs;

	public Tile this [int index] {
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

	public IEnumerator<Tile> GetEnumerator () {
		return tilePrefabs.GetEnumerator ();
	}

	IEnumerator IEnumerable.GetEnumerator () {
		return GetEnumerator ();
	}
}