using UnityEngine;
using System;


[Serializable]
public class PuzzleGameDataDictionary : SerializableDictionary<PuzzleKey, PuzzleGameData> {


	public PuzzleSaveDataDictionary MapToSaveData () {
		PuzzleSaveDataDictionary puzzleGameDataDictionary = new PuzzleSaveDataDictionary();
		foreach (PuzzleKey key in this.Keys) {
			puzzleGameDataDictionary.Add (key, this [key].MapToSaveData ());
		}
		return puzzleGameDataDictionary;
	}
}