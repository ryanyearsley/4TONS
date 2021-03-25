using System;
using UnityEngine;
[Serializable]
public class SpellSaveData {
    public SpellData spellData;
    public int spellDataIndex;
    public Vector2Int spellGemOriginCoordinate;
    public int spellGemRotation;
    public int spellIndex;

    //[System.NonSerialized]
    public Vector2Int[] currentCoordinates;

    [System.NonSerialized]
    public Spell spellCast;

    [System.NonSerialized]
    public SpellGemEntity spellGemEntity;

    public SpellSaveData Clone() {
        SpellSaveData spellSaveData = new SpellSaveData();
        spellSaveData.spellData = spellData;
        spellSaveData.spellDataIndex = spellDataIndex;
        spellSaveData.spellGemOriginCoordinate = spellGemOriginCoordinate;
        spellSaveData.spellGemRotation = spellGemRotation;
        spellSaveData.spellIndex = spellIndex;
        spellSaveData.currentCoordinates = (Vector2Int[]) currentCoordinates.Clone ();
        return spellSaveData;
	}
}