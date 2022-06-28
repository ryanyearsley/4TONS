using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettingsUI : AbstractSettingsUI
{
	private AudioSettingsData currentSaveData;
	public AudioMixerSliderUI masterSlider;
	public AudioMixerSliderUI musicSlider;
	public AudioMixerSliderUI sfxSlider;

	public override void InitializeUI () {
		base.InitializeUI ();
		//SettingsManager.instance.updateAudioEvent += UpdateAudioSettingsUI;
	}

	private void OnEnable () {
		SettingsManager.instance.updateAudioEvent += UpdateAudioSettingsUI;
	}
	private void OnDisable () {
		SettingsManager.instance.updateAudioEvent -= UpdateAudioSettingsUI;
	}

	public void UpdateAudioSettingsUI(AudioSettingsData audioSettingsData) {
		Debug.Log ("AudioSettingsUI: Updating UI");
		currentSaveData = audioSettingsData;
		masterSlider.SetSliderValue (currentSaveData.masterVolume);
		musicSlider.SetSliderValue (currentSaveData.musicVolume);
		sfxSlider.SetSliderValue (currentSaveData.sfxVolume);
	}
	public override void LoadSettings () {
		UpdateAudioSettingsUI (SettingsManager.instance.LoadAudioSettingsFromDisk ());
	}

	public override void ApplySettingsUpdate () {
		AudioSettingsData newSaveData = new AudioSettingsData();
		newSaveData.masterVolume = masterSlider.GetSliderValue ();
		newSaveData.musicVolume = musicSlider.GetSliderValue ();
		newSaveData.sfxVolume = sfxSlider.GetSliderValue ();
		SettingsManager.instance.SaveAudioSettingsToDisk (newSaveData);
	}
	public override void ResetDefaults () {
		SettingsManager.instance.ResetAudioSettings ();
	}

}
