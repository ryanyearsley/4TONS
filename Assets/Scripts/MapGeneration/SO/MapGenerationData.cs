using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "_MapGenerationData", menuName = "ScriptableObjects/Map Generation/MapGenerationData")]
public class MapGenerationData : ScriptableObject
{
	public Vector2Int mapSize = new Vector2Int (60, 80);
	[Range (0, 100)] public int randomFillPercent = 45;
	[Range (0, 100)] public int floorDecorFillPercent = 10;
	[Range (0, 100)] public int topDecorFillPercent = 15;
	[Range (1, 100)] public int borderSize = 5;
	[Range (0, 100)] public int minimumRoomSize = 50;
	[Range (0, 100)] public int minimumIslandSize = 50;
	[Range (1, 10)] public int passageRadius = 2;
	public int smoothingIterations = 5;

	[MinMaxSlider(0, 20)]
	public Vector2Int smallSetpieceCountRange;
	[MinMaxSlider(0, 20)]
	public Vector2Int largeSetpieceCountRange;
	public int GetSmallSetpieceCount() {
		return UnityEngine.Random.Range (smallSetpieceCountRange.x, smallSetpieceCountRange.y);
	}
	public int GetLargeSetpieceCount () {
		return UnityEngine.Random.Range (largeSetpieceCountRange.x, largeSetpieceCountRange.y);
	}
}
