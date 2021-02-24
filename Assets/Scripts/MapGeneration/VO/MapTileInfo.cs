using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileInfo
{
	public Vector2Int mapCoordinate;
	public int value;
	public bool isSpawnConflict;
	public bool walkable = true;
}
