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

    //CSV FILE TO VO
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

    //VO TO TILEMAP
    public static void BuildPuzzleTilemap (PuzzleTileInfo [,] staffData, Tilemap tilemap) {
        Tile tile = ConstantsManager.instance.puzzleTile;
        for (int y = 0; y < staffData.GetLength (1); y++) {
            for (int x = 0; x < staffData.GetLength (0); x++) {
                int tilePrefabIndex = staffData[x, y].value;
                if (tilePrefabIndex != 0) {
                    Debug.Log("PuzzleFactory: Setting tile");
                    tilemap.SetTile (new Vector3Int (x, y, 0), tile);
                } 
            }
        }
    }

    public static void ValidateAndMap (PuzzleGameData puzzleGameData, SpellGemSaveDataDictionary spellGemSaveDataDictionary) {
        Debug.Log ("Loading player spellgem entities");
        foreach (SpellGemSaveData spellSaveData in spellGemSaveDataDictionary.Values) {
            SpellGemGameData spellGemGameData = WizardGameDataMapper.MapSpellGemSaveToGameData(spellSaveData);
            if (PuzzleUtility.CheckSpellFitmentEligibility (puzzleGameData, spellGemGameData)) {
                //puzzleComponent onBindSpellgem creates sg entity, 
                puzzleGameData.spellGemGameDataDictionary.Add (spellGemGameData.spellGemOriginCoordinate, spellGemGameData);
            } else {
                Debug.Log ("PlayerPuzzleComponent: Cannot convert SpellGemSaveData -> Game data. SpellGem does not fit in alleged spot.");
            }
        }
    }

}
