using System.Collections.Generic;
using System;


[Serializable]
public class GameProgress {
	public int totalKills;
	public int currentLevelIndex;
	public int finalLevelIndex;

	public List<GauntletObjectiveComponent> currentFloorRemainingEnemies = new List<GauntletObjectiveComponent>();
	public int currentFloorSpawnCount;


	public GameProgress (int finalFloorIndex) {
		currentLevelIndex = 0;
		finalLevelIndex = finalFloorIndex;
	}
}
