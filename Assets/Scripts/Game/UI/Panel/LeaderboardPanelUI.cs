using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System;

public class LeaderboardPanelUI : AbstractPanelUI {

	private LeaderboardUI leaderboardUI;

	protected override void InitializePanel () {
		base.InitializePanel ();
		leaderboardUI = GetComponent<LeaderboardUI> ();
	}
	protected override void OnUIChange (GameState gameState) {
		if (panelActiveStates.Contains (gameState)) {
			panelObject.SetActive (true);
			//might want to add some delay to allow for player's leaderboard update to go through.
			if (GameManager.instance != null) {
				GameContext context = GameManager.instance.gameContext;
				leaderboardUI.GetLeaderboard (context.objectiveData.objective, context.zoneData.zone);
			} 
		} else {
			panelObject.SetActive (false);
		}
	}
}
