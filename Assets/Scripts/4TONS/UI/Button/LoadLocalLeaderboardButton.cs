using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLocalLeaderboardButton : AbstractButtonClick
{
	private LeaderboardUI leaderboardUI;

	protected override void Awake () {
		base.Awake ();
		leaderboardUI = GetComponentInParent < LeaderboardUI> ();
	}

	public override void OnClick () {
		base.OnClick ();
		leaderboardUI.LoadTestLeaderboard ();
		//leaderboardUI.LoadCurrentLocalLeaderboard ();
	}
}
