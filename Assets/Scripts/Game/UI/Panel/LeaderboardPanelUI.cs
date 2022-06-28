using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System;

public class LeaderboardPanelUI : AbstractPanelUI {

	[SerializeField]
	private LeaderboardUI leaderboardUI;

	protected override void InitializePanel () {
		base.InitializePanel ();
	}
	protected override void OnUIChange (GameState gameState) {
		if (panelActiveStates.Contains (gameState)) {
			panelObject.SetActive (true);
			//might want to add some delay to allow for player's leaderboard update to go through.
			leaderboardUI.LoadGlobalLeaderboardDelayed ();
		} else {
			panelObject.SetActive (false);
		}
	}
}
