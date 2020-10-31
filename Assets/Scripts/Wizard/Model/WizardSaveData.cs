using System;

[Serializable]
public class WizardSaveData {
	public string wizardName;
	[NonSerialized]
	public SpellSchoolData spellSchoolData;
	public string spellSchoolDataPath;
	public StaffSaveData primaryStaffSaveData;
	public StaffSaveData secondaryStaffSaveData;
	public SpellSaveData[] inventorySaveData;
}