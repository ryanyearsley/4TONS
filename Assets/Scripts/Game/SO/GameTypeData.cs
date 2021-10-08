using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Game/Game Type Data")]
public class GameTypeData : ScriptableObject
{
	public ZoneData worldData;
	public CreatureSpawnInfo playerSpawnInfo;
	public LootTableData lootTableData;
}
