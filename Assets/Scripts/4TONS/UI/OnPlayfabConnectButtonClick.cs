using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class OnPlayfabConnectButtonClick : AbstractButtonClick {
	private const int MAX_CONNECTION_ATTEMPTS = 5;
	[SerializeField]
	private Sprite connectedIcon;
	[SerializeField]
	private Sprite offlineIcon;
	[SerializeField]
	private Image currentConnectionIconImage;
	[SerializeField]
	private TMP_Text welcomePlayerText;
	private void Start () {

		StartCoroutine (PollForConnectionRoutine ());
	}
	public IEnumerator PollForConnectionRoutine () {
		bool isConnected = false;
		int connectionAttempts = 0;
		while (connectionAttempts < MAX_CONNECTION_ATTEMPTS) {
			yield return new WaitForSeconds (0.5f);
			if (PlayFabManager.instance != null && PlayFabManager.instance.CheckConnectionStatus ()) {
				LoginResult loginResult = PlayFabManager.instance.GetCachedLoginResult ();
				if (loginResult.NewlyCreated || loginResult.InfoResultPayload.PlayerProfile == null) {
					SetConnectionText ("Welcome,\nPlayer!");
				} else if (loginResult.InfoResultPayload.PlayerProfile != null && loginResult.InfoResultPayload.PlayerProfile.DisplayName != null) {
					SetConnectionText ("Welcome back,\n" + loginResult.InfoResultPayload.PlayerProfile.DisplayName + "...");
				}
				SetIndicator (true);
				isConnected = true;
				yield break;
			}
			connectionAttempts++;
		}
		if (!isConnected) {
			SetConnectionText ("Offline mode.\n(No Leaderboards)");
			SetIndicator (false);
		}
	}
	public override void OnClick () {
		base.OnClick ();
		if (PlayFabManager.instance != null) {
			PlayFabManager.instance.LogIn (OnLoginSuccess, OnError);
		}
	}

	private void SetIndicator (bool isConnected) {
		if (isConnected) {
			currentConnectionIconImage.sprite = connectedIcon;
		} else {
			currentConnectionIconImage.sprite = offlineIcon;
		}
	}

	void OnLoginSuccess (LoginResult result) {
		Debug.Log ("PlayFabConnectButton: Successful login");
		SetIndicator (true);
		if (result.InfoResultPayload.PlayerProfile != null && result.InfoResultPayload.PlayerProfile.DisplayName != null) {
			SetConnectionText ("Welcome back,\n" + result.InfoResultPayload.PlayerProfile.DisplayName + "...");
		} else {
			SetConnectionText ("Welcome,\nPlayer...");
		}
	}
	void OnError (PlayFabError error) {
		SetConnectionText ("Offline mode.\n(No Leaderboards)");
		SetIndicator (false);
		Debug.Log ("Error while logging in/creating account. Error: " + error.GenerateErrorReport ());
	}

	public void SetConnectionText (string text) {
		welcomePlayerText.text = text;
	}
}
