using System.Collections.Generic;
using UnityEngine;

public class SpawnUtility {

	private static int MAX_RETRY_COUNT = 10;
	public static List<SpawnPoint> GenerateCreatureSpawnPoints (MapDetails details, CreatureSpawnInfo creatureSpawnInfo, SpawnSectorInfo sectorInfo) {
		List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
		int spawnCount = creatureSpawnInfo.GetSpawnCountWithinRange();
		for (int i = 0; i < spawnCount; i++) {
			bool spawnPointAdded = false;
			int attempts = 0;
			while (spawnPointAdded == false && attempts < 20) {
				int randomX = Random.Range (sectorInfo.minCoord.x, sectorInfo.maxCoord.x);
				int randomY = Random.Range (sectorInfo.minCoord.y, sectorInfo.maxCoord.y);

				if (details.mapTileInfo [randomX, randomY].baseValue == 0) {
					if (MapGenerationUtility.CheckSpawnPointEligibility (details, new Vector2Int (randomX, randomY), creatureSpawnInfo.creatureData.outerClearance)) {
						Vector2Int coord = new Vector2Int (randomX, randomY);
						MapGenerationUtility.ClearSpawnPointArea (details, coord, creatureSpawnInfo.creatureData.outerClearance);
						SpawnPoint spawnPoint = new SpawnPoint (coord, creatureSpawnInfo.creatureData);
						spawnPoints.Add (spawnPoint);
						spawnPointAdded = true;
					}
				}
				attempts++;
			}
			if (spawnPointAdded == false) {
				Debug.Log ("SpawnUtility: Failed to create all creature spawn points. current count: " + i + ", target spawn count: " + spawnCount);
			}
		}
		return spawnPoints;
	}

	public static List<SpawnPoint> GenerateSetPieceSpawnPoints (MapDetails details, SetPieceData setPieceData, int count, SpawnSectorInfo sector, SpawnSectorInfo fallbackSector)
	{
		SpawnSectorInfo currentSector = sector;
		Debug.Log("SpawnUtility: Generating " + count + " Spawn Points for " + setPieceData.spawnObjectPrefab.name + " between " + sector.minCoord.ToString() + " and " + sector.maxCoord.ToString());
		List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
		for (int i = 0; i < count; i++) {
			bool spawnPointAdded = false;
			int attemptCount = 0;
			while (spawnPointAdded == false) {
				if (attemptCount > MAX_RETRY_COUNT)
				{
					currentSector = fallbackSector; 
					Debug.Log("SpawnUtility: Max retry attempt exceeded, using fallback sector...");

				}
				int randomX = UnityEngine.Random.Range (currentSector.minCoord.x, currentSector.maxCoord.x);
				int randomY = UnityEngine.Random.Range  (currentSector.minCoord.y, currentSector.maxCoord.y); 
				Debug.Log("SpawnUtility: Checking SpawnPointEligibility for " + randomX + ", " + randomY);

				Vector2Int coord = new Vector2Int (randomX, randomY);
				if (details.mapTileInfo [randomX, randomY].baseValue == 0) {
					if (MapGenerationUtility.CheckSpawnPointEligibility (details, coord, setPieceData.outerClearance)) {
						MapGenerationUtility.ClearSpawnPointArea (details, coord, setPieceData.outerClearance);
						SpawnPoint spawnPoint = new SpawnPoint (new Vector2Int (randomX, randomY), setPieceData);
						spawnPoints.Add (spawnPoint);
						spawnPointAdded = true;
					}
				}
				attemptCount++;
			}
		}
		return spawnPoints;
	}


	public static List<SpawnPoint> GenerateRandomLargeSetpieceSpawnPoints (MapDetails details) {
		List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
		int largeSetpieceSpawnCount = details.mapData.mapGenerationData.GetLargeSetpieceCount();
		Debug.Log ("SpawnUtility: Setpiece count: " + largeSetpieceSpawnCount);
		for (int i = 0; i < largeSetpieceSpawnCount; i++) {
			bool spawnPointAdded = false; 
			int randomSetpieceIndex = UnityEngine.Random.Range(0, details.zoneData.largeSetpieceDatas.Count);

			SetPieceData setPieceData = details.zoneData.largeSetpieceDatas[randomSetpieceIndex];
			while (spawnPointAdded == false) {
				int randomX = UnityEngine.Random.Range (5, details.mapData.mapGenerationData.mapSize.x - 5);
				int randomY = UnityEngine.Random.Range (5, details.mapData.mapGenerationData.mapSize.y - 5);
				Vector2Int coord = new Vector2Int (randomX, randomY);
				if (details.mapTileInfo [randomX, randomY].baseValue == 0) {
					if (MapGenerationUtility.CheckSpawnPointEligibility (details, coord, setPieceData.outerClearance)) {
						MapGenerationUtility.ClearSetpieceSpawnPointArea (details, coord, setPieceData.outerClearance, details.zoneData.secondaryFloorTile);
						MapGenerationUtility.ClearSetpieceSpawnPointArea (details, coord, setPieceData.innerClearance, details.zoneData.underTile);
						SpawnPoint spawnPoint = new SpawnPoint (new Vector2Int (randomX, randomY), setPieceData);
						spawnPoints.Add (spawnPoint);
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
			int randomX = UnityEngine.Random.Range (3, details.mapData.mapGenerationData.mapSize.x - 3);
			int randomY = UnityEngine.Random.Range (3, details.mapData.mapGenerationData.mapSize.y - 3);
			Vector2Int coord = new Vector2Int(randomX, randomY);
			if (details.mapTileInfo [randomX, randomY].baseValue == 0) {
				if (MapGenerationUtility.CheckSpawnPointEligibility (details, new Vector2Int (randomX, randomY), 2)) {
					MapGenerationUtility.ClearSpawnPointArea (details, coord, 2);
					SpellGemSpawnPoint spawnPoint = new SpellGemSpawnPoint (coord, ConstantsManager.instance.spellGemPickUpData, spellData);
					spawnPoints.Add (spawnPoint);
					spawnPointAdded = true;
				}
			}
		}
		return spawnPoints;
	}

	public static List<StaffSpawnPoint> GenerateStaffSpawnPoints (MapDetails details, PuzzleData puzzleData) {
		List<StaffSpawnPoint> spawnPoints = new List<StaffSpawnPoint>();
		bool spawnPointAdded = false;
		while (spawnPointAdded == false) {
			int randomX = UnityEngine.Random.Range (3, details.mapData.mapGenerationData.mapSize.x - 3);
			int randomY = UnityEngine.Random.Range (3, details.mapData.mapGenerationData.mapSize.y - 3);
			Vector2Int coord = new Vector2Int(randomX, randomY);
			if (details.mapTileInfo [randomX, randomY].baseValue == 0) {
				if (MapGenerationUtility.CheckSpawnPointEligibility (details, new Vector2Int (randomX, randomY), 1)) {
					MapGenerationUtility.ClearSpawnPointArea (details, coord, 1);
					StaffSpawnPoint spawnPoint = new StaffSpawnPoint (coord, ConstantsManager.instance.staffPickUpData, puzzleData);
					spawnPoints.Add (spawnPoint);
					spawnPointAdded = true;
				}
			}
		}
		return spawnPoints;
	}

}
