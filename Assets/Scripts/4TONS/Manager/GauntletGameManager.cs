using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Gauntlet Progress VOs


#endregion
public class GauntletGameManager : MonoBehaviour {
	#region Singleton
	public static GauntletGameManager instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	//towers are separated by scenes. Hub is also it's own scene.
	[SerializeField]
	private GauntletLevelProgress levelProgress;

	public GauntletLevelProgress GetLevelProgress () {
		return levelProgress;
	}

	public Action<EnemyDeathInfo> enemyDeathEvent;

	private void Awake () {
		SingletonInitialization ();
	}

	private void Start () {
		levelProgress = new GauntletLevelProgress ();
		InitializeManager ();
	}

	public void InitializeManager () {
		GameManager.instance.beginLevelEvent += OnBeginLevel;
	}

	public void OnBeginLevel(int levelIndex) {
		levelProgress.currentFloorSpawnCount = levelProgress.currentFloorRemainingEnemies.Count;
	}

	public void RegisterEnemy(GauntletObjectiveComponent gauntletObjectiveComponent) {
		if (!levelProgress.currentFloorRemainingEnemies.Contains (gauntletObjectiveComponent)) {
			levelProgress.currentFloorRemainingEnemies.Add (gauntletObjectiveComponent);
			if (levelProgress.currentFloorSpawnCount != 0) {
				UpdateDeathCount ();
			}
		} else {
			Debug.Log ("GauntletGameManager: Cannot register enemy. Reason: Already registered.");
		}
	}



	public void ReportEnemyDeath (GauntletObjectiveComponent gauntletComponent) {
		if (levelProgress.currentFloorRemainingEnemies.Contains (gauntletComponent)) {
			levelProgress.currentFloorRemainingEnemies.Remove (gauntletComponent);
			UpdateDeathCount ();
			if (levelProgress.currentFloorRemainingEnemies.Count == 0) {
				GameManager.instance.LevelObjectiveComplete ();
				levelProgress.currentFloorSpawnCount = 0;
			}
		} else {
			Debug.LogError ("GauntletGameManager: Cannot process enemy death report.. Reason: Not registered.");
		}
	}

	public void UpdateDeathCount() {
		int remainingEnemies = levelProgress.currentFloorRemainingEnemies.Count;
		int percentageRemaining = remainingEnemies * 100 / levelProgress.currentFloorSpawnCount;
		int percentageCompleted = 100 - percentageRemaining;
		EnemyDeathInfo enemyDeathInfo = new EnemyDeathInfo(percentageCompleted);
		enemyDeathEvent?.Invoke (enemyDeathInfo);
	}
}
