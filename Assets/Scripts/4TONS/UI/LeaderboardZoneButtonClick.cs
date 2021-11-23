using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardZoneButtonClick : AbstractButtonClick {
	[SerializeField]
	private Zone zone;

	[SerializeField]
	LeaderboardScreenUI leaderboardScreenUI;
	public override void OnClick () {
		base.OnClick ();
		leaderboardScreenUI.UpdateSelectedZone (zone);
	}

}
