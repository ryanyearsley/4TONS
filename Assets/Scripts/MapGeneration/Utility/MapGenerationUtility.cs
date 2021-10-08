using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerationUtility
{
	//CREATURE SPAWN LOGIC
	public static bool CheckSpawnPointEligibility (MapDetails details, Vector2Int spawnCoordinate, int clearance) {
		Vector2Int startingPoint = spawnCoordinate - new Vector2Int (clearance, clearance);
		int width = 1 + (clearance * 2);
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < width; y++) {
				Vector2Int coordinate = new Vector2Int(startingPoint.x + x, startingPoint.y + y);
				if (!details.mapBounds.isWithinBounds (coordinate)) {
					return false;
				}
				MapTileInfo mapTile = details.mapTileInfo[coordinate.x, coordinate.y];
				if (mapTile.isSpawnConflict)
					return false;
			}
		}
		return true;
	}


	//Clears all tiles AROUND a spawnpoint. for example:
	/*
	 * if clearDistance is 1, all tiles around the spawn (S) are marked as invalid.
	 * x x x
	 * x S x
	 * x x x
	 * 
	 */
	public static void ClearSpawnPointArea (MapDetails details, Vector2Int coord, int clearance) {
		MapTileInfo [,] map = details.mapTileInfo;
		Vector2Int startingPoint = coord - new Vector2Int (clearance, clearance);
		int clearDiameter = 1 + (clearance * 2);
		for (int x = 0; x < clearDiameter; x++) {
			for (int y = 0; y < clearDiameter; y++) {
				Vector2Int coordinate = new Vector2Int(startingPoint.x + x, startingPoint.y + y);
				MapTileInfo clearingTile = map[coordinate.x, coordinate.y];
				clearingTile.value = 0;
				clearingTile.isSpawnConflict = true;
				clearingTile.tileLayer = TileLayer.FLOOR;
			}
		}
	}


	public static void ClearSpawnPointArea (MapDetails details, Vector2Int coord, int clearance, int clearAreaIndex) {
		MapTileInfo [,] map = details.mapTileInfo;
		Vector2Int startingPoint = coord - new Vector2Int (clearance, clearance);
		int clearDiameter = 1 + (clearance * 2);
		for (int x = 0; x < clearDiameter; x++) {
			for (int y = 0; y < clearDiameter; y++) {
				Vector2Int coordinate = new Vector2Int(startingPoint.x + x, startingPoint.y + y);
				MapTileInfo clearingTile = map[coordinate.x, coordinate.y];
				clearingTile.value = clearAreaIndex;
				clearingTile.isSpawnConflict = true;
				clearingTile.tileLayer = TileLayer.FLOOR;
			}
		}
	}


	public static List<SpawnPoint> GenerateRandomLargeSetpieces (MapDetails details) {
		List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
		int largeSetpieceSpawnCount = details.mapData.mapGenerationData.GetLargeSetpieceCount();
		Debug.Log ("MapGenerationUtility: Setpiece count: " + largeSetpieceSpawnCount);
		for (int i = 0; i < largeSetpieceSpawnCount; i++) {
			bool spawnPointAdded = false;
			int randomSetpieceIndex = UnityEngine.Random.Range(0, details.zoneData.largeSetpieceDatas.Count);

			SetPieceData setPieceData = details.zoneData.largeSetpieceDatas[randomSetpieceIndex];
			while (spawnPointAdded == false) {
				int randomX = UnityEngine.Random.Range (5, details.mapData.mapGenerationData.mapSize.x - 5);
				int randomY = UnityEngine.Random.Range (5, details.mapData.mapGenerationData.mapSize.y - 5);
				Vector2Int coord = new Vector2Int (randomX, randomY);
				MapTileInfo mapTileInfo = details.mapTileInfo[randomX, randomY];
				if (mapTileInfo.value == 0) {
					if (MapGenerationUtility.CheckSpawnPointEligibility (details, coord, setPieceData.outerClearance)) {
						MapGenerationUtility.ClearSpawnPointArea (details, coord, setPieceData.outerClearance);
						SpawnPoint spawnPoint = new SpawnPoint (new Vector2Int (randomX, randomY), setPieceData);
						spawnPoints.Add (spawnPoint);
						mapTileInfo.value = spawnPoint.spawnObjectData.id;
						spawnPointAdded = true;
					}
				}
			}
		}
		return spawnPoints;
	}

	public static void GenerateRandomSmallSetPieces (MapDetails details) {
		int baseDecorSpawnCount = details.mapData.mapGenerationData.GetSmallSetpieceCount();
		Debug.Log ("MapGenerationUtility: Base decor spawn count: " + baseDecorSpawnCount);
		if (details.zoneData.baseDecorTiles.Count > 0) {
			for (int i = 0; i < baseDecorSpawnCount; i++) {
				bool spawnPointAdded = false;
				int randomBaseDecorIndex = UnityEngine.Random.Range(0, details.zoneData.baseDecorTiles.Count);
				TileData randomTileData = details.zoneData.baseDecorTiles [randomBaseDecorIndex];
				while (spawnPointAdded == false) {
					int randomX = UnityEngine.Random.Range (5, details.mapData.mapGenerationData.mapSize.x - 5);
					int randomY = UnityEngine.Random.Range (5, details.mapData.mapGenerationData.mapSize.y - 5);

					Vector2Int coord = new Vector2Int(randomX, randomY);
					MapTileInfo mapTileInfo = details.mapTileInfo[randomX, randomY];
					if (mapTileInfo.value == 0) {
						if (MapGenerationUtility.CheckSpawnPointEligibility (details, coord, 2)) {
							MapGenerationUtility.ClearSpawnPointArea (details, coord, 2);
							mapTileInfo.tileLayer = TileLayer.BASE;
							mapTileInfo.value = randomTileData.id;
							mapTileInfo.isSpawnConflict = true;
							spawnPointAdded = true;
						}
					}
				}
			}
		}
	}
	public static void GenerateRandomFloorDecor (MapDetails details, List<MapTileInfo> floorTiles) {

		if (details.zoneData.floorDecorTiles.Count > 0) {
			int fillPercent = details.mapData.mapGenerationData.floorDecorFillPercent;
			foreach (MapTileInfo mapTileInfo in floorTiles) {
				int floorDecorPercentageRoll = UnityEngine.Random.Range(0, 100);
				if (floorDecorPercentageRoll < fillPercent) {
					int randomFloorDecorIndex = Random.Range(0, details.zoneData.floorDecorTiles.Count);
					mapTileInfo.value = details.zoneData.floorDecorTiles [randomFloorDecorIndex].id;
					mapTileInfo.tileLayer = TileLayer.FLOOR_DECOR;
				}
			}
		}
	}
	public static void GenerateRandomTopDecor (MapDetails details, List<MapTileInfo> baseTiles) {

		if (details.zoneData.topDecorTiles.Count > 0) {
			int fillPercent = details.mapData.mapGenerationData.topDecorFillPercent;
			foreach (MapTileInfo mapTileInfo in baseTiles) {
				int topDecorPercentageRoll = UnityEngine.Random.Range(0, 100);
				if (topDecorPercentageRoll < fillPercent) {
					int randomTopDecorIndex = UnityEngine.Random.Range(0, details.zoneData.topDecorTiles.Count);
					mapTileInfo.value = details.zoneData.topDecorTiles [randomTopDecorIndex].id;
					mapTileInfo.tileLayer = TileLayer.TOP;
				}
			}
		}
	}
}
