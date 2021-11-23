using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Environment/World Data")]

/*LEGEND
 * */
public class ZoneData : ScriptableObject {

	public int schoolIndexStart;
	public SpellSchool school;
	public Zone zone;
	public Sound music;
	public Sprite loadingBackgroundSprite;
	public ParallaxBackgroundData backgroundGroup;
	public GameObject floorPrefab;
	public TileData primaryFloorTile;//bulk of floor
	public TileData secondaryFloorTile;//Goes around large setpieces
	public TileData underTile;//Goes under large setpieces
	public TileData baseTile;
	public TileData baseBlankTopTile;
	public TileData borderTile;
	public GameObject destructionDebrisObject;
	public List<TileData> floorTiles;//All remaining standard flooring is overriden with random from this.
	public List<TileData> randomFloorDecorTiles;//non-obstructive random floor tiles i.e. grass
	public List<TileData> surroundingDecorTiles;//non-obstructive floor tiles surrounding objects, i.e. flowers
	public List<TileData> baseDecorTiles;//placed on floor tiles, but clears base around.
	public List<TileData> topDecorTiles;//placed on top of base tiles.
	public List<SetPieceData> largeSetpieceDatas;
	public List<CreatureData> enemyDatas;
	public LootTableData lootTableData;
	public MapData[] mapDatas;
}
