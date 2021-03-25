using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleUtility {


	public static PuzzleTileInfo CheckMapValue (PuzzleGroupingDetails details, Vector2Int gridCoord) {
		Vector2Int mapCell = gridCoord - details.groupingOrigin;
		if (details.mapBounds.isWithinBounds (mapCell)) {
			return details.map [mapCell.x, mapCell.y];
		} else return null;
	}

	public static Vector2Int [] RotateCoordinates (Vector2Int [] originCoordinates, int rotation) {
		/*rotation is stored like so:
        0: original rotation
        1: 90* clockwise
        2: 180*
        3: 270*
        4+: invalid
        */
		Vector2Int[] outputCoordinates = (Vector2Int[]) originCoordinates.Clone();
		for (int r = 0; r < rotation; r++) {
			Debug.Log ("rotation calc: " + r);
			for (int i = 0; i < outputCoordinates.Length; i++) {
				outputCoordinates [i] = new Vector2Int (-outputCoordinates [i].y, outputCoordinates [i].x);
			}
		}

		return outputCoordinates;
	}
	public static bool CheckSpellFitmentEligibility (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		bool canEquip = true;
		spellSaveData.currentCoordinates = RotateCoordinates (spellSaveData.spellData.puzzlePieceData.coordinates, spellSaveData.spellGemRotation);
		Vector2Int centerPoint = spellSaveData.spellGemOriginCoordinate;
		foreach (Vector2Int spellGemCoordinate in spellSaveData.currentCoordinates) {
			Vector2Int relativePosition = centerPoint + spellGemCoordinate;
			if (!details.mapBounds.isWithinBounds (relativePosition)) {
				Debug.Log ("Invalid placement. coordinate outside of puzzle bounds. Spell name: " + spellSaveData.spellData.spellName + ", Coord: " + relativePosition + ", bounds: " + details.mapBounds.ToString ());
				canEquip = false;
			} else if (details.map [relativePosition.x, relativePosition.y].value == 0) {
				Debug.Log ("Invalid placement. No tile.");
				canEquip = false;
			} else if (details.map [relativePosition.x, relativePosition.y].value != 1) {
				Debug.Log ("Invalid placement. tile occupied");
				canEquip = false;
			}
			if (canEquip == false)
				return false;
		}
		return canEquip;
	}
	public static void AddSpellGemToPuzzle (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		spellSaveData.currentCoordinates = RotateCoordinates (spellSaveData.spellData.puzzlePieceData.coordinates, spellSaveData.spellGemRotation);
		if (!details.puzzleSaveDataDictionary.ContainsKey (spellSaveData.spellGemOriginCoordinate)) {
			Debug.Log ("spellgem coordinate is not registered, adding to dictionary.");
			details.puzzleSaveDataDictionary.Add (spellSaveData.spellGemOriginCoordinate, spellSaveData);
		}
		Vector2Int spellGemCenterPoint = spellSaveData.spellGemOriginCoordinate;
		foreach (Vector2Int spellGemCoordinate in spellSaveData.currentCoordinates) {
			Vector2Int relativePosition = spellGemCenterPoint + spellGemCoordinate;
			PuzzleTileInfo puzzleTileInfo = details.map [relativePosition.x, relativePosition.y];
			puzzleTileInfo.spellSaveData = spellSaveData;
			//1 == open tile. (0 = no tile)
			if (puzzleTileInfo.value == 1) {
				puzzleTileInfo.value = spellSaveData.spellData.id;
			}
		}
	}

	public static void RemoveSpellGemFromPuzzle (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		spellSaveData.currentCoordinates = RotateCoordinates (spellSaveData.spellData.puzzlePieceData.coordinates, spellSaveData.spellGemRotation);

		details.puzzleSaveDataDictionary.Remove (spellSaveData.spellGemOriginCoordinate);
		Vector2Int spellGemCenterPoint = spellSaveData.spellGemOriginCoordinate;
		foreach (Vector2Int spellGemCoordinate in spellSaveData.currentCoordinates) {
			Vector2Int relativePosition = spellGemCenterPoint + spellGemCoordinate;
			PuzzleTileInfo puzzleTileInfo = details.map [relativePosition.x, relativePosition.y];
			//1 == open tile. (0 = no tile)
			if (puzzleTileInfo.value == spellSaveData.spellData.id) {
				puzzleTileInfo.value = 1;
				puzzleTileInfo.spellSaveData = null;
			}
		}
	}

}