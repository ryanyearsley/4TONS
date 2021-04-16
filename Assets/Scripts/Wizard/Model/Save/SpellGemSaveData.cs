using System;
using UnityEngine;
[Serializable]
public class SpellGemSaveData {

    public SpellData spellData;
    public int spellDataIndex;//don't worry about setting in SO prebuildWizards.
    public Vector2Int spellGemOriginCoordinate;
    public int spellGemRotation;
    public int spellBindIndex;
    public SpellGemSaveData Clone() {
        SpellGemSaveData spellSaveData = new SpellGemSaveData();
        spellSaveData.spellData = spellData;
        spellSaveData.spellDataIndex = spellDataIndex;
        spellSaveData.spellGemOriginCoordinate = spellGemOriginCoordinate;
        spellSaveData.spellGemRotation = spellGemRotation;
        spellSaveData.spellBindIndex = spellBindIndex;
        return spellSaveData;
	}
    public SpellGemGameData MapToGameData() {
        SpellGemGameData spellGemGameData = new SpellGemGameData();
        spellGemGameData.spellData = spellData;
        spellGemGameData.spellGemOriginCoordinate = spellGemOriginCoordinate;
        spellGemGameData.spellGemRotation = spellGemRotation;
        spellGemGameData.spellBindIndex = spellBindIndex;
        return spellGemGameData;
	}
}