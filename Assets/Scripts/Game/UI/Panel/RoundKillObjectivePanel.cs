using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
public class RoundKillObjectivePanel : AbstractPanelUI
{
	[SerializeField]
	private TMP_Text currentFloorNumberText;

	[SerializeField]
	private Image floorCompletionFillImage;
	[SerializeField]
	private TMP_Text timeText;


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
		UpdateObjectiveIconFill (0);
		currentFloorNumberText.text = (levelIndex + 1).ToString ();
	}

	public void OnEnemyDeath (EnemyDeathInfo enemyDeathInfo) {
		UpdateObjectiveIconFill (enemyDeathInfo.percentageNormalized);
	}
	private void UpdateObjectiveIconFill (float percentageNormalized) {
		floorCompletionFillImage.fillAmount = percentageNormalized;
	}

}
