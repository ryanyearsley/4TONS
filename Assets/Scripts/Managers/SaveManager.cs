using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour {

	public static SaveManager instance;

	private SaveData saveData;
	protected string savePath;
	public SaveData defaultData;

	private void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}

	public void SaveDataToDisk (SaveData data) {
		saveData = data;
		savePath = Application.persistentDataPath + "/save.dat";
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (savePath);
		bf.Serialize (file, saveData);
		file.Close ();
	}

	public SaveData LoadDataFromDisk () {
		savePath = Application.persistentDataPath + "/save.dat";
		if (File.Exists (savePath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (savePath, FileMode.Open);
			saveData = (SaveData) bf.Deserialize (file);
			file.Close ();
			return saveData;
		} else {
			return defaultData;
		}
	}
}

[System.Serializable]
public class SaveData {
	[Range (0f, 1f)] public float masterVolume;
	[Range (0f, 1f)] public float musicVolume;
	[Range (0f, 1f)] public float soundEffectsVolume;
	public bool isAutomaticTransmission;
}