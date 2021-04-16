using System;
using UnityEngine;

[Serializable]
public class PuzzleGameData
{
	public PuzzleData puzzleData;
	public PuzzleTileInfo[,] map;
	public CoordinateBounds mapBounds;

	//populated on re-use object
	public PuzzleEntity puzzleEntity;//visual component. 

	public PuzzleKey puzzleKey;
	public SpellGemGameDataDictionary spellGemGameDataDictionary = new SpellGemGameDataDictionary();

	public SpellBindingDictionary spellBindingDictionary = new SpellBindingDictionary();

	public PuzzleGameData (PuzzleData puzzleData, PuzzleKey key) {
		this.puzzleData = puzzleData;
		this.puzzleKey = key;
		map = PuzzleFactory.DeserializePuzzleFile (puzzleData.puzzleFile);
		mapBounds = new CoordinateBounds (Vector2Int.zero, new Vector2Int( map.GetLength(0) - 1, map.GetLength(1) - 1));
		if (puzzleData.puzzleType != PuzzleType.INVENTORY) {
			for (int i = 0; i <= 3; i++)
				spellBindingDictionary.Add (i, null);
		}
		//puzzle entity added from playerPuzzleUIComponent.
	}

	public PuzzleSaveData MapToSaveData() {
		PuzzleSaveData puzzleSaveData = new PuzzleSaveData();
		puzzleSaveData.puzzleData = puzzleData;
		puzzleSaveData.puzzleDataIndex = puzzleData.id;
		puzzleSaveData.spellGemSaveDataDictionary = spellGemGameDataDictionary.MapToSaveData ();
		return puzzleSaveData;
	}
}
