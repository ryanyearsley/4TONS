using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Map Data", menuName = "ScriptableObjects/Map Generation/Map Data")]
public class MapData : ScriptableObject {

	//looks at which ever is present and acts accordingly.
	public CustomMapData customMapData;
	public MapGenerationData mapGenerationData;
	
	public CreatureSpawnInfo[] enemySpawnInfos;

	[MinMaxSlider(0, 100)]
	public Vector2Int floorRollRange;


	[ConditionalHide ("useCustomSeed")]
	public string seed;
	public bool useCustomSeed;
}