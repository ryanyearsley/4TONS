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
		GameManager.instance.gameCompleteEvent += OnGameComplete;
	}

	public void OnBeginLevel(int levelIndex) {
		levelProgress.currentFloorSpawnCount = levelProgress.currentFloorRemainingEnemies.Count;
	}

	public void OnGameComplete() {
		string leaderboardName = "Gauntlet: " + GameManager.instance.gameContext.zoneData.zone.ToString();
		PlayFabManager.instance.SendLeaderboardUpdate (Mathf.RoundToInt (Time.time * 1000), leaderboardName);//x1000 going in, /1000 when retrieved

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
		float remainingEnemies = levelProgress.currentFloorRemainingEnemies.Count;
		float percentageNormalized = 1 - remainingEnemies/levelProgress.currentFloorSpawnCount;
		EnemyDeathInfo enemyDeathInfo = new EnemyDeathInfo(percentageNormalized);
		enemyDeathEvent?.Invoke (enemyDeathInfo);
	}

}
