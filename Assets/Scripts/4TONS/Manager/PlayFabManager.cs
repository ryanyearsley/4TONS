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

	private LoginResult cachedResult = null;

	private void Awake () {
		SingletonInitialization ();
		PlayFabManager.instance.LogIn (OnInitialLogInSuccessCallback, OnLogInFailure);
	}
	void GenericErrorExample (PlayFabError error) {
		Debug.Log ("Generic Error: " + error.GenerateErrorReport ());
	}

	public void LogIn (Action<LoginResult> loginSuccessCallback, Action<PlayFabError> loginFailureCallback) {
		LoginWithCustomIDRequest loginRequest = new LoginWithCustomIDRequest {
			CustomId = SystemInfo.deviceUniqueIdentifier,
			CreateAccount = true,
			InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
				GetPlayerProfile = true
			}
		};
		loginSuccessCallback += UpdateCachedLoginResult;
		PlayFabClientAPI.LoginWithCustomID (loginRequest, loginSuccessCallback, loginFailureCallback);
		loginSuccessCallback -= UpdateCachedLoginResult;
	}
	void UpdateCachedLoginResult(LoginResult result) {
		cachedResult = result;
	}

	public LoginResult GetCachedLoginResult() {
		if (cachedResult != null) {
			return cachedResult;
		} else return null;
	}

	private void OnInitialLogInSuccessCallback (LoginResult result) {
		Debug.Log ("PlayFabManager: Log in success.");
		string name = null;
		if (result.InfoResultPayload.PlayerProfile != null) {
			name = result.InfoResultPayload.PlayerProfile.DisplayName;
			Debug.Log ("PlayFabManager: Player Profile present. Display name: " + name +"]") ;
			if (name == null || name.Equals("")) {
				SetDisplayName (GenerateString (), OnInitialSetDisplayNameSuccess, OnInitialSetDisplayNameFailure);
			}
		} 
	}
	private void OnInitialSetDisplayNameSuccess(UpdateUserTitleDisplayNameResult result) {
		Debug.Log ("PlayFabManager: Successfully set player name to gibberish: " + result.DisplayName);
	}
	private void OnInitialSetDisplayNameFailure(PlayFabError error) {

		switch (error.Error) {
			case PlayFabErrorCode.UsernameNotAvailable:
				Debug.Log ("PlayFabManager: ERROR: Username Not Available!");
				break;
			case PlayFabErrorCode.DuplicateUsername:
				Debug.Log ("PlayFabManager: ERROR: Username Not Available!");
				break;
			case PlayFabErrorCode.InvalidUsername:
				Debug.Log ("PlayFabManager: ERROR: Username Invalid!");
				break;
			case PlayFabErrorCode.AccountBanned:
				Debug.Log ("PlayFabManager: ERROR: Account Banned");
				break;
			default:
				break;
		}
	}

	const string glyphs = "abcdefghijklmnopqrstuvwxyz";
	private string GenerateString () {
		int charAmount = UnityEngine.Random.Range(6, 12); //set those to the minimum and maximum length of your string
		string myString = null;
		for (int i = 0; i < charAmount; i++) {
			myString += glyphs [UnityEngine.Random.Range (0, glyphs.Length)];
		}

		return myString;
	}

	private void OnLogInFailure (PlayFabError error) {
		Debug.Log ("PlayFabManager: Log in failure.");
	}

	public bool CheckConnectionStatus () {
		return PlayFabClientAPI.IsClientLoggedIn ();
	}

	public void SetDisplayName (string displayName, Action<UpdateUserTitleDisplayNameResult> resultCallback, Action<PlayFabError> errorCallback) {
		UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest {
			DisplayName = displayName,
		};
		PlayFabClientAPI.UpdateUserTitleDisplayName (request, resultCallback, errorCallback);
	}

	public void SendLeaderboardUpdate (int time, string leaderboardName) {
		UpdatePlayerStatisticsRequest updateRequest = new UpdatePlayerStatisticsRequest{
			Statistics = new List<StatisticUpdate> {
				new StatisticUpdate {
					StatisticName = leaderboardName, Value = time
				}
			}
		};
		Debug.Log ("PlayfabManager: Sending Leaderboard Update for " + leaderboardName);
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
	public void GetTitleData (Action<GetTitleDataResult> successCallback, Action<PlayFabError> errorCallback) {
		PlayFabClientAPI.GetTitleData (new GetTitleDataRequest (), successCallback, errorCallback);
	}
}

