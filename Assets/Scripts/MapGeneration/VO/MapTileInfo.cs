using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileInfo
{
	public Vector2Int mapCoordinate;
	public SpawnSector spawnSector;
	public Vector3 worldPosition;
	public int baseValue;

	public TileData floorTile;
	public TileData floorDecorTile;
	public TileData baseTile;
	public TileData topDecorTile;
	public SpawnObjectData spawnObjectData;

	public bool isSpawnConflict = false;
	public bool nearSetpiece = false;
	public bool walkable = true;
}
