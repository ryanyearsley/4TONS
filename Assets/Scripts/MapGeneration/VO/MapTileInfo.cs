using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileInfo
{
	public Vector2Int mapCoordinate;
	public int value;
	public TileLayer tileLayer;
	public bool isSpawnConflict = false;
	public bool walkable = true;
}

public enum TileLayer {
	FLOOR, BASE, TOP, FLOOR_DECOR
}
