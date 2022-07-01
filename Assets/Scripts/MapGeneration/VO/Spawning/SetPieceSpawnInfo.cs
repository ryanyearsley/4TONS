using System;
[Serializable]
public class SetPieceSpawnInfo {
	public int spawnCount;
	public SetPieceData setPieceData;
	public SpawnSector setPieceSpawnSector = SpawnSector.Anywhere;
}
