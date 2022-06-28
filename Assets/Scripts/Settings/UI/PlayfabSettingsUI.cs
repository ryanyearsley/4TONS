using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab.ClientModels;
public class PlayfabSettingsUI : AbstractSettingsUI
{

	public TMP_InputField displayNameInputField;
	public TMP_Text displayNameUpdateErrorText;

	public override void LoadSettings () {
		if (PlayFabManager.instance != null && PlayFabManager.instance.CheckConnectionStatus ()) {
			displayNameInputField.text = PlayFabManager.instance.GetCachedLoginResult ().InfoResultPayload.PlayerProfile.DisplayName;
		} else {
			displayNameInputField.text = "Offline";
		}
	}
	public override void ApplySettingsUpdate () {
		string displayName = displayNameInputField.text;

		if (displayName.Length < 15) {
			if (PlayFabManager.instance != null && PlayFabManager.instance.CheckConnectionStatus ()) {
				PlayFabManager.instance.SetDisplayName (displayName, OnUpdateDisplayNameCallback, OnErrorCallback);
			} else {
				SetUsernameErrorText ("Offline");//Update the name anyway for offline use, but override it when eventually connects.
			}
		} else {
			SetUsernameErrorText ("Name too long (max 15 characters)");
		}
	}

	public override void ResetDefaults() {
		SetUsernameErrorText (""); 
		displayNameInputField.text = "Enter name...";
	}
	private void OnUpdateDisplayNameCallback (UpdateUserTitleDisplayNameResult result) {
		SetUsernameErrorText ("Update Success!");
	}
	private void OnErrorCallback(PlayFab.PlayFabError error) {
		SetUsernameErrorText ("Update failed.");
	}

	private void SetUsernameErrorText (string text) {
		displayNameUpdateErrorText.text = text;
	}
}
