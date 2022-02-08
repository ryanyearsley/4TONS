using System.Collections.Generic;
using System;

[Serializable]
public class GauntletLevelProgress {

	public List<GauntletObjectiveComponent> currentFloorRemainingEnemies = new List<GauntletObjectiveComponent>();
	public int currentFloorSpawnCount;


	public GauntletLevelProgress () {
	}
}
