using System;
using System.Collections.Generic;
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
		saveData = LoadDataFromDisk ();
		CreatePersistentDataDirectories ();
	}

	public void CreatePersistentDataDirectories() {
		if (!Directory.Exists(Application.persistentDataPath + "/wizards")) {
			Directory.CreateDirectory (Application.persistentDataPath + "/wizards");
		}
	}

	public SaveData GetSaveData () {
		return saveData;
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
	public void SaveNewWizardDataJSON (WizardSaveData wizardSaveData) {
		string wizardSavePath = Application.persistentDataPath + "/wizards/" + wizardSaveData.wizardName + ".json";
		if (File.Exists (wizardSavePath)) {
			Debug.Log ("Save failed: Wizard already exists with this name.");
			return;
		} else {
			string json = JsonUtility.ToJson(wizardSaveData);
			File.WriteAllText (wizardSavePath, json);
		}
	}

	public List<WizardSaveData> LoadWizardSavesFromDiskJSON () {
		string wizardDirectory = Application.persistentDataPath + "/wizards/";
		List<WizardSaveData> wizards = new List<WizardSaveData>();

		String[] wizardFilePaths = Directory.GetFiles (wizardDirectory);
		Debug.Log ("Found " + wizardFilePaths.Length + " saves");

		foreach (String wizardFilePath in wizardFilePaths) {
			String json = File.ReadAllText (wizardFilePath);
			WizardSaveData wizard = JsonUtility.FromJson<WizardSaveData>(json);
			wizards.Add (wizard);
		}
		return wizards;
	}
	public void DeleteAllWizardData() {
		string wizardDirectory = Application.persistentDataPath + "/wizards/";
		String[] wizardFileNames = Directory.GetFiles (wizardDirectory);
		List<WizardSaveData> wizards = new List<WizardSaveData>();
		foreach (String wizardFileName in wizardFileNames) {
			File.Delete (wizardFileName);
		}
	}
}



[System.Serializable]
public class SaveData {
	[Range (0f, 1f)] public float masterVolume;
	[Range (0f, 1f)] public float musicVolume;
	[Range (0f, 1f)] public float soundEffectsVolume;
	public List<WizardSaveData> wizards;
}