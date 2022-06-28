using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOnlineLeaderboardButton : AbstractButtonClick {
	private LeaderboardUI leaderboardUI;

	protected override void Awake () {
		base.Awake ();
		leaderboardUI = GetComponentInParent<LeaderboardUI> ();
	}

	public override void OnClick () {
		base.OnClick ();
		leaderboardUI.LoadCurrentOnlineLeaderboard ();
	}
}
