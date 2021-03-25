using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text.RegularExpressions;

//processes CSV files
//creates staff in accordance with any tile prefab + grid (UI or physical)
public class PuzzleFactory
{
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	public static PuzzleTileInfo [,] DeserializePuzzleFile (TextAsset csv) {

        string[] rows = Regex.Split(csv.text, LINE_SPLIT_RE).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        int rowCount = rows.Length;
        int columnCount = rows[0].Split(","[0]).Length;
        PuzzleTileInfo[,] output = new PuzzleTileInfo[columnCount, rowCount];
        for (int y = 0; y < rowCount; y++) {
            string currentRow = rows[rows.Length - (1 + y)];
            string[] rowTiles = currentRow.Split(',');
            for (int x = 0; x < columnCount; x++) {
                output [x, y] = new PuzzleTileInfo ();
                output [x, y].value = int.Parse (rowTiles [x]);
                output [x, y].mapCoordinate = new Vector2Int (x, y);
            }
        }
        return output;
    }

    public static PuzzleTileInfo [,] GenerateEmptyInventory(Vector2Int size) {
        PuzzleTileInfo[,] output = new PuzzleTileInfo[size.x, size.y];
        int xLength = output.GetLength (0);
        int yLength = output.GetLength (1);
        for (int y = 0; y < yLength; y++) {
            for (int x = 0; x < xLength; x++) {
                output [x, y] = new PuzzleTileInfo ();
                output [x, y].value = 1;
                output [x, y].mapCoordinate = new Vector2Int (x, y)
;            }
        }
        return output;
    }
    public static void BuildInventoryUI (PuzzleTileInfo [,] inventoryData, Vector2Int origin, Tilemap tilemap, Tile tile) {
        for (int y = 0; y < inventoryData.GetLength (1); y++) {
            for (int x = 0; x < inventoryData.GetLength (0); x++) {
                int tilePrefabIndex = inventoryData[x, y].value;
                if (tilePrefabIndex == 1) {
                    tilemap.SetTile (new Vector3Int (origin.x + x, origin.y + y, 0), tile);
                }

            }
        }
    }
    public static void BuildStaffUI (PuzzleTileInfo [,] staffData, Vector2Int origin, Tilemap tilemap, Tile tile) {
        for (int y = 0; y < staffData.GetLength (1); y++) {
            for (int x = 0; x < staffData.GetLength (0); x++) {
                int tilePrefabIndex = staffData[x, y].value;
                if (tilePrefabIndex != 0) {
                    tilemap.SetTile (new Vector3Int (origin.x + x, origin.y + y, 0), tile);
                } 

            }
        }
    }

}
