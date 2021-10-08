using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapConversionUtility {

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
	public static List<MapTileInfo> ConvertValueInTileInfo (MapDetails details, int replacingValue, TileData tileData
		) {

		List<MapTileInfo> convertedTiles = new List<MapTileInfo>();
		for (int x = 0; x < details.mapData.mapGenerationData.mapSize.x; x++) {
			for (int y = 0; y < details.mapData.mapGenerationData.mapSize.y; y++) {
				MapTileInfo convertingTile = details.mapTileInfo[x, y];
				if (convertingTile.value == replacingValue) {
					convertingTile.value = tileData.id;
					convertingTile.tileLayer = tileData.layer;
					convertedTiles.Add (convertingTile);
				}
			}
		}

		return convertedTiles;
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
