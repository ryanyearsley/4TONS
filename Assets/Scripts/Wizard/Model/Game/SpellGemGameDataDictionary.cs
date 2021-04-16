using UnityEngine;
using System;


[Serializable]
public class SpellGemGameDataDictionary : SerializableDictionary<Vector2Int, SpellGemGameData> {


	public SpellGemSaveDataDictionary MapToSaveData() {
		SpellGemSaveDataDictionary spellGemSaveDataDictionary = new SpellGemSaveDataDictionary();
		foreach (Vector2Int key in this.Keys) {
			spellGemSaveDataDictionary.Add (key, this [key].MapToSaveData ());
		}
		return spellGemSaveDataDictionary;
	}
}