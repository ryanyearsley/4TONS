using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapUtility {

	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

	public static int[,] Generate2DArrayFromTilemap(Tilemap tilemap, CoordinateBounds mapBounds) {
		
		int[,] output = new int[mapBounds.dimensions.x, mapBounds.dimensions.y];
		int tileCount = 0;
		for (int i = 0; i <= mapBounds.dimensions.x - 1; i++) {
			for (int j = 0; j <= mapBounds.dimensions.y - 1; j++) {
				if (tilemap.GetTile (new Vector3Int (i, j, 0)) != null) {
					output [i, j] = 1;
				} else
					output [i, j] = 0;

				tileCount++;
			}
		}
		return output;
	}


	//CREATURE SPAWN LOGIC
	public static bool CheckSpawnPointEligibility (MapDetails details, Vector2Int spawnCoordinate, int clearance) {
		Vector2Int startingPoint = spawnCoordinate - new Vector2Int (clearance, clearance);
		int width = 1 + (clearance * 2);
		bool canSpawn = true;
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
		return canSpawn;
	}
	public static bool CheckDecorPlacementEligibility (MapDetails details, Vector2Int spawnCoordinate, int clearance) {
		Vector2Int startingPoint = spawnCoordinate - new Vector2Int (clearance, clearance);
		int width = 1 + (clearance * 2);
		bool canSpawn = true;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < width; y++) {
				Vector2Int coordinate = new Vector2Int(startingPoint.x + x, startingPoint.y + y);
				if (!details.mapBounds.isWithinBounds (coordinate)) {
					return false;
				}
				MapTileInfo mapTile = details.mapTileInfo[coordinate.x, coordinate.y];
				if (mapTile.value != 0 && mapTile.value != 1)
					return false;
			}
		}
		return canSpawn;
	}


	//Clears all tiles AROUND a spawnpoint. for example:
	/*
	 * if clearDistance is 1, all tiles around the spawn (S) are marked as invalid.
	 * x x x
	 * x S x
	 * x x x
	 * 
	 */
	public static void ClearSpawnPointArea (MapTileInfo [,] map, SpawnPoint spawnPoint) {
		SpawnObjectData spawnObjectData = spawnPoint.spawnObjectData;
		Vector2Int startingPoint = spawnPoint.spawnCoordinate - new Vector2Int (spawnObjectData.clearance, spawnObjectData.clearance);
		int clearDiameter = 1 + (spawnPoint.spawnObjectData.clearance * 2);
		for (int x = 0; x < clearDiameter; x++) {
			for (int y = 0; y < clearDiameter; y++) {
				Vector2Int coordinate = new Vector2Int(startingPoint.x + x, startingPoint.y + y);

				map [coordinate.x, coordinate.y].value = 0;
				map [coordinate.x, coordinate.y].isSpawnConflict = true;
			}
		}
		map [spawnPoint.spawnCoordinate.x, spawnPoint.spawnCoordinate.y].value = spawnObjectData.id;
	}


	public static void ClearDecorArea (MapTileInfo [,] map, int id, Vector2Int coord, int clearance) {
		int clearDiameter = 1 + (clearance * 2);
		Vector2Int startingPoint = coord - new Vector2Int (clearance, clearance);
		for (int x = 0; x < clearDiameter; x++) {
			for (int y = 0; y < clearDiameter; y++) {
				Vector2Int coordinate = new Vector2Int(startingPoint.x + x, startingPoint.y + y);
				MapTileInfo tileInfo = map [coordinate.x, coordinate.y];
				tileInfo.value = 0;
				tileInfo.isSpawnConflict = true;
			}
		}
		map [coord.x, coord.y].value = id;
	}

	public static void ClearSpellGemSpawnPointArea (MapTileInfo [,] map, SpellGemSpawnPoint spawnPoint) {
		SpawnObjectData spawnObjectData = spawnPoint.spawnObjectData;
		Vector2Int startingPoint = spawnPoint.spawnCoordinate - new Vector2Int (spawnObjectData.clearance, spawnObjectData.clearance);
		int clearDiameter = 1 + (spawnPoint.spawnObjectData.clearance * 2);
		for (int x = 0; x < clearDiameter; x++) {
			for (int y = 0; y < clearDiameter; y++) {
				map [startingPoint.x + x, startingPoint.y + y].value = -spawnPoint.spellData.id;
				map [startingPoint.x + x, startingPoint.y + y].isSpawnConflict = true;
			}
		}
		map [spawnPoint.spawnCoordinate.x, spawnPoint.spawnCoordinate.y].value = spawnPoint.spellData.id;
	}
	public static void ClearStaffSpawnPointArea (MapTileInfo [,] map, StaffSpawnPoint spawnPoint) {
		SpawnObjectData spawnObjectData = spawnPoint.spawnObjectData;
		Vector2Int startingPoint = spawnPoint.spawnCoordinate - new Vector2Int (spawnObjectData.clearance, spawnObjectData.clearance);
		int clearDiameter = 1 + (spawnPoint.spawnObjectData.clearance * 2);
		for (int x = 0; x < clearDiameter; x++) {
			for (int y = 0; y < clearDiameter; y++) {
				map [startingPoint.x + x, startingPoint.y + y].value = -spawnPoint.puzzleData.id;
				map [startingPoint.x + x, startingPoint.y + y].isSpawnConflict = true;
			}
		}
		map [spawnPoint.spawnCoordinate.x, spawnPoint.spawnCoordinate.y].value = spawnPoint.puzzleData.id;
	}

	public static Vector2Int CalculateLevelOrigin (MapDetails currentMapDetails, MapDetails nextMapDetails) {
		Vector2Int buffer = new Vector2Int(5, 5);

		//WORLD COORDINATES
		Vector2Int currentFloorOrigin = currentMapDetails.floorOrigin;
		Vector2Int currentFloorHighestCoordinate = currentFloorOrigin + currentMapDetails.mapData.mapGenerationData.mapSize;

		int largerDimension = 0;
		if (currentFloorHighestCoordinate.x > largerDimension)
			largerDimension = currentFloorHighestCoordinate.x;
		if (currentFloorHighestCoordinate.y > largerDimension)
			largerDimension = currentFloorHighestCoordinate.y;

		//LOCAL (To their own level) COORDINATES
		Vector2Int currentFloorPortalCoordinate = currentMapDetails.spawnPoints.portalSpawnPoint.spawnCoordinate;
		Vector2Int nextSpawnCoordinate = nextMapDetails.spawnPoints.playerSpawnPoints[0].spawnCoordinate;

		Vector2Int projectedWorldPlayerSpawnCoordinate =
			currentFloorOrigin +
			currentFloorPortalCoordinate +
			new Vector2Int (largerDimension, largerDimension)
			+ nextSpawnCoordinate + buffer;
		return projectedWorldPlayerSpawnCoordinate - nextSpawnCoordinate;
	}

	#region Conversion Methods
	public static MapTileInfo [,] ConvertMapToTileInfo (int [,] map) {
		int xLength = map.GetLength (0);
		int yLength = map.GetLength (1);
		MapTileInfo[,] output = new MapTileInfo[xLength, yLength];
		for (int y = 0; y < yLength; y++) {
			for (int x = 0; x < xLength; x++) {
				MapTileInfo tileInfo = new MapTileInfo ();
				output [x, y] = tileInfo;
				tileInfo.mapCoordinate = new Vector2Int (x, y);
				int mapValue = map[x, y];
				tileInfo.value = mapValue;
				if (mapValue == 0) {
					tileInfo.walkable = true;
					tileInfo.tileLayer = TileLayer.FLOOR;
				} else if (mapValue == 1) {
					tileInfo.walkable = false;
					tileInfo.tileLayer = TileLayer.BASE;
				}
			}
		}
		return output;
	}
	public static void ConvertValueInTileInfo (MapDetails details, int replacingValue, TileData tileData
		) {
		for (int x = 0; x < details.mapData.mapGenerationData.mapSize.x; x++) {
			for (int y = 0; y < details.mapData.mapGenerationData.mapSize.y; y++) {
				if (details.mapTileInfo [x, y].value == replacingValue) {
					details.mapTileInfo [x, y].value = tileData.id;
					details.mapTileInfo [x, y].tileLayer = tileData.layer;
				}
			}
		}
	}
	#endregion
	public static int [,] DeserializeLevelFile (TextAsset csv) {
		string[] rows = Regex.Split(csv.text, LINE_SPLIT_RE).Where(s => !string.IsNullOrEmpty(s)).ToArray();
		int rowCount = rows.Length;
		Debug.Log ("rowCount: " + rowCount);
		int columnCount = rows[0].Split(","[0]).Length;
		Debug.Log ("columnCount: " + columnCount);
		int[,] output = new int[columnCount, rowCount];
		MapSpawnPoints spawnPoints = new MapSpawnPoints();
		for (int y = 0; y < rowCount; y++) {
			string currentRow = rows[rows.Length - (1 + y)];
			string[] rowTiles = currentRow.Split(',');
			for (int x = 0; x < columnCount; x++) {
				int value =  int.Parse (rowTiles [x]);
				output [x, y] = value;
				/* TODO: Scan int[,] output for spawnpoints
                if (objectDictionary.ContainsKey(value)) {
                    if (spawnPoints.spawnDictionary.ContainsKey (value)) {
                        spawnPoints.spawnDictionary [value].Add (new SpawnPoint( new Vector2Int (x, y), objectDictionary[value]));
					}
				}*/
			}
		}
		return output;
	}

}
