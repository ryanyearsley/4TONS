using System;
using UnityEngine;

[Serializable]
public class StaffSpawnPoint : SpawnPoint {
	public PuzzleData puzzleData;

	public StaffSpawnPoint (Vector2Int coord, SpawnObjectData spawnObjectData, PuzzleData puzzleData) : base (coord, null) {
		this.spawnCoordinate = coord;
		this.spawnObjectData = spawnObjectData;
		this.puzzleData = puzzleData;
		this.value = puzzleData.id;
	}
}