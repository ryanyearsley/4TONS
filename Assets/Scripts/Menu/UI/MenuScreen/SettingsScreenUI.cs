using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;
using System;
using PlayFab;

public class SettingsScreenUI : AbstractScreenUI
{

	public AudioMixerSliderUI masterSlider;
	public AudioMixerSliderUI musicSlider;
	public AudioMixerSliderUI sfxSlider;

	public TMP_InputField displayNameInputField;
	public TMP_Text displayNameUpdateErrorText;

	protected override void Start () {
		base.Start ();
		LoadSettings (SettingsManager.instance.LoadSettingsDataFromDisk ());
		SettingsManager.instance.updateSettingsEvent += LoadSettings;
	}
	protected override void OnScreenChange (MenuScreen mainMenuScreen) {
		if (screenActiveStates.Contains (mainMenuScreen)) {
			screenObject.SetActive (true);
			LoadSettings (SettingsManager.instance.LoadSettingsDataFromDisk ());
		} else {
			if (screenObject.activeInHierarchy == true && CheckForUnsavedChanges()){
				SettingsManager.instance.RevertSettingsData ();
			}

			screenObject.SetActive (false);
		}
	}

	private void LoadSettings(SettingsData settingsData) {
		SettingsData currentSaveData = settingsData;
		masterSlider.SetSliderValue(currentSaveData.masterVolume);
		musicSlider.SetSliderValue (currentSaveData.musicVolume);
		sfxSlider.SetSliderValue (currentSaveData.sfxVolume);
		if (PlayFabManager.instance != null && PlayFabManager.instance.CheckConnectionStatus ()) {
			displayNameInputField.text = PlayFabManager.instance.GetCachedLoginResult ().InfoResultPayload.PlayerProfile.DisplayName;
		} else {
			displayNameInputField.text = "Offline";
		}
	}

	//called any time a settings UI element is changed
	public bool CheckForUnsavedChanges () {
		bool unsavedChanges = false;
		SettingsData currentSettings = SettingsManager.instance.GetSettingsData();
		if (masterSlider.GetSliderValue () != currentSettings.masterVolume)
			unsavedChanges = true;
		else if (musicSlider.GetSliderValue () != currentSettings.musicVolume)
			unsavedChanges = true;
		else if (sfxSlider.GetSliderValue () != currentSettings.sfxVolume)
			unsavedChanges = true;
		else if (displayNameInputField.text != currentSettings.playerName)
			unsavedChanges = true;
		return unsavedChanges;
	}

	public void ApplyAudioSettings() {
		SettingsData newSaveData = new SettingsData();
		newSaveData.masterVolume = masterSlider.GetSliderValue ();
		newSaveData.musicVolume = musicSlider.GetSliderValue ();
		newSaveData.sfxVolume = sfxSlider.GetSliderValue ();
		SettingsManager.instance.SaveSettingsDataToDisk (newSaveData);

	}

	public void UpdateDisplayName() {
		string displayName = displayNameInputField.text;

		if (displayName.Length < 15) {
			if (PlayFabManager.instance != null && PlayFabManager.instance.CheckConnectionStatus ()) {
				PlayFabManager.instance.SetDisplayName (displayName, OnUpdateDisplayNameCallback, OnErrorCallback);
			} else {
				SetUsernameErrorText ("Offline mode.");//Update the name anyway for offline use, but override it when eventually connects.
			}
		}
		else {

		}
	}

	private void OnUpdateDisplayNameCallback (UpdateUserTitleDisplayNameResult result) {
		SetUsernameErrorText ("Update Success!");
	}

	private void OnErrorCallback (PlayFabError error) {
		SetUsernameErrorText ("PlayFab Error:" + error.Error.ToString());
	}

	private void SetUsernameErrorText(string text) {
		displayNameUpdateErrorText.text = text;
	}

}
