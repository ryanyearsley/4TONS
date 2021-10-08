using System;
using UnityEngine;
[Serializable]
public class CreatureSpawnInfo {


	[MinMaxSlider(0, 20)]
	public Vector2Int spawnCountRange;
	public CreatureData creatureData;


	public int GetSpawnCountWithinRange () {
		return UnityEngine.Random.Range (spawnCountRange.x, spawnCountRange.y);
	}
}

