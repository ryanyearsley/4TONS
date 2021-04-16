using UnityEngine;
using System;


[Serializable]
public class SpellGemSaveDataDictionary : SerializableDictionary<Vector2Int, SpellGemSaveData> {

	public SpellGemSaveDataDictionary CloneDictionary () {
		SpellGemSaveDataDictionary cloneDictionary = new SpellGemSaveDataDictionary();
		foreach (Vector2Int key in this.Keys) {
			cloneDictionary.Add (key, this [key].Clone());
		}
		return cloneDictionary;
	}
}