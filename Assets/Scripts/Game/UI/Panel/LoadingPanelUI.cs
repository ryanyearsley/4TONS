﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LoadingPanelUI : AbstractPanelUI
{
	public TipData tipData;

	[SerializeField]
	private Image backgroundImage;

	[SerializeField]
	private TMP_Text loadingText;
	[SerializeField]
	private TMP_Text tipText;

	[SerializeField]
	private TMP_Text loadLogText;
	private List<string> Eventlog = new List<string>();
	private string logText = "";
	public int maxLines = 10;
	protected override void OnUIChange (GameState gameState) {
		if (panelActiveStates.Contains (gameState)) {
			panelObject.SetActive (true);
			ClearLoadingLog ();
			if (gameState == GameState.LOADING) {

				string loadingTextString = "Loading: \n";
				if (GameManager.instance != null) {
					loadingTextString += GameManager.instance.gameContext.zoneData.zone;
					backgroundImage.sprite = GameManager.instance.gameContext.zoneData.loadingBackgroundSprite;
				}

				if (GauntletGameManager.instance != null) {
					loadingTextString += " Tower\nFloor " + (GameManager.instance.GetProgress ().currentLevelIndex + 1);
				}
				loadingText.text = loadingTextString;

				int randomTipIndex = Random.Range(0, tipData.tips.Length - 1);
				tipText.text = tipData.tips [randomTipIndex];
			}
			else if (gameState == GameState.GAME_COMPLETE) {
				loadingText.text = "";
				tipText.text = "";
			}
		} else {
			panelObject.SetActive (false);
		}
	}
	public void ClearLoadingLog() {
		Eventlog.Clear ();
		loadLogText.text = "";
	}
	public void UpdateLoadingLog (string eventString) {
		Eventlog.Add (eventString);

		if (Eventlog.Count >= maxLines)
			Eventlog.RemoveAt (0);

		logText = "";

		foreach (string logEvent in Eventlog) {
			logText += logEvent;
			logText += "\n";
		}
		loadLogText.text = logText;
	}
}
