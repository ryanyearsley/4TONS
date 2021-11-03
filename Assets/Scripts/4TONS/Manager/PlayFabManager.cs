using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class PlayFabManager : MonoBehaviour {
	#region Singleton
	public static PlayFabManager instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	private void Awake () {
		SingletonInitialization ();
	}
	void Start () {
		LogIn ();
	}

	void LogIn () {
		LoginWithCustomIDRequest loginRequest = new LoginWithCustomIDRequest {
			CustomId = SystemInfo.deviceUniqueIdentifier,
			CreateAccount = true
		};
		PlayFabClientAPI.LoginWithCustomID (loginRequest, OnLoginSuccess, OnError);
	}

	void OnLoginSuccess (LoginResult result) {
		Debug.Log ("Successful login/account creation");
		GetTitleData ();
	}
	void OnError (PlayFabError error) {
		Debug.Log ("Error while logging in/creating account. Error: " + error.GenerateErrorReport ());
	}

	public void SendLeaderboardUpdate (int time, Zone zone) {
		UpdatePlayerStatisticsRequest updateRequest = new UpdatePlayerStatisticsRequest{
			Statistics = new List<StatisticUpdate> {
				new StatisticUpdate {
					StatisticName = zone.ToString(), Value = time
				}
			}
		};
		Debug.Log ("PlayfabManager: Sending Leaderboard Update for " +  zone.ToString());
		PlayFabClientAPI.UpdatePlayerStatistics (updateRequest, OnLeaderboardUpdate, OnLeaderboardUpdateError);

	}

	private void OnLeaderboardUpdate (UpdatePlayerStatisticsResult obj) {
		Debug.Log ("Successful Leaderboard update");
	}
	void OnLeaderboardUpdateError (PlayFabError error) {
		Debug.Log ("Error while updating Leaderboard. Error: " + error.GenerateErrorReport ());
	}

	public void GetLeaderboard (string leaderboardName, Action<GetLeaderboardResult> successCallback, Action<PlayFabError> errorCallback) {
		GetLeaderboardRequest request = new GetLeaderboardRequest {
			StatisticName = leaderboardName,
			StartPosition = 0,
			MaxResultsCount = 100
		};
		PlayFabClientAPI.GetLeaderboard (request, successCallback, errorCallback);
	}

	void GetTitleData () {
		PlayFabClientAPI.GetTitleData (new GetTitleDataRequest (), OnTitleDataReceived, OnError);
	}

	void OnTitleDataReceived (GetTitleDataResult result) {
		if (result.Data == null || result.Data.ContainsKey ("Message") == false) {
			Debug.Log ("No message!");
			return;
		} else if (MessageOfTheDayManager.instance != null) {
			MessageOfTheDayManager.instance.UpdateMessageOfTheDay (result.Data ["Message"]);
		}
	}

}

