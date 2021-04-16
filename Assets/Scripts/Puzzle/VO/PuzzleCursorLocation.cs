using UnityEngine;
using System;
[Serializable]
public class PuzzleCursorLocation
{
	public PuzzleKey puzzleRegion;
	public Vector3Int coordinate;

	public PuzzleCursorLocation (PuzzleKey region, Vector3Int coord) {
		puzzleRegion = region;
		coordinate = coord;
	}
}
