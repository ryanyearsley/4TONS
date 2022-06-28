using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ViewNextLeaderboardPageButton : LeaderboardButton {
	private TMP_Text text;

	[SerializeField]
	private Color disabledColor;

	protected override void Awake () {
		base.Awake ();
		text = GetComponentInChildren<TMP_Text> ();
	}
	protected override void OnLeaderboardUpdate (List<LeaderboardEntry> entries, int startingIndex) {
		if (startingIndex + 100 > entries.Count) {
			text.color = disabledColor;
		} else text.color = Color.white;
	}

	public override void OnClick () {
		leaderboardUI.LoadNextPage ();
	}
}

