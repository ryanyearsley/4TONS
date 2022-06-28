using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoManager : PersistentManager
{
	#region Singleton
	public static VideoManager instance { get; private set; }
	protected override void InitializeSingleton () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	public VideoSettingsData currentVideoSettings;

	public VideoSettingsData defaultVideoSettings;


	public override void InitializePersistentManager () {
		if (SettingsManager.instance != null) {
			currentVideoSettings = SettingsManager.instance.GetVideoSettings ();
		}
		else {
			currentVideoSettings = defaultVideoSettings;
		}
	}
	public override void SubscribeToEvents () {
		base.SubscribeToEvents ();
		SettingsManager.instance.updateVideoEvent += ApplyVideoSettings;
	}
	public override void UnsubscribeFromEvents () {
		base.UnsubscribeFromEvents ();
		SettingsManager.instance.updateVideoEvent -= ApplyVideoSettings;
	}

	public void ApplyVideoSettings (VideoSettingsData videoSettingsData) {
		if (!videoSettingsData.isVSyncOn)
			QualitySettings.vSyncCount = 0;
		else
			QualitySettings.vSyncCount = 1;
		
		//FullScreenMode mode = videoSettingsData.fullScreenMode;
		ResolutionSetting resSetting = ConstantsManager.instance.validResolutions[videoSettingsData.resolutionIndex];
		Screen.SetResolution (resSetting.resX, resSetting.resY, true);
		Application.targetFrameRate = ConstantsManager.instance.validTargetFramerates [videoSettingsData.framerateIndex];
	}
}
