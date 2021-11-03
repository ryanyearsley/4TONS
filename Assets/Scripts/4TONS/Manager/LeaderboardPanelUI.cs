using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System;

public class LeaderboardPanelUI : AbstractPanelUI {
	#region Singleton
	public static LeaderboardPanelUI instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion



	private bool initialized;

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

	protected override void InitializePanel () {
		scrollRect = GetComponentInChildren<ScrollRect> ();
		defaultRectVerticalSize = verticalLayoutGroupRectTransform.rect.height;
		initialized = true;
		base.InitializePanel ();
		if (GameManager.instance != null) {
			GameManager.instance.gameCompleteEvent += OnGameComplete;
		}
	}


	public void OnGameComplete () {

		if (PlayFabManager.instance != null) {
			PlayFabManager.instance.GetLeaderboard ("Dark Tower Demo (Speed Run)", UpdateLeaderboardSuccess, UpdateLeaderboardFail);
		}
	}

	public void UpdateLeaderboardSuccess (GetLeaderboardResult results) {
		if (!initialized) {
			InitializePanel ();
		}
		if (results.Leaderboard.Count == 0) {
			noLeaderboardResultsObject.SetActive (true);
		} else {
			noLeaderboardResultsObject.SetActive (false);
		}
		Debug.Log ("Updating Leaderboard UI. Leaderboard entry count: " + results.Leaderboard.Count);
		int leaderboardEntryCount = results.Leaderboard.Count;
		UpdateLeaderboardEntryGrouping (leaderboardEntryCount);
		for (int i = 0; i < leaderboardEntryCount; i++) {
			if (i >= loadedLeaderboardEntries.Count) {
				//adds a button if necessary
				loadedLeaderboardEntries.Add (Instantiate (leaderboardEntryPrefab, verticalLayoutGroupRectTransform.transform).GetComponent<LeaderboardEntryUI> ());
			}
			PlayerLeaderboardEntry entry = results.Leaderboard[i];
			float time = entry.StatValue/1000;
			TimeSpan ts = TimeSpan.FromSeconds(time);
			string timeFormatted = string.Format ("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);
			loadedLeaderboardEntries [i].DisplayLeaderboardEntry (entry.Position, entry.PlayFabId, timeFormatted);
		}
	}
	public void UpdateLeaderboardFail (PlayFabError error) {
		noLeaderboardResultsObject.SetActive (true);
	}

	void Awake () {
		SingletonInitialization ();
		Debug.Log ("wizard select panel player awake...");
		scrollRect = GetComponentInChildren<ScrollRect> ();
		defaultRectVerticalSize = verticalLayoutGroupRectTransform.rect.height;
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
