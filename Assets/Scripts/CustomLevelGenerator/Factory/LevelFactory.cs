using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.Tilemaps;

//This class will either...
//1. Take a CSV file
public class LevelFactory {
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    public static int [,] DeserializeLevelFile (TextAsset csv) {

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
        Debug.Log ("Level file successfully processed.");
        return output;
    }

    public static void BuildLevel (int [,] levelData, Tile[] tileSet, Tilemap tilemap) {
        int yLength = levelData.GetLength(1);
        int xLength = levelData.GetLength(0);
        Transform parent = tilemap.transform;
        for (int y = 0; y < levelData.GetLength (1); y++) {
            for (int x = 0; x < levelData.GetLength (0); x++) {
                int tilePrefabIndex = levelData[x, y];
                if (tilePrefabIndex != 0 && tilePrefabIndex < 100) {
                    tilemap.SetTile (new Vector3Int (x, y, 0), tileSet [tilePrefabIndex - 1]);
                }

            }
        }
    }
}