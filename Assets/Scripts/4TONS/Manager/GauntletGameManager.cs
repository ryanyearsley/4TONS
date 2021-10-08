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

	public MapDetails currentMapDetails;

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

	private bool isPortalOpen;

	public Action<EnemyDeathInfo> enemyDeathEvent;

	private void Awake () {
		SingletonInitialization ();
	}

	private void Start () {
		InitializeManager ();
	}

	public void InitializeManager () {
		towerProgress = new TowerProgress (GameManager.instance.gameContext.worldData.mapDatas.Length - 1);
		GameManager.instance.beginLevelEvent += OnBeginLevel;
	}

	public void OnBeginLevel(int levelIndex) {
		isPortalOpen = true;
		Debug.Log ("gauntlet manager begin level: " + levelIndex);
		towerProgress.currentMapDetails = LevelManager.instance.GetMapDetails (levelIndex);
	}

	public void ReportEnemyDeath (GauntletObjectiveComponent gauntletEnemyEntity) {
		towerProgress.totalKills++;
		Debug.Log ("Enemy death reported. ");

		towerProgress.currentMapDetails.remainingEnemies.Remove (gauntletEnemyEntity);
		int remainingEnemies = towerProgress.currentMapDetails.remainingEnemies.Count;
		int percentageRemaining = remainingEnemies * 100 / towerProgress.currentMapDetails.totalEnemiesCount;
		int percentageCompleted = 100 - percentageRemaining;
		EnemyDeathInfo enemyDeathInfo = new EnemyDeathInfo(percentageCompleted, towerProgress.totalKills);
		enemyDeathEvent?.Invoke (enemyDeathInfo);
		if (towerProgress.currentMapDetails.remainingEnemies.Count <= 0) {
			//end game
			GameManager.instance.LevelObjectiveComplete(towerProgress.currentLevelIndex);
		}
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
			yield break;
		}
		//time for loading screen....
		yield return new WaitForSeconds (1f);
	}

}
