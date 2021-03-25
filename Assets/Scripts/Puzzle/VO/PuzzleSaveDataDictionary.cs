using UnityEngine;
using System;


[Serializable]
public class PuzzleSaveDataDictionary : SerializableDictionary<Vector2Int, SpellSaveData> {

	public PuzzleSaveDataDictionary CloneDictionary () {
		PuzzleSaveDataDictionary cloneDictionary = new PuzzleSaveDataDictionary();
		foreach (Vector2Int key in this.Keys) {
			cloneDictionary.Add (key, this [key].Clone());
		}
		return cloneDictionary;
	}
}