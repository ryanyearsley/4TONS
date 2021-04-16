using System;
using UnityEngine;

[Serializable]
public class PuzzleSaveDataDictionary : SerializableDictionary<PuzzleKey, PuzzleSaveData> {

	public PuzzleSaveDataDictionary CloneDictionary () {
		PuzzleSaveDataDictionary cloneDictionary = new PuzzleSaveDataDictionary();
		foreach (PuzzleKey key in this.Keys) {
			cloneDictionary.Add (key, this [key].Clone ());
		}
		return cloneDictionary;
	}
}