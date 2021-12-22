using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (fileName = "NewRandomTileData", menuName = "ScriptableObjects/Environment/RandomTileData")]
public class RandomSelectTileData : ScriptableObject {
	public List<TileData> tiles;
	public TileData GetRandomTile () {
		if (tiles != null) {
			int randomTileIndex = UnityEngine.Random.Range(0, tiles.Count);
			return tiles [randomTileIndex];
		} else return null;
	}
}
