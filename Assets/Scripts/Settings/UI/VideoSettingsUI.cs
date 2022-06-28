using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingsUI : AbstractSettingsUI
{
	public Toggle vSyncToggle;
	public TMP_Text framerateText;
	public TMP_Text resolutionText;
	private int selectedResolutionIndex = 2;
	private int selectedFramerateIndex = 1;

	public VideoSettingsData currentlyDisplayedSettings;
	public override void InitializeUI () {
		base.InitializeUI ();
	}
	private void OnEnable () {
		SettingsManager.instance.updateVideoEvent += UpdateVideoSettingsUI;
	}
	private void OnDisable () {
		SettingsManager.instance.updateVideoEvent -= UpdateVideoSettingsUI;
	}

	public void UpdateVideoSettingsUI (VideoSettingsData videoSettingsData) {
		currentlyDisplayedSettings = videoSettingsData;
		vSyncToggle.isOn = currentlyDisplayedSettings.isVSyncOn;
		selectedResolutionIndex = currentlyDisplayedSettings.resolutionIndex;
		CalculateResolutionFromIndex (currentlyDisplayedSettings.resolutionIndex);
		selectedFramerateIndex = currentlyDisplayedSettings.framerateIndex;
		framerateText.text = ConstantsManager.instance.validTargetFramerates [selectedFramerateIndex].ToString ();

	}
	public override void LoadSettings () {
		if (SettingsManager.instance != null) {
			UpdateVideoSettingsUI (SettingsManager.instance.GetVideoSettings ());
		}
	}

	public void SmallerResolutionButtonPressed () {
		if (selectedResolutionIndex > 0) {
			CalculateResolutionFromIndex (selectedResolutionIndex - 1);
		}
	}
	public void LargerResolutionButtonPressed () {
		if (selectedResolutionIndex < 3) {
			CalculateResolutionFromIndex (selectedResolutionIndex + 1);
		}
	}

	public void LowerFramerateButtonPressed() {
		if (selectedFramerateIndex > 0) {
			selectedFramerateIndex -= 1;
			framerateText.text = ConstantsManager.instance.validTargetFramerates [selectedFramerateIndex].ToString();
		}
	}

	public void HigherFramerateButtonPressed() {
		if (selectedFramerateIndex < ConstantsManager.instance.validTargetFramerates.Count - 1) {
			selectedFramerateIndex += 1;
			framerateText.text = ConstantsManager.instance.validTargetFramerates [selectedFramerateIndex].ToString ();
		}
	}
	private void CalculateResolutionFromIndex (int resIndex) {
		selectedResolutionIndex = resIndex;
		int resX = (selectedResolutionIndex + 1) * 640;
		int resY =  (selectedResolutionIndex + 1) * 360;
		resolutionText.text = resX + " x " + resY;
	}


	public override void ApplySettingsUpdate () {
		VideoSettingsData newSettingsData = new VideoSettingsData();
		newSettingsData.isVSyncOn = vSyncToggle.isOn;
		newSettingsData.resolutionIndex = selectedResolutionIndex;
		newSettingsData.framerateIndex = selectedFramerateIndex;
		SettingsManager.instance.SaveVideoSettingsToDisk (newSettingsData);
	}
}
