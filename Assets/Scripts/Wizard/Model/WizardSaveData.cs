using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class WizardSaveData : ICloneable {
	public string wizardName;
	public int spellSchoolDataIndex;
	public SpellSchoolData spellSchoolData;
	public PuzzleSaveData primaryStaffSaveData;
	public PuzzleSaveData secondaryStaffSaveData;
	public PuzzleSaveData inventorySaveData;

	

	public WizardSaveData Clone () {
		WizardSaveData wizard =  (WizardSaveData) this.MemberwiseClone();
		wizard.primaryStaffSaveData = new PuzzleSaveData (primaryStaffSaveData.puzzleData, primaryStaffSaveData.puzzleSaveDataDictionary.CloneDictionary());
		wizard.secondaryStaffSaveData = new PuzzleSaveData (secondaryStaffSaveData.puzzleData, secondaryStaffSaveData.puzzleSaveDataDictionary.CloneDictionary());
		wizard.inventorySaveData = new PuzzleSaveData (inventorySaveData.puzzleData, inventorySaveData.puzzleSaveDataDictionary.CloneDictionary());
		return wizard;
	}

	object ICloneable.Clone () {
		return Clone ();
	}


}