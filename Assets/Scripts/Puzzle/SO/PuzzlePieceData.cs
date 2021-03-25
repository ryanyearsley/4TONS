using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu (fileName = "PuzzleData", menuName = "ScriptableObjects/Puzzle/Puzzle Piece Data", order = 1)]

public class PuzzlePieceData : ScriptableObject
{
	public Sprite puzzlePieceSprite;
	public Vector2Int[] coordinates;

}
