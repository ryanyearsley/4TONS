using System;
using UnityEngine;
using UnityEngine.UI;


public class TowerCompletePanelUI : AbstractPanelUI {

	[SerializeField]
	private Text timeText;


	protected override void InitializePanel () {
		base.InitializePanel ();
		if (GameManager.instance != null) {
			GameManager.instance.gameCompleteEvent += OnGameComplete;
		}
	}


	public void OnGameComplete () {

		if (GauntletGameManager.instance != null) {
			TimeSpan ts = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
			timeText.text = "Time: " + string.Format ("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);
		}
	}
}
