using System;
using UnityEngine;

[Serializable]
public class SpellGemGameData {

    //SAVE DATA STRAIGHT MAP
	public SpellData spellData;
	public Vector2Int spellGemOriginCoordinate;
	public int spellGemRotation;
	public int spellBindIndex;

    //GAME-ONLY DATA (Don't map to SaveData)
    [NonSerialized]
    public Vector2Int[] currentCoordinates;

    [NonSerialized]
    public Spell spellCast;

    [NonSerialized]
    public SpellGemEntity spellGemEntity;


    public SpellGemSaveData MapToSaveData() {
        SpellGemSaveData spellGemSaveData = new SpellGemSaveData();
        spellGemSaveData.spellData = spellData;
        spellGemSaveData.spellDataIndex = spellData.id;
        spellGemSaveData.spellGemOriginCoordinate = spellGemOriginCoordinate;
        spellGemSaveData.spellGemRotation = spellGemRotation;
        spellGemSaveData.spellBindIndex = spellBindIndex;
        return spellGemSaveData;
	}

}
