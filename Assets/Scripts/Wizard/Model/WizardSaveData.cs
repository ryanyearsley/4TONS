using System;
using System.Collections.Generic;

[Serializable]
public class WizardSaveData {
	public string wizardName;
	public SpellSchoolData spellSchoolData;
	public string spellSchoolDataPath;
	public StaffSaveData primaryStaffSaveData;
	public StaffSaveData secondaryStaffSaveData;
	public PuzzleSaveDataDictionary inventorySaveDataDictionary;
}