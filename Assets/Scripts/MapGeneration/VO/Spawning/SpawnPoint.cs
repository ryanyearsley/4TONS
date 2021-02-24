using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint {
	public Vector2Int spawnCoordinate;
	public SpawnObjectData spawnObjectData;
	public int value;

	public SpawnPoint (Vector2Int spawnCoordinate, SpawnObjectData spawnObjectData) {
		this.spawnCoordinate = spawnCoordinate;
		this.spawnObjectData = spawnObjectData;
		if (spawnObjectData != null) 
		this.value = spawnObjectData.id;
	}
}
