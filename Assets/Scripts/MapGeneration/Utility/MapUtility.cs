using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;

public class MapUtility {

	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";


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
		Vector2Int currentFloorHighestCoordinate = currentFloorOrigin + currentMapDetails.mapData.mapSize;

		int largerDimension = 0;
		if (currentFloorHighestCoordinate.x > largerDimension)
			largerDimension = currentFloorHighestCoordinate.x;
		if (currentFloorHighestCoordinate.y > largerDimension)
			largerDimension = currentFloorHighestCoordinate.y;

		//LOCAL (To their own level) COORDINATES
		Vector2Int currentFloorPortalCoordinate = currentMapDetails.spawnPoints.nextPortalSpawnPoint.spawnCoordinate;
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
				output [x, y] = new MapTileInfo ();
				output [x, y].mapCoordinate = new Vector2Int (x, y);
				int mapValue = map[x, y];
				output [x, y].value = mapValue;
				output [x, y].isSpawnConflict = false;
			}
		}
		return output;
	}
	public static void ConvertValueInTileInfo (MapDetails details, int replacingValue, int newValue) {
		for (int x = 0; x < details.mapData.mapSize.x; x++) {
			for (int y = 0; y < details.mapData.mapSize.y; y++) {
				if (details.mapTileInfo [x, y].value == replacingValue) {
					details.mapTileInfo [x, y].value = newValue;
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
