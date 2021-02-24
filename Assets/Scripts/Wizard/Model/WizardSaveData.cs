using System;
using System.Collections.Generic;

[Serializable]
public class WizardSaveData : ICloneable {
	public string wizardName;
	public SpellSchoolData spellSchoolData;
	public string spellSchoolDataPath;
	public PuzzleSaveData primaryStaffSaveData;
	public PuzzleSaveData secondaryStaffSaveData;
	public PuzzleSaveData inventorySaveData;

	public WizardSaveData Clone () {
		WizardSaveData wizard =  (WizardSaveData) this.MemberwiseClone();
		wizard.primaryStaffSaveData = new PuzzleSaveData (primaryStaffSaveData.puzzleData, primaryStaffSaveData.puzzleSaveDataDictionary);
		wizard.secondaryStaffSaveData = new PuzzleSaveData (secondaryStaffSaveData.puzzleData, secondaryStaffSaveData.puzzleSaveDataDictionary);
		wizard.inventorySaveData = new PuzzleSaveData (inventorySaveData.puzzleData, inventorySaveData.puzzleSaveDataDictionary);
		return wizard;
	}

	object ICloneable.Clone () {
		return Clone ();
	}


}