
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
	private SettingsData settingsData;

	protected string savePath;
	public SettingsData defaultData;

	public event Action<SettingsData> updateSettingsEvent;

	private void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (this.gameObject);
		settingsData = LoadSettingsDataFromDisk ();
	}

	private void Start () {

	}
	private void OnLevelWasLoaded (int level) {
	}

	public SettingsData GetSettingsData () {
		return settingsData;
	}
	
	public SettingsData RevertSettingsData () {
		SaveSettingsDataToDisk (settingsData);
		return settingsData;
	}
	public SettingsData ResetSettingsData() {
		SaveSettingsDataToDisk (defaultData);
		return settingsData;
	}
	public void SaveSettingsDataToDisk (SettingsData data) {
		settingsData = data;
		updateSettingsEvent?.Invoke (settingsData);
		savePath = Application.persistentDataPath + "/save.dat";
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (savePath);
		bf.Serialize (file, settingsData);
		file.Close ();
	}


	public SettingsData LoadSettingsDataFromDisk () {
		savePath = Application.persistentDataPath + "/save.dat";
		if (File.Exists (savePath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (savePath, FileMode.Open);
			settingsData = (SettingsData)bf.Deserialize (file);
			file.Close ();
			return settingsData;
		} else {
			return defaultData;
		}
	}
}



[System.Serializable]
public class SettingsData {
	//AUDIO
	[Range (0f, 1f)] public float masterVolume;
	[Range (0f, 1f)] public float musicVolume;
	[Range (0f, 1f)] public float sfxVolume;
}