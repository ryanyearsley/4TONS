using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardObjectiveTypeButtonClick : AbstractButtonClick
{
	[SerializeField]
	private Objective objective;

	[SerializeField]
	LeaderboardScreenUI leaderboardScreenUI;
	public override void OnClick () {
		base.OnClick ();
		leaderboardScreenUI.UpdateSelectedObjective (objective);
	}

}
