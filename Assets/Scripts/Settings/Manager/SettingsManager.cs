
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour {

	private bool initialized;
	public static SettingsManager instance;

	//currentSettings
	private AudioSettingsData currentAudioSettings;
	private VideoSettingsData currentVideoSettings;
	private PlayfabSettingsData currrentPlayfabSettings;

	protected string savePath;
	public AudioSettingsData defaultAudioSettings;
	public VideoSettingsData defaultVideoSettings;
	public PlayfabSettingsData defaultPlayfabSettings;


	public event Action<AudioSettingsData> updateAudioEvent;
	public event Action<VideoSettingsData> updateVideoEvent;
	public event Action checkUnsavedChangesEvent;

	private void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (this.gameObject);
		//Application.targetFrameRate = 60;
		currentAudioSettings = LoadAudioSettingsFromDisk ();
		currentVideoSettings = LoadVideoSettingsFromDisk ();
		currrentPlayfabSettings = LoadPlayfabSettingsFromDisk ();
	}


	public AudioSettingsData GetAudioSettings () {
		return currentAudioSettings;
	}
	public VideoSettingsData GetVideoSettings () {
		return currentVideoSettings;
	}
	
	public AudioSettingsData RevertSettingsData () {
		SaveAudioSettingsToDisk (currentAudioSettings);
		return currentAudioSettings;
	}
	public AudioSettingsData ResetAudioSettings() {
		SaveAudioSettingsToDisk (defaultAudioSettings);
		return currentAudioSettings;
	}

	public VideoSettingsData ResetVideoSettings() {
		SaveVideoSettingsToDisk (defaultVideoSettings);
		return currentVideoSettings;
	}
	public void SaveAudioSettingsToDisk (AudioSettingsData data) {
		currentAudioSettings = data;
		updateAudioEvent?.Invoke (currentAudioSettings);
		savePath = Application.persistentDataPath + "/audio_settings.dat";
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (savePath);
		bf.Serialize (file, currentAudioSettings);
		file.Close ();
	}

	public void SaveVideoSettingsToDisk (VideoSettingsData data) {
		currentVideoSettings = data;
		updateVideoEvent?.Invoke (currentVideoSettings);
		savePath = Application.persistentDataPath + "/video_settings.dat";
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (savePath);
		bf.Serialize (file, currentVideoSettings);
		file.Close ();
	}


	public AudioSettingsData LoadAudioSettingsFromDisk () {
		savePath = Application.persistentDataPath + "/audio_settings.dat";
		if (File.Exists (savePath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (savePath, FileMode.Open);
			currentAudioSettings = (AudioSettingsData)bf.Deserialize (file);
			file.Close ();
			return currentAudioSettings;
		} else {
			return defaultAudioSettings;
		}
	}

	public VideoSettingsData LoadVideoSettingsFromDisk () {
		savePath = Application.persistentDataPath + "/video_settings.dat";
		if (File.Exists (savePath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (savePath, FileMode.Open);
			currentVideoSettings = (VideoSettingsData)bf.Deserialize (file);
			file.Close ();
			return currentVideoSettings;
		} else {
			return defaultVideoSettings;
		}
	}
	public PlayfabSettingsData LoadPlayfabSettingsFromDisk () {
		savePath = Application.persistentDataPath + "/playfab_settings.dat";
		if (File.Exists (savePath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (savePath, FileMode.Open);
			currrentPlayfabSettings = (PlayfabSettingsData)bf.Deserialize (file);
			file.Close ();
			return currrentPlayfabSettings;
		} else {
			return defaultPlayfabSettings;
		}
	}

}






public class PlayfabSettingsData {
	public string playfabDisplayName = "Player 1";
}