using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Map Data", menuName = "ScriptableObjects/Map Data")]
public class MapData : ScriptableObject {

	//looks at which ever is present and acts accordingly.
	public CustomMapData customMapData;
	public MapGenerationData mapGenerationData;

	public GameObject floorPrefab;//one-piece floor object spawned at 0, 0 (includes front fascia) 
	public CreatureSpawnInfo[] enemySpawnInfos;
	public SetPieceSpawnInfo[] setPieceSpawnInfos;
	public SpellGemSpawnInfo [] spellGemSpawnInfos;
	public StaffSpawnInfo [] staffSpawnInfos;
	[ConditionalHide ("useCustomSeed")]
	public string seed;
	public bool useCustomSeed;
}