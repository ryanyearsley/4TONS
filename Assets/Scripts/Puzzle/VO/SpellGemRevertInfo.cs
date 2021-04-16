using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemRevertInfo
{
	public PuzzleKey puzzleKey;
	public Vector2Int spellGemOriginCoordinate;
	public int spellGemRotation;
	public int spellBindIndex;
	public SpellGemRevertInfo (PuzzleKey key, SpellGemGameData spellGemGameData) {
		puzzleKey = key;
		spellGemOriginCoordinate = spellGemGameData.spellGemOriginCoordinate;
		spellGemRotation = spellGemGameData.spellGemRotation;
		spellBindIndex = spellGemGameData.spellBindIndex;
	}
}
