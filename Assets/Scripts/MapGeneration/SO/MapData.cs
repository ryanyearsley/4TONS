using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Map Data", menuName = "ScriptableObjects/Map Generation/Map Data")]
public class MapData : ScriptableObject {

	//looks at which ever is present and acts accordingly.
	public CustomMapData customMapData;
	public MapGenerationData mapGenerationData;

	public GameObject floorPrefab;//one-piece floor object spawned at 0, 0 (includes front fascia) 
	
	public SetPieceSpawnInfo[] setPieceSpawnInfos;
	public CreatureSpawnInfo[] enemySpawnInfos;

	public SpellGemSpawnInfo[] spellGemSpawnInfos;
	public StaffSpawnInfo [] staffSpawnInfos;

	[MinMaxSlider(0, 100)]
	public Vector2Int floorRollRange;


	[ConditionalHide ("useCustomSeed")]
	public string seed;
	public bool useCustomSeed;
}