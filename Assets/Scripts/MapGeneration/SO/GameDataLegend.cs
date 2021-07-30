using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Legend", menuName = "ScriptableObjects/Editor/World Data Legend", order = 2)]

public class GameDataLegend : ScriptableObject {

	//school/zone index starts (grouped by 1000s)
	public int GENERIC_INDEX_START = 0;
	public int LIGHT_INDEX_START = 1000;
	public int DARK_INDEX_START = 2000;
	public int ICE_INDEX_START = 3000;
	public int FIRE_INDEX_START = 4000;
	public int HUB_INDEX_START = 5000;
	public int TUTORIAL_INDEX_START = 6000;
	//5000-10000 reserved for additional schools

	//All of these numbers are offset by 1000 based on which world is in play.
	//For example....  1100 would represent light world (between 1000 and 1999) base-tile (+100)

	//world-specific
	//and so on

	public int TILE_INDEX_START = 100;  
	public int FLOOR_TILE_INDEX_START = 0; //2100
	public int BASE_TILE_INDEX_START = 1; //2101
	public int BORDER_TILE_INDEX_START = 2; //2102
	public int BASE_DECOR_TILE_INDEX_START = 3; //2103 - 2149
	public int TOP_DECOR_TILE_INDEX_START = 50; //2150 - 2199
	public int SETPIECE_INDEX_START = 200;
	public int ENEMY_SPAWN_INDEX_START = 300;
	public int SPELL_INDEX_START = 400;
	public int STAFF_INDEX_START = 500;
	public int OBJECTIVE_INDEX_START = 600;

	public int PLAYER_ONE_SPAWN_INDEX = 1;
	public int NEXT_LEVEL_PORTAL_INDEX = 2;
	//600-999 reserved for additional mechanics
}
