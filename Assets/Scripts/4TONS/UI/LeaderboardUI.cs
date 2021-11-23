using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class LeaderboardUI : MonoBehaviour
{

	private bool initialized;
	private string leaderboardName;

	[SerializeField]
	private Text leaderboardNameText; 

	[SerializeField]
	private GameObject leaderboardEntryPrefab;

	[SerializeField]
	private RectTransform verticalLayoutGroupRectTransform;

	[SerializeField]
	private ScrollRect scrollRect;
	private float defaultRectVerticalSize;

	[SerializeReference]
	private GameObject noLeaderboardResultsObject;

	[SerializeField]
	private List<LeaderboardEntryUI> loadedLeaderboardEntries;

	void Awake() {
		InitializeLeaderboard ();
	}
	public void InitializeLeaderboard () {
		scrollRect = GetComponentInChildren<ScrollRect> ();
		defaultRectVerticalSize = verticalLayoutGroupRectTransform.rect.height;
		initialized = true;
	}
	public void GetLeaderboard (Objective objective, Zone zone ) {
		string leaderboardName = objective.ToString() + ": " + zone.ToString();
		leaderboardNameText.text = leaderboardName;
		if (PlayFabManager.instance != null) {
			PlayFabManager.instance.GetLeaderboard (leaderboardName, GetLeaderboardSuccess, UpdateLeaderboardFail);
		}
	}

	public void GetLeaderboardSuccess (GetLeaderboardResult results) {
		Debug.Log ("LeaderboardUI: GetLeaderboardSuccess. Leaderboard entry count: " + results.Leaderboard.Count);

		if (!initialized) {
			InitializeLeaderboard ();
		}
		if (results.Leaderboard.Count == 0) {
			noLeaderboardResultsObject.SetActive (true);
		} else {
			noLeaderboardResultsObject.SetActive (false);
		}
		int leaderboardEntryCount = results.Leaderboard.Count;
		UpdateLeaderboardEntryGrouping (leaderboardEntryCount);
		for (int i = 0; i < leaderboardEntryCount; i++) {
			if (i >= loadedLeaderboardEntries.Count) {
				//adds a button if necessary
				loadedLeaderboardEntries.Add (Instantiate (leaderboardEntryPrefab, verticalLayoutGroupRectTransform.transform).GetComponent<LeaderboardEntryUI> ());
			}
			PlayerLeaderboardEntry entry = results.Leaderboard[i];
			float time = ((float)entry.StatValue)/1000;
			TimeSpan ts = TimeSpan.FromSeconds(time);
			string timeFormatted = string.Format ("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);
			loadedLeaderboardEntries [i].DisplayLeaderboardEntry (entry.Position, entry.DisplayName, timeFormatted);
		}
	}
	public void UpdateLeaderboardFail (PlayFabError error) {
		Debug.Log ("LeaderboardUI: GetLeaderboardFailure. Reason: " + error.ErrorMessage);
		noLeaderboardResultsObject.SetActive (true);
	}


	private void UpdateLeaderboardEntryGrouping (int leaderboardEntryCount) {
		for (int i = 0; i < loadedLeaderboardEntries.Count; i++) {
			if (i >= leaderboardEntryCount) {
				LeaderboardEntryUI deletingButton = loadedLeaderboardEntries[i];
				loadedLeaderboardEntries.RemoveAt (i);
				Destroy (deletingButton.gameObject);
			}
		}
		if (leaderboardEntryCount >= 5) {
			Debug.Log ("resizing wizard select panel/rect Transform.");
			verticalLayoutGroupRectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, leaderboardEntryCount * 160);
			scrollRect.vertical = true;
		} else {
			Debug.Log ("5 or less entries. setting to default size and disabling scroll.");
			verticalLayoutGroupRectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, defaultRectVerticalSize);
			scrollRect.vertical = false;
		}

	}
}
