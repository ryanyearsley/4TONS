using UnityEngine.UI;
using UnityEngine;
using System;

public class RoundKillObjectivePanel : AbstractPanelUI
{
	[SerializeField]
	private Text currentFloorProgressText;
	[SerializeField]
	private Text timeText;


	protected override void InitializePanel () {
		base.InitializePanel ();
		if (GameManager.instance != null) {
			GameManager.instance.loadLevelEvent += OnLoadLevel;
		}
		if (GauntletGameManager.instance != null) {
			GauntletGameManager.instance.enemyDeathEvent += OnEnemyDeath;
		}
	}

	void Update () {
		TimeSpan ts = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
		timeText.text = string.Format ("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);
	}

	public void OnLoadLevel (int levelIndex) {
		UpdateObjectiveProgress (0);
	}

	public void OnEnemyDeath (EnemyDeathInfo enemyDeathInfo) {
		UpdateObjectiveProgress (enemyDeathInfo.percentageFloorCleared);
	}
	private void UpdateObjectiveProgress (int percentageCleared) {
		currentFloorProgressText.text = "Current Floor: " + percentageCleared + "%";
	}

}
