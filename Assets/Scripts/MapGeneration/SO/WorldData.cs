using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Environment/World Data", order = 2)]

/*LEGEND
 * */
public class WorldData : ScriptableObject {

	public int schoolIndexStart;
	public SpellSchool school;
	public Zone zone;
	public Sprite background;

	public TileData floorTile;
	public TileData baseTile;
	public TileData borderTile;
	public List<TileData> baseDecorTiles;
	public List<TileData> topDecorTiles;

	public CreatureData playerCreatureData;
	public List<CreatureData> enemyDatas;

	public CreatureSpawnInfo playerSpawnInfo;
	public SetPieceSpawnInfo playerSpawnSetpieceSpawnInfo;
	public SetPieceSpawnInfo nextLevelPortalSpawnInfo;

	public LootTableData lootTableData;
	public List<SetPieceData> setPieceDatas;

	public MapData[] mapDatas;

}
