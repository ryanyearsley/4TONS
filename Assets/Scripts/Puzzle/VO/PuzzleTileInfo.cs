using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTileInfo
{
	public Vector2Int mapCoordinate;
	public int value;//0 = off the puzzle. 1 = vacant tile. anything else = respective spellgem id
	public SpellGemGameData spellGemGameData;//if this tile is occupied by a spellgem, you can get everything you need from this bad boy.

}
