using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardButton : AbstractButtonClick {
	protected LeaderboardUI leaderboardUI;
	
	protected override void Awake () {
		base.Awake ();
		leaderboardUI = GetComponentInParent<LeaderboardUI> ();
		if (leaderboardUI != null) {
			leaderboardUI.UpdateLeaderboardUIEvent += OnLeaderboardUpdate;
		}
	}

	private void OnDisable () {
		if (leaderboardUI != null) {
			leaderboardUI.UpdateLeaderboardUIEvent -= OnLeaderboardUpdate;
		}
	}

	protected virtual void OnLeaderboardUpdate(List<LeaderboardEntry> entries, int startingIndex) {

	}

}
