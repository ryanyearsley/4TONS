using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Legend", menuName = "ScriptableObjects/Editor/World Data Legend", order = 2)]

public class GameDataLegend : ScriptableObject {

	//schooling
	public int GENERIC_INDEX_START = 0;
	public int LIGHT_INDEX_START = 1000;
	public int DARK_INDEX_START = 2000;
	public int ICE_INDEX_START = 3000;
	public int FIRE_INDEX_START = 4000;
	//5000-10000 reserved for additional schools

	//All of these numbers are offset by 1000 based on which world is in play.
	//For example....  1100 would represent light world (between 1000 and 1999) single-tile setpiece (x100)

	//world-specific
	//1-100 allocated to world-specific objective setpieces only)
	//Example: a generic spawn would be 1
	//But....  a dark spawn would be 2001 (dark = 2000, + 1)
	public int PLAYER_ONE_SPAWN_INDEX = 1;
	//Example: a generic portal would be 13
	//But....  a dark portal would be 2013 (dark = 2000, + 13)
	public int NEXT_LEVEL_PORTAL_INDEX = 13;
	//and so on

	public int TILE_INDEX_START = 100;
	//Example: generic single-tile setpiece would be 100-199
	//But....  a dark portal would be 2100-2199 (dark = 2000, + 100, + setpiece index)
	//2100 = dark empty tile
	//2101 = dark basic block
	public int OBJECTIVE_INDEX_START = 200;
	public int MULTI_TILE_SETPIECE_INDEX_START = 200;
	public int ENEMY_SPAWN_INDEX_START = 300;
	public int SPELLGEM_BLOCK_SPAWN_INDEX_START = 400;
	public int STAFF_SPAWN_INDEX_START = 500;
	//600-999 reserved for additional mechanics
	//Wizard Data indexing
	public int PUZZLEDATA_INDEX_START = 600;


	public SetPieceData spellGemPickUpObjectData;

}
