using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreenUI : AbstractScreenUI
{

	public AudioMixerSliderUI masterSlider;
	public AudioMixerSliderUI musicSlider;
	public AudioMixerSliderUI sfxSlider;

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
				SettingsManager.instance.ResetSettingsData ();
			}

			screenObject.SetActive (false);
		}
	}

	private void LoadSettings(SettingsData settingsData) {
		SettingsData currentSaveData = settingsData;
		masterSlider.SetSliderValue(currentSaveData.masterVolume);
		musicSlider.SetSliderValue (currentSaveData.musicVolume);
		sfxSlider.SetSliderValue (currentSaveData.sfxVolume);
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

		return unsavedChanges;
	}

	public void ApplySettings() {
		SettingsData newSaveData = new SettingsData();
		newSaveData.masterVolume = masterSlider.GetSliderValue ();
		newSaveData.musicVolume = musicSlider.GetSliderValue ();
		newSaveData.sfxVolume = sfxSlider.GetSliderValue ();
		SettingsManager.instance.SaveSettingsDataToDisk (newSaveData);

	}

}
