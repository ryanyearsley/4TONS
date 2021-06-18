using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Environment/World Data", order = 2)]

/*LEGEND
 * */
public class WorldData : ScriptableObject, IEnumerable<Tile> {

	public int schoolIndexStart;
	public SpellSchool school;
	public Zone zone;
	public Sprite background;
	public CreatureData playerCreatureData;
	public List<CreatureData> enemyDatas;

	public List<Tile> tileset;
	public List<TileData> tiles;
	public CreatureSpawnInfo playerSpawnInfo;
	public SetPieceSpawnInfo playerSpawnSetpieceSpawnInfo;
	public SetPieceSpawnInfo nextLevelPortalSpawnInfo;
	public List<SetPieceData> setPieceDatas;

	public MapData[] mapDatas;
	public IEnumerator<Tile> GetEnumerator () {
		return tileset.GetEnumerator ();
	}

	IEnumerator IEnumerable.GetEnumerator () {
		return GetEnumerator ();
	}

}
