using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Gauntlet Progress VOs


[Serializable]
public class TowerProgress {
	public int totalKills;
	public int currentLevelIndex;
	public int finalLevelIndex;

	public List<GauntletObjectiveComponent> currentFloorRemainingEnemies = new List<GauntletObjectiveComponent>();
	public int currentFloorSpawnCount;


	public TowerProgress (int finalFloorIndex) {
		currentLevelIndex = 0;
		finalLevelIndex = finalFloorIndex;
	}
}


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
	private TowerProgress towerProgress;

	public TowerProgress GetProgress() {
		return towerProgress;
	}

	private bool isPortalOpen;

	public Action<EnemyDeathInfo> enemyDeathEvent;

	private void Awake () {
		SingletonInitialization ();
	}

	private void Start () {
		InitializeManager ();
	}

	public void InitializeManager () {
		towerProgress = new TowerProgress (GameManager.instance.gameContext.zoneData.mapDatas.Length - 1);
		GameManager.instance.beginLevelEvent += OnBeginLevel;
	}

	public void OnBeginLevel(int levelIndex) {
		towerProgress.currentFloorSpawnCount = towerProgress.currentFloorRemainingEnemies.Count;
		isPortalOpen = true;
		Debug.Log ("gauntlet manager begin level: " + levelIndex);
	}

	public void RegisterEnemy(GauntletObjectiveComponent gauntletObjectiveComponent) {
		if (!towerProgress.currentFloorRemainingEnemies.Contains (gauntletObjectiveComponent)) {
			towerProgress.currentFloorRemainingEnemies.Add (gauntletObjectiveComponent);
			if (towerProgress.currentFloorSpawnCount != 0) {
				UpdateDeathCount ();
			}
		} else {
			Debug.LogError ("GauntletGameManager: Cannot register enemy. Reason: Already registered.");
		}
	}



	public void ReportEnemyDeath (GauntletObjectiveComponent gauntletComponent) {
		if (towerProgress.currentFloorRemainingEnemies.Contains (gauntletComponent)) {
			towerProgress.currentFloorRemainingEnemies.Remove (gauntletComponent);
			UpdateDeathCount ();
			if (towerProgress.currentFloorRemainingEnemies.Count == 0) {
				GameManager.instance.LevelObjectiveComplete (towerProgress.currentLevelIndex);
				towerProgress.totalKills += towerProgress.currentFloorSpawnCount;
				towerProgress.currentFloorSpawnCount = 0;
			}
		} else {
			Debug.LogError ("GauntletGameManager: Cannot process enemy death report.. Reason: Not registered.");
		}
	}

	public void UpdateDeathCount() {
		int remainingEnemies = towerProgress.currentFloorRemainingEnemies.Count;
		int percentageRemaining = remainingEnemies * 100 / towerProgress.currentFloorSpawnCount;
		int percentageCompleted = 100 - percentageRemaining;
		EnemyDeathInfo enemyDeathInfo = new EnemyDeathInfo(percentageCompleted, towerProgress.totalKills);
		enemyDeathEvent?.Invoke (enemyDeathInfo);
	}

	public void PortalEntered () {
		if (isPortalOpen) {
			StartCoroutine (PortalEntryRoutine ());
		}
	}

	public IEnumerator PortalEntryRoutine () {
		isPortalOpen = false;
		int nextLevelIndex = towerProgress.currentLevelIndex + 1;
		//time for portal entry animation...
		yield return new WaitForSeconds (0.5f);
		Debug.Log ("next floor index: " + nextLevelIndex + ", highest floor index: " + towerProgress.finalLevelIndex);
		if (nextLevelIndex <= towerProgress.finalLevelIndex) {
			GameManager.instance.LevelEnd (towerProgress.currentLevelIndex);
			towerProgress.currentLevelIndex = nextLevelIndex;
			yield return new WaitForSeconds (0.5f);
			GameManager.instance.LoadLevel (towerProgress.currentLevelIndex);
		} else {
			//final level completed. return to hub.
			GameManager.instance.LevelEnd (towerProgress.currentLevelIndex);
			GameManager.instance.GameComplete();
			string leaderboardName = GameManager.instance.gameContext.objectiveData.objectiveName + ": " + GameManager.instance.gameContext.zoneData.zone.ToString();
			PlayFabManager.instance.SendLeaderboardUpdate (Mathf.RoundToInt (Time.time * 1000), leaderboardName);//x1000 going in, /1000 when retrieved
			yield break;
		}
		//time for loading screen....
		yield return new WaitForSeconds (1f);
	}

}
