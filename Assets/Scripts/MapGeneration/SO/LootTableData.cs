using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewLootTableData", menuName = "ScriptableObjects/Map Generation/LootTableData")]

public class LootTableData : ScriptableObject {

	public List <SpellGemLootTableEntity> spellGemTable;
	public List <StaffLootTableEntity> staffTable;

	public List<SpellData> RollForGems(int rollValue) {
		List <SpellData> output = new List<SpellData>();
		foreach (SpellGemLootTableEntity lootTableEntity in spellGemTable) {
			if (lootTableEntity.IsWithinRange(rollValue)) {
				output.Add (lootTableEntity.spellData);
			}
		}
		return output;
	}
	public List<PuzzleData> RollForStaves (int rollValue) {
		List <PuzzleData> output = new List<PuzzleData>();
		foreach (StaffLootTableEntity lootTableEntity in staffTable) {
			if (lootTableEntity.IsWithinRange (rollValue)) {
				output.Add (lootTableEntity.puzzleData);
			}
		}
		return output;
	}
}

[Serializable]
public class SpellGemLootTableEntity {
	[MinMaxSlider(0, 100)]
	public Vector2Int spellGemRollRange;
	public SpellData spellData;

	public bool IsWithinRange(int rollValue) {
		if (rollValue >= spellGemRollRange.x && rollValue <= spellGemRollRange.y) {
			return true;
		} else return false;
	}
}

[Serializable]
public class StaffLootTableEntity {
	[MinMaxSlider(0, 100)]
	public Vector2Int spellGemRollRange;
	public PuzzleData puzzleData;

	public bool IsWithinRange (int rollValue) {
		if (rollValue >= spellGemRollRange.x && rollValue <= spellGemRollRange.y) {
			return true;
		} else return false;
	}
}