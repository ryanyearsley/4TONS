using UnityEngine;
using System;
[Serializable]
public class PuzzleCursorLocation
{
	public PuzzleKey puzzleKey;
	public Vector3Int coordinate;

	public PuzzleCursorLocation (PuzzleKey key, Vector3Int coord) {
		puzzleKey = key;
		coordinate = coord;
	}
}
