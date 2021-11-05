﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class WizardSaveDataManager : MonoBehaviour {

	private bool initialized;
	public static WizardSaveDataManager instance;

	public WizardData defaultWizardData;

	public List<WizardSaveData> infamousWizardSaveDatas;

	public Dictionary<string, WizardSaveData> infamousWizardDictionary = new Dictionary<string, WizardSaveData>();

	private void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (this.gameObject);
		CreatePersistentDataDirectories ();
	}

	private void Start () {
		infamousWizardSaveDatas = LoadInfamousWizardSavesJSON ();
	}
	public void CreatePersistentDataDirectories () {
		if (!Directory.Exists (Application.persistentDataPath + "/gauntlet_wizards")) {
			Directory.CreateDirectory (Application.persistentDataPath + "/gauntlet_wizards");
		}
		if (!Directory.Exists (Application.persistentDataPath + "/infamous_wizards")) {
			Directory.CreateDirectory (Application.persistentDataPath + "/infamous_wizards");
		}
	}
	public void SaveInfamousWizard (WizardSaveData wizardSaveData) {
		String directoryPath = Application.persistentDataPath + "/infamous_wizards/";
		SaveWizardJSON (wizardSaveData, directoryPath);
	}
	public void SaveInProgressWizard (WizardSaveData wizardSaveData) {
		String directoryPath = Application.persistentDataPath + "/gauntlet_wizards/";
		SaveWizardJSON (wizardSaveData, directoryPath);
	}

	private void SaveWizardJSON (WizardSaveData wizardSaveData, string directoryPath) {
		OnBeforeWizardSave (wizardSaveData);
		string wizardSavePath = directoryPath + wizardSaveData.wizardName + ".json";
		if (File.Exists (wizardSavePath)) {
			Debug.Log ("Save failed: Wizard already exists with this name.");
			return;
		} else {
			string json = JsonUtility.ToJson(wizardSaveData);
			File.WriteAllText (wizardSavePath, json);
		}
	}
	public void OnBeforeWizardSave (WizardSaveData wizardSaveData) {
		wizardSaveData.spellSchoolDataIndex = wizardSaveData.spellSchoolData.schoolIndexStart;

		if (wizardSaveData.primaryStaffSaveData != null) {
			wizardSaveData.primaryStaffSaveData.puzzleDataIndex = wizardSaveData.primaryStaffSaveData.puzzleData.id;
			foreach (SpellGemSaveData spellSaveData in wizardSaveData.primaryStaffSaveData.spellGemSaveDataDictionary.Values) {
				spellSaveData.spellDataIndex = spellSaveData.spellData.id;
			}
		}
		if (wizardSaveData.secondaryStaffSaveData != null) {
			wizardSaveData.secondaryStaffSaveData.puzzleDataIndex = wizardSaveData.secondaryStaffSaveData.puzzleData.id;
			foreach (SpellGemSaveData spellSaveData in wizardSaveData.secondaryStaffSaveData.spellGemSaveDataDictionary.Values) {
				spellSaveData.spellDataIndex = spellSaveData.spellData.id;
			}
		}

		wizardSaveData.inventorySaveData.puzzleDataIndex = wizardSaveData.inventorySaveData.puzzleData.id;
		foreach (SpellGemSaveData spellSaveData in wizardSaveData.inventorySaveData.spellGemSaveDataDictionary.Values) {
			spellSaveData.spellDataIndex = spellSaveData.spellData.id;
		}
	}

	public List<WizardSaveData> LoadInfamousWizardSavesJSON () {
		string wizardDirectory = Application.persistentDataPath + "/infamous_wizards/";
		List<WizardSaveData> infamousWizards = new List<WizardSaveData>();

		String[] wizardFilePaths = Directory.GetFiles (wizardDirectory);
		Debug.Log ("Found " + wizardFilePaths.Length + " saves");

		foreach (String wizardFilePath in wizardFilePaths) {
			String json = File.ReadAllText (wizardFilePath);
			WizardSaveData wizard = JsonUtility.FromJson<WizardSaveData>(json);
			OnAfterLoad (wizard);
			infamousWizards.Add (wizard);
			if (isWizardNameAvailable (wizard.wizardName))
				infamousWizardDictionary.Add (wizard.wizardName, wizard);
		}
		return infamousWizards;
	}
	public bool isWizardNameAvailable (string wizardName) {
		if (infamousWizardDictionary.ContainsKey (wizardName)) {
			return false;
		} else return true;
	}

	public void OnAfterLoad (WizardSaveData wizardSaveData) {
		//gets SO data objects by id.
		wizardSaveData.spellSchoolData = ConstantsManager.instance.GetSpellSchoolData (wizardSaveData.spellSchoolDataIndex);
		wizardSaveData.primaryStaffSaveData.puzzleData = ConstantsManager.instance.GetPuzzleData (wizardSaveData.primaryStaffSaveData.puzzleDataIndex);
		wizardSaveData.secondaryStaffSaveData.puzzleData = ConstantsManager.instance.GetPuzzleData (wizardSaveData.secondaryStaffSaveData.puzzleDataIndex);
		wizardSaveData.inventorySaveData.puzzleData = ConstantsManager.instance.GetPuzzleData (wizardSaveData.inventorySaveData.puzzleDataIndex);

		foreach (SpellGemSaveData spellSaveData in wizardSaveData.primaryStaffSaveData.spellGemSaveDataDictionary.Values) {
			spellSaveData.spellData = ConstantsManager.instance.GetSpellData (spellSaveData.spellDataIndex);
		}
		foreach (SpellGemSaveData spellSaveData in wizardSaveData.secondaryStaffSaveData.spellGemSaveDataDictionary.Values) {
			spellSaveData.spellData = ConstantsManager.instance.GetSpellData (spellSaveData.spellDataIndex);
		}
		foreach (SpellGemSaveData spellSaveData in wizardSaveData.inventorySaveData.spellGemSaveDataDictionary.Values) {
			spellSaveData.spellData = ConstantsManager.instance.GetSpellData (spellSaveData.spellDataIndex);
		}
	}
	/*
		public void EnrichWizardDataAssetPaths (WizardSaveData wizardSaveData) {
			//wizardSaveData.spellSchoolDataPath = Resources.GetBuiltinResource(typeof(SpellSchoolData), wizardSaveData.spellSchoolData);

			EnrichPuzzleData (wizardSaveData.primaryStaffSaveData);
			EnrichPuzzleData (wizardSaveData.secondaryStaffSaveData);
			EnrichPuzzleData (wizardSaveData.inventorySaveData);
		}

		private void EnrichPuzzleData(PuzzleSaveData puzzleSaveData) {
			puzzleSaveData.puzzlePath = AssetDatabase.GetAssetPath (puzzleSaveData.puzzleData);
			foreach (SpellSaveData spellSaveData in puzzleSaveData.puzzleSaveDataDictionary.Values) {
				spellSaveData.spellDataPath = AssetDatabase.GetAssetPath (spellSaveData.spellData);
			}
		}*/

	public void DeleteWizardData (string wizardName) {
		string wizardFilePath = Application.persistentDataPath + "/infamous_wizards/" + wizardName + ".json";
		if (File.Exists (wizardFilePath)) {
			File.Delete (wizardFilePath);
		}
		infamousWizardSaveDatas = LoadInfamousWizardSavesJSON ();
	}

	public void DeleteAllWizardData () {
		string wizardDirectory = Application.persistentDataPath + "/infamous_wizards/";
		String[] wizardFileNames = Directory.GetFiles (wizardDirectory);
		List<WizardSaveData> wizards = new List<WizardSaveData>();
		foreach (String wizardFileName in wizardFileNames) {
			File.Delete (wizardFileName);
		}
	}
}
