using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public enum LeaderboardType {
	ONLINE, LOCAL, TEST
}
public class LeaderboardUI : MonoBehaviour {

	[SerializeField]
	private int maxEntriesPerPage;

	[SerializeField]
	private TestLeaderboardData testLeaderboardData;
	private bool initialized;

	[SerializeField]
	private TMP_Text leaderboardNameText;
	[SerializeField]
	private TMP_Text leaderboardPageInfoText;
	[SerializeField]
	private GameObject leaderboardEntryPrefab;

	[SerializeField]
	private RectTransform verticalLayoutGroupRectTransform;

	[SerializeField]
	private ScrollRect scrollRect;
	private float defaultRectVerticalSize;
	private Vector2 defaultRectPosition;

	[SerializeReference]
	private GameObject noLeaderboardResultsObject;

	private GetLeaderboardResult cachedOnlineResults;

	private List<LeaderboardEntry> currentLeaderboardEntries;
	[SerializeField]
	private List<LeaderboardEntryUI> leaderboardEntryUIs;

	private int currentHighestRankDisplayed;
	private int currentLowestRankDisplayed;

	public Action<List<LeaderboardEntry>, int> UpdateLeaderboardUIEvent;

	void Awake () {
		InitializeLeaderboard ();
	}
	public void InitializeLeaderboard () {
		scrollRect = GetComponentInChildren<ScrollRect> ();
		defaultRectVerticalSize = verticalLayoutGroupRectTransform.rect.height;
		defaultRectPosition = verticalLayoutGroupRectTransform.anchoredPosition;
		noLeaderboardResultsObject.SetActive (false);
		for (int i = 0; i < maxEntriesPerPage; i++) {
			leaderboardEntryUIs.Add (Instantiate (leaderboardEntryPrefab, verticalLayoutGroupRectTransform.transform).GetComponent<LeaderboardEntryUI> ());
			leaderboardEntryUIs [i].gameObject.SetActive (false);
		}
		initialized = true;
	}

	public void LoadGlobalLeaderboardDelayed () {
		StartCoroutine (LoadLeaderboardRoutine ());
	}
	public IEnumerator LoadLeaderboardRoutine () {
		yield return new WaitForSeconds (1);
		LoadCurrentOnlineLeaderboard ();
	}
	public void LoadCurrentOnlineLeaderboard() {
		if (GameManager.instance != null) {
			GameContext context = GameManager.instance.gameContext;
			GetOnlineLeaderboard (context.objectiveData.objective, context.zoneData.zone);
		}
	}


	public void LoadTestLeaderboard () {
		
		if (testLeaderboardData != null) {
			if (!testLeaderboardData.initialized) {
				LeaderboardUtilities.GenerateFakeTestData (testLeaderboardData);
			}
			UpdateLeaderboardUI (testLeaderboardData.testLeaderboardEntries, 0);
		}
	}
	//Gets Online leaderboard if present.
	public void GetOnlineLeaderboard (Objective objective, Zone zone) {
		string leaderboardName = objective.ToString() + ": " + zone.ToString();
		leaderboardNameText.text = leaderboardName;
		if (PlayFabManager.instance != null) {
			if (PlayFabManager.instance.GetCachedLoginResult () != null) {
				PlayFabManager.instance.GetLeaderboard (leaderboardName, GetOnlineLeaderboardSuccess, UpdateLeaderboardFail);
		} else {
				UpdateLeaderboardFail (new PlayFabError ());
			}
		}
	}

	public void GetOnlineLeaderboardSuccess (GetLeaderboardResult results) {
		Debug.Log ("LeaderboardUI: GetLeaderboardSuccess. Leaderboard entry count: " + results.Leaderboard.Count);
		cachedOnlineResults = results;
		List <LeaderboardEntry> onlineEntries = LeaderboardUtilities.ConvertPlayfabLeaderboardEntries(results.Leaderboard);
		if (onlineEntries.Count > 0) {
			UpdateLeaderboardUI (onlineEntries, 0);
		} else {
			UpdateLeaderboardFail (new PlayFabError());
		}
	}
	public void LoadPreviousPage() {
		if (currentLowestRankDisplayed > 0) {
			UpdateLeaderboardUI (currentLeaderboardEntries, currentLowestRankDisplayed - maxEntriesPerPage);
		}
	}
	public void LoadNextPage() {
		if (currentLeaderboardEntries.Count > currentHighestRankDisplayed) {
			UpdateLeaderboardUI (currentLeaderboardEntries, currentHighestRankDisplayed);
		} 
	}
	public void UpdateLeaderboardUI (List<LeaderboardEntry> leaderboardEntries, int startingRankIndex) {
		if (!initialized) {
			InitializeLeaderboard ();
		}
		noLeaderboardResultsObject.SetActive (false);
		currentLeaderboardEntries = leaderboardEntries;
		UpdateLeaderboardUIEvent?.Invoke (currentLeaderboardEntries, startingRankIndex);

		startingRankIndex = Mathf.Clamp (startingRankIndex, 0, leaderboardEntries.Count - 1);
		int totalDisplayedEntryCount = Mathf.Clamp ((leaderboardEntries.Count - startingRankIndex), 0, maxEntriesPerPage);
		
		int lastDisplayedRank = startingRankIndex + totalDisplayedEntryCount;
		leaderboardPageInfoText.text = (startingRankIndex + 1) + "-" + lastDisplayedRank;
		if (leaderboardEntries.Count > maxEntriesPerPage) {
			leaderboardPageInfoText.text += " (out of " + leaderboardEntries.Count + ")";
		}
		currentLowestRankDisplayed = startingRankIndex;
		currentHighestRankDisplayed = lastDisplayedRank;


		for (int i = 0; i < leaderboardEntryUIs.Count; i++) {
			if (leaderboardEntries.Count > startingRankIndex + i) {
				leaderboardEntryUIs [i].gameObject.SetActive (true);
				leaderboardEntryUIs [i].DisplayLeaderboardEntry (leaderboardEntries [startingRankIndex + i]);
			} else {
				leaderboardEntryUIs [i].gameObject.SetActive (false);
			}
		}

		if (totalDisplayedEntryCount >= 5) {
			Debug.Log ("resizing wizard select panel/rect Transform.");
			verticalLayoutGroupRectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, totalDisplayedEntryCount * 26);
			scrollRect.vertical = true;
			scrollRect.verticalNormalizedPosition = 1;
		} else {
			Debug.Log ("5 or less entries. setting to default size and disabling scroll.");
			verticalLayoutGroupRectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, defaultRectVerticalSize);
			verticalLayoutGroupRectTransform.anchoredPosition = defaultRectPosition;
			scrollRect.vertical = false;
		}
		
	}
	public void UpdateLeaderboardFail (PlayFabError error) {
		//TODO: Get local leaderboard here.
		Debug.Log ("LeaderboardUI: GetLeaderboardFailure. Defaulting to local leaderboard. Reason: " + error.ErrorMessage);
		noLeaderboardResultsObject.SetActive (true);
	}


}
