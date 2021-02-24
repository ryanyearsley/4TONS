using System.Collections.Generic;
using UnityEngine;

public class SpawnUtility
{
	#region Spawn Generation
	public static List<SpawnPoint> GenerateCreatureSpawnPoints (MapDetails details, CreatureSpawnInfo creatureSpawnInfo) {
		List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
		SpawnObjectData spawnObjectData = creatureSpawnInfo.spawnObjectData;
		for (int i = 0; i < creatureSpawnInfo.spawnCount; i++) {
			bool spawnPointAdded = false;
			while (spawnPointAdded == false) {
				int randomX = Random.Range (1, details.mapData.mapSize.x - 1);
				int randomY = Random.Range (1, details.mapData.mapSize.y - 1);

				if (details.mapTileInfo [randomX, randomY].value == 0) {
					if (MapUtility.CheckSpawnPointEligibility (details, new Vector2Int (randomX, randomY), spawnObjectData.clearance)) {
						SpawnPoint spawnPoint = new SpawnPoint (new Vector2Int (randomX, randomY), spawnObjectData);
						MapUtility.ClearSpawnPointArea (details.mapTileInfo, spawnPoint);
						spawnPoints.Add (spawnPoint);
						details.mapTileInfo [randomX, randomY].value = spawnPoint.spawnObjectData.id;
						spawnPointAdded = true;
					}
				}
			}
		}
		return spawnPoints;
	}
	public static List<SpawnPoint> GenerateSetPieceSpawnPoints (MapDetails details, SetPieceSpawnInfo setPieceSpawnInfo) {
		List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
		SpawnObjectData setPieceData = setPieceSpawnInfo.setPieceData;
		for (int i = 0; i < setPieceSpawnInfo.spawnCount; i++) {
			bool spawnPointAdded = false;
			while (spawnPointAdded == false) {
				int randomX = UnityEngine.Random.Range (1, details.mapData.mapSize.x - 1);
				int randomY = UnityEngine.Random.Range (1, details.mapData.mapSize.y - 1);

				if (details.mapTileInfo [randomX, randomY].value == 0) {
					if (MapUtility.CheckSpawnPointEligibility (details, new Vector2Int (randomX, randomY), setPieceData.clearance)) {
						SpawnPoint spawnPoint = new SpawnPoint (new Vector2Int (randomX, randomY), setPieceData);
						MapUtility.ClearSpawnPointArea (details.mapTileInfo, spawnPoint);
						spawnPoints.Add (spawnPoint);
						details.mapTileInfo [randomX, randomY].value = spawnPoint.spawnObjectData.id;
						details.mapTileInfo [randomX, randomY].isSpawnConflict = true;
						spawnPointAdded = true;
					}
				}
			}
		}
		return spawnPoints;
	}
	public static List<SpellGemSpawnPoint> GenerateSpellGemSpawnPoints (MapDetails details, SpellGemSpawnInfo spellGemSpawnInfo) {
		List<SpellGemSpawnPoint> spawnPoints = new List<SpellGemSpawnPoint>();
		SpellData spellData = spellGemSpawnInfo.spellData;
		bool spawnPointAdded = false;
		int roll = UnityEngine.Random.Range (0, 100);
		Debug.Log ("rolling... " + roll + ", needed under " + spellGemSpawnInfo.dropPercentage);
		if (roll < spellGemSpawnInfo.dropPercentage)
			while (spawnPointAdded == false) {
				int randomX = UnityEngine.Random.Range (1, details.mapData.mapSize.x - 1);
				int randomY = UnityEngine.Random.Range (1, details.mapData.mapSize.y - 1);

				if (details.mapTileInfo [randomX, randomY].value == 0) {
					if (MapUtility.CheckSpawnPointEligibility (details, new Vector2Int (randomX, randomY), 1)) {
						SpellGemSpawnPoint spawnPoint = new SpellGemSpawnPoint (new Vector2Int (randomX, randomY), details.worldData.spellGemPickUpData, spellData);
						MapUtility.ClearSpellGemSpawnPointArea (details.mapTileInfo, spawnPoint);
						spawnPoints.Add (spawnPoint);
						details.mapTileInfo [randomX, randomY].value = spellData.id;
						spawnPointAdded = true;
					}
				}
			}
		return spawnPoints;
	}
	#endregion

}
