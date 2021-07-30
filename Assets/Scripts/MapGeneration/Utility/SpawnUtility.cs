using System.Collections.Generic;
using UnityEngine;

public class SpawnUtility {
	#region Spawn Generation
	public static List<SpawnPoint> GenerateCreatureSpawnPoints (MapDetails details, CreatureSpawnInfo creatureSpawnInfo) {
		List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
		SpawnObjectData spawnObjectData = creatureSpawnInfo.spawnObjectData;
		if (creatureSpawnInfo.spawnCountRange.y == 0)
			return null;

		int spawnCount = creatureSpawnInfo.GetSpawnCountWithinRange();
		for (int i = 0; i < spawnCount; i++) {
			bool spawnPointAdded = false;
			while (spawnPointAdded == false) {
				int randomX = Random.Range (2, details.mapData.mapGenerationData.mapSize.x - 2);
				int randomY = Random.Range (2, details.mapData.mapGenerationData.mapSize.y - 2);

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
	public static void GenerateTileSetpieces (MapDetails details, TileSpawnInfo tileSpawnInfo) {
		for (int i = 0; i < tileSpawnInfo.spawnCount; i++) {

			bool decorAdded = false;
			while (decorAdded == false) {
				int randomX = UnityEngine.Random.Range (1, details.mapData.mapGenerationData.mapSize.x - 1);
				int randomY = UnityEngine.Random.Range (1, details.mapData.mapGenerationData.mapSize.y - 1);
				Vector2Int randomCoord =  new Vector2Int (randomX, randomY);
				if (details.mapTileInfo [randomX, randomY].value == 0) {
					if (MapUtility.CheckSpawnPointEligibility (details, randomCoord, 1)) {
						MapUtility.ClearDecorArea (details.mapTileInfo, tileSpawnInfo.tileData.id, randomCoord, 1);
						details.mapTileInfo [randomX, randomY].value = tileSpawnInfo.tileData.id;
						details.mapTileInfo [randomX, randomY].isSpawnConflict = true;
						details.mapTileInfo [randomX, randomY].tileLayer = TileLayer.BASE;
						decorAdded = true;
					}
				}
			}
		}
	}

	public static void GenerateTopDecorTiles (MapDetails details, TileSpawnInfo tileSpawnInfo) {
		for (int i = 0; i < tileSpawnInfo.spawnCount; i++) {
			bool decorAdded = false;
			while (decorAdded == false) {
				int randomX = UnityEngine.Random.Range (1, details.mapData.mapGenerationData.mapSize.x - 1);
				int randomY = UnityEngine.Random.Range (1, details.mapData.mapGenerationData.mapSize.y - 1);

				if (details.mapTileInfo [randomX, randomY].value == 1) {
					details.mapTileInfo [randomX, randomY].value = tileSpawnInfo.tileData.id;
					details.mapTileInfo [randomX, randomY].isSpawnConflict = true;
					details.mapTileInfo [randomX, randomY].tileLayer = tileSpawnInfo.tileData.layer;
					decorAdded = true;
				}
			}
		}
	}
	public static List<SpawnPoint> GenerateSetPieceSpawnPoints (MapDetails details, SetPieceSpawnInfo setPieceSpawnInfo) {
		List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
		SpawnObjectData setPieceData = setPieceSpawnInfo.setPieceData;
		for (int i = 0; i < setPieceSpawnInfo.spawnCount; i++) {
			bool spawnPointAdded = false;
			while (spawnPointAdded == false) {
				int randomX = UnityEngine.Random.Range (5, details.mapData.mapGenerationData.mapSize.x - 5);
				int randomY = UnityEngine.Random.Range (5, details.mapData.mapGenerationData.mapSize.y - 5);

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
	public static List<SpellGemSpawnPoint> GenerateSpellGemSpawnPoints (MapDetails details, SpellData spellData) {
		List<SpellGemSpawnPoint> spawnPoints = new List<SpellGemSpawnPoint>();
		bool spawnPointAdded = false;
			while (spawnPointAdded == false) {
				int randomX = UnityEngine.Random.Range (1, details.mapData.mapGenerationData.mapSize.x - 1);
				int randomY = UnityEngine.Random.Range (1, details.mapData.mapGenerationData.mapSize.y - 1);

				if (details.mapTileInfo [randomX, randomY].value == 0) {
					if (MapUtility.CheckSpawnPointEligibility (details, new Vector2Int (randomX, randomY), 1)) {
						SpellGemSpawnPoint spawnPoint = new SpellGemSpawnPoint (new Vector2Int (randomX, randomY), ConstantsManager.instance.spellGemPickUpData, spellData);
						MapUtility.ClearSpellGemSpawnPointArea (details.mapTileInfo, spawnPoint);
						spawnPoints.Add (spawnPoint);
						details.mapTileInfo [randomX, randomY].value = spellData.id;
						spawnPointAdded = true;
					}
				}
			}
		return spawnPoints;
	}

	public static List<StaffSpawnPoint> GenerateStaffSpawnPoints (MapDetails details, StaffSpawnInfo staffSpawnInfo) {
		List<StaffSpawnPoint> spawnPoints = new List<StaffSpawnPoint>();
		PuzzleData puzzleData = staffSpawnInfo.puzzleData;
		bool spawnPointAdded = false;
		int roll = UnityEngine.Random.Range (0, 100);
		if (roll < staffSpawnInfo.dropPercentage)
			while (spawnPointAdded == false) {
				int randomX = UnityEngine.Random.Range (3, details.mapData.mapGenerationData.mapSize.x - 3);
				int randomY = UnityEngine.Random.Range (3, details.mapData.mapGenerationData.mapSize.y - 3);

				if (details.mapTileInfo [randomX, randomY].value == 0) {
					if (MapUtility.CheckSpawnPointEligibility (details, new Vector2Int (randomX, randomY), 1)) {
						StaffSpawnPoint spawnPoint = new StaffSpawnPoint (new Vector2Int (randomX, randomY), ConstantsManager.instance.staffPickUpData, puzzleData);
						MapUtility.ClearStaffSpawnPointArea (details.mapTileInfo, spawnPoint);
						spawnPoints.Add (spawnPoint);
						details.mapTileInfo [randomX, randomY].value = puzzleData.id;
						spawnPointAdded = true;
					}
				}
			}
		return spawnPoints;
	}
	#endregion

}
