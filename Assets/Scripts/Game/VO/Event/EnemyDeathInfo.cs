using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathInfo
{

	public int percentageFloorCleared;
	public int totalKills;

	public EnemyDeathInfo (int percentageFloorCleared, int totalKills) {
		this.percentageFloorCleared = percentageFloorCleared;
		this.totalKills = totalKills;
	}
}
