using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardScreenUI : AbstractScreenUI
{
	private LeaderboardUI leaderboardUI;

	private Objective selectedObjective = Objective.Gauntlet;
	private Zone selectedZone = Zone.Dark;

	private void Awake () {
		leaderboardUI = GetComponent<LeaderboardUI> ();
	}

	protected override void OnScreenChange (MenuScreen mainMenuScreen) {
		if (screenActiveStates.Contains (mainMenuScreen)) {
			screenObject.SetActive (true);
			leaderboardUI.GetLeaderboard (selectedObjective, selectedZone);
		} else {
			screenObject.SetActive (false);
		}
	}

	public void UpdateSelectedObjective(Objective objective) {
		if (objective != selectedObjective) {
			//new selection, update leaderboard
			selectedObjective = objective;
			leaderboardUI.GetLeaderboard (selectedObjective, selectedZone);

		}
	}
	public void UpdateSelectedZone (Zone zone) {
		if (zone != selectedZone) {
			//new selection, update leaderboard
			selectedZone = zone;
			leaderboardUI.GetLeaderboard (selectedObjective, selectedZone);

		}
	}



}