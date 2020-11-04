using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text.RegularExpressions;

//processes CSV files
//creates staff in accordance with any tile prefab + grid (UI or physical)
public class StaffFactory
{
    static Vector2 worldSpaceTileScale = new Vector2 (0.625f, 0.625f);

    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	public static int [,] DeserializeStaffFile (TextAsset csv) {

        string[] rows = Regex.Split(csv.text, LINE_SPLIT_RE).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        int rowCount = rows.Length;
        Debug.Log ("rowCount: " + rowCount);
        int columnCount = rows[0].Split(","[0]).Length;
        Debug.Log ("columnCount: " + columnCount);
        int[,] output = new int[columnCount, rowCount];
        for (int y = 0; y < rowCount; y++) {
            string currentRow = rows[rows.Length - (1 + y)];
            string[] rowTiles = currentRow.Split(',');
            for (int x = 0; x < columnCount; x++) {
                output [x, y] = int.Parse (rowTiles [x]);
            }
        }
        Debug.Log ("Staff file successfully processed.");
        return output;
    }
    public static GameObject BuildStaff (int [,] staffData, Tilemap tilemap, Tile tile) {
        int yLength = staffData.GetLength(1);
        int xLength = staffData.GetLength(0);
        Transform parent = tilemap.transform;
        for (int y = 0; y < staffData.GetLength (1); y++) {
            for (int x = 0; x < staffData.GetLength (0); x++) {
                int tilePrefabIndex = staffData[x, y];
                if (tilePrefabIndex != 0) {
                    tilemap.SetTile (new Vector3Int (x, y, 0), tile);
                } 

            }
        }
        return parent.gameObject;
    }

}
