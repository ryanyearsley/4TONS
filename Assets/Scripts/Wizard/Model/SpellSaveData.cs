using System;
using UnityEngine;
[Serializable]
public class SpellSaveData {

    [NonSerialized]
    public SpellData spellData;
    public string spellDataPath;
    public Vector2Int spellGemCoordinates;
    public int spellGemRotation;
}