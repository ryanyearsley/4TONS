using System;
using UnityEngine;
[Serializable]
public class SpellSaveData {
    public SpellData spellData;
    public string spellDataPath;
    public Vector2Int spellGemOriginCoordinate;
    public int spellGemRotation;
    public int spellIndex;

    //[System.NonSerialized]
    public Vector2Int[] currentCoordinates;

    [System.NonSerialized]
    public Spell spellCast;

    [System.NonSerialized]
    public SpellGemEntity spellGemEntity;
}