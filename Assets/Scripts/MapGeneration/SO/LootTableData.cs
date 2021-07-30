using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewLootTableData", menuName = "ScriptableObjects/Map Generation/LootTableData")]

public class LootTableData : ScriptableObject {

	public List <LootTableEntity> lootTable;

	public List<SpellData> RollForGems(int rollValue) {
		List <SpellData> output = new List<SpellData>();
		foreach (LootTableEntity lootTableEntity in lootTable) {
			if (lootTableEntity.IsWithinRange(rollValue)) {
				output.Add (lootTableEntity.spellData);
			}
		}
		return output;
	}
}

[Serializable]
public class LootTableEntity {
	[MinMaxSlider(0, 100)]
	public Vector2Int spellGemRollRange;
	public SpellData spellData;

	public bool IsWithinRange(int rollValue) {
		if (rollValue >= spellGemRollRange.x && rollValue <= spellGemRollRange.y) {
			return true;
		} else return false;
	}
}