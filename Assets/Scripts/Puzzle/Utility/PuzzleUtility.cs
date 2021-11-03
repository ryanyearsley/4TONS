using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//static methods for any transaction movements on spell gems.
public static class PuzzleUtility {


	public static PuzzleTileInfo CheckMapValue (PuzzleGameData puzzleGameData, Vector2Int mapCoord) {
		if (puzzleGameData.mapBounds.isWithinBounds (mapCoord)) {
			return puzzleGameData.map [mapCoord.x, mapCoord.y];
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
	public static bool CheckSpellFitmentEligibility (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData) {
		bool canEquip = true;
		spellGameData.currentCoordinates = RotateCoordinates (spellGameData.spellData.puzzlePieceData.coordinates, spellGameData.spellGemRotation);
		Vector2Int centerPoint = spellGameData.spellGemOriginCoordinate;
		if (puzzleGameData.puzzleData.puzzleType != PuzzleType.INVENTORY) {
			//add logic for verifying there is vacancy for a spell binding on a staff.
		}
		foreach (Vector2Int spellGemCoordinate in spellGameData.currentCoordinates) {
			Vector2Int relativePosition = centerPoint + spellGemCoordinate;
			if (!puzzleGameData.mapBounds.isWithinBounds (relativePosition)) {
				Debug.Log ("Invalid placement. coordinate outside of puzzle bounds. Spell name: " + spellGameData.spellData.spellName + ", Coord: " + relativePosition + ", bounds: " + puzzleGameData.mapBounds.ToString ());
				canEquip = false;
			} else if (puzzleGameData.map [relativePosition.x, relativePosition.y].value == 0) {
				Debug.Log ("Invalid placement. No tile.");
				canEquip = false;
			} else if (puzzleGameData.map [relativePosition.x, relativePosition.y].value != 1) {
				Debug.Log ("Invalid placement. tile occupied");
				canEquip = false;
			}
			if (canEquip == false)
				return false;
		}
		return canEquip;
	}
	public static void AddSpellGemToPuzzle (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData) {
		spellGemGameData.currentCoordinates = RotateCoordinates (spellGemGameData.spellData.puzzlePieceData.coordinates, spellGemGameData.spellGemRotation);
		puzzleGameData.spellGemGameDataDictionary.Add (spellGemGameData.spellGemOriginCoordinate, spellGemGameData);
		Vector2Int spellGemCenterPoint = spellGemGameData.spellGemOriginCoordinate;
		foreach (Vector2Int coord in spellGemGameData.currentCoordinates) {
			Vector2Int relativePosition = spellGemCenterPoint + coord;
			PuzzleTileInfo puzzleTileInfo = puzzleGameData.map [relativePosition.x, relativePosition.y];
			puzzleTileInfo.spellGemGameData = spellGemGameData;
			//1 == open tile. (0 = no tile)
			if (puzzleTileInfo.value == 1) {
				puzzleTileInfo.value = spellGemGameData.spellData.id;
			}
		}
	}

	public static void RemoveSpellGemFromPuzzle (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData) {
		spellGemGameData.currentCoordinates = RotateCoordinates (spellGemGameData.spellData.puzzlePieceData.coordinates, spellGemGameData.spellGemRotation);
		puzzleGameData.spellGemGameDataDictionary.Remove (spellGemGameData.spellGemOriginCoordinate);
		Vector2Int spellGemCenterPoint = spellGemGameData.spellGemOriginCoordinate;
		foreach (Vector2Int spellGemCoordinate in spellGemGameData.currentCoordinates) {
			Vector2Int relativePosition = spellGemCenterPoint + spellGemCoordinate;
			PuzzleTileInfo puzzleTileInfo = puzzleGameData.map [relativePosition.x, relativePosition.y];
			//1 == open tile. (0 = no tile)
			if (puzzleTileInfo.value == spellGemGameData.spellData.id) {
				puzzleTileInfo.value = 1;
				puzzleTileInfo.spellGemGameData = null;
			}
		}
	}

	public static int CalculateSpellBind(SpellBindingDictionary spellBindingDictionary) {

			for (int i = 0; i < spellBindingDictionary.Count; i++) {
				if (spellBindingDictionary [i] == null) {
					return i;
				}
			}
			return 10;
	}
}