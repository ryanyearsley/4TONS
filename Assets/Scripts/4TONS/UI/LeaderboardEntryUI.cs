using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LeaderboardEntryUI : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI rankText;
	[SerializeField]
	private TextMeshProUGUI nameText;
	[SerializeField]
	private TextMeshProUGUI timeText;

	public void DisplayLeaderboardEntry(int rank, string name, string time) {
		rankText.text = rank.ToString ();
		nameText.text = name;
		timeText.text = time;
	}

	public void BlankOutFields () {
		rankText.text = " - - - - ";
		nameText.text = " - - - - ";
		timeText.text = " - - - - ";
		nameText.text = " - - - - ";
	}
}
