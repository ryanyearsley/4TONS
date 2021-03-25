using UnityEngine.UI;
using UnityEngine;

public class RoundKillObjectivePanel : AbstractPanelUI
{
	[SerializeField]
	private Text currentFloorProgressText;
	[SerializeField]
	private Text totalKillsText;


	protected override void InitializePanel () {
		base.InitializePanel ();
		UpdateTotalKills (0);
		if (GameManager.instance != null) {
			GameManager.instance.loadLevelEvent += OnLoadLevel;
		}
		if (GameManager.instance != null) {
			GauntletGameManager.instance.enemyDeathEvent += OnEnemyDeath;
		}
	}

	public void OnLoadLevel (int levelIndex) {
		UpdateObjectiveProgress (0);
	}

	public void OnEnemyDeath (EnemyDeathInfo enemyDeathInfo) {
		UpdateTotalKills (enemyDeathInfo.totalKills);
		UpdateObjectiveProgress (enemyDeathInfo.percentageFloorCleared);
	}
	private void UpdateTotalKills (int totalKills) {
		totalKillsText.text = "Total Kills: " + totalKills;
	}
	private void UpdateObjectiveProgress (int percentageCleared) {
		currentFloorProgressText.text = "Current Floor: " + percentageCleared + "%";
	}

}
