using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerationUtility {
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
				clearingTile.baseValue = 0;
				clearingTile.baseTile = null;
				clearingTile.isSpawnConflict = true;
				clearingTile.walkable = true;
			}
		}
	}

	public static void ClearSmallSetpieceSpawnArea (MapDetails details, Vector2Int coord, int clearance) {
		MapTileInfo [,] map = details.mapTileInfo;
		Vector2Int startingPoint = coord - new Vector2Int (clearance, clearance);
		int clearDiameter = 1 + (clearance * 2);
		for (int x = 0; x < clearDiameter; x++) {
			for (int y = 0; y < clearDiameter; y++) {
				Vector2Int coordinate = new Vector2Int(startingPoint.x + x, startingPoint.y + y);
				MapTileInfo clearingTile = map[coordinate.x, coordinate.y];
				clearingTile.baseValue = 0;
				clearingTile.baseTile = null;
				clearingTile.nearSetpiece = true;
				clearingTile.isSpawnConflict = true;
				clearingTile.walkable = true;
			}
		}
	}


	public static void ClearSetpieceSpawnPointArea (MapDetails details, Vector2Int coord, int clearance, TileData surroundingTile) {
		MapTileInfo [,] map = details.mapTileInfo;
		Vector2Int startingPoint = coord - new Vector2Int (clearance, clearance);
		int clearDiameter = 1 + (clearance * 2);
		for (int x = 0; x < clearDiameter; x++) {
			for (int y = 0; y < clearDiameter; y++) {
				Vector2Int coordinate = new Vector2Int(startingPoint.x + x, startingPoint.y + y);
				MapTileInfo clearingTile = map[coordinate.x, coordinate.y];
				clearingTile.baseValue = 0;
				clearingTile.baseTile = null;
				clearingTile.floorTile = surroundingTile;
				clearingTile.nearSetpiece = true;
				clearingTile.isSpawnConflict = true;
				clearingTile.walkable = true;
			}
		}
	}


	//converts all base tiles with NO floor neighbors to a base tile with blank top.
	public static void ConvertBasesWithoutFloorNeighbors (List<MapTileInfo> baseTiles, MapDetails details) {
		int floorIndex = details.zoneData.primaryFloorTile.id;
		int baseIndex = details.zoneData.baseTile.id;
		TileData blankBaseTile = details.zoneData.baseBlankTopTile;
		foreach (MapTileInfo tile in baseTiles) {
			bool hasFloorNeighbors = false;
			for (int x = tile.mapCoordinate.x - 1; x <= tile.mapCoordinate.x + 1; x++) {
				for (int y = tile.mapCoordinate.y - 1; y <= tile.mapCoordinate.y + 1; y++) {
					MapTileInfo neighborTile = details.mapTileInfo [x, y];
					if (neighborTile.baseTile == null) {
						hasFloorNeighbors = true;
					}
				}
			}
			if (!hasFloorNeighbors) {
				tile.baseTile = blankBaseTile;
			}
		}

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
					if (mapTileInfo.baseValue == 0) {
						if (MapGenerationUtility.CheckSpawnPointEligibility (details, coord, 1)) {
							MapGenerationUtility.ClearSmallSetpieceSpawnArea (details, coord, 1);
							mapTileInfo.baseTile = randomTileData;
							mapTileInfo.isSpawnConflict = true;
							mapTileInfo.walkable = false;
							spawnPointAdded = true;
						}
					}
				}
			}
		}
	}

	public static void RandomizeFloor (MapDetails details, List<MapTileInfo> floorTiles) {
		if (details.zoneData.floorData != null) {
			foreach (MapTileInfo mapTileInfo in floorTiles) {
				if (mapTileInfo.floorTile == details.zoneData.primaryFloorTile) {
					mapTileInfo.floorTile = details.zoneData.floorData.GetRandomTile ();
				}
			}
		}
	}
	public static void GenerateFloorDecor (MapDetails details, List<MapTileInfo> floorTiles) {

		int randomFillPercent = details.mapData.mapGenerationData.floorDecorFillPercent;
		int surroundingFillPercent = details.mapData.mapGenerationData.surroundingDecorFillPercent;
		if (details.zoneData.randomFloorDecorTiles.Count > 0) {
			foreach (MapTileInfo tile in floorTiles) {
				if (!tile.nearSetpiece) {
					int floorDecorPercentageRoll = UnityEngine.Random.Range(0, 100);
					if (floorDecorPercentageRoll < randomFillPercent) {
						int randomFloorDecorIndex = Random.Range(0, details.zoneData.randomFloorDecorTiles.Count);
						tile.floorDecorTile = details.zoneData.randomFloorDecorTiles [randomFloorDecorIndex];
					}
				} else {
					int surroundingDecorPercentageRoll = UnityEngine.Random.Range(0, 100);
					if (surroundingDecorPercentageRoll < surroundingFillPercent) {
						int randomSurroundingDecorIndex = Random.Range(0, details.zoneData.surroundingDecorTiles.Count);
						tile.floorDecorTile = details.zoneData.surroundingDecorTiles [randomSurroundingDecorIndex];
					}
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
					mapTileInfo.topDecorTile = details.zoneData.topDecorTiles [randomTopDecorIndex];
				}
			}
		}
	}
}
