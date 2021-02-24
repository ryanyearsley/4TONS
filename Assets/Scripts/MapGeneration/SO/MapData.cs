using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Map Data", menuName = "ScriptableObjects/Map Data")]
public class MapData : ScriptableObject {

	public TextAsset mapFile;//optional. if not present, a level will be generated.
	public bool isGenerated = true;
	[Space (10, order = 0)]
	[Header ("Map Settings", order = 1)]
	[Space (15, order = 2)]
	public Vector2Int mapSize = new Vector2Int (60, 80);
	[Range (0, 100)] public int randomFillPercent = 45;
	[Range (1, 100)] public int borderSize = 5;
	[Space (10, order = 0)]
	[Header ("Processing Rules", order = 1)]
	[Space (15, order = 2)]
	[Range (0, 100)] public int minimumRoomSize = 50;
	[Range (0, 100)] public int minimumIslandSize = 50;
	[Range (1, 10)] public int passageRadius = 2;
	[Space (10, order = 0)]
	[Header ("Randomization Settings", order = 1)]
	[Space (15, order = 2)]
	public int smoothingIterations = 5;
	public int playerCount = 2;
	public GameObject floorPrefab;
	public CreatureSpawnInfo playerSpawnInfo;
	public CreatureSpawnInfo[] enemySpawnInfos;
	public CreatureSpawnInfo[] setPieceSpawnInfos;
	public SpellGemSpawnInfo [] spellGemSpawnInfos;
	[ConditionalHide ("useCustomSeed")]
	public string seed;
	public bool useCustomSeed;
}