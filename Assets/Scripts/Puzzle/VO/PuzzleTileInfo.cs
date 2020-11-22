using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTileInfo
{
	public Vector2Int mapCoordinate;
	public int value;
	//if this tile is occupied by a spellgem, you can get everything you need from this bad boy.
	public SpellSaveData spellSaveData;
}
