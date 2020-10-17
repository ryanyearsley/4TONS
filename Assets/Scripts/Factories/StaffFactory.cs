using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

public class StaffFactory
{
    static Vector2 worldSpaceTileScale = new Vector2 (0.5f, 0.5f);

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
    public static void BuildStaff (int [,] staffData, GameObject tile, Vector2 origin) {
        int yLength = staffData.GetLength(1);
        int xLength = staffData.GetLength(0);
        Transform parent = new GameObject ("StaffParent").transform;
        for (int y = 0; y < staffData.GetLength (1); y++) {
            for (int x = 0; x < staffData.GetLength (0); x++) {
                int tilePrefabIndex = staffData[x, y];
                if (tilePrefabIndex != 0) {
                    GameObject.Instantiate (
                        tile, origin + (new Vector2(x, y) * worldSpaceTileScale),
                        Quaternion.identity,
                        parent);
                }

            }
        }
    }

}
